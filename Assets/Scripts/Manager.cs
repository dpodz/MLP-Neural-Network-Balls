using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    public GameObject ballPrefab;
    public GameObject pickup;

    public Text genText;
    public Text fitText;
    public Text nnText;
    public Text timeText;
    public int timeSetting;

    private bool isTraining = false;
    private int populationSize = 50;
    private int generationNumber = 0;
    private int[] layers = new int[] { 2, 10, 10, 2 }; //1 input and 2 outputs
    private List<NeuralNetwork> nets;
    private bool leftMouseDown = false;
    private List<Ball> ballList = null;


    void Timer()
    {
        isTraining = false;
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
        pickup.transform.position = new Vector3(Mathf.Sin(Time.fixedTime/2) * 9, 0.5f, Mathf.Cos(UnityEngine.Time.fixedTime/2) * 9);
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
