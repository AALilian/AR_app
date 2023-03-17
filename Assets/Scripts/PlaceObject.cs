using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

// ei anna poistaa näit objektei ja luo ne jossei niit oo yhistettynä kait ..
[RequireComponent(requiredComponent:typeof(ARRaycastManager), requiredComponent2: typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    // ref to prefab to-be-spawn
    [SerializeField]
    private GameObject objPrefab;

    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;

    public GameObject placement;
    private GameObject spawned;
    private List<GameObject> SpawnedObjects = new List<GameObject>();
    private Pose pose;
    private bool validPlacement = false;
    private float distance;
    private Vector3 scale;

    private int i = 0;

    private List<ARRaycastHit> hits = new List<ARRaycastHit> ();

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
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

    private void Update()
    {
        if (spawned == null && validPlacement && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            setObject();
            //EnhancedTouch.Touch.onFingerDown += FingerDown;
        }

        if (Input.touchCount == 2)
        {
            // skaalaus tapahtuu kahdella sormella: 
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
            {
                return;
            }

            if (touch0.phase == TouchPhase.Began && touch1.phase == TouchPhase.Began)
            {
                distance = Vector2.Distance(touch0.position, touch1.position);
                scale = spawned.transform.localScale;
            }
            else
            {
                var currentDistance = Vector2.Distance(touch0.position, touch1.position);

                var updateScale = currentDistance / distance;

                spawned.transform.localScale = scale * updateScale;
            }

        }

        UpdatePose();
        UpdatePlacementIndicator();
        
    }

    void UpdatePose()
    {
        var centre = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        aRRaycastManager.Raycast(centre, hits, TrackableType.PlaneWithinPolygon);

        validPlacement = hits.Count > 0;   
        if (validPlacement)
        {
            pose = hits[0].pose;
        }

    }

    void UpdatePlacementIndicator()
    {
        if (spawned != null && validPlacement)
        {
            placement.SetActive(true);
            placement.transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
        else
        {
            placement.SetActive(false);
        }
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        // varmistetaan, että ruutua painaa vain yksi sormi:
        if (finger.index != 0) return;

        if (aRRaycastManager.Raycast(finger.currentTouch.startScreenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach(ARRaycastHit hit in hits)
            {
                pose = hit.pose;

                GameObject obj = Instantiate(objPrefab, pose.position, pose.rotation);
            }

        }

    }

    private void setObject()
    {
        
        if (i < 4)
        {
            SpawnedObjects[i] = Instantiate(objPrefab, pose.position, pose.rotation);
            i++;
        }
        
        
    }
}
