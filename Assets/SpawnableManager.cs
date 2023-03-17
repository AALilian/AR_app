using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour
{
    // spawn stuff
    [SerializeField]
    ARRaycastManager raycastManager;

    [SerializeField]
    GameObject spawnablePref;
    //List<GameObject> spawnables;

    Camera cam;
    GameObject spawnedObj;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // scale object
    [SerializeField]
    float scaling = 0.01f;
    Touch touch0;
    Touch touch1;

    // change color
    [SerializeField]
    Material[] materials;

    [SerializeField]
    GameObject panel;

    [SerializeField]
    GameObject startText;

    int totalOfObjects;


    // Start is called before the first frame update
    void Start()
    {
        spawnedObj = null;
        cam = GameObject.Find("AR Camera").GetComponent<Camera>();

        panel.SetActive(false);

        totalOfObjects = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0) return;

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);

        if (raycastManager.Raycast(Input.GetTouch(0).position, hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObj == null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObj = hit.collider.gameObject;
                        spawnedObj.GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length)];
                    }
                    else
                    {
                        SpawnPref(hits[0].pose.position);
                    }
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObj != null)
            {
                
                if (Input.touchCount == 2)
                {
                    // scale object using two fingers

                    touch0 = Input.GetTouch(0);
                    touch1 = Input.GetTouch(1);

                    Vector2 touch0_pos = touch0.position - touch0.deltaPosition;
                    Vector2 touch1_pos = touch1.position - touch1.deltaPosition;

                    float touchMagnitude = (touch0_pos - touch1_pos).magnitude;
                    float new_touchMagnitude = (touch0.position - touch1.position).magnitude;

                    float magnitude_difference = touchMagnitude - new_touchMagnitude;

                    Vector3 scale = spawnedObj.transform.localScale;
                    scale += new Vector3(magnitude_difference, magnitude_difference, magnitude_difference) * scaling;
                    
                    spawnedObj.transform.localScale = scale;    
                }
                else if (Input.touchCount == 1)
                {
                    // rotate object 
                    //Vector2 touchPos = Input.GetTouch(0).deltaPosition;
                    //spawnedObj.transform.Rotate(Vector3.up, -touchPos.x * 0.5f);
                }
                //else
                //{
                //    spawnedObj.transform.position = hits[0].pose.position;
                //}
            }

            if(Input.touchCount == 1 && spawnedObj != null)
            {
                Touch touch = Input.GetTouch(0);

                if (raycastManager.Raycast(touch.position, hits))
                {
                    Vector3 touch_pos = hits[0].pose.position;
                    spawnedObj.transform.position = touch_pos;
                }
            }


            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                spawnedObj = null;
            }
        }


        // check the amount of objects 
        if(totalOfObjects == 50)
        {
            panel.SetActive(true);
            startText.SetActive(false);
        }

    }

    void SpawnPref(Vector3 spawnPosition)
    {
        //int random = Random.Range(0, spawnables.Count);
        //GameObject prefToSpawn = spawnables[random];
        //spawnedObj = Instantiate(prefToSpawn, spawnPosition, Quaternion.identity);

        spawnedObj = Instantiate(spawnablePref, spawnPosition, Quaternion.identity);
        totalOfObjects++;
    }

}
