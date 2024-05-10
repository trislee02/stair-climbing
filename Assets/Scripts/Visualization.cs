using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualization : MonoBehaviour
{
    public float pointSize = 0.075f;
    public string fileName = "log_tri/head_2";
    public Color color = Color.red;
    public bool drawRay = true;

    // Start is called before the first frame update
    void Start()
    {
        //TextAsset csvData = Resources.Load<TextAsset>("log_tri/head_2");

        TextAsset csvData = Resources.Load<TextAsset>(fileName);

        string[] data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            // (row[1], row[2], row[3]) is the position of the head point, (row[4], row[5], row[6], row[7]) is the quaternion of the head point
            // Draw a ray starting at the position of the head point and pointing in the direction of the quaternion
            if (row.Length > 0)
            {
                float x = float.Parse(row[1]);
                float y = float.Parse(row[2]);
                float z = float.Parse(row[3]);

                float qx = float.Parse(row[4]);
                float qy = float.Parse(row[5]);
                float qz = float.Parse(row[6]);
                float qw = float.Parse(row[7]);

                Vector3 position = new Vector3(x, y, z);
                Quaternion rotation = new Quaternion(qx, qy, qz, qw);

                GameObject headPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                headPoint.transform.parent = this.transform;
                headPoint.transform.position = position;
                headPoint.transform.rotation = rotation;
                headPoint.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
                headPoint.GetComponent<Renderer>().material.color = color;

                // Draw a ray starting at the position of the head point and pointing in the direction of the quaternion
                if (drawRay)
                {
                    GameObject ray = new GameObject();
                    ray.transform.parent = this.transform;
                    ray.transform.position = position;
                    ray.transform.rotation = rotation;
                    ray.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    ray.AddComponent<LineRenderer>();
                    LineRenderer lr = ray.GetComponent<LineRenderer>();
                    lr.material = new Material(Shader.Find("Sprites/Default"));
                    lr.startColor = color;
                    lr.endColor = new Color(color.r, color.g, color.b, 0);
                    lr.startWidth = 0.01f;
                    lr.endWidth = 0.0f;
                    lr.SetPosition(0, position);
                    lr.SetPosition(1, position + rotation * Vector3.forward);
                }
            }
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
