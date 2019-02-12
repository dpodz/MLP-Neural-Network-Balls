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

            Vector3 deltaVector = (pickup.position - transform.position);
            float[] inputs = new float[6];
            inputs[0] = deltaVector.x;
            inputs[1] = deltaVector.z;
            inputs[2] = rBody.velocity.x;
            inputs[3] = rBody.velocity.z;
            inputs[4] = pickup.position.x;
            inputs[5] = pickup.position.z;
            //inputs[2] = transform.position.x;
            //inputs[3] = transform.position.z;

            float[] output = net.FeedForward(inputs);

            Vector3 movement = new Vector3(output[0], 0, output[1]);
            // rBody.velocity = new Vector3(movement.x * 16, 0, movement.z * 16);
            rBody.AddForce(movement * 24);

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
