using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;


[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrackedImages : MonoBehaviour
{
    //Reference ti AR tracked image manager component
    private ARTrackedImageManager trackedImageManager;

    //List of prefabs to instantiate
    //Named based on corresponding 2D images in reference library
    public GameObject[] ArPrefabs;

    //blic Canvas canvas;

    public TMP_Text _text;

    //Dictionary array of created prefabs
    private readonly Dictionary<string, GameObject> instantiatedPrefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        //Cache a reference to the ARTrackedImageManager component
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        //Attach event handler when tracked image change
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        //Remove event handler
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void Update()
    {
        if (instantiatedPrefabs.Count == 3)
        {
            _text.gameObject.SetActive(true);
        }
    }

    //Event Handler
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //Loop through all new tracked images that have been detected
        foreach (var trackedImage in eventArgs.added)
        {
            //Get the name of the reference image
            var imageName = trackedImage.referenceImage.name;
            //Now loop over the array of prefabs
            foreach (var curPrefab in ArPrefabs)
            {
                //Check whether this prefab matches the tracked image name, 
                //and that the prefab hasn't already been created
                if (string.Compare(curPrefab.name, imageName, System.StringComparison.OrdinalIgnoreCase) == 0
                    && !instantiatedPrefabs.ContainsKey(imageName))
                {
                    //Instantiate the prefab, parenting it to the ARTrackedImage
                    var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    //Add the created prefab to our array
                    instantiatedPrefabs[imageName] = newPrefab;
                }
            }
        }
        //For all prefabs that have been created so far, set them active or not depending
        //on whether their corresponding image is currently being tracked
        foreach (var trackedImage in eventArgs.updated)
        {
            instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);

        }

        ////If AR subsytem has given up looking for a tracked image
        foreach (var trackedImage in eventArgs.removed)
        {
            //Destroy its prefab
            Destroy(instantiatedPrefabs[trackedImage.referenceImage.name]);
            //Also remove the instance from array
            //instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
        }

        
    }


}

