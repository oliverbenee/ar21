using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class XRMaterialrecognition : MonoBehaviour
{
    private ARTrackedImageManager imageManager;

    // Save a copy of the materials for the vases. Necessary for grabbing the material upon recogintion.
    [SerializeField]
    private Material amphora, hydria, voluteKrater, mate;
    // Used for feedback. 
    [SerializeField]
    private GameObject textField;

    // The object on the image
    private GameObject go;
    
    void Start(){
        Debug.Log("-----------------------------------------");
        Debug.Log("XR Material Recognition script started!!!");
        Debug.Log("-----------------------------------------");
    }

    // Start is called before the first frame update
    void Awake(){
        imageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    //Subscribe to image tracking
    void OnEnable(){imageManager.trackedImagesChanged += OnImageChanged;}

    //Unsubscribe from image tracking
    void OnDisable(){imageManager.trackedImagesChanged -= OnImageChanged;}
    
    void OnImageChanged(ARTrackedImagesChangedEventArgs args){
        Debug.Log("FOUND IMAGE LOLOLOL");
        foreach (var trackedImage in args.updated){
            // Tell us which image, we are looking at, and where it is. 
            // Debug.Log("Still found: " + trackedImage.name + " at location: " + trackedImage.transform);
            Vector3 imageLocation = trackedImage.transform.position;
            string imageName = trackedImage.referenceImage.name;

            // set correct material. 
            //Debug.Log("Name: " + trackedImage.name);
            //Debug.Log("Reference image: " + trackedImage.referenceImage); // Useless. Just prints guid. 
            Debug.Log("Reference image name: " + imageName);

            if(trackedImage.referenceImage.name.Contains("Amphora_Albedo")){
                Debug.Log("MATE IS AMPHORA");
                mate = amphora;
            }
            if(trackedImage.referenceImage.name.Contains("Hydria_Albedo")){
                Debug.Log("MATE IS HYDRIA");
                mate = hydria;
            }
            if(trackedImage.referenceImage.name.Contains("VoluteKrater_Albedo")){
                Debug.Log("MATE IS VOLUTE KRATER");
                mate = voluteKrater;
            }

            // Find close gameobjects and set their materials. 
            var closetsobjects = Physics.OverlapSphere(imageLocation, 10);
            foreach (var item in closetsobjects){
                bool isPlaneOrCam = !item.transform.gameObject.name.Contains("ARPlane") && !item.transform.gameObject.name.Contains("AR Camera");
                if(!item.transform.gameObject.name.Contains("ARPlane")){
                    Debug.Log("Found gameobject: " + item.transform.gameObject.name);
                    Debug.Log("Setting material to: " + mate.name);

                    // These lines of code are used to double-check the size of the gameobject. 
                    var renderer = item.GetComponentInChildren<MeshRenderer>();
                    Debug.Log("LALALALALA RENDERER: " + renderer);
                    var size =  item.GetComponentsInChildren<MeshRenderer>()[0].bounds.size;
                    // Expect objects to be of size 1*1.5*1
                    Debug.Log("Gameobject's size is: (x: " +size.x + ", y: " + size.y + ", z: " + size.z);
                    
                    List<MeshRenderer> rendererList = new List<MeshRenderer>();
                    foreach (MeshRenderer objectRenderer in item.transform.gameObject.GetComponentsInChildren<MeshRenderer>()) {
                        if(objectRenderer.name != "ARPlane"){
                            Debug.Log("Manual log: DADADADADADAD: " + objectRenderer.transform.parent.gameObject.name);
                            objectRenderer.material = mate;
                            objectRenderer.sharedMaterial = mate;
                            objectRenderer.materials = new Material[1] {mate};
                            objectRenderer.sharedMaterials = new Material[1] {mate};
                        } else {
                            Debug.Log("Manual log: Error: Tried to change the color of the plane.");
                        }
                    }
                }
            }
                
            /**
            // Now figure out if there is a prefab on top. 
            var objectsWithTag = GameObject.FindGameObjectsWithTag("selectable");
            var closestObject = new GameObject();
            foreach (GameObject obj in objectsWithTag){
                Debug.Log("OBJECT: " + obj.name);
                // if the closest object does not exist, set it. 
                if(!closestObject){
                    closestObject = obj;
                }
                //compares distances
                if(Vector3.Distance(transform.position, obj.transform.position) <= Vector3.Distance(transform.position, closestObject.transform.position)){
                 closestObject = obj;
                 Debug.Log("Closest object: " + closestObject.name);
                }
            }
            */
        }
        foreach (var trackedImage in args.removed){
            Debug.Log("Can no longer find " + trackedImage.referenceImage.name);
        }
    }
}
