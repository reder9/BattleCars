using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGun : MonoBehaviour {

    public Transform muzzlePosition;
    public GameObject projectile;
    public GameObject shotEffect;
    public KeyCode triggerButton;
    public  float fireRate;
    public int force;
    private float nextFire;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 45.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        rotY = Mathf.Clamp(rotY, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX + 45f, rotY, 0.0f);
        transform.rotation = localRotation;

        if (Input.GetKey(triggerButton) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject shell = Instantiate(projectile, muzzlePosition.position, transform.rotation);
            shell.GetComponent<Rigidbody>().AddRelativeForce(transform.forward * -force);
            //GetComponent<AudioSource>().Play();
        }
    }
}
