using UnityEngine;
//Automaticly add rigidbody to the gameobject
[RequireComponent(typeof(Rigidbody))]
public class BasicRBController : MonoBehaviour
{
    public float movementSpeed = 6;
    public float rotationSpeed = 6;
    public float jumpForce = 6;
    public bool canJump = false;
    private Rigidbody rb;
    private Vector2 input;
    private Vector3 movementVector;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(canJump)
        {
            if (Input.GetButtonDown("Jump") && IsGrounded())
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (input == Vector2.zero) return;

        Vector3 movement = new Vector3(input.x, 0.0f, input.y).normalized;
        rb.velocity = movement * Time.fixedDeltaTime * movementSpeed;
        
        Rotating();
    }

    private void Rotating()
    {
        Vector3 axisDirection = new Vector3(input.x, 0, input.y);
        Vector3 targetDirection = Camera.main.transform.TransformDirection(axisDirection);
        targetDirection.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(targetDirection);
        Vector3 rot;
        rot = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime).eulerAngles;
        transform.eulerAngles = rot;
    }

    public void LookAt(Transform target)
    {
        if (target)
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }

    private bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.5f))
            return true;
        else
            return false;
    }
}