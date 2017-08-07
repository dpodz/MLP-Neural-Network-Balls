using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    private bool initilized = false;
    private Transform pickup;

    public NeuralNetwork net;
    private Rigidbody rBody;
    private Material[] mats;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        mats = new Material[transform.childCount];
        for (int i = 0; i < mats.Length; i++)
            mats[i] = transform.GetChild(i).GetComponent<Renderer>().material;
    }

    void FixedUpdate()
    {
        if (initilized == true)
        {
            float distance = Vector3.Distance(transform.position, pickup.position);
            if (distance > 20f)
                distance = 20f;
            for (int i = 0; i < mats.Length; i++)
                mats[i].color = new Color(distance / 20f, (1f - (distance / 20f)), (1f - (distance / 20f)));

            float[] inputs = new float[2];

            Vector3 deltaVector = (pickup.position - transform.position);

            //float rad = Mathf.Atan2(deltaVector.z, deltaVector.x);
            //rad *= Mathf.Rad2Deg;

            //rad = rad % 360;
            //if (rad < 0)
            //{
            //    rad = 360 + rad;
            //}

            //rad = 90f - rad;
            //if (rad < 0f)
            //{
            //    rad += 360f;
            //}
            //rad = 360 - rad;
            //rad -= angle;
            //if (rad < 0)
            //    rad = 360 + rad;
            //if (rad >= 180f)
            //{
            //    rad = 360 - rad;
            //    rad *= -1f;
            //}
            //rad *= Mathf.Deg2Rad;

            //inputs[0] = rad / (Mathf.PI);
            inputs[0] = deltaVector.x;
            inputs[1] = deltaVector.z;

            float[] output = net.FeedForward(inputs);

            Vector3 movement = new Vector3(output[0], 0, output[1]);
            rBody.AddForce(movement * 8);

            net.AddFitness((5f - Mathf.Abs(deltaVector.magnitude)));
        }
    }

    public string GetDeltaVectorMagnitude()
    {
        Vector3 deltaVector = (pickup.position - transform.position);
        return "Magnitude: " + deltaVector.magnitude;
    }

    public void Init(NeuralNetwork net, Transform pickup)
    {
        this.pickup = pickup;
        this.net = net;
        initilized = true;
    }
}
