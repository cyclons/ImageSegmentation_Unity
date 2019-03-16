using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorInfo
{
    public Vector3 colorSum;
    public int ColorCount;
    public void Clear()
    {
        colorSum = Vector3.zero;
        ColorCount = 0;
    }
}

public class KMeanSegmentation : MonoBehaviour {
    public Texture rawTex;
    public InputField K_InputField;


    private Texture2D tex;
    Color[] randomColors;
    Dictionary<int, ColorInfo> colorDict = new Dictionary<int, ColorInfo>();
    // Use this for initialization
    void Start () {
        //KMeansImage(3);
    }
	
    public void KMeansImage(int k)
    {
        if (K_InputField != null && K_InputField.text != "")
        {
            int tempK = int.Parse(K_InputField.text);
            k = tempK;
        }

        tex = Instantiate(rawTex) as Texture2D;

        Color[] texPixColors = tex.GetPixels();
        colorDict = new Dictionary<int, ColorInfo>();

        randomColors = new Color[k];
        for(int i = 0; i < k; i++)
        {
            randomColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            colorDict.Add(i, new ColorInfo());
        }


        float diff = 100;
        while (diff > 0.1f)
        {
            //Debug.Log(diff);
            for (int i = 0; i < texPixColors.Length; i++)
            {
                int groupIndex = GetColorGroup(texPixColors[i]);
                colorDict[groupIndex].colorSum += new Vector3(texPixColors[i].r, texPixColors[i].g, texPixColors[i].b);
                colorDict[groupIndex].ColorCount++;
            }
            diff = ReplaceColors();
        }
        Color[] finalColors = new Color[texPixColors.Length];
        for (int i = 0; i < texPixColors.Length; i++)
        {
            int groupIndex = GetColorGroup(texPixColors[i]);

            //finalColors[i] = Color.green;
            finalColors[i] = randomColors[groupIndex];
        }
        tex.SetPixels(finalColors);
        tex.Apply();
        GetComponent<Renderer>().material.mainTexture = tex;

    }


    public void ResetImage()
    {
        GetComponent<Renderer>().material.mainTexture = rawTex;
        colorDict = new Dictionary<int, ColorInfo>();

    }

    int GetColorGroup(Color c)
    {
        int belongedGroup = -1;
        float distance = 10;
        
        for(int i = 0; i < randomColors.Length; i++)
        {
            Vector3 v1 = new Vector3(c.r, c.g, c.b);
            Vector3 v2 = new Vector3(randomColors[i].r, randomColors[i].g, randomColors[i].b);
            float tempDistance = Vector3.SqrMagnitude(v1 - v2);
            if (distance > tempDistance)
            {
                distance = tempDistance;
                belongedGroup = i;
            }
        }
        return belongedGroup;
    }

    float ReplaceColors()
    {
        float diff = 0;
        for(int i = 0; i < randomColors.Length; i++)
        {
            Vector3 v = new Vector3(colorDict[i].colorSum.x, colorDict[i].colorSum.y, colorDict[i].colorSum.z);

            if (colorDict[i].ColorCount == 0)
            {
                v = Vector3.zero;
            }
            else
            {
                v = v / colorDict[i].ColorCount;

            }

            diff += Vector3.Distance(v , new Vector3(randomColors[i].r, randomColors[i].g, randomColors[i].b));
            randomColors[i] = new Color(v.x, v.y, v.z, 1);
        }
        foreach(var v in colorDict.Values)
        {
            v.Clear();
        }
        return diff;
    }
    
}
