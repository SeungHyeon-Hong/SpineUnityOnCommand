using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.IO;

/*
    source : 
    https://github.com/EsotericSoftware/spine-runtimes/tree/4.0/spine-unity/Assets/Spine%20Examples

    how to : 
    1.  Edit > Project Settings > Graphics 
        You must add shader name "Spine/Skeleton" to 'Built-in Shader Settings'.
    2.  Check the 'Example.cs' 

 */

namespace Spine.OnCommand
{
    static public class SpineUnityOnCommand
    {
        static Dictionary<string, SkeletonDataAsset> SkeletonDataAssetDict = new Dictionary<string, SkeletonDataAsset>();

        static public void LoadSkeletonDataAsset(string keyname, string path)
        {
            if (SkeletonDataAssetDict.ContainsKey(keyname))
            {
                Debug.LogError("keyname Already Exist!!");
                return;
            }
            if (!Directory.Exists(path))
            {
                Debug.LogError("path Not Exist!!");
                return;
            }


            //------------------- TextAsset.
            // Load TextAsset datas from path.
            string[] files = Directory.GetFiles(path);
            int jsonidx = -1;
            int atlasidx = -1;
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".json")) jsonidx = i;
                else if (files[i].EndsWith("atlas.txt")) atlasidx = i;
            }
            TextAsset skeletonJson = jsonidx < 0 ? null : new TextAsset(File.ReadAllText(files[jsonidx]));
            TextAsset atlasText = atlasidx < 0 ? null : new TextAsset(File.ReadAllText(files[atlasidx]));

            if (skeletonJson == null || atlasText == null)
            {
                Debug.LogError("TextAsset Data Fail!!");
                return;
            }


            //------------------- Texture.
            // Load Textures info from (TextAsset)atlasText.
            string atlasString = atlasText.text;
            atlasString = atlasString.Replace("\r", "");
            string[] atlasLines = atlasString.Split('\n');
            List<string> pngNames = new List<string>();
            List<string> pngSizes = new List<string>();
            for (int i = 0; i < atlasLines.Length - 1; i++)
            {
                string line = atlasLines[i].Trim();
                if (line.EndsWith(".png"))
                {
                    if (File.Exists(path + "/" + line))
                    {
                        pngNames.Add(line);
                    }
                    else
                    {
                        Debug.LogError("No PNG : " + path + "/" + line);
                        return;
                    }
                }
                else if (line.StartsWith("size:"))
                {
                    pngSizes.Add(line.Replace("size:", ""));
                }
            }

            if (pngNames.Count <= 0)
            {
                Debug.LogError("PNG No Exist!");
                return;
            }

            // Create Texture2D[] from loaded texture data.
            Texture2D[] textures = new Texture2D[pngNames.Count];
            for (int i = 0; i < pngNames.Count; i++)
            {
                //load data.
                byte[] data = File.ReadAllBytes(path + "/" + pngNames[i]);

                //get size.
                string[] size = pngSizes[i].Split(',');

                //create texture.
                //rename for serch from spine library.
                Texture2D tex = new Texture2D(int.Parse(size[0]), int.Parse(size[1]));
                ImageConversion.LoadImage(tex, data);
                tex.name = pngNames[i].Replace(".png", "");
                textures[i] = tex;
            }


            //------------------- Mtrl.
            // Edit > Project Settings > Graphics 
            // You must add shader name "Spine/Skeleton" to 'Built-in Shader Settings'.
            Material materialPropertySource = new Material(Shader.Find("Spine/Skeleton"));


            //------------------- SkeletonDataAsset.
            // 1. Create the AtlasAsset (needs atlas text asset and textures, and materials/shader);
            // 2. Create SkeletonDataAsset (needs json or binary asset file, and an AtlasAsset)
            // 3. Create SkeletonAnimation (needs a valid SkeletonDataAsset)

            SpineAtlasAsset runtimeAtlasAsset = SpineAtlasAsset.CreateRuntimeInstance(atlasText, textures, materialPropertySource, true);
            SkeletonDataAsset runtimeSkeletonDataAsset = SkeletonDataAsset.CreateRuntimeInstance(skeletonJson, runtimeAtlasAsset, true);

            runtimeSkeletonDataAsset.GetSkeletonData(false); // preload.


            //------------------- Save in Stack.
            SkeletonDataAssetDict.Add(keyname, runtimeSkeletonDataAsset);

        }

        static public void ReleseSkeletonDataAsset(string keyname)
        {
            // boolean dictionary.Remove(Tkey);
            // If the keyname not exist in it, nothing happens :D
            SkeletonDataAssetDict.Remove(keyname);
        }

        static public SkeletonDataAsset GetCommandSkeletonDataAsset(string keyname)
        {
            SkeletonDataAsset result;
            return SkeletonDataAssetDict.TryGetValue(keyname, out result) ? result : null;
        }

    }
}