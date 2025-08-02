using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlaneSpawnerAndScaler : MonoBehaviour
{
    public GameObject planePrefab; // Assign your plane prefab in the Inspector

    private InputDevice leftController;
    private InputDevice rightController;

    private GameObject spawnedPlane;
    private bool planeSpawned = false;

    private float initialControllerDistance;
    private Vector3 initialScale;

    void Start()
    {
        TryInitializeControllers();
    }

    void Update()
    {
        if (!leftController.isValid || !rightController.isValid)
        {
            TryInitializeControllers();
        }

        bool leftTriggerPressed = false;
        bool rightTriggerPressed = false;

        if (leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftTriggerPressed) &&
            rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTriggerPressed))
        {
            if (leftTriggerPressed && rightTriggerPressed && !planeSpawned)
            {
                SpawnPlane();
            }

            if (planeSpawned)
            {
                ScalePlane();
            }
        }
    }

    void SpawnPlane()
    {
        // Get midpoint between controllers
        if (leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPos) &&
            rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPos))
        {
            Vector3 spawnPosition = (leftPos + rightPos) / 2;
            spawnedPlane = Instantiate(planePrefab, spawnPosition, Quaternion.identity);
            initialControllerDistance = Vector3.Distance(leftPos, rightPos);
            initialScale = spawnedPlane.transform.localScale;
            planeSpawned = true;
        }
    }

    void ScalePlane()
    {
        if (spawnedPlane == null) return;

        if (leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPos) &&
            rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPos))
        {
            float currentDistance = Vector3.Distance(leftPos, rightPos);
            float scaleMultiplier = currentDistance / initialControllerDistance;
            spawnedPlane.transform.localScale = initialScale * scaleMultiplier;
        }
    }

    void TryInitializeControllers()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        if (devices.Count > 0) leftController = devices[0];

        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
        if (devices.Count > 0) rightController = devices[0];
    }
}
