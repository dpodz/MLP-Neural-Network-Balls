using System.Collections.Generic;
using System;

public class NeuralNetwork : IComparable<NeuralNetwork>{
    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;
    private float fitness;

    // Neural Network Ctor sets up layers and initializes neurons and weights arrays with information from layers
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

    // Deep copy ctor
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();
        CopyWeights(copyNetwork.weights);
    }

    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    // Init the neurons jagged array
    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        //neuronsList[1][neuronsList[1].Length - 1] = 1;
        //neuronsList[2][neuronsList[2].Length - 1] = 1;
        neurons = neuronsList.ToArray();
    }

    // Init the weights jagged array
    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();

        // iterate over each layer starting at 1
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightList = new List<float[]>();

            int neuronsInPreviousLayer = layers[i - 1];

            // iterate over each neuron in the layer
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                // iterate over each neuron in the previous layer, to give value for neuron in this layer
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    // give random weights in range (-1, 1)
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }
                layerWeightList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    // Send data forward from inputs
    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f; // 0f is the const bias

                for (int k = 0; k < neurons[i-1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        return neurons[neurons.Length-1]; // Return the output neurons; could use neurons[-1]
    }

    // Mutate neural network weights
    public void Mutate()
    {
        // iterate through each layer
        for (int i = 0; i < weights.Length; i++)
        {
            // iterate through each neuron
            for (int j = 0; j < weights[i].Length; j++)
            {
                // iterate through each weight 
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    // mutate weight value
                    float randomNumber = UnityEngine.Random.Range(0f, 200f);

                    if (randomNumber <= 2f)
                    { // random mutation 1
                      // flip sign of weight
                        weight *= -1f;
                    }
                    else if (randomNumber <= 4f)
                    { // random mutation 2
                      // pick a random weight between -.5 and .5
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (randomNumber <= 6f)
                    { // random mutation 3
                      // decrease by 0% to 100%
                        weight *= UnityEngine.Random.Range(0f, 1f);
                    }
                    else if (randomNumber <= 8f)
                    { // random mutation 4
                      // increase to 100% or 200%
                        weight *= (UnityEngine.Random.Range(1f, 2f));
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    public void SetFitness(float fit)
    {
        fitness = fit;
    }

    public float GetFitness()
    {
        return fitness;
    }

    public string GetNeuralNetString()
    {
        string nnDataString = "";
        // Neurons info
        //for (int i = 0; i < neurons.Length; i++)
        //{
        //    for (int j = 0; j < neurons[i].Length; j++)
        //    {
        //        nnDataString += " " + neurons[i][j];
        //    }
        //    nnDataString += " :: ";
        //}

        //Input and Output Neurons info (just get first and last neuron layer)
        for (int i = 0; i < neurons.Length; i+=3)
        {
            nnDataString += "[";
            for (int j = 0; j < neurons[i].Length; j++)
            {
                nnDataString += neurons[i][j] + ", ";
            }
            nnDataString += "] :: ";
        }

        //for (int i = 0; i < weights.Length; i++)
        //{
        //    nnDataString += "{ ";
        //    for (int j = 0; j < weights[i].Length; j++)
        //    {
        //        nnDataString += "[";
        //        for (int k = 0; k < weights[i][j].Length; k++)
        //        {
        //            nnDataString += weights[i][j][k] + ", ";
        //        }
        //        nnDataString += "], ";
        //    }
        //    nnDataString += "} :: ";
        //}
        return nnDataString;
    }

    // Compare fitness between Neural Networks
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;

        if (fitness > other.fitness)
        {
            return 1;
        }
        else if (fitness < other.fitness)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
