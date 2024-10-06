using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.UI;

public class NeckPositioner : MonoBehaviour
{
    public Slider xSlider;
    public Slider ySlider;
    public Slider zSlider;

    private bool currentNecklace = false;

    private int ChinVertexIndex = 152;
    private int LeftVertexIndex = 58;
    private int RightVertexIndex = 288;

    public ARFaceManager faceManager;

    public GameObject necklacePrefab;
    public GameObject necklacePrefab2;

    private Transform faceTransform;
    private GameObject necklaceInstance;

    void OnEnable()
    {
        faceManager.facesChanged += OnFaceChanged;
    }

    void OnDisable()
    {
        faceManager.facesChanged -= OnFaceChanged;
    }

    public void SwitchNecklace()
    {     
        if (necklaceInstance != null)
        {
            Destroy(necklaceInstance);
        }

        if (currentNecklace)
            necklaceInstance = Instantiate(necklacePrefab, faceTransform);
        else
            necklaceInstance = Instantiate(necklacePrefab2, faceTransform);

        currentNecklace = !currentNecklace;
    }

    private void OnFaceChanged(ARFacesChangedEventArgs eventArgs)
    {
        foreach (var face in eventArgs.added)
        {
            if (necklaceInstance == null)
            {
                faceTransform = face.transform;
                necklaceInstance = Instantiate(necklacePrefab, faceTransform);
            }
            UpdateNecklacePositionAndScale(face);
        }

        foreach (var face in eventArgs.updated)
        {
            UpdateNecklacePositionAndScale(face);
        }

        foreach (var face in eventArgs.removed)
        {
            if (necklaceInstance != null)
            {
                Destroy(necklaceInstance);
            }
        }
    }

    private void UpdateNecklacePositionAndScale(ARFace face)
    {
        necklaceInstance.transform.position = face.vertices[ChinVertexIndex] + new Vector3(0, -2.5f, 20.0f) + new Vector3(xSlider.value, ySlider.value, zSlider.value);
        necklaceInstance.transform.rotation = Quaternion.Euler(-45, 0, 0);

        // Update scale dynamically based on face width (distance between cheeks)
        Vector3 leftCheek = face.vertices[LeftVertexIndex];
        Vector3 rightCheek = face.vertices[RightVertexIndex];

        // Calculate the face width
        float faceWidth = Vector3.Distance(leftCheek, rightCheek);

        // Adjust scale based on the face width (customize the scaling factor)
        float scaleFactor = faceWidth / 0.15f; // Assuming 0.15f is the average face width
        necklaceInstance.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
