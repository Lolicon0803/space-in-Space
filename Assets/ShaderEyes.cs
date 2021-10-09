using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderEyes : MonoBehaviour
{
    private Material m_Material;

    float[] targetY = new float[] { 0.25f, 0.5f, 2f };

    int count = 0;
    bool up = true;
    bool can = false;
    // Start is called before the first frame update
    void Start()
    {
        m_Material = GetComponent<SpriteRenderer>().material;
    }

    public void Enable()
    {
        can = true;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F10))
        {
            can = true;
        }
#endif
        if (can)
        {
            if (count < 3)
            {
                float uvY = m_Material.GetVector("_Param").y;

                uvY += Time.deltaTime * 0.5f;

                if (uvY < targetY[count] && up)
                {
                    m_Material.SetVector("_Param", new Vector4(0.6f, uvY, 0.0f, 0.0f));
                }
                else
                {
                    up = false;


                    if (uvY > 0 && !up)
                    {
                        m_Material.SetVector("_Param", new Vector4(0.6f, -uvY, 0.0f, 0.0f));
                    }
                    else
                    {
                        up = true;
                        count++;
                    }
                    //  count++;
                }
            }
            else
            {
                float uvX = m_Material.GetVector("_Param").x;

                uvX += Time.deltaTime * 0.5f;
                m_Material.SetVector("_Param", new Vector4(uvX, 2f, 0.0f, 0.0f));

                if (uvX > 1)
                {
                    Destroy(this.gameObject);
                }
               
            }
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, m_Material);
    }

}
