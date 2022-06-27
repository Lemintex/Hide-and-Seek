using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5;
    public float turnSpeed = 5;

    float angle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float magnitude = input.magnitude;
        float target = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg;

        angle = Mathf.LerpAngle(angle, target, turnSpeed * Time.deltaTime * magnitude);
        transform.eulerAngles = Vector3.up * angle;
        transform.Translate(magnitude * transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }
}
