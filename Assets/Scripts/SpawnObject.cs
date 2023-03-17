using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class SpawnObject : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefs;

    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private GameObject selected; // valittu objekti, jonka skaalausta halutaan muuttaa
    private Vector3 initialScale;

    private bool validPlacement = false;
    private UnityEngine.Pose pose;

    [SerializeField]
    private GameObject placement;
    

    // Start is called before the first frame update
    void Start()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
        initialScale = Vector3.one; // (1, 1, 1)

    }

    private void OnEnable()
    {
        //EnhancedTouch.TouchSimulation.Enable();
        //EnhancedTouch.EnhancedTouchSupport.Enable();
        //EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        //EnhancedTouch.TouchSimulation.Disable();
        //EnhancedTouch.EnhancedTouchSupport.Disable();
        //EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // yksi sormi näytöllä ... 

                if (aRRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    selected = Instantiate(prefs[0], hits[0].pose.position, hits[0].pose.rotation);
                    initialScale = selected.transform.localScale;
                    spawnedObjects.Add(selected);
                }
                    
            }
            
        }
        

        if (Input.touchCount == 2)
        {
            // skaalaus tapahtuu kahdella sormella: 
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
            {
                float distance = Vector2.Distance(touch0.position, touch1.position);
                

                foreach (GameObject o in spawnedObjects)
                {
                    // skaalaus
                    o.transform.localScale = initialScale * distance / 200.0f;
                }
            }
        }

        // raycast hitit, spawnataan uusia objekteja: 

        //if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        //{
        //    if (aRRaycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
        //    {
        //        for (int i = 0; i < prefs.Length; i++)
        //        {
        //            GameObject o = Instantiate(prefs[i], hits[0].pose.position, hits[0].pose.rotation);
        //            spawnedObjects.Add(o);
                    
        //        }
        //    }
        //}

        UpdatePlacementIndicator();
    }

    void UpdatePlacementIndicator()
    {
        if (spawnedObjects != null && validPlacement)
        {
            placement.SetActive(true);
            placement.transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
        else
        {
            placement.SetActive(false);
        }
    }
}

