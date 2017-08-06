﻿using System.Collections.Generic;
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

        for (int i = 1; i < inputs.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0.25f; // 0.25 is the const bias

                for (int k = 0; k < neurons[i-1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        return neurons[neurons.Length-1];
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
                    float randomNumber = UnityEngine.Random.Range(0f, 1000f)
                        ;

                    if (randomNumber <= 2f)
                    { // random mutation 1
                        weight *= -1f;
                    }
                    else if (randomNumber <= 4f)
                    { // random mutation 2
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (randomNumber <= 6f)
                    { // random mutation 3
                        weight *= UnityEngine.Random.Range(0f, 1f);
                    }
                    else if (randomNumber <= 8f)
                    { // random mutation 4
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

    public float GetFitness(float fit)
    {
        return fitness;
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