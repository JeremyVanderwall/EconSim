using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeMap : MonoBehaviour {

    public int size = 20;
    public static int minPop = 50;

    public GameObject[,] map;
    public List<GameObject> sims;

    public GameObject tilePrefab;
    public GameObject cubePrefab;
    public GameObject simPrefab;
	// Use this for initialization
	void Start () {
        sims = new List<GameObject>();
        map = new GameObject[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i == 0 || i == size - 1 || j == 0 || j == size -1)
                {
                    map[i, j] = Instantiate(cubePrefab, new Vector3(i, 0, j), Quaternion.identity);

                }
                else
                {
                    map[i, j] = Instantiate(tilePrefab, new Vector3(i, 0, j), Quaternion.identity);
                    sims.Add(Instantiate(simPrefab, new Vector3(i, 1, j), Quaternion.identity));
                }
           }
        }
	}
	
	// Update is called once per frame
	void Update () {
       
        GameObject bestSim = sims[0];
        Brains bestBrain = sims[0].GetComponent<Brains>();
        foreach (GameObject s in sims.ToArray())
        {
            if (s.GetComponent<Brains>().age > Brains.maxAge)
            {
                //got old and died
                sims.Remove(s);
                Destroy(s);
                
            }

            else if (s.GetComponent<Brains>().fitness > bestBrain.fitness)
            {
                bestSim = s;
                bestBrain = s.GetComponent<Brains>();
            }

            
            
        }
        
        foreach (GameObject s in sims.ToArray())
        {
            if (s.GetComponent<Brains>().hunger < 0 || s.GetComponent<Brains>().warmth < 0)
            {
                if (sims.Count < minPop)
                {
                    s.GetComponent<Brains>().clone(bestBrain);
                    s.GetComponent<Brains>().mutate();
                    s.GetComponent<Brains>().hunger = Brains.maxHunger / 2;
                    s.GetComponent<Brains>().warmth = Brains.maxWarmth / 2;
                    s.GetComponent<Brains>().happy = 0;
                    s.GetComponent<Brains>().age = 0;
                }
                else
                {
                    sims.Remove(s);
                    Destroy(s);
                }
            }
            makeBaby(s);
        }
        if (sims.Count < minPop)
        {
            GameObject newSpawn =  Instantiate(simPrefab, new Vector3(Random.Range(2, size - 2), 2, Random.Range(2, size - 2)), Quaternion.identity);
            sims.Add(newSpawn);
            newSpawn.GetComponent<Brains>().clone(bestBrain);
            newSpawn.GetComponent<Brains>().mutate();
            newSpawn.GetComponent<Brains>().hunger = Brains.maxHunger / 2;
            newSpawn.GetComponent<Brains>().warmth = Brains.maxWarmth / 2;
            newSpawn.GetComponent<Brains>().happy = 0;
            newSpawn.GetComponent<Brains>().age = 0;

        }

    }

    private void makeBaby(GameObject s)
    {
        //try to make a baby!!

        if (s.GetComponent<Brains>().hunger > Brains.maxHunger - Brains.foodValue 
            && s.GetComponent<Brains>().warmth > Brains.maxWarmth - Brains.clothValue 
            && s.GetComponent<Brains>().age > 200  && s.GetComponent<Brains>().birthCoolDown <= 0)
        {
            //make baby!!
            GameObject baby = Instantiate(simPrefab, new Vector3(Random.Range(2,size -2), 2, Random.Range(2, size - 2)), Quaternion.identity);
            baby.GetComponent<Brains>().clone(s.GetComponent<Brains>());
            baby.GetComponent<Brains>().mutate();
            s.GetComponent<Brains>().food = s.GetComponent<Brains>().food / 2;
            s.GetComponent<Brains>().cloth = s.GetComponent<Brains>().cloth / 2;
            s.GetComponent<Brains>().birthCoolDown = Brains.maxBirthCoolDown;
            sims.Add(baby);
      
            
        }
    }
}
