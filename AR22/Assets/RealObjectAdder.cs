using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class RealObjectAdder : MonoBehaviour
{
    [SerializeField]
    private GameObject cursorChildObject;
    [SerializeField]
    private GameObject ObjectToPlace;
    [SerializeField]
    private ARRaycastManager raycastManager;
    [SerializeField]
    private bool useCursor = true;
    // Start is called before the first frame update
    void Start()
    {
        cursorChildObject.SetActive(useCursor);
    }

    // Update is called once per frame
    void Update()
    {
        if(useCursor){UpdateCursor();}
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
            Vector3 position = transform.position; 
            Vector3 position2 = new Vector3(position.x, position.y, position.z+0.1f);
            GameObject.Instantiate(ObjectToPlace, position2, transform.rotation);
        } 
    }

    void UpdateCursor(){
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
        if(hits.Count > 0){
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
            cursorChildObject.SetActive(true);
         } else {
             cursorChildObject.SetActive(false);
         }
    }
}
