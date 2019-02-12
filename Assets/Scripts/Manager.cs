using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // Settings, prefabs, etc
    public GameObject ballPrefab;
    public GameObject pickup;
    public int timeSetting;

    // Text related variables
    public Text genText;
    public Text fitText;
    public Text prevFitText;
    public Text prevMedianFitText;
    public Text tenthMedianFitText;
    public Text nnText;
    public Text speedText;
    private float pickupSpeed = 1f;
    private float[] tenthMedianArray = new float[11];

    // Variables used in methods throughout manager.cs
    private bool isTraining = false;
    private int populationSize = 50;
    private int generationNumber = 0;
    private int[] layers = new int[] { 6, 20, 20, 2 }; // 6 inputs and 2 outputs
    private List<NeuralNetwork> nets;
    private bool leftMouseDown = false;
    private List<Ball> ballList = null;


    void Timer()
    {
        // Reset training loop
        isTraining = false;

        // Get average and median fitness of gen before reset
        float currentFit = 0;
        List<float> fitList = new List<float>();
        for (int i = 0; i < ballList.Count; i++)
        {
            currentFit += ballList[i].net.GetFitness();
            fitList.Add(ballList[i].net.GetFitness());
        }
        prevFitText.text = "Average Previous Gen Fitness: " + (currentFit / populationSize);
        fitList.Sort();
        float medianFitness = fitList[fitList.Count / 2];
        prevMedianFitText.text = "Median Previous Gen Fitness: " + medianFitness;

        // display for the fitness 10 generations ago
        tenthMedianArray[generationNumber % 11] = medianFitness;
        if (generationNumber > 10)
        {
            tenthMedianFitText.text = "Median Fitness 10 Gens Ago: " + tenthMedianArray[(generationNumber - 10) % 11];
        }
    }

    private void FixedUpdate()
    {
        if (ballList != null && ballList.Count != 0)
        {
            float tempFit = ballList[0].net.GetFitness();
            //for (int i = 0; i < ballList.Count; i++)
            //{
            //    tempFit += ballList[i].net.GetFitness();
            //}
            fitText.text = "Sum of Fitness: " + tempFit + " " + ballList[0].GetDeltaVectorMagnitude();
            string tempNN = ballList[0].net.GetNeuralNetString();
            nnText.text = tempNN;
        }
    }

    void Update()
    {
        if (isTraining == false)
        {
            if (generationNumber == 0)
            {
                InitBallNeuralNetworks();
            }
            else
            {
                nets.Sort();
                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                    nets[i].Mutate();

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }

            generationNumber++;
            genText.text = "Generation: " + generationNumber;

            isTraining = true;
            Invoke("Timer", timeSetting);
            CreateBallBodies();
        }

        if (Input.GetMouseButtonDown(0))
        {
            leftMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftMouseDown = false;
        }
        if (Input.GetKeyDown("="))
        {
            pickupSpeed*=1.25f;
            speedText.text = "Pickup Speed: " + pickupSpeed;
        }
        if (Input.GetKeyDown("-"))
        {
            pickupSpeed/= 1.25f;
            speedText.text = "Pickup Speed: " + pickupSpeed;
        }
        if (leftMouseDown == true)
        {
            // A few different ways of moving the pickup object
            //Plane plane = new Plane(Vector3.up, Vector3.zero);
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //float distance;
            //if (plane.Raycast(ray, out distance))
            //{
            //    Vector3 point = ray.GetPoint(distance);
            //    pickup.transform.position = new Vector3 (point.x, 0.5f, point.z);
            //}
        }
        // move the pickup in a circle
        pickup.transform.position = new Vector3(Mathf.Sin(Time.fixedTime/2 * pickupSpeed) * 9, 0.5f, Mathf.Cos(UnityEngine.Time.fixedTime/2 * pickupSpeed) * 9);
    }


    private void CreateBallBodies()
    {
        if (ballList != null)
        {
            for (int i = 0; i < ballList.Count; i++)
            {
                GameObject.Destroy(ballList[i].gameObject);
            }

        }

        ballList = new List<Ball>();

        for (int i = 0; i < populationSize; i++)
        {
            Ball ball = ((GameObject)Instantiate(ballPrefab)).GetComponent<Ball>();
            ball.Init(nets[i], pickup.transform);
            ballList.Add(ball);
        }

    }

    void InitBallNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetwork>();


        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }
}
