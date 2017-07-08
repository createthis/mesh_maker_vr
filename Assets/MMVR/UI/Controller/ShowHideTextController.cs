using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShowHideTextController : MonoBehaviour {
    public string prefix;
    public GameObject textTarget;
    public string activeTag;


    public void Changed() {
        GameObject activeTagObj = GameObject.FindWithTag(activeTag);
        bool isTrue = activeTagObj && activeTagObj.activeSelf;
        textTarget.GetComponent<TextMesh>().text = prefix + ":\n " + ((isTrue) ? "Hide" : "Show");
    }

	// Use this for initialization
	void Start () {
        Changed();
	}
	
	// Update is called once per frame
	void Update () {
        Changed();
    }
}
