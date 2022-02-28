using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialMenuManager : MonoBehaviour
{
    public Material material;
    // This is used to enable placement. 
    public bool isObjectNull = true;
    [SerializeField]
    
    // Start is called before the first frame update
    void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(selectMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void selectMaterial(){
        DataHandler.Instance.material = material;
        Debug.Log("MATERIAL: " + material.name);
        // This line disables the menu. 
        this.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(false);
        isObjectNull = false;
        GameObject.Find("AR Cursor").GetComponent<RealObjectAdder>().closeMenu();
    }

    public bool getIsObjectNull(){
        return isObjectNull;
    }
}
