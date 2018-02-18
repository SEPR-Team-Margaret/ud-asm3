using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVC : MonoBehaviour
{
    public Vector3 vector;

    // Use this for initialization
    void Start()
    {
        vector = new Vector3(17, 14, 0);
    }

    public void OnCollisionEnter(Collision collision)           //For each different side of the border there is a seperate if function, as each side has a different reflection normal
    {                                                           //There is a different gameobject referenced for each plane of border i.e."Border_Right"
        if (collision.gameObject.name == "Border_Right")        //If the sprite collides with the right side of the minigame border, reflect with respect to the right normal (Vector3.right)
        {
            vector = Vector3.Reflect(vector, Vector3.right);
        }

        if (collision.gameObject.name == "Border_Left")         //If the sprite collides with the right side of the minigame border, reflect with respect to the left normal (Vector3.left)
        {
			vector = Vector3.Reflect(vector, Vector3.left);
        }

        if (collision.gameObject.name == "Border_Top")          //If the sprite collides with the right side of the minigame border, reflect with respect to right normal (Vector3.top)
        {
			vector = Vector3.Reflect(vector, Vector3.up);
        }

        if (collision.gameObject.name == "Border_Bottom")       //If the sprite collides with the right side of the minigame border, reflect with respect to right normal (Vector3.bottom)
        {
			vector = Vector3.Reflect(vector, Vector3.down);
        }


    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(vector);
    }
}
