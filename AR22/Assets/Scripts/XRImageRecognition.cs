using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class XRImageRecognition : MonoBehaviour
{
    private ARTrackedImageManager imageManager;
    [SerializeField]

    // The prefab we spawn
    private GameObject prefab;

    // tracks all prefabs
    Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    
    void Start(){
        Debug.Log("HELLO BISH HOW YA DOIN");
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
        foreach (var trackedImage in args.added)
        {
            // Tell us which image, we are looking at. 
            Debug.Log("Name: " + trackedImage.name);
            Instantiate(prefab, trackedImage.transform);
        }
        foreach (var trackedImage in args.updated)
        {
            UpdateImage(trackedImage);
        }
        foreach (var trackedImage in args.removed)
        {
            spawnedPrefabs[trackedImage.name].SetActive(false); 
        }
    }

    void UpdateImage(ARTrackedImage trackedImage){
        string name = trackedImage.referenceImage.name;
        Vector3 position = trackedImage.transform.position;

        GameObject prefab = spawnedPrefabs[name];
        prefab.transform.position = position;
        prefab.SetActive(true);

        foreach(GameObject go in spawnedPrefabs.Values){
            if(go.name != name){go.SetActive(false);}
        }
    }
}
