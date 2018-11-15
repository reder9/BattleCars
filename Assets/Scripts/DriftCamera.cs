using System;
using UnityEngine;

public class DriftCamera : MonoBehaviour
{
    [Serializable]
    public class AdvancedOptions
    {
        public bool updateCameraInUpdate;
        public bool updateCameraInFixedUpdate = true;
        public bool updateCameraInLateUpdate;
        public KeyCode switchViewKey = KeyCode.Space;
    }

    public float smoothing = 6f;
    public Transform lookAtTarget;
    public Transform positionTarget;
    public Transform sideView;
    public AdvancedOptions advancedOptions;

    bool m_ShowingSideView = false;

    private void FixedUpdate ()
    {
        if(advancedOptions.updateCameraInFixedUpdate)
            UpdateCamera ();
    }

    private void Update ()
    {
        if (Input.GetKeyDown (advancedOptions.switchViewKey))
            m_ShowingSideView = !m_ShowingSideView;

        if(advancedOptions.updateCameraInUpdate)
            UpdateCamera ();
        //float mouseX = Input.GetAxis("Mouse X");
        //float mouseY = -Input.GetAxis("Mouse Y");

        //rotY += mouseX * mouseSensitivity * Time.deltaTime;
        //rotX += mouseY * mouseSensitivity * Time.deltaTime;

        //rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        //rotY = Mathf.Clamp(rotY, -clampAngle, clampAngle);

        //Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        //transform.rotation = localRotation;
    }

    private void LateUpdate ()
    {
        if(advancedOptions.updateCameraInLateUpdate)
            UpdateCamera ();
    }

    private void UpdateCamera ()
    {
        if (m_ShowingSideView)
        {
            transform.position = sideView.position;
            transform.rotation = sideView.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, positionTarget.position, Time.deltaTime * smoothing);
            transform.LookAt(lookAtTarget);
        }
    }
}
