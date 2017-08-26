using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public static int maxFood = 10;
    public static int minFood = 2;
    public static int maxCloth = 4;
    public static int minCloth = 1;
    public static int maxLux = 3;
    public static int minLux = 0;
    public static int maxGold = 20;
    public static int minGold = 5;

    public int foodProduction;
    public int clothingProduction;
    public int luxProduction;
    public int goldProduction;

	// Use this for initialization
	void Start () {
        foodProduction = Random.Range(minFood, maxFood);
        clothingProduction = Random.Range(minCloth, maxCloth);
        luxProduction = Random.Range(minLux, maxLux);
        goldProduction = Random.Range(minGold, maxGold);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
