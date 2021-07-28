using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PapyScript : MonoBehaviour
{

    Rigidbody ailes_Bone;
    Rigidbody ailesSup_Bone;
    Rigidbody ailes_Bone2;
    Rigidbody ailesSup_Bone2;
    Rigidbody def_Bone;
    Rigidbody jambesSup_Bone;
    Rigidbody def_Bone1;

    Rigidbody jambesSup_Bone2;
    Rigidbody bras_Bone;
    Rigidbody bras_Bone1;

    Rigidbody[] rs;
    NeuralNetwork net;

    float[][][][] tabChromosome;
    int[] layers = { 30, 25, 25, 30 };

    float[] notes;

    System.Random random = new System.Random();

    public Vector3 force;

    // Start is called before the first frame update
    void Start()
    {
        //RagdollScript r = new RagdollScript(transform.Find("z_Armature"));

        this.GetComponent<RagdollScript>().bones();

        ailes_Bone = transform.Find("z_Armature/Bone/Ailes-Bone").GetComponent<Rigidbody>();
        ailesSup_Bone = transform.Find("z_Armature/Bone/Ailes-Bone/AilesSup-Bone").GetComponent<Rigidbody>();
        ailes_Bone2 = transform.Find("z_Armature/Bone/Ailes-Bone2").GetComponent<Rigidbody>();
        ailesSup_Bone2 = transform.Find("z_Armature/Bone/Ailes-Bone2/AilesSup2-Bone").GetComponent<Rigidbody>();
        def_Bone = transform.Find("z_Armature/Bone/Def-Bone").GetComponent<Rigidbody>();
        jambesSup_Bone = transform.Find("z_Armature/Bone/Def-Bone/JambesSup-Bone").GetComponent<Rigidbody>();
        def_Bone1 = transform.Find("z_Armature/Bone/Def-Bone1").GetComponent<Rigidbody>();
        jambesSup_Bone2 = transform.Find("z_Armature/Bone/Def-Bone1/JambesSup-Bone2").GetComponent<Rigidbody>();
        bras_Bone = transform.Find("z_Armature/Bone/Bras-Bone").GetComponent<Rigidbody>();
        bras_Bone1 = transform.Find("z_Armature/Bone/Bras-Bone1").GetComponent<Rigidbody>();

        rs = new Rigidbody[10];
        rs[0] = ailes_Bone;
        rs[1] = ailesSup_Bone;
        rs[2] = ailes_Bone2;
        rs[3] = ailesSup_Bone2;
        rs[4] = def_Bone;
        rs[5] = jambesSup_Bone;
        rs[6] = def_Bone1;
        rs[7] = jambesSup_Bone2;
        rs[8] = bras_Bone;
        rs[9] = bras_Bone1;

        force = new Vector3(0, 0, 0);

        net = new NeuralNetwork(layers);
    }

    // Update is called once per frame
    void Update()
    {

        float[] t = new float[30];

        for (int i = 0; i < rs.Length; i++) {
            t[i * 3] = rs[i].velocity.x;
            t[i * 3 + 1] = rs[i].velocity.y;
            t[i * 3 + 2] = rs[i].velocity.z;
        }

        float[] res = net.FeedForward(t);

        for (int i = 0; i < rs.Length; i++) {
            rs[i].AddForce(new Vector3(res[i * 3] * 5, res[i * 3 + 1] * 5, res[i * 3 + 2] * 5));
        }


    }

    public void initNeural(float[][][] weights) {
        net = new NeuralNetwork(layers);
        net.setWeights(weights);
    }

    public float[][][] getWeights() {
        return net.getWeights();
    }


}
