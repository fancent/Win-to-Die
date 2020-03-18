using System.Collections;
using System.Collections.Generic;
using PathCreation.Utility;
using UnityEngine;

namespace PathCreation.Examples
{
    public class WallTangent : MonoBehaviour
    {
        public PathCreator pc;
        Vector3 OnCollisionEnter(Collision collisionInfo)
        {
            foreach (ContactPoint cp in collisionInfo.contacts)
            {
                RaycastHit hit;
                Ray ray = new Ray(cp.point-cp.normal, cp.normal);
                if (Physics.Raycast(ray, out hit)){
                    return pc.path.GetDirection(hit.textureCoord.y);
                }
                
            }
            return Vector3.zero;
        }
    }
}