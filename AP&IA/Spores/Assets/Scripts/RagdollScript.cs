using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollScript : MonoBehaviour
{

    public Transform principalBone;
    private Transform[] tabBones;
    // Start is called before the first frame update
    //public RagdollScript(Transform pb) {
    //    this.principalBone = pb;
    //}
    public void bones()
    {
        principalBone = this.transform.GetChild(1).GetChild(0);
        Vector3 initial = new Vector3(1, 1, 1);
        Vector3 destination = new Vector3(1, 1, 1);
        principalBone.gameObject.AddComponent<Rigidbody>();
        principalBone.GetComponent<Rigidbody>().mass = 1;
        //principalBone.GetComponent<Rigidbody>().useGravity = false;
        for (int i = 0; i < principalBone.childCount; ++i)
        {
            tabBones = principalBone.GetChild(i).GetComponentsInChildren<Transform>();
            for (int j = 0; j < tabBones.Length - 1; ++j)
            {
                if(j == 0)
                {
                    CreateJoint(initial, destination, principalBone, tabBones[j], tabBones[j + 1]);
                }
                else
                {
                    CreateJoint(initial, destination, tabBones[j-1], tabBones[j], tabBones[j + 1]);
                }
            }
        }
    }
    
    void CreateJoint(Vector3 initial, Vector3 destination,Transform PreviousBone ,Transform BoneActuel, Transform nextBone)
    {
        CapsuleCollider cap = BoneActuel.gameObject.AddComponent<CapsuleCollider>();
        Vector3 center = ( nextBone.transform.position + BoneActuel.transform.position) / 2;
        cap.center = cap.transform.InverseTransformPoint(center);
        float distance = Vector3.Distance(nextBone.position, BoneActuel.position);
        cap.height = distance;
        CharacterJoint joint = BoneActuel.gameObject.AddComponent<CharacterJoint>();
        joint.connectedBody = PreviousBone.GetComponent<Rigidbody>();
    }

    private Vector3 absoluteVec3(Vector3 init)
    {
        float x = init.x;
        float y = init.y;
        float z = init.z;
        Vector3 res = new Vector3(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z));


        return res;
    }
}
