using System.Collections.Generic;
using UnityEngine;

public class AtlasManager : MonoBehaviour {
    public class AtlasedList
    {
        public Transform atlasedTransform;
        public Texture oldTexture;
        public Vector2[] oldUv;
    }

    public List<AtlasedList> atlasedLists = new List<AtlasedList>(); 
 
    public void AddNewAtlased(Transform _atlasedTransform , Vector2[] _uv , Texture _texture)
    {
        if (!IsAtlasedTransform(_atlasedTransform))
        {
            atlasedLists.Add(new AtlasedList());
            atlasedLists[atlasedLists.Count - 1].atlasedTransform = _atlasedTransform;
            atlasedLists[atlasedLists.Count - 1].oldUv = _uv;
            atlasedLists[atlasedLists.Count - 1].oldTexture = _texture;
        }
    }

    public void UpdateOldTexture(Transform _oldTransform , Texture _texture)
    {
        for(int i = 0 ; i <atlasedLists.Count ; i ++)
        {
            if (_oldTransform == atlasedLists[i].atlasedTransform)
                atlasedLists[i].oldTexture = _texture;
        }
    }

    public bool IsAtlasedTransform(Transform _atlasedTransform)
    {
        for (int i = atlasedLists.Count - 1; i >= 0 ; i--)
        {
            if (atlasedLists[i].atlasedTransform == _atlasedTransform)
                return true;

            if (atlasedLists[i].atlasedTransform == null)
                atlasedLists.RemoveAt(i);
        }
        return false;
    }

    public Texture GetOldTexture(Transform _atlasedTransform)
    {
        for (int i = 0; i < atlasedLists.Count; i++)
        {
            if (_atlasedTransform == atlasedLists[i].atlasedTransform)
                return atlasedLists[i].oldTexture;
        }

        return null;
    }

    public Vector2[] GetAtlasedUv(Transform _atlasedTransform)
    {
        for (int i = 0; i < atlasedLists.Count; i++)
        {
            if (_atlasedTransform == atlasedLists[i].atlasedTransform)
                return atlasedLists[i].oldUv;
        }

        return null;
    }

    public void RemoveAtlasedObject(Transform _atlasedTransform)
    {
        _atlasedTransform.renderer.sharedMaterial = null;
        _atlasedTransform.GetComponent<MeshFilter>().sharedMesh.uv = GetAtlasedUv(_atlasedTransform);
        
        for(int i = 0 ;i < atlasedLists.Count;i++)
        {
            if(atlasedLists[i].atlasedTransform == _atlasedTransform)
                atlasedLists.RemoveAt(i);
        }
    }
}
