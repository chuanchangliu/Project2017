using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace DigitalOpus.MB.Core
{

    /*
     Like a material but also stores its tiling info since the same texture
     with different tiling may need to be baked to a separate spot in the atlas
     note that it is sometimes possible for textures with different tiling to share an atlas rectangle
     To accomplish this need to store:
             uvTiling per TexSet (can be set to 0,0,1,1 by pushing tiling down into material tiling)
             matTiling per MeshBakerMaterialTexture (this is the total tiling baked into the atlas)
             matSubrectInFullSamplingRect per material (a MeshBakerMaterialTexture can be used by multiple materials. This is the subrect in the atlas)
     Normally UVTilings is applied first then material tiling after. This is difficult for us to use when baking meshes. It is better to apply material
     tiling first then UV Tiling. There is a transform for modifying the material tiling to handle this.
     once the material tiling is applied first then the uvTiling can be pushed down into the material tiling.

         Also note that this can wrap a procedural texture. The procedural texture is converted to a Texture2D in Step2 NOT BEFORE. This is important so that can
         build packing layout quickly. 

             Should always check if texture is null using 'isNull' function since Texture2D could be null but ProceduralTexture not
             Should not call GetTexture2D before procedural textures are created

         there will be one of these per material texture property (maintex, bump etc...)
     */
    public class MeshBakerMaterialTexture
    {
        //private ProceduralTexture _procT;
        private Texture2D _t;

        public Texture2D t
        {
            set { _t = value; }
        }

        public float texelDensity; //how many pixels per polygon area
        internal static bool readyToBuildAtlases = false;
        //if these are the same for all properties then these can be merged
        public DRect encapsulatingSamplingRect; //sampling rect including both material tiling and uv Tiling
        public DRect matTilingRect;

        public MeshBakerMaterialTexture() { }
        public MeshBakerMaterialTexture(Texture tx)
        {
            if (tx is Texture2D)
            {
                _t = (Texture2D)tx;
            }
            //else if (tx is ProceduralTexture)
            //{
            //    _procT = (ProceduralTexture)tx;
            //}
            else if (tx == null)
            {
                //do nothing
            }
            else
            {
                Debug.LogError("An error occured. Texture must be Texture2D " + tx);
            }
        }

        public MeshBakerMaterialTexture(Texture tx, Vector2 o, Vector2 s, float texelDens)
        {
            if (tx is Texture2D)
            {
                _t = (Texture2D)tx;
            }
            //else if (tx is ProceduralTexture)
            //{
            //    _procT = (ProceduralTexture)tx;
            //}
            else if (tx == null)
            {
                //do nothing
            }
            else
            {
                Debug.LogError("An error occured. Texture must be Texture2D " + tx);
            }
            matTilingRect = new DRect(o, s);
            texelDensity = texelDens;
        }

        //This should never be called until we are ready to build atlases
        public Texture2D GetTexture2D()
        {
            if (!readyToBuildAtlases)
            {
                Debug.LogError("This function should not be called before Step3. For steps 1 and 2 should always call methods like isNull, width, height");
                throw new Exception("GetTexture2D called before ready to build atlases");
            }
            return _t;
        }

        public bool isNull
        {
            get { return _t == null/* && _procT == null*/; }
        }

        public int width
        {
            get
            {
                if (_t != null) return _t.width;
                //else if (_procT != null) return _procT.width;
                throw new Exception("Texture was null. can't get width");
            }
        }

        public int height
        {
            get
            {
                if (_t != null) return _t.height;
                //else if (_procT != null) return _procT.height;
                throw new Exception("Texture was null. can't get height");
            }
        }

        public string GetTexName()
        {
            if (_t != null) return _t.name;
            //else if (_procT != null) return _procT.name;
            return "";
        }

        public bool AreTexturesEqual(MeshBakerMaterialTexture b)
        {
            if (_t == b._t /*&& _procT == b._procT*/) return true;
            return false;
        }

        /*
        public bool IsProceduralTexture()
        {
            return (_procT != null);
        }

        public ProceduralTexture GetProceduralTexture()
        {
            return _procT;
        }

        public Texture2D ConvertProceduralToTexture2D(List<Texture2D> temporaryTextures)
        {
            int w = _procT.width;
            int h = _procT.height;

            bool mips = true;
            bool isLinear = false;
            GC.Collect(3, GCCollectionMode.Forced);
            Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, mips, isLinear);
            Color32[] pixels = _procT.GetPixels32(0, 0, w, h);
            tex.SetPixels32(0, 0, w, h, pixels);
            tex.Apply();
            tex.name = _procT.name;
            temporaryTextures.Add(tex);
            return tex;
        }
        */
    }

    public class MatAndTransformToMerged
    {
        public Material mat;
        /*MB_TexSets can be merged into a combined MB_TexSet if the texutures overlap enough. When this happens
        the materials may not use the whole rect in the atlas. This property defines the rect relative to the combined
        source rect that was used */
        //public DRect atlasSubrectMaterialOnly = new DRect(0f, 0f, 1f, 1f);

        public DRect obUVRectIfTilingSame = new DRect(0f, 0f, 1f, 1f);
        public DRect samplingRectMatAndUVTiling = new DRect(); //cached value this is full sampling rect used by this material. It is not updated as material sampling rects get merged. It will always be source sampling rect.
        public DRect materialTiling = new DRect();
        public string objName;

        public MatAndTransformToMerged()  { }

        public MatAndTransformToMerged(Material m)
        {
            mat = m;
        }

        public override bool Equals(object obj)
        {
            if (obj is MatAndTransformToMerged)
            {
                MatAndTransformToMerged o = (MatAndTransformToMerged)obj;


                if (o.mat == mat && o.obUVRectIfTilingSame == obUVRectIfTilingSame)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return mat.GetHashCode() ^ obUVRectIfTilingSame.GetHashCode() ^ samplingRectMatAndUVTiling.GetHashCode();
        }

        public string GetMaterialName()
        {
            if (mat != null)
            {
                return mat.name;
            }
            else if (objName != null)
            {
                return string.Format("[matFor: {0}]",objName);
            }
            else
            {
                return "Unknown";
            }
        }
    }

    public class MatsAndGOs
    {
        public List<MatAndTransformToMerged> mats;
        public List<GameObject> gos;
    }

    //a set of textures one for each "maintex","bump" that one or more materials use.
    public class MB_TexSet
    {
        public MeshBakerMaterialTexture[] ts; //one per "maintex", "bump"

        public MatsAndGOs matsAndGOs;

        //public TextureBlender textureBlender; //only used if _considerNonTextureProperties is true
        public bool allTexturesUseSameMatTiling = false;

        public Vector2 obUVoffset = new Vector2(0f, 0f);
        public Vector2 obUVscale = new Vector2(1f, 1f);
        public int idealWidth; //all textures will be resized to this size
        public int idealHeight;

        internal DRect obUVrect
        {
            get { return new DRect(obUVoffset, obUVscale); }
        }

        public MB_TexSet(MeshBakerMaterialTexture[] tss, Vector2 uvOffset, Vector2 uvScale)
        {
            ts = tss;
            obUVoffset = uvOffset;
            obUVscale = uvScale;
            allTexturesUseSameMatTiling = false;
            matsAndGOs = new MatsAndGOs();
            matsAndGOs.mats = new List<MatAndTransformToMerged>();
            matsAndGOs.gos = new List<GameObject>();
        }

        // The two texture sets are equal if they are using the same 
        // textures/color properties for each map and have the same
        // tiling for each of those color properties
        internal bool IsEqual(object obj, bool fixOutOfBoundsUVs, bool considerNonTextureProperties, MB3_TextureCombinerNonTextureProperties resultMaterialTextureBlender)
        {
            if (!(obj is MB_TexSet))
            {
                return false;
            }
            MB_TexSet other = (MB_TexSet)obj;
            if (other.ts.Length != ts.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < ts.Length; i++)
                {
                    if (ts[i].matTilingRect != other.ts[i].matTilingRect)
                        return false;
                    if (!ts[i].AreTexturesEqual(other.ts[i]))
                        return false;
                    if (considerNonTextureProperties)
                    {
                        if (resultMaterialTextureBlender != null)
                        {
                            if (!resultMaterialTextureBlender.NonTexturePropertiesAreEqual(matsAndGOs.mats[0].mat, other.matsAndGOs.mats[0].mat))
                            {
                                return false;
                            }
                        }
                    }
                }

                //IMPORTANT don't use Vector2 != Vector2 because it is only acurate to about 5 decimal places
                //this can lead to tiled rectangles that can't accept rectangles.
                if (fixOutOfBoundsUVs && (obUVoffset.x != other.obUVoffset.x ||
                                            obUVoffset.y != other.obUVoffset.y))
                    return false;
                if (fixOutOfBoundsUVs && (obUVscale.x != other.obUVscale.x ||
                                            obUVscale.y != other.obUVscale.y))
                    return false;
                return true;
            }
        }

        //assumes all materials use the same obUVrects.
        public void CalcInitialFullSamplingRects(bool fixOutOfBoundsUVs)
        {
            DRect validFullSamplingRect = new Core.DRect(0, 0, 1, 1);
            if (fixOutOfBoundsUVs)
            {
                validFullSamplingRect = obUVrect;
            }

            for (int propIdx = 0; propIdx < ts.Length; propIdx++)
            {
                if (!ts[propIdx].isNull)
                {
                    DRect matTiling = ts[propIdx].matTilingRect;
                    DRect ruv;
                    if (fixOutOfBoundsUVs)
                    {
                        ruv = obUVrect;
                    }
                    else
                    {
                        ruv = new DRect(0.0, 0.0, 1.0, 1.0);
                    }
                    ts[propIdx].encapsulatingSamplingRect = MB3_UVTransformUtility.CombineTransforms(ref ruv, ref matTiling);
                    validFullSamplingRect = ts[propIdx].encapsulatingSamplingRect;

                }
            }
            //if some of the textures were null make them match the sampling of one of the other textures
            for (int propIdx = 0; propIdx < ts.Length; propIdx++)
            {
                if (ts[propIdx].isNull)
                {
                    ts[propIdx].encapsulatingSamplingRect = validFullSamplingRect;
                }
            }
        }

        public void CalcMatAndUVSamplingRects()
        {
            if (allTexturesUseSameMatTiling)
            {
                DRect matTiling = new DRect(0f, 0f, 1f, 1f);
                for (int propIdx = 0; propIdx < ts.Length; propIdx++)
                {
                    if (!ts[propIdx].isNull)
                    {
                        matTiling = ts[propIdx].matTilingRect;
                    }
                }
                for (int matIdx = 0; matIdx < matsAndGOs.mats.Count; matIdx++)
                {
                    matsAndGOs.mats[matIdx].materialTiling = matTiling;
                    matsAndGOs.mats[matIdx].samplingRectMatAndUVTiling = MB3_UVTransformUtility.CombineTransforms(ref matsAndGOs.mats[matIdx].obUVRectIfTilingSame, ref matTiling);  //MB3_TextureCombiner.GetSourceSamplingRect(ts[matIdx], obUVoffset, obUVscale);
                }
            }
            else
            {
                // material tiling is different for each texture property
                // in this case we set the material tiling to 1 here
                for (int matIdx = 0; matIdx < matsAndGOs.mats.Count; matIdx++)
                {
                    DRect matTiling = new DRect(0f, 0f, 1f, 1f);
                    matsAndGOs.mats[matIdx].materialTiling = matTiling;
                    matsAndGOs.mats[matIdx].samplingRectMatAndUVTiling = MB3_UVTransformUtility.CombineTransforms(ref matsAndGOs.mats[matIdx].obUVRectIfTilingSame, ref matTiling);  //MB3_TextureCombiner.GetSourceSamplingRect(ts[matIdx], obUVoffset, obUVscale);
                }
            }
        }

        public bool AllTexturesAreSameForMerge(MB_TexSet other, /*bool considerTintColor*/ bool considerNonTextureProperties, MB3_TextureCombinerNonTextureProperties resultMaterialTextureBlender)
        {
            if (other.ts.Length != ts.Length)
            {
                return false;
            }
            else
            {
                if (!other.allTexturesUseSameMatTiling || !allTexturesUseSameMatTiling)
                {
                    return false;
                }
                // must use same set of textures
                int idxOfFirstNoneNull = -1;
                for (int i = 0; i < ts.Length; i++)
                {
                    if (!ts[i].AreTexturesEqual(other.ts[i]))
                        return false;
                    if (idxOfFirstNoneNull == -1 && !ts[i].isNull)
                    {
                        idxOfFirstNoneNull = i;
                    }
                    if (considerNonTextureProperties)
                    {
                        if (resultMaterialTextureBlender != null)
                        {
                            if (!resultMaterialTextureBlender.NonTexturePropertiesAreEqual(matsAndGOs.mats[0].mat, other.matsAndGOs.mats[0].mat))
                            {
                                return false;
                            }
                        }
                    }
                }
                if (idxOfFirstNoneNull != -1)
                {
                    //check that all textures are the same. Have already checked all tiling is same
                    for (int i = 0; i < ts.Length; i++)
                    {
                        if (!ts[i].AreTexturesEqual(other.ts[i]))
                        {
                            return false;
                        }
                    }

                    //=========================================================
                    // OLD check less strict
                    //When comparting two sets of textures (main, bump, spec ...) A and B that have different scales & offsets.They can share if:
                    //    - the scales of each texPropertyName (main, bump ...) are the same ratio: ASmain / BSmain = ASbump / BSbump = ASspec / BSspec
                    //    - the offset of A to B in uv space is the same for each texPropertyName:
                    //        offset = final - initial = OA / SB - OB must be the same
                    /*
                    MeshBakerMaterialTexture ma = ts[idxOfFirstNoneNull];
                    MeshBakerMaterialTexture mb = other.ts[idxOfFirstNoneNull];
                    //construct a rect that will ratio and offset
                    DRect r1 = new DRect(   (ma.matTilingRect.x / mb.matTilingRect.width - mb.matTilingRect.x),
                                            (ma.matTilingRect.y / mb.matTilingRect.height - mb.matTilingRect.y),
                                            (mb.matTilingRect.width / ma.matTilingRect.width),
                                            (mb.matTilingRect.height / ma.matTilingRect.height));
                    for (int i = 0; i < ts.Length; i++)
                    {
                        if (ts[i].t != null)
                        {
                            ma = ts[i];
                            mb = other.ts[i];
                            DRect r2 = new DRect(   (ma.matTilingRect.x / mb.matTilingRect.width - mb.matTilingRect.x),
                                                    (ma.matTilingRect.y / mb.matTilingRect.height - mb.matTilingRect.y),
                                                    (mb.matTilingRect.width / ma.matTilingRect.width),
                                                    (mb.matTilingRect.height / ma.matTilingRect.height));
                            if (Math.Abs(r2.x - r1.x) > 10e-10f) return false;
                            if (Math.Abs(r2.y - r1.y) > 10e-10f) return false;
                            if (Math.Abs(r2.width - r1.width) > 10e-10f) return false;
                            if (Math.Abs(r2.height - r1.height) > 10e-10f) return false;
                        }
                    }
                    */
                }
                return true;
            }
        }

        internal void DrawRectsToMergeGizmos(Color encC, Color innerC)
        {
            DRect r = ts[0].encapsulatingSamplingRect;
            r.Expand(.05f);
            Gizmos.color = encC;
            Gizmos.DrawWireCube(r.center.GetVector2(),r.size);
            for (int i = 0; i < matsAndGOs.mats.Count; i++)
            {
                DRect rr = matsAndGOs.mats[i].samplingRectMatAndUVTiling;
                DRect trans = MB3_UVTransformUtility.GetShiftTransformToFitBinA(ref r, ref rr);
                Vector2 xy = MB3_UVTransformUtility.TransformPoint(ref trans, rr.min);
                rr.x = xy.x;
                rr.y = xy.y;
                //Debug.Log("r " + r + " rr" + rr);
                Gizmos.color = innerC;
                Gizmos.DrawWireCube(rr.center.GetVector2(), rr.size);
            }
        }

        internal string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[GAME_OBJS=");
            for (int i = 0; i < matsAndGOs.gos.Count; i++)
            {
                sb.AppendFormat("{0},", matsAndGOs.gos[i].name);
            }
            sb.AppendFormat("MATS=");
            for (int i = 0; i < matsAndGOs.mats.Count; i++)
            {
                sb.AppendFormat("{0},", matsAndGOs.mats[i].GetMaterialName());
            }
            sb.Append("]");
            return sb.ToString();
        }

        internal string GetMatSubrectDescriptions()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < matsAndGOs.mats.Count; i++)
            {
                sb.AppendFormat("\n    {0}={1},", matsAndGOs.mats[i].GetMaterialName(), matsAndGOs.mats[i].samplingRectMatAndUVTiling);
            }
            return sb.ToString();
        }
    }

    class ProceduralMaterialInfo
    {
        public ProceduralMaterial proceduralMat;
        public bool originalIsReadableVal;
    }

}
