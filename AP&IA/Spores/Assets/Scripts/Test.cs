using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test : MonoBehaviour
{




    float[][][][] tabChromosome;
    int[] layers = { 30, 25, 25, 30 };

    float[] notes;


    [SerializeField]
    Transform spawn;
    [SerializeField]
    Transform end;
    [SerializeField]
    GameObject papy;

    GameObject[] pop;
    int n_pop;

    System.Random random = new System.Random();


    // Start is called before the first frame update
    void Start() {
        StartCoroutine(genetic_algorithm(20, 50, (float)0.3, (float)0.1));

    }

    // Update is called once per frame
    void Update() {


    }

    IEnumerator genetic_algorithm(int n_iter, int n_pop, float r_cross, float r_mut) {
        chromosomeAleatoire(n_pop);
        notes = new float[n_pop];
        float best_eval = 10;
        float[][][] best = tabChromosome[0];
        pop = new GameObject[n_pop];

        //# enumerate generations
        for (int i = 0; i < n_iter; i++) {

            for (int j = 0; j < n_pop; j++) {
                GameObject clone = Instantiate(papy, spawn.position, Quaternion.identity);
                clone.GetComponent<PapyScript>().initNeural(tabChromosome[j]);
                pop[j] = clone;
            }

            yield return new WaitForSeconds(15);
            UnityEngine.Debug.Log("stop sim");



            //# evaluate all candidates in the population
            UnityEngine.Debug.Log(i + " best " + best_eval);
            for (int j = 0; j < n_pop; j++) {
                float distDebutFin = Vector3.Distance(spawn.position, end.position);
                float distToEnd = Vector3.Distance(pop[j].GetComponent<RagdollScript>().principalBone.transform.position, end.position);
                float note = 10 * (distToEnd / distDebutFin);
                notes[j] = note;
            }

            //# check for new best solution   
            for (int j = 0; j < n_pop; j++) {
                if (notes[j] < best_eval) {
                    best = tabChromosome[j];
                    best_eval = notes[j];
                    UnityEngine.Debug.Log(j + " new best " + best_eval);
                }
            }


            //# select parents
            float[][][][] selected = new float[n_pop][][][];
            for (int j = 0; j < n_pop; j++) {
                selected[j] = selection(n_pop, notes);
            }

            for (int j = n_pop; j < 0; j--) {
                for (int k = 0; k < j - 1; k++) {
                    float[][][] temp;
                    float noteTemp;
                    if (notes[j] > notes[j + 1]) {

                        temp = tabChromosome[j];
                        noteTemp = notes[j];

                        tabChromosome[j] = tabChromosome[j + 1];
                        notes[j] = notes[j + 1];

                        tabChromosome[j + 1] = temp;
                        notes[j + 1] = noteTemp;
                    }
                }
            }

            float[][][][] keep = new float[n_pop][][][];
            for (int j = 0; j < n_pop - 10; j++) {
                keep[j] = tabChromosome[j];
            }

            //# create the next generation
            float[][][][] children = new float[n_pop][][][];
            float[][][] p1, p2;
            for (int j = 0; j < n_pop; j += 2) {

                //# get selected parents in pairs
                p1 = selected[i];
                p2 = selected[i + 1];

                //# crossover and mutation
                float[][][][] c = crossover(p1, p2, r_cross);
                for (int k = 0; k < 2; k++) {

                    //# mutation
                    mutation(c[k], r_mut);

                    //# store for next generation
                    children[j + k] = c[k];
                }
            }

            //replace population
            for (int j = 0; j < n_pop; j++) {
                Destroy(pop[j]);
            }
            tabChromosome = children;
        }
   
        //pop = children[0:19]
        //pop.append(keep)
        //print(pop)
    }


//# tournament selection
    public float[][][] selection(int n_pop, float[] scores, int k= 3) {
        //first random selection

        int selection_ix = random.Next(0, n_pop - 1);
        for (int ix = 0; ix < n_pop - 1; ix += k) {
            //check if better (e.g. perform a tournament)
            if (scores[ix] > scores[selection_ix])
                selection_ix = ix;
        }
        return tabChromosome[selection_ix];
    }


    //# crossover two parents to create two children
    public float[][][][] crossover(float[][][] p1, float[][][] p2, float r_cross) {
        //# children are copies of parents by default
        float[][][] c1 = new float[p1.Length][][];
        float[][][] c2 = new float[p2.Length][][];
        p1.CopyTo(c1, 0);
        p2.CopyTo(c2, 0);
        //# check for recombination
        for (int j = 0; j < p1.Length - 1; j++) {
            for (int k = 0; k < p1[j].Length; k++) {
                for (int n = 0; n < p1[j][k].Length; n++) {
                    float r = (float)random.NextDouble();
                    if (r < r_cross) {
                        c1[j][k][n] = p1[j][k][n];
                        c2[j][k][n] = p2[j][k][n];
                    }
                    else {
                        c1[j][k][n] = p2[j][k][n];
                        c2[j][k][n] = p1[j][k][n];
                    }
                }
            }
        }
        float[][][][] res = { c1, c2 };

        return res;
    }

    //# mutation operator
    public void mutation(float[][][] bitstring, float r_mut) {

        for (int j = 0; j < bitstring.Length - 1; j++) {
            for (int k = 0; k < bitstring[j].Length; k++) {
                for (int n = 0; n < bitstring[j][k].Length; n++) {
                    float r = (float)random.NextDouble();
                    //check for a mutation
                    if (r < r_mut) {
                        bitstring[j][k][n] = (float)random.NextDouble();
                    }
                }
            }
            
        }
    }

    public float[][][][] chromosomeAleatoire(int n_pop) {

        tabChromosome = new float[n_pop][][][];
    

        for (int i = 0; i < n_pop; i++) {
            tabChromosome[i] = new float[layers.Length][][];
            for (int j = 0; j < layers.Length - 1; j++) {
                tabChromosome[i][j] = new float[layers[j]][];
                for(int k = 0; k < layers[j]; k++) {
                    tabChromosome[i][j][k] = new float[layers[j + 1]];
                    for (int n = 0; n < layers[j + 1]; n++) {
                        tabChromosome[i][j][k][n] = (float)random.NextDouble() - 0.5f;

                    }
                }
            }
        }
        return tabChromosome;
    }

}
