using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Brains : MonoBehaviour {

    public int action;
    public int fitness;

    private bool idle;
    public static int maxHunger = 1000;
    public static int foodValue = 200;

    public static int maxWarmth = 1000;
    public static int clothValue = 400;
    public static int luxValue = 100;
    public static int maxBirthCoolDown = 200;

    public static int maxAge = 6000; 

    NeuralNetwork myBrain;
    public int hunger;
    public int warmth;
    public int happy;
    public int gold;

    public int food;
    public int cloth;
    public int lux;
    public int workRemaining;
    public int age;

    public int birthCoolDown;

    public GameObject simPrefab;
   
	// Use this for initialization
	void Start () {
        //the starting net will have food and cloth inputs and make food & cloth outputs
        int[] layers = { 2, 3, 2 };
        myBrain = new NeuralNetwork(layers);
        idle = true;

        hunger = maxHunger / 2;
        warmth = maxWarmth / 2;
        happy = 0;
        gold = 0;
        birthCoolDown = 0;
        workRemaining = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
        //first use resources
        hunger--;
        warmth--;
        age++;
        if (happy > 0)
        {
            happy--;
        }
        if (birthCoolDown > 0)
        {
            birthCoolDown--;
        }

        //next consume if possible
        if (hunger < maxHunger - foodValue && food > 0)
        {
            food--;
            hunger += foodValue;
        }

        if (warmth < maxWarmth - clothValue && cloth > 0)
        {
            cloth--;
            warmth += clothValue;
        }

        if (lux > 0)
        {
            lux--;
            happy += luxValue;
        }

        

        //changes from last cycle are made.  Calculate fitness
        fitness = this.getFitness();

        if (idle)
        {
           
            //if idle we need to gather the information for the 
            //input layer of the NN then forward feed the vector
            //this will give us an action to preform
            GameObject map = GameObject.Find("Map");
            MakeMap mapScript = map.GetComponent<MakeMap>();
            Vector3 position = this.transform.position;
            GameObject currentLoc = mapScript.map[(int)position.x, (int)position.z];
            Tile currentTile = currentLoc.GetComponent<Tile>();
            float[] input = { hunger - maxHunger/2, warmth- maxWarmth/2, happy, gold, currentTile.foodProduction,
                currentTile.clothingProduction, currentTile.luxProduction,
               currentTile.goldProduction };
            

            float[] output = myBrain.FeedForward(input);

            action = 0;


            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] >= output[action])
                    action = i;
            }
            switch (action)
            {
                case 0:
                    makeFood(currentTile);
                    break;
                case 1:
                    makeCloth(currentTile);
                    break;
                case 3:
                    makeLux(currentTile);
                    break;
                case 4:
                    makeGold(currentTile);
                    break;
                case 5:
                    moveNorth();
                    break;
                case 6:
                    moveSouth();
                    break;
                case 7:
                    moveEast();
                    break;
                case 8:
                    moveWest();
                    break;
                default:
                    //do nothing;
                    break;
            }
        }
        else
        {
            workRemaining--;
            if (workRemaining <= 0)
            {
                idle = true;
                ThirdPersonCharacter mycontroler = GetComponent<ThirdPersonCharacter>();
                mycontroler.Move(new Vector3(0, 0, 0), false, false);
            }

        }
        		
	}


    //mutate the brain
    internal void mutate()
    {
        myBrain.Mutate();
    }

    internal void clone(Brains bestBrain)
    {
        if (bestBrain == null)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.LogError(trace.ToString());
        }
        if (myBrain == null)
        {
            int[] layers = { 8, 8, 8, 8 };
            myBrain = new NeuralNetwork(layers);
            idle = true;

            hunger = maxHunger / 2;
            warmth = maxWarmth / 2;
            happy = 0;
            gold = 0;

            workRemaining = 0;
        }
        myBrain.clone(bestBrain.myBrain);
    }

    /// <summary>
    /// the fitness function for this brain.  
    /// If not below half on hunger or warmth then fitness is 
    /// happy hunger and warmth.
    /// If below half on hunger or warmth, then fitness is the
    /// lesser of hunger or warmth
    /// You can get really big fitness scores because happy isn't capped
    /// but happy only counts if not freezing or starving.
    /// </summary>
    /// <returns>fitness score</returns>
    private int getFitness()
    {
        if (warmth > maxWarmth / 2 && hunger > maxHunger / 2)
        {
            return warmth + hunger + happy;
        }
        if (warmth > hunger)
        {
            return hunger + warmth / 2;
        }
        return warmth + hunger / 2;
    }

    private void moveWest()
    {
        if ((int)this.transform.position.z >= 1)
        {
            ThirdPersonCharacter mycontroler = GetComponent<ThirdPersonCharacter>();
            mycontroler.Move(new Vector3(-1, 0, 0), false, false);
            idle = false;
            workRemaining = 15;
        }
    }

    private void moveEast()
    {
        GameObject map = GameObject.Find("Map");
        MakeMap mapScript = map.GetComponent<MakeMap>();
        
        if ((int)this.transform.position.z < mapScript.map.Length) {
            ThirdPersonCharacter mycontroler = GetComponent<ThirdPersonCharacter>();
            mycontroler.Move(new Vector3(1, 0, 0), false, false);
            idle = false;
            workRemaining = 15;
        }
    }

    private void moveSouth()
    {
        if ((int)this.transform.position.x >= 1)
        {
            ThirdPersonCharacter mycontroler = GetComponent<ThirdPersonCharacter>();
            mycontroler.Move(new Vector3(0, 0, -1), false, false);
            idle = false;
            workRemaining = 15;
        }
    }

    private void moveNorth()
    {
        GameObject map = GameObject.Find("Map");
        MakeMap mapScript = map.GetComponent<MakeMap>();

        if ((int)this.transform.position.x < mapScript.map.Length)
        {
            ThirdPersonCharacter mycontroler = GetComponent<ThirdPersonCharacter>();
            mycontroler.Move(new Vector3(0, 0, 1), false, false);
            idle = false;
            workRemaining = 15;
        }
    }

    private void makeGold(Tile loc)
    {
        ThirdPersonCharacter mycontroler = GetComponent<ThirdPersonCharacter>();
        mycontroler.Move(new Vector3(0, 0, 0), true, false);
        this.gold += loc.goldProduction;
        idle = false;
        workRemaining = 60;
    }

    private void makeLux(Tile loc)
    {
        ThirdPersonCharacter mycontroler = GetComponent<ThirdPersonCharacter>();
        mycontroler.Move(new Vector3(0, 0, 0), true, false);
        this.lux += loc.luxProduction;
        idle = false;
        workRemaining = 60;
    }

    private void makeCloth(Tile loc)
    {
        ThirdPersonCharacter mycontroler = GetComponent<ThirdPersonCharacter>();
        mycontroler.Move(new Vector3(0, 0, 0), true, false);
        this.cloth += loc.clothingProduction;
        idle = false;
        workRemaining = 60;
    }

    private void makeFood(Tile loc)
    {
        ThirdPersonCharacter mycontroler = GetComponent<ThirdPersonCharacter>();
        mycontroler.Move(new Vector3(0, 0, 0), true, false);
        this.food += loc.foodProduction;
        idle = false;
        workRemaining = 60;
    }
}
