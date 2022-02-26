using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{
    public GameObject artifact;
    // This is used to enable placement. 
    public bool isObjectNull = true;
    
    // Start is called before the first frame update
    void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(selectObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void selectObject(){
        DataHandler.Instance.artifact = artifact;
        Debug.Log("ARTIFACT: " + artifact.name);
        // This line disables the menu. 
        this.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(false);
        isObjectNull = false;
    }

    public bool getIsObjectNull(){
        return isObjectNull;
    }
}
