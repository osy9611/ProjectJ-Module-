using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Unity.Core
{
    public class SpriteAtlasManager
    {
        private Dictionary<string, Sprite[]> bindDic = new Dictionary<string, Sprite[]>();
        private Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
        bool isRegisted = false;

        public void Register()
        {
            if (isRegisted)
                return;

            isRegisted = true;
            UnityEngine.U2D.SpriteAtlasManager.atlasRegistered += AtlasRegistered;
        }

        private void AtlasRegistered(UnityEngine.U2D.SpriteAtlas spriteAtlas)
        {
            Debug.LogFormat("Registered {0}.", spriteAtlas.name);
        }

        public void Add(UnityEngine.U2D.SpriteAtlas atlas)
        {
            Register();

            if (!bindDic.ContainsKey(atlas.name))
            {
                Sprite[] sprites = new Sprite[atlas.spriteCount];
                int result = atlas.GetSprites(sprites);
                for (int i = 0, range = sprites.Length; i < range; ++i)
                {
                    sprites[i].name = sprites[i].name.Substring(0, sprites[i].name.IndexOf("("));

                    spriteDic.Add(sprites[i].name, sprites[i]);
                }

                bindDic.Add(atlas.name, sprites);
            }
            else
            {
                throw new System.Exception(System.String.Format("thie sprite has already existed [name : {0}]", atlas.name));
            }
        }

        public void Remove(UnityEngine.U2D.SpriteAtlas atlas)
        {
            if (bindDic.ContainsKey(atlas.name))
            {
                Sprite[] sprites = bindDic[atlas.name];

                for (int i = 0, range = sprites.Length; i < range; ++i)
                {
                    spriteDic.Remove(sprites[i].name);
                }

                bindDic.Remove(atlas.name);
            }
        }

        public void Clear()
        {
            bindDic.Clear();
            spriteDic.Clear();
        }

        public Sprite Get(string name)
        {
            Sprite result = null;
            spriteDic.TryGetValue(name, out result);
            return result;
        }

    }
}