using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    public Transform parent;

    public GameObject floor;
    public GameObject background;
    public GameObject roof;
    public GameObject gargoyle;

    public float distance;

    public int iLimit;
    public int jLimit;

    public void Create()
    {
        for(int i = 0; i < iLimit;  i++)
        {
            for (int j = 0; j < jLimit; j++)
            {
                GameObject.Instantiate(background, new Vector3(distance * j, distance * i, 0), new Quaternion(0, 0, 0, 0), parent);
                if (i == 0)
                {
                    GameObject.Instantiate(floor, new Vector3(distance * j, distance * i, 0), new Quaternion(0, 0, 0, 0), parent);
                }
                else if(i == iLimit - 1)
                {
                    GameObject.Instantiate(roof, new Vector3(distance * j, distance * i, 0), new Quaternion(0, 0, 0, 0), parent);
                }

            } 
        }
    }
}
