using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    [Title("Base Attributes")]
    public float cooldownReduction = 0f;
    public float manacostReduction = 0f;

    private float addedCooldownReduction = 0;
    private float addedManaCostReduction = 0;

    public float CooldownReduction
    {
        get => cooldownReduction + addedCooldownReduction;
    }

    public float ManaCostReduction
    {
        get => manacostReduction + addedManaCostReduction;
    }

    [Title("Skill and Talents")]
    public SkillDatabase allSkill;
    public TalentDatabase allTalent;
    List<SkillData> allSkills { get => allSkill.db; }
    List<TalentData> allTalents { get => allTalent.db; }
    public float lookRadius = 100;
    public float maxMana = 200f;
    [ReadOnly] public float currentMana;
    SkillData skillData;
    public LayerMask layer;
    public List<string> collidedTags = new List<string>() { "Enemy" };

    int currentTalentIndex;
    int currentIndex;

    List<SkillInstance> skillInstances;
    List<TalentInstance> talentInstances;

    [Title("Talent UI")]
    public GameObject locked;
    public Image talentIcon;
    public TextMeshProUGUI talentName;
    public TextMeshProUGUI talentDesc;

    [Title("Skill UI")]
    public Image disabled;
    public Image icon;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI skillDesc;
    public TextMeshProUGUI skillCD;
    public TextMeshProUGUI skillCost;
    public TextMeshProUGUI skillLevel;

    [Title("Etc")]
    public TextMeshProUGUI currentManaText;
    public TextMeshProUGUI currentHealthText;
    public AudioClip skillFailedClip;
    PlayerHealth playerHealth;
    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerHealth.OnHealthUpdate += SetHealth;
        SetHealth(playerHealth.maxHealth);
        skillInstances = new List<SkillInstance>();
        talentInstances = new List<TalentInstance>();
        currentIndex = 0;
        currentTalentIndex = 0;
        ChangeTalent();
        ChangeSkill();
        SetCurrentMana(maxMana);
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthUpdate -= SetHealth;
    }

    void SetHealth(float current)
    {
        currentHealthText.text = current.ToString();
    }
    void SetCurrentMana(float mana)
    {
        currentMana = mana;
        currentManaText.text = currentMana.ToString();
    }

    [ReadOnly] public TalentInstance currentTalentInstance;
    [ReadOnly] public TalentData selectedTalentData;
    private void ChangeTalent()
    {
        selectedTalentData = allTalents[currentTalentIndex];
        talentName.text = selectedTalentData.Name();
        talentDesc.text = selectedTalentData.Description();
        talentIcon.sprite = selectedTalentData.icon;

        var find = talentInstances.Find(x => x.talentID == selectedTalentData.id);
        if (find == null)
        {
            talentInstances.Add(new TalentInstance()
            {
                talentID = selectedTalentData.id,
                isUnlocked = false,
            });
        }

        currentTalentInstance = talentInstances.Find(x => x.talentID == selectedTalentData.id);
        locked.SetActive(!currentTalentInstance.isUnlocked);
    }

    SkillInstance currentSkillInstance;
    private void ChangeSkill()
    {
        skillData = allSkills[currentIndex];
        skillName.text = skillData.Name();
        skillDesc.text = skillData.Description();
        icon.sprite = skillData.icon;

        var find = skillInstances.Find(x => x.skillID == skillData.id);
        if (find == null)
        {
            skillInstances.Add(new SkillInstance()
            {
                skillID = skillData.id,
                currentLevel = 0
            });
        }

        currentSkillInstance = skillInstances.Find(x => x.skillID == skillData.id);
        UpdateLevelSkill();
    }

    private void UpdateLevelSkill()
    {
        var manaCost = skillData.GetManaCost(currentSkillInstance.currentLevel) - currentSkillInstance.bonusManacostReduction;
        var manaCostReduce = manaCost * ManaCostReduction / 100f;

        manaCost = manaCost - manaCostReduce;
        skillCost.text = manaCost.ToString();
        skillLevel.text = "Lv. " + (currentSkillInstance.currentLevel + 1).ToString();
    }

    void UpdateUISkill()
    {
        if(skillData)
        {
            var skillInstance = skillInstances.Find(x => x.skillID == skillData.id);
            if(skillInstance != null)
            {
                var second = skillInstance.GetSecond(Time.time);
                skillCD.text = second < 0 ? "" : second.ToString("F1");
                disabled.fillAmount = 1 - skillInstance.GetPercentage(Time.time);
            }
            else
            {
                skillCD.text = "";
                disabled.fillAmount = 0;
            }
        }
    }

    private void Update()
    {
        UpdateUISkill();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            currentIndex++;
            if(Utilities.OutOfIndex(allSkills.Count, currentIndex))
                currentIndex = 0;

            ChangeSkill();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentTalentIndex++;
            if (Utilities.OutOfIndex(allTalents.Count, currentTalentIndex))
                currentTalentIndex = 0;

            ChangeTalent();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            DowngradeSkill();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UpgradeSkill();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleTalent();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseSkill();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            SetCurrentMana(maxMana);
            foreach (var skill in skillInstances)
            {
                skill.lastTimeUsed = 1;
                skill.readyTime = 1;
            }
        }
    }

    public void ToggleTalent()
    {
        if (currentTalentInstance == null) return;
        currentTalentInstance.isUnlocked = !currentTalentInstance.isUnlocked;

        //Reset attr
        addedCooldownReduction = 0;
        addedManaCostReduction = 0;

        foreach (var skillInstance in skillInstances)
        {
            skillInstance.bonusCooldownReduction = 0;
            skillInstance.bonusManacostReduction = 0;
        }

        foreach (var talentInstance in talentInstances)
        {
            if (!talentInstance.isUnlocked) continue;
            var findTalent = allTalents.Find(x => x.id == talentInstance.talentID);
            if(findTalent)
            {
                foreach (var modifier in findTalent.talents)
                {
                    float bonusCD = 0;
                    float bonusMana = 0;
                    if (modifier is StatsTalent)
                    {
                        var statTalent = (StatsTalent)modifier;
                        foreach (var statAdded in statTalent.statsAdded)
                        {
                            switch (statAdded.stats)
                            {
                                case Stats.CDReduction:
                                    addedCooldownReduction += statAdded.GetValue(0);
                                    break;

                                case Stats.ManaCostReduction:
                                    addedManaCostReduction += statAdded.GetValue(0);
                                    break;
                            }
                        }
                    }

                    else if(modifier is DefaultValueTalent)
                    {
                        var defaultTalent = (DefaultValueTalent)modifier;
                        foreach (var stat in defaultTalent.modifiedAttributes)
                        {
                            switch(stat.attributeType)
                            {
                                case DefaultAttributeType.Cooldown:
                                    bonusCD += stat.GetValue(0);
                                    break;

                                case DefaultAttributeType.ManaCost:
                                    bonusMana += stat.GetValue(0);
                                    break;
                            }
                        }
                    }

                    foreach (var skill in findTalent.affectedSkill)
                    {
                        var find = skillInstances.Find(x => x.skillID == skill.id);
                        if (find != null)
                        {
                            find.bonusManacostReduction += bonusMana;
                            find.bonusCooldownReduction += bonusCD;
                        }
                        else
                        {
                            skillInstances.Add(new SkillInstance()
                            {
                                skillID = skillData.id,
                                bonusCooldownReduction = bonusCD,
                                bonusManacostReduction = bonusMana
                            });
                        }
                    }
                }
            }
        }

        locked.SetActive(!currentTalentInstance.isUnlocked);
        UpdateLevelSkill();
    }

    public void UpgradeSkill()
    {
        if (currentSkillInstance == null) return;
        if (currentSkillInstance.currentLevel < skillData.maxLevel - 1) currentSkillInstance.currentLevel++;
        UpdateLevelSkill();
    }

    public void DowngradeSkill()
    {
        if (currentSkillInstance == null) return;
        if (currentSkillInstance.currentLevel > 0) currentSkillInstance.currentLevel--;
        UpdateLevelSkill();
    }

    public void UseSkill()
    {
        //Can use skill ? 
        int canUse = CanUseSkill(skillData);
        if (canUse != 200)
        {
            if (canUse == 300)
                if (HUDManager.Instance) HUDManager.Instance.PopDamage(transform, "Not enough mana..", Color.white, 1f);

            if (canUse == 301)
                if (HUDManager.Instance) HUDManager.Instance.PopDamage(transform, "Skill is on cooldown..", Color.white, 1f);

            AudioSource.PlayClipAtPoint(skillFailedClip, transform.position);
            return;
        }

        List<CustomEvent> allEvents = new List<CustomEvent>();
        Dictionary<string, float> addedValue = new Dictionary<string, float>();
        foreach (var talent in talentInstances)
        {
            if (!talent.isUnlocked) continue;
            if (selectedTalentData.affectedSkill.FindIndex(x => x.id == skillData.id) == -1) continue;

            var findData = allTalents.Find(x => x.id == talent.talentID);
            if(findData)
            {
                foreach (var modifier in findData.talents)
                {
                    if (modifier is EventModifier)
                    {
                        allEvents.AddRange(((EventModifier)modifier).AddedEvent);
                    }

                    else if(modifier is SpecialValueTalent)
                    {
                        var spValue = (SpecialValueTalent)modifier;
                        foreach (var specialAttr in spValue.specialAttributes)
                        {
                            addedValue.Add(specialAttr.key, specialAttr.GetValue(0));
                        }
                    }
                }
            }
        }



        switch (skillData.targeting)
        {
            case Targeting.NearestUnitTarget:
                skillData.UseSkill(new EventRequest() { caster = transform, 
                    target = GetClosestEnemy(lookRadius),
                    events = allEvents, 
                    valueCollection = addedValue }, currentSkillInstance.currentLevel);
                break;
            case Targeting.RandomUnitInArea:
                skillData.UseSkill(new EventRequest() { caster = transform, 
                    target = GetRandomEnemy(lookRadius),
                    events = allEvents,
                    valueCollection = addedValue
                }, currentSkillInstance.currentLevel);
                break;
            case Targeting.AroundCaster:
                skillData.UseSkill(new EventRequest()
                {
                    caster = transform,
                    target = GetClosestEnemy(lookRadius),
                    targets = GetEnemiesAround(lookRadius),
                    events = allEvents,
                    valueCollection = addedValue
                }, currentSkillInstance.currentLevel);
                break;
            default:
                skillData.UseSkill(new EventRequest() { caster = transform, 
                    target = transform,
                    events = allEvents,
                    valueCollection = addedValue
                }, currentSkillInstance.currentLevel);
                break;
        }

        var manaCost = skillData.GetManaCost(currentSkillInstance.currentLevel) - currentSkillInstance.bonusManacostReduction;
        var manaCostReduce = manaCost * ManaCostReduction / 100f;

        manaCost = manaCost - manaCostReduce;
        SetCurrentMana(currentMana - manaCost);

        var skill = skillInstances.Find(x => x.skillID == skillData.id);
        if(skill != null)
        {
            skill.lastTimeUsed = Time.time;
            var cd = skillData.GetCooldown(currentSkillInstance.currentLevel) - currentSkillInstance.bonusCooldownReduction;
            var cdReduce = cd * CooldownReduction / 100f;
            skill.readyTime = Time.time + (cd - cdReduce);
        }

        else
        {
            var cd = skillData.GetCooldown(currentSkillInstance.currentLevel) - currentSkillInstance.bonusCooldownReduction;
            var cdReduce = cd * CooldownReduction / 100f;
            skillInstances.Add(new SkillInstance()
            {
                skillID = skillData.id,
                lastTimeUsed = Time.time,
                readyTime = Time.time + (cd - cdReduce)
            });
        }
    }

    public int CanUseSkill(SkillData skillData)
    {
        var manaCost = skillData.GetManaCost(currentSkillInstance.currentLevel) - currentSkillInstance.bonusManacostReduction;
        var manaCostReduce = manaCost * ManaCostReduction / 100f;

        manaCost = manaCost - manaCostReduce;
        if (currentMana - manaCost < 0) return 300; //No mana

        var skill = skillInstances.Find(x => x.skillID == skillData.id);
        if(skill != null)
        {
            if (Time.time > skill.readyTime) return 200;
            else return 301;
        }

        return 200;
    }
    public Transform GetClosestEnemy(float lookRadius)
    {
        Transform bestTargetEnemy = null;
        float closestDistanceEnemy = Mathf.Infinity;

        var victims = Physics2D.OverlapCircleAll(transform.position, lookRadius, layer);
        foreach (var c in victims)
        {
            float range = (c.transform.position - transform.position).magnitude;
            if (collidedTags.Contains(c.tag))
            {
                var enemyHealth = c.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    if (range < closestDistanceEnemy)
                    {
                        closestDistanceEnemy = range;
                        bestTargetEnemy = c.transform;
                    }
                }
            }
        }

        return bestTargetEnemy;
    }
    public List<Transform> GetEnemiesAround(float lookRadius)
    {
        List<Transform> enemiesAround = new List<Transform>();

        var victims = Physics2D.OverlapCircleAll(transform.position, lookRadius, layer);
        foreach (var c in victims)
        {
            if (collidedTags.Contains(c.tag))
            {
                var enemyHealth = c.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemiesAround.Add(enemyHealth.transform);
                }
            }
        }

        return enemiesAround;
    }
    public Transform GetRandomEnemy(float lookRadius)
    {
        var victims = Physics2D.OverlapCircleAll(transform.position, lookRadius, layer).ToList();
        victims = victims.FindAll(x => x.GetComponent<EnemyHealth>() != null);
        return victims.Count > 0 ? victims[Random.Range(0, victims.Count)].transform : null;
    }
}

[System.Serializable]
public class SkillInstance
{
    public string skillID;
    public float lastTimeUsed;
    public float readyTime;
    public int currentLevel;

    public float bonusCooldownReduction;
    public float bonusManacostReduction;

    public float GetPercentage(float currentTime)
    {
        return (currentTime - lastTimeUsed) / (readyTime - lastTimeUsed);
    }

    public float GetSecond(float currentTime)
    {
        return readyTime - currentTime;
    }
}

[System.Serializable]
public class TalentInstance
{
    public string talentID;
    public bool isUnlocked;
}