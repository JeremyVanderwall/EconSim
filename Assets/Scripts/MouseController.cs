using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {
    Ray mousePointer;
    RaycastHit target;
    GameObject textBox;
	// Use this for initialization
	void Start () {
        
        textBox = GameObject.Find("HoverInfo");
		
	}
	
	// Update is called once per frame
	void Update () {
        mousePointer = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(mousePointer, out target, float.MaxValue))
        {
            TextMesh myText = textBox.GetComponent<TextMesh>();
            myText.text = "distance = " + target.distance;
            textBox.transform.position = target.transform.position;
            print("distance " + target.distance);
            try
            {

                Collider col = target.collider;
                GameObject per = col.gameObject;
                Brains brain = per.GetComponent<Brains>();
                textBox.transform.position = per.transform.position;
                myText.text = "Fitness = " + brain.fitness + "/n Food = " + brain.food;
            }
            catch (NullReferenceException)
            {
                Debug.Log("no person hit");
                //not over a person, don't worry about it
            }
        }
		
	}
}
