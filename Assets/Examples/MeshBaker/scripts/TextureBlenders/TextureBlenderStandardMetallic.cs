using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DigitalOpus.MB.Core
{
    public class TextureBlenderStandardMetallic : TextureBlender
    {
        private enum Prop{
            doColor,
            doMetallic,
            doEmission,
            doBump,
            doNone,
        }

        Color m_tintColor;
        float m_smoothness;
        float m_metallic;
        float m_bumpScale;
        Color m_emissionColor;
        float m_emissionSlider;
        bool m_doEmission = false;
        Prop propertyToDo = Prop.doNone; //this just makes things more efficient so we arn't doing a string comparison for each pixel.

        Color m_defaultColor = Color.white;
        float m_defaultMetallic = 0f;
        float m_defaultGlossiness = 1f;
        float m_defaultBumpScale = 1f;
        Color m_defaultEmission = Color.white;

        public bool DoesShaderNameMatch(string shaderName)
        {
            return shaderName.Equals("Standard");
        }

        public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
        {
            if (shaderTexturePropertyName.Equals("_MainTex"))
            {
                propertyToDo = Prop.doColor;
                if (sourceMat.HasProperty(shaderTexturePropertyName))
                {
                    m_tintColor = sourceMat.GetColor("_Color");
                } else
                {
                    m_tintColor = m_defaultColor;
                }
            } else if (shaderTexturePropertyName.Equals("_MetallicGlossMap"))
            {
                propertyToDo = Prop.doMetallic;
                if (sourceMat.HasProperty(shaderTexturePropertyName))
                {
                    if (sourceMat.HasProperty("_GlossMapScale"))
                        m_smoothness = sourceMat.GetFloat("_GlossMapScale");
                    if (sourceMat.HasProperty("_Metallic"))
                        m_metallic = sourceMat.GetFloat("_Metallic");
                } else
                {
                    m_smoothness = m_defaultGlossiness;
                    m_metallic = m_defaultMetallic;
                }
            } else if (shaderTexturePropertyName.Equals("_BumpMap"))
            {
                propertyToDo = Prop.doBump;
                if (sourceMat.HasProperty(shaderTexturePropertyName))
                {
                    if (sourceMat.HasProperty("_BumpScale"))
                        m_bumpScale = sourceMat.GetFloat("_BumpScale");
                }
                else
                {
                    m_bumpScale = m_defaultBumpScale;
                }
            } else if (shaderTexturePropertyName.Equals("_EmissionMap"))
            {
                propertyToDo = Prop.doEmission;
                if (sourceMat.HasProperty("_EmissionColor")) {
                    m_emissionColor = sourceMat.GetColor("_EmissionColor");
                } else
                {
                    m_defaultColor = m_defaultEmission;
                }
            } else
            {
                propertyToDo = Prop.doNone;
            } 
        }

        public Color OnBlendTexturePixel(string propertyToDoshaderPropertyName, Color pixelColor)
        {
            if (propertyToDo == Prop.doColor)
            {
                return new Color(pixelColor.r * m_tintColor.r, pixelColor.g * m_tintColor.g, pixelColor.b * m_tintColor.b, pixelColor.a * m_tintColor.a);
            } else if (propertyToDo == Prop.doMetallic)
            {
                return new Color(pixelColor.r * m_smoothness, pixelColor.g * m_smoothness, pixelColor.b * m_smoothness, pixelColor.a * m_smoothness);
            } else if (propertyToDo == Prop.doBump)
            {
                return pixelColor * m_bumpScale;
            } else if (propertyToDo == Prop.doEmission)
            {
                return new Color(pixelColor.r * m_emissionColor.r, pixelColor.g * m_emissionColor.g, pixelColor.b * m_emissionColor.b, pixelColor.a * m_emissionColor.a);
            }
            return pixelColor;
        }

        public bool NonTexturePropertiesAreEqual(Material a, Material b)
        {
            if (!TextureBlenderFallback._compareColor(a, b, m_defaultColor, "_Color"))
            {
                return false;
            }
            if (!TextureBlenderFallback._compareFloat(a, b, m_defaultMetallic, "_Metallic"))
            {
                return false;
            }
            if (!TextureBlenderFallback._compareFloat(a, b, m_defaultBumpScale, "_bumpScale"))
            {
                return false;
            }
            if (!TextureBlenderFallback._compareFloat(a, b, m_defaultGlossiness, "_GlossMapScale"))
            {
                return false;
            }
            if (!TextureBlenderFallback._compareColor(a, b, m_defaultEmission, "_EmissionColor"))
            {
                return false;
            }
            return true;
        }

        public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
        {
            resultMaterial.SetColor("_Color", m_defaultColor);
            resultMaterial.SetFloat("_Metallic", m_defaultMetallic);
            resultMaterial.SetFloat("_GlossMapScale", m_defaultGlossiness);
            resultMaterial.SetFloat("_BumpScale", 1f);
            if (resultMaterial.GetTexture("_EmissionMap") == null)
            {
                resultMaterial.SetColor("_EmissionColor", Color.white);
            }
            else {
                resultMaterial.SetColor("_EmissionColor", Color.white);
            }
        }


        public Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texPropertyName)
        {
            if (texPropertyName.name.Equals("_BumpMap"))
            {
                return new Color(.5f, .5f, 1f);
            }
            else if (texPropertyName.name.Equals("_MainTex"))
            {
                if (mat != null && mat.HasProperty("_Color"))
                {
                    try
                    { //need try because can't garantee _Color is a color
                        return mat.GetColor("_Color");
                    }
                    catch (Exception) { }
                }
            }
            else if (texPropertyName.name.Equals("_MetallicGlossMap"))
            {
                if (mat != null && mat.HasProperty("_Metallic"))
                {
                    try
                    { //need try because can't garantee _Metallic is a float
                        float v = mat.GetFloat("_Metallic");
                        Color c = new Color(v, v, v);
                        if (mat.HasProperty("_GlossMapScale"))
                        {
                            try
                            {
                                c.a = mat.GetFloat("_GlossMapScale");
                            }
                            catch (Exception) { }
                        }
                        return c;
                    }
                    catch (Exception) { }
                } else
                {
                    return new Color(0f,0f,0f,.5f);
                }
            }
            else if (texPropertyName.name.Equals("_ParallaxMap"))
            {
                return new Color(0f, 0f, 0f, 0f);
            }
            else if (texPropertyName.name.Equals("_OcclusionMap"))
            {
                return new Color(1f, 1f, 1f, 1f);
            }
            else if (texPropertyName.name.Equals("_EmissionMap"))
            {
                if (mat != null)
                {
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        try
                        {
                            return mat.GetColor("_EmissionColor");
                        }
                        catch (Exception) { }
                    } 
                    else
                    {
                        return Color.black;
                    }
                }
            }
            else if (texPropertyName.name.Equals("_DetailMask"))
            {
                return new Color(0f, 0f, 0f, 0f);
            } 
            return new Color(1f, 1f, 1f, 0f);
        }
    }
}
