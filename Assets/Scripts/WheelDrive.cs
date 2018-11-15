using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public enum DriveType
{
    RearWheelDrive,
    FrontWheelDrive,
    AllWheelDrive
}

public class WheelDrive : MonoBehaviour
{
    [Tooltip("Maximum steering angle of the wheels")]
    public float maxAngle = 30f;
    [Tooltip("Maximum torque applied to the driving wheels")]
    public float maxTorque = 300f;
    [Tooltip("Maximum brake torque applied to the driving wheels")]
    public float brakeTorque = 30000f;
    [Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
    public GameObject wheelShape;

    [Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
    public float criticalSpeed = 5f;
    [Tooltip("Simulation sub-steps when the speed is above critical.")]
    public int stepsBelow = 5;
    [Tooltip("Simulation sub-steps when the speed is below critical.")]
    public int stepsAbove = 1;

    [Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
    public DriveType driveType;

    private WheelCollider[] m_Wheels;

    public Slider boosterSlider;
    private Rigidbody carRigidbody;
    private List<GameObject> boosters = new List<GameObject>();
    public float boostCooldown = 5f;
    public float boostDuration = 2f;
    public float speedBoost = 3;
    private bool hasCooldown;

    // Find all the WheelColliders down in the hierarchy.
    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        StartCoroutine(ActivateCooldown());
        m_Wheels = GetComponentsInChildren<WheelCollider>();

        for (int i = 0; i < m_Wheels.Length; ++i)
        {
            var wheel = m_Wheels[i];

            // Create wheel shapes only when needed.
            if (wheelShape != null)
            {
                var ws = Instantiate(wheelShape);
                ws.transform.parent = wheel.transform;
            }
        }

        // Find all of the cars booster game objects and initialize them.
        foreach(Transform child in transform) 
        {
            if (child.tag == "Booster")
            {
                boosters.Add(child.gameObject);
            }
        }

    }

    // This is a really simple approach to updating wheels.
    // We simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero.
    // This helps us to figure our which wheels are front ones and which are rear.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !hasCooldown)
        {
            StartCoroutine(StartBoost());
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            carRigidbody.AddRelativeForce(transform.up * speedBoost * .5f, ForceMode.Impulse);
        }

        m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

        float angle = maxAngle * Input.GetAxis("Horizontal");
        float torque = maxTorque * Input.GetAxis("Vertical");

        float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;

        foreach (WheelCollider wheel in m_Wheels)
        {
            // A simple car where front wheels steer while rear ones drive.
            if (wheel.transform.localPosition.z > 0)
                wheel.steerAngle = angle;

            if (wheel.transform.localPosition.z < 0)
            {
                wheel.brakeTorque = handBrake;
            }

            if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive)
            {
                wheel.motorTorque = torque;
            }

            if (wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive)
            {
                wheel.motorTorque = torque;
            }

            // Update visual wheels if any.
            if (wheelShape)
            {
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose(out p, out q);

                // Assume that the only child of the wheelcollider is the wheel shape.
                Transform shapeTransform = wheel.transform.GetChild(0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }
        }
    }


    IEnumerator StartBoost()
    {
        Debug.Log("Boost activated.");
        // Activate the booster particle effects and log that the boost has been initilized.
        foreach (GameObject booster in this.boosters)
        {
            booster.SetActive(true);
        }
        carRigidbody.AddRelativeForce(Vector3.forward * speedBoost, ForceMode.Impulse);
        StartCoroutine(ActivateCooldown());
        // wait some seconds
        yield return new WaitForSeconds(boostDuration);
        // Deactivate the boosters.
        foreach (GameObject booster in this.boosters)
        {
            booster.SetActive(false);
        }
        // return to normal speed
        Debug.Log("Boost ended.");
    }

    IEnumerator ActivateCooldown()
    {
        // put some code to disable the boost-is-ready bar
        // diable the ability to use boost
        hasCooldown = true;
        // wait until the boost is ready again
        yield return new WaitForSeconds(boostCooldown);
        // make the boost usable
        hasCooldown = false;
        Debug.Log("Boost available.");
        // put some code to enable the boost-is-ready bar
    }

}
