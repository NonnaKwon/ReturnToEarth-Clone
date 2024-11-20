using GooglePlayGames.BasicApi;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, Object> resources = new Dictionary<string, Object>();
    public AssetBundle UIsBundle { set { uisBundle = value; } }
    private AssetBundle uisBundle;
    
    #region assetbundle

    // -----> 권새롬 추가 : 에셋번들
    public T Load<T>(string key) where T : Object
    {
        if (typeof(T) == typeof(Sprite))
        {
            key = key + ".sprite";
            if (resources.TryGetValue(key, out Object temp))
                return temp as T;
        }

        if (resources.TryGetValue(key, out Object resource))
        {
            if (resource as T == null)
                return resource.GetComponent<T>();
            return resource as T;
        }

        return null;
    }
    

    public void Init(List<AssetBundle> bundles)
    {
        foreach(AssetBundle bundle in bundles)
        {
            Init(bundle);
        }
    }

    public void Init(AssetBundle bundle)
    {
        if (bundle == null)
        {
            return;
        }

        Object[] objects = bundle.LoadAllAssets(); //번들을 에셋으로 로드하는 함수 호출.
        foreach (Object obj in objects)
        {
            if (obj.name.Contains(".sprite")) //.sprite 이름이 적혀있으면 sprite하나만 넣기(Texture2D는 넣지않음. (키값이 겹친다))
            {
                if (obj as Sprite && resources.ContainsKey(obj.name) == false)
                    resources.Add(obj.name, obj);
                continue;
            }
            if (resources.ContainsKey(obj.name) == false)
                resources.Add(obj.name, obj);
        }
        if(bundle.name != "uis")
            bundle.Unload(false);
    }

    public void InitVideo()
    {
        VideoClip[] clips = Resources.LoadAll<VideoClip>("Video");
        for(int i=0;i<clips.Length;i++)
            resources.Add(clips[i].name, clips[i]);
    }

    public void Clear()
    {
        resources.Clear();
        Init(uisBundle); //공통으로 쓰는 데이터묶음
    }
    #endregion

    /// -------------------------
}
