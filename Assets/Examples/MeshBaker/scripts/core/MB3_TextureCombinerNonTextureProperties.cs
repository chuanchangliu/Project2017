using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DigitalOpus.MB.Core
{
    public class MB3_TextureCombinerNonTextureProperties
    {
        MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;
        bool _considerNonTextureProperties = false;
        private TextureBlender resultMaterialTextureBlender;
        private TextureBlender[] textureBlenders = new TextureBlender[0];

        public TextureBlender GetTextureBlender()
        {
            return resultMaterialTextureBlender;
        }

        public MB3_TextureCombinerNonTextureProperties(MB2_LogLevel ll, bool considerNonTextureProps)
        {
            LOG_LEVEL = ll;
            _considerNonTextureProperties = considerNonTextureProps;
        }

#if UNITY_WSA && !UNITY_EDITOR
        //not defined for WSA runtime
#else 
        static bool InterfaceFilter(Type typeObj, System.Object criteriaObj)
        {
            return typeObj.ToString() == criteriaObj.ToString();
        }
#endif

        internal void FindBestTextureBlender(Material resultMaterial)
        {
            resultMaterialTextureBlender = FindMatchingTextureBlender(resultMaterial.shader.name);
            if (resultMaterialTextureBlender != null)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Using _considerNonTextureProperties found a TextureBlender for result material. Using: " + resultMaterialTextureBlender);
            }
            else
            {
                if (LOG_LEVEL >= MB2_LogLevel.error) Debug.LogWarning("Using _considerNonTextureProperties could not find a TextureBlender that matches the shader on the result material. Using the Fallback Texture Blender.");
                resultMaterialTextureBlender = new TextureBlenderFallback();
            }
        }

        internal void LoadTextureBlenders()
        {
#if UNITY_WSA && !UNITY_EDITOR
        //not defined for WSA runtime
#else   
            string qualifiedInterfaceName = "DigitalOpus.MB.Core.TextureBlender";
            var interfaceFilter = new TypeFilter(InterfaceFilter);
            List<Type> types = new List<Type>();
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Collections.IEnumerable typesIterator = null;
                try
                {
                    typesIterator = ass.GetTypes();
                }
                catch (Exception e)
                {
                    //Debug.Log("The assembly that I could not read types for was: " + ass.GetName());
                    //suppress error
                    e.Equals(null);
                }
                if (typesIterator != null)
                {
                    foreach (Type ty in ass.GetTypes())
                    {
                        var myInterfaces = ty.FindInterfaces(interfaceFilter, qualifiedInterfaceName);
                        if (myInterfaces.Length > 0)
                        {
                            types.Add(ty);
                        }
                    }
                }
            }
            TextureBlender fallbackTB = null;
            List<TextureBlender> textureBlendersList = new List<TextureBlender>();
            foreach (Type tt in types)
            {
                if (!tt.IsAbstract && !tt.IsInterface)
                {
                    TextureBlender instance = (TextureBlender)Activator.CreateInstance(tt);
                    if (instance is TextureBlenderFallback)
                    {
                        fallbackTB = instance;
                    }
                    else
                    {
                        textureBlendersList.Add(instance);
                    }
                }
            }
            if (fallbackTB != null) textureBlendersList.Add(fallbackTB); // must come last in list
            textureBlenders = textureBlendersList.ToArray();
            if (LOG_LEVEL >= MB2_LogLevel.debug)
            {
                Debug.Log(string.Format("Loaded {0} TextureBlenders.", textureBlenders.Length));
            }
#endif
        }

        internal Color GetColorIfNoTexture(Material m, ShaderTextureProperty shaderPropertyName)
        {
            return resultMaterialTextureBlender.GetColorIfNoTexture(m, shaderPropertyName);
        }

        internal bool NonTexturePropertiesAreEqual(Material a, Material b)
        {
            return resultMaterialTextureBlender.NonTexturePropertiesAreEqual(a, b);
        }

        internal Texture2D TintTextureWithTextureCombiner(Texture2D t, MB_TexSet sourceMaterial, ShaderTextureProperty shaderPropertyName)
        {
            resultMaterialTextureBlender.OnBeforeTintTexture(sourceMaterial.matsAndGOs.mats[0].mat, shaderPropertyName.name);
            if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log(string.Format("Blending texture {0} mat {1} with non-texture properties using TextureBlender {2}", t.name, sourceMaterial.matsAndGOs.mats[0].mat, resultMaterialTextureBlender));
            for (int i = 0; i < t.height; i++)
            {
                Color[] cs = t.GetPixels(0, i, t.width, 1);
                for (int j = 0; j < cs.Length; j++)
                {
                    cs[j] = resultMaterialTextureBlender.OnBlendTexturePixel(shaderPropertyName.name, cs[j]);
                }
                t.SetPixels(0, i, t.width, 1, cs);
            }
            t.Apply();
            return t;
        }

        internal TextureBlender FindMatchingTextureBlender(string shaderName)
        {
            for (int i = 0; i < textureBlenders.Length; i++)
            {
                if (textureBlenders[i].DoesShaderNameMatch(shaderName))
                {
                    return textureBlenders[i];
                }
            }
            return null;
        }

        //If we are switching from a Material that uses color properties to
        //using atlases don't want some properties such as _Color to be copied
        //from the original material because the atlas texture will be multiplied
        //by that color
        internal void AdjustNonTextureProperties(Material mat, List<ShaderTextureProperty> texPropertyNames, List<MB_TexSet> distinctMaterialTextures, bool considerTintColor, MB2_EditorMethodsInterface editorMethods)
        {
            if (mat == null || texPropertyNames == null) return;
            if (_considerNonTextureProperties)
            {
                //try to use a texture blender if we can find one to set the non-texture property values
                if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Adjusting non texture properties using TextureBlender for shader: " + mat.shader.name);
                resultMaterialTextureBlender.SetNonTexturePropertyValuesOnResultMaterial(mat);
                return;
            }
            if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Adjusting non texture properties on result material");
            for (int i = 0; i < texPropertyNames.Count; i++)
            {
                string nm = texPropertyNames[i].name;
                if (nm.Equals("_MainTex"))
                {
                    if (mat.HasProperty("_Color"))
                    {
                        try
                        {
                            if (considerTintColor)
                            {
                                //tint color was baked into atlas so set to white;
                                mat.SetColor("_Color", Color.white);
                            }
                            else
                            {
                                //mat.SetColor("_Color", distinctMaterialTextures[0].ts[i].tintColor);
                            }
                        }
                        catch (Exception) { }
                    }
                }
                if (nm.Equals("_BumpMap"))
                {
                    if (mat.HasProperty("_BumpScale"))
                    {
                        try
                        {
                            mat.SetFloat("_BumpScale", 1f);
                        }
                        catch (Exception) { }
                    }
                }
                if (nm.Equals("_ParallaxMap"))
                {
                    if (mat.HasProperty("_Parallax"))
                    {
                        try
                        {
                            mat.SetFloat("_Parallax", .02f);
                        }
                        catch (Exception) { }
                    }
                }
                if (nm.Equals("_OcclusionMap"))
                {
                    if (mat.HasProperty("_OcclusionStrength"))
                    {
                        try
                        {
                            mat.SetFloat("_OcclusionStrength", 1f);
                        }
                        catch (Exception) { }
                    }
                }
                if (nm.Equals("_EmissionMap"))
                {
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        try
                        {
                            mat.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 0f));
                        }
                        catch (Exception) { }
                    }
                    if (mat.HasProperty("_EmissionScaleUI"))
                    {
                        try
                        {
                            mat.SetFloat("_EmissionScaleUI", 1f);
                        }
                        catch (Exception) { }
                    }
                }
            }
            if (editorMethods != null)
            {
                editorMethods.CommitChangesToAssets();
            }
        }

        internal static Color GetColorIfNoTexture(ShaderTextureProperty texProperty)
        {
            if (texProperty.isNormalMap)
            {
                return new Color(.5f, .5f, 1f);
            }
            else if (texProperty.name.Equals("_MetallicGlossMap"))
            {
                return new Color(0f, 0f, 0f, 1f);
            }
            else if (texProperty.name.Equals("_ParallaxMap"))
            {
                return new Color(0f, 0f, 0f, 0f);
            }
            else if (texProperty.name.Equals("_OcclusionMap"))
            {
                return new Color(1f, 1f, 1f, 1f);
            }
            else if (texProperty.name.Equals("_EmissionMap"))
            {
                return new Color(0f, 0f, 0f, 0f);
            }
            else if (texProperty.name.Equals("_DetailMask"))
            {
                return new Color(0f, 0f, 0f, 0f);
            }
            return new Color(1f, 1f, 1f, 0f);
        }
    }
}
