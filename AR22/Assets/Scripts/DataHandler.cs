using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class DataHandler : MonoBehaviour {
  public GameObject artifact;
  private static DataHandler instance; 
  // Use this method anywhere to call properties from the data handler. 
  public static DataHandler Instance {
    get {
      // if this object doesn't exist, create one. 
      if(instance == null){
        instance = FindObjectOfType<DataHandler>();
      }
      // if it does exist, return it. 
      return instance;
    }
  }
}