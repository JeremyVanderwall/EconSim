
using System.Collections.Generic;
using System;

public class NeuralNetwork{

    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;

    //the numbers for mutations, with range going from
    //1 to randMax
    public static int randMax = 1001;
    public static int weightMutate = 100;
    public static int inputMutate = 10;
    public static int outputMutate = 10;
    public static int neuronMutate = 10;
    public static int layerMutate = 1;

    /// <summary>
    /// Initialize the Neural net with random weights
    /// </summary>
    /// <param name="layers">the number of neruons in each layer</param>
	public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitWeights();
    }

    /// <summary>
    /// Initialize the neurons
    /// </summary>
    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>(); //a list to hold the arrays of neurons

        //step through each layer making an array with the correct number of neurons
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }


    /// <summary>
    /// Init random weights to synapse
    /// </summary>
    private void InitWeights()
    {
        // a list holding the jagged arrays of connection weights
        List<float[][]> weightsList = new List<float[][]>();

        //step through the layers
        for (int i = 1; i < layers.Length; i++)
        {
            int neuronsInPreviousLayer = layers[i - 1];
            List<float[]> layerWeightList = new List<float[]>();

            //step through the neurons in this layer
            for ( int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                //step through neurons of previous layer making random weight for each connection
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-1.0f, 1.0f);
                }
                layerWeightList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightList.ToArray());
        }
        weights = weightsList.ToArray();
    }


    /// <summary>
    /// Feed in input to get output
    /// </summary>
    /// <param name="inputs">input array, must be shorter than neurons of input layer</param>
    /// <returns>The weights for each output</returns>
    public float[] FeedForward (float[] inputs)
    {
        for (int i = 0; i < neurons[0].Length; i++)
        {
            neurons[0][i] = sigmoid(inputs[i]);
        }

        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0.25f;

                for (int k =0; k <neurons[i-1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                neurons[i][j] = sigmoid(value);
            }
        }
        return neurons[neurons.Length -1];

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    float sigmoid (float num)
    {
        return 1 / (1 + (float)Math.Exp(num));
    }

    /// <summary>
    /// mutate weights randomly
    /// with small chance to add input, output, neurons and hidden layers.
    /// </summary>
    public void Mutate()
    {
        //first start with chance network mutations
        if (UnityEngine.Random.Range(1, randMax) < inputMutate)
        {
            addInput();
        }
        if (UnityEngine.Random.Range(1, randMax) < outputMutate)
        {
            //addOutput();
        }
        if (UnityEngine.Random.Range(1, randMax) < neuronMutate)
        {
            addNeuron();
        }
        if (UnityEngine.Random.Range(1, randMax) < layerMutate)
        {
            addLayer();
        }

        //now mutate weights.  
        for(int i = 1; i < layers.Length; i++)
        {
            for(int j = 0; j < neurons[i].Length; j++)
            {
                for (int k = 0; k < neurons[i-1].Length; k++)
                {
                    //make 10% chance to mutate each synapse
                    if (UnityEngine.Random.Range(1, randMax) <= weightMutate)
                    {
                        weights[i - 1][j][k] += UnityEngine.Random.Range(-0.01f, 0.01f);
                        if (weights[i - 1][j][k] > 1)
                        {
                            weights[i - 1][j][k] += -1;
                        }
                        if (weights[i - 1][j][k] < -1)
                        {
                            weights[i - 1][j][k] += 1;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// adds a new layer to the neural net
    /// This layer is going to directly copy the previous layer
    /// and rely on mutations in the future to give the layer meaning
    /// </summary>
    private void addLayer()
    {
        int[] newLayers = new int[layers.Length + 1];
        //length of input layer
        newLayers[0] = layers[0];
        //length of output layer
        newLayers[newLayers.Length - 1] = layers[layers.Length - 1];
        //lengths if hidden layers
        for (int i = 1; i < newLayers.Length - 2; i++)
        {
            if (i == newLayers.Length - 2)
            {
                newLayers[i] = layers[i - 1];
            }
            else
            {
                newLayers[i] = layers[i];
            }
        }
        layers = newLayers;

        InitNeurons();

        // a list holding the jagged arrays of connection weights
        List<float[][]> weightsList = new List<float[][]>();

        //step through the layers
        for (int i = 1; i < layers.Length; i++)
        {
            int neuronsInPreviousLayer = layers[i - 1];
            List<float[]> layerWeightList = new List<float[]>();

            //step through the neurons in this layer
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                //if this is the new layer, we need to specifically define weights
                if (i == weights.Length - 2)
                {
                    //step through neurons of previous layer making random weight for each connection
                    for (int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        if (j == k)
                        {
                            //coppy straight accross
                            neuronWeights[k] = 1;
                        }
                        else
                        {
                            //if not straight accross, set to zero
                            neuronWeights[k] = 0;
                        }
                        
                    }
                    layerWeightList.Add(neuronWeights);
                }
                else
                {
                    //step through neurons of previous layer making random weight for each connection
                    for (int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        neuronWeights[k] = weights[i - 1][j][k];
                    }
                    layerWeightList.Add(neuronWeights);
                }
            }
            weightsList.Add(layerWeightList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    /// <summary>
    /// add a new neuron to every hidden layer
    /// all weights for the new neurons will be 0
    /// so it won't have an immediate affect and will
    /// rely on future mutations to give it meaning
    /// </summary>
    private void addNeuron()
    {
        for (int i = 1; i < layers.Length - 2; i++)
        {
            layers[i]++;
        }
        InitNeurons();
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                float[] newWeights = new float[weights[i][j].Length + 1];
                for (int k = 0; k < newWeights.Length; k++)
                {
                    if (k == newWeights.Length - 1) {
                        newWeights[k] = 0;
                    }
                    else
                    {
                        newWeights[k] = weights[i][j][k];
                    }
                }
                weights[i][j] = newWeights;
            }
        }
    }

    /// <summary>
    /// add a new output neuron, it's weights will all be
    /// set to zero so that it doesn't have an immediate affect
    /// and relies on future mutations to do something
    /// </summary>
    private void addOutput()
    {
        layers[layers.Length - 1] = layers[layers.Length - 1] + 1;
        InitNeurons();
        for (int i = 0; i < weights[weights.Length - 1].Length; i++)
        {
            float[] newWeights = new float[weights[weights.Length - 1][i].Length + 1];
            for (int j = 0; j < newWeights.Length; j++)
            {
                if (j == newWeights.Length - 1)
                {
                    newWeights[j] = 0;
                }
                else
                {
                    newWeights[j] = weights[weights.Length - 1][i][j];
                }
            }
            weights[weights.Length - 1][i] = newWeights;
        }
    }

    /// <summary>
    /// add a new input neuron, it's weights will all be set
    /// to zero so that it doesn't have an immediate affect and
    /// relies on futer mutations to do something.
    /// </summary>
    private void addInput()
    {
        layers[0] = layers[0] + 1;
        InitNeurons();
        for (int i = 0; i < weights[0].Length; i++)
        {
            float[] newWeights = new float[weights[0][i].Length + 1];
            for (int j = 0; j< newWeights.Length; j++)
            {
                if (j == newWeights.Length - 1)
                {
                    newWeights[j] = 0;
                }
                else
                {
                    newWeights[j] = weights[0][i][j];
                }
            }
            weights[0][i] = newWeights;
        }
    }



    /// <summary>
    /// Make a deep copy of the NN passed in
    /// </summary>
    /// <param name="original">the NN to be copied onto this NN</param>
    public void clone(NeuralNetwork original)
    {
       original.copyWeights(weights);
    }

    /// <summary>
    /// does a deep copy of this NN weights into the var passed in
    /// </summary>
    /// <param name="newWeights">The destination weights var for a NN</param>
    public void copyWeights(float[][][] newWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    newWeights[i][j][k] = this.weights[i][j][k];
                }
            }
        }
    }
}
