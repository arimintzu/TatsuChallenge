using UnityEngine;
//Automaticly add rigidbody to the gameobject
[RequireComponent(typeof(CharacterController))]
public class BasicCharacterController : MonoBehaviour
{
    public float movementSpeed = 6;
    public float rotationSpeed = 6;
    private CharacterController _controller;
    private Vector2 input;
    private Vector3 movementVector;
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        _controller.Move(Vector2.down * movementSpeed * Time.deltaTime);

        if (input == Vector2.zero) return;
        _controller.Move(transform.forward * movementSpeed * Time.deltaTime);
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