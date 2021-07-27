using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
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

    public Random random = new Random();

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
            rs[i].AddForce(new Vector3(res[i * 3] * 80 , res[i * 3 + 1] * 80, res[i * 3 + 2] * 80));
        }


    }

    public void genetic_algorithm(int n_bits, int n_iter, int n_pop, float r_cross, float r_mut) {
        chromosomeAleatoire(n_pop);
        notes = new float[n_pop];
        float best_eval = 0;
        float[][][] best = tabChromosome[0];
        //# enumerate generations
        for (int i = 0; i < n_iter; i++) {
            //# evaluate all candidates in the population
            UnityEngine.Debug.Log(i + " best " + best);  
                for (int j = 0; j < n_pop; j++) {
                    
                notes.append(nbBitJambes * 10 + nbBitAiles * 10 + nbBitBras * 10);
                }

                //# check for new best solution   
                for (int j = 0; j < n_pop; j++) {
                    if (notes[i] > best_eval){
                        best = tabChromosome[i];
                        best_eval = notes[i];
                        print(">%d, new best f(%s) = %.3f" % (gen, pop[i], int(notes[i]));
                    }
                }
            }

            //# select parents    
            selected = [selection(pop, notes) for _ in range(n_pop);

            bests = [bests for _, bests in sorted(zip(notes, pop))];


            keep = bests[-6:];

            //# create the next generation
            children = [];

            for i in range(0, n_pop, 2) {

                //# get selected parents in pairs
                p1, p2 = selected[i], selected[i + 1];

                //# crossover and mutation
                for c in crossover(p1, p2, r_cross) {

                    //# mutation
                    mutation(c, r_mut);

                    //# store for next generation
                    children.append(c);
                }
            }

            //replace population
            pop = children;
        }
        //pop = children[0:19]
        //pop.append(keep)
        //print(pop)
        return [pop, best, best_eval];
}


//# tournament selection
    public void selection(pop, scores, k= 3) {
        //first random selection
        selection_ix = randint(0, len(pop) - 1)
        for ix in range(0, len(pop), k - 1) {
            //check if better (e.g. perform a tournament)
            if scores[ix] > scores[selection_ix]
                selection_ix = ix;
        }
        return pop[selection_ix];
    }


    //# crossover two parents to create two children
    public void crossover(p1, p2, r_cross) {
        //# children are copies of parents by default
        c1, c2 = p1.copy(), p2.copy();
        //# check for recombination
        if randint(0, 1) < r_cross{
            //# select crossover point that is not on the end of the string
            pt = randint(1, len(p1) - 2);
            //# perform crossover
            c1 = p1[:pt] + p2[pt:];
            c2 = p2[:pt] + p1[pt:];
        }
        return [c1, c2];
    }

    //# mutation operator
    public void mutation(bitstring, r_mut) {
        for i in range(len(bitstring)) {
            //check for a mutation
            if (uniform(0, 1) < r_mut) {
                //flip the bit
                bitstring[i] = 1 - bitstring[i];
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
                        tabChromosome[i][j][k][n] = (float)random.NextDouble() - 0.5f; ;

                    }
                }
            }
        }
        return tabChromosome;
    }

}
