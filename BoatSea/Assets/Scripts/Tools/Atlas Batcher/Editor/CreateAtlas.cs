using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class TextureList
{
    public ArrayList textures = new ArrayList();
    public Rect[] uvs;
}

public class CreateAtlas : ScriptableWizard
{
    public enum TEXTURE_FORMAT
    {
        TGA,
        JPG,
        PNG
    }
    public Material atlasMaterial;
    public bool createNewMesh = true;
    public string SavingAtlasFilePath;
    public TEXTURE_FORMAT savingAtlasFileFormat;
    public GameObject[] gameObjects = new GameObject[0];

    public Texture[] textures = new Texture[0];
    private List<GameObject> GO = new List<GameObject>();
    private List<Texture> tex = new List<Texture>();

    private string saveAtlasPath;
    private string loadAtlasPath;
    private string meshFilePath;

    private string warnings;

    private AtlasManager atlasManagerScript;

    void OnEnable()
    {
        if (!GameObject.Find("AtlasManager"))
        {
            GameObject atlasManagerGO = new GameObject("AtlasManager");
            atlasManagerScript = atlasManagerGO.AddComponent("AtlasManager") as AtlasManager;
        }
        else
        {
            atlasManagerScript = GameObject.Find("AtlasManager").GetComponent<AtlasManager>();
            if (atlasManagerScript == null)
                atlasManagerScript = GameObject.Find("AtlasManager").AddComponent("AtlasManager") as AtlasManager;
        }
    }

    void OnWizardCreate()
    {
        if (atlasMaterial == null)
            AddWarning("Please Select using material from asset.");
        else if (gameObjects.Length == 0)
            AddWarning("Please Select using GameObject or GameObjects.");
        else
        {
            FindChildsGOAndTex();
            BuildAtlas(GenerateTextureList());
        }
        ShowWarnings();
    }

    void OnWizardUpdate()
    {
        if (gameObjects.Length != textures.Length)
            textures = new Texture[gameObjects.Length];
			//TexturesAutoAssignment();
    }
	
	void OnWizardOtherButton()
	{
        if (gameObjects.Length != textures.Length)
            textures = new Texture[gameObjects.Length];
		TexturesAutoAssignment();
	}
	
	void TexturesAutoAssignment()
	{
		for(int i=0; i<textures.Length; i++)
		{
			if (gameObjects[i])
			{
				if (gameObjects[i].renderer && gameObjects[i].renderer.sharedMaterial)
				{
					textures[i] = gameObjects[i].renderer.sharedMaterial.mainTexture;
				}
				else if (!gameObjects[i].renderer)
				{
					MeshRenderer childRenderer = gameObjects[i].GetComponentInChildren<MeshRenderer>();
					if (childRenderer && childRenderer.sharedMaterial)
						textures[i] = childRenderer.sharedMaterial.mainTexture;
				}
			}
		}
	}

    [MenuItem("Tools/Create Atlas")]
    static void BuildSpriteAtlases()
    {
        CreateAtlas ca = (CreateAtlas)DisplayWizard("Build Atlases", typeof(CreateAtlas), "Create", "Assign textures");
        ca.Show();
    }

    void BuildAtlas(TextureList texList)
    {
        if (GO.Count > 0)
        {
            SetFolderOptions();
            Texture2D atlas = new Texture2D(4096, 4096);
            texList.uvs = atlas.PackTextures((Texture2D[])texList.textures.ToArray(typeof(Texture2D)), 1, 4096);
            byte[] bytes = atlas.EncodeToPNG();


            using (FileStream fs = File.Create(saveAtlasPath))
            {
                fs.Write(bytes, 0, bytes.Length);
            }

            Vector2[] oldUV, newUV;
            for (int i = 0; i < GO.Count; i++)
            {
                if (createNewMesh)
                    CreateNewMesh(GO[i]);

                oldUV = atlasManagerScript.GetAtlasedUv(GO[i].transform);
                newUV = new Vector2[oldUV.Length];

                for (int j = 0; j < oldUV.Length; j++)
                {
                    newUV[j] = new Vector2((oldUV[j].x * texList.uvs[i].width) + texList.uvs[i].x,
                             (oldUV[j].y * texList.uvs[i].height) + texList.uvs[i].y);
                }
                GO[i].GetComponent<MeshFilter>().sharedMesh.uv = newUV;
                GO[i].renderer.sharedMaterial = atlasMaterial;
            }
            SetAssetOptions();
            Debug.Log(GO.Count + " object(s) successfully atlased. Do not delete your original texture and AtlasManager. If you make sure finished your project then you can clear.");
        }
        else
            AddWarning("No objects have meshFilter.Objects must have meshFilter for atlas.");


    }

    private void SetFolderOptions()
    {
        if (!string.IsNullOrEmpty(SavingAtlasFilePath))
        {
            if (SavingAtlasFilePath[SavingAtlasFilePath.Length - 1] != '/')
                SavingAtlasFilePath += "/";
            if (SavingAtlasFilePath[0] != '/')
                SavingAtlasFilePath = "/" + SavingAtlasFilePath;
        }
        else
            SavingAtlasFilePath = "/";

        meshFilePath = SavingAtlasFilePath;
        try
        {
            bool pathControl = true;
            for (int i = 0; i < SavingAtlasFilePath.Length - 2; i++)
            {
                if (SavingAtlasFilePath[i].ToString() + SavingAtlasFilePath[i + 1].ToString() == "//")
                {
                    pathControl = false;
                    break;
                }

            }
            if (pathControl)
            {
                Directory.CreateDirectory(Application.dataPath + SavingAtlasFilePath);
                saveAtlasPath = Application.dataPath + SavingAtlasFilePath + atlasMaterial.name + "." + savingAtlasFileFormat.ToString();

                loadAtlasPath = "Assets" + SavingAtlasFilePath + atlasMaterial.name + "." + savingAtlasFileFormat.ToString();
            }
            else
            {
                saveAtlasPath = Application.dataPath + "/" + atlasMaterial.name + "." + savingAtlasFileFormat.ToString();
                loadAtlasPath = "Assets/" + atlasMaterial.name + "." + savingAtlasFileFormat.ToString();
            }
        }
        catch
        {
            saveAtlasPath = Application.dataPath + "/" + atlasMaterial.name + "." + savingAtlasFileFormat.ToString();
            loadAtlasPath = "Assets/" + atlasMaterial.name + "." + savingAtlasFileFormat.ToString();
        }
    }

    private void SetAssetOptions()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Texture tex = (Texture)AssetDatabase.LoadAssetAtPath(loadAtlasPath, typeof(Texture));
        atlasMaterial.SetTexture("_MainTex", tex);
    }

    private TextureList GenerateTextureList()
    {
        TextureList texList = new TextureList();
        for (int i = 0; i < tex.Count; i++)
        {
            if (tex[i] != null)
                SetTextureFormat((Texture2D)tex[i]);

            texList.textures.Add(tex[i]);
        }
        return texList;
    }

    private void SetTextureFormat(Texture2D _tex)
    {
        string texturePath;
        texturePath = AssetDatabase.GetAssetPath(_tex);
        TextureImporter imp = (TextureImporter)TextureImporter.GetAtPath(texturePath);

        if (!imp.isReadable || imp.textureFormat != TextureImporterFormat.ARGB32 || imp.npotScale != TextureImporterNPOTScale.None)
        {
            imp.isReadable = true;
            imp.textureFormat = TextureImporterFormat.ARGB32;
            imp.npotScale = TextureImporterNPOTScale.None;
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);
        }
    }

    private void CreateNewMesh(GameObject GO)
    {
        Mesh currentMesh = GO.GetComponent<MeshFilter>().sharedMesh;
        Mesh mesh = new Mesh();
        mesh.vertices = currentMesh.vertices;
        mesh.triangles = currentMesh.triangles;
        mesh.tangents = currentMesh.tangents;
        mesh.uv = currentMesh.uv;
        mesh.normals = currentMesh.normals;
        mesh.triangles = currentMesh.triangles;
        mesh.name = GO.name;
        SaveNewMesh(mesh);
        GO.GetComponent<MeshFilter>().sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets" + meshFilePath + GO.name + ".asset", typeof(Mesh));
    }

    private void SaveNewMesh(Mesh mesh)
    {
        AssetDatabase.CreateAsset(mesh, "Assets" + meshFilePath + mesh.name + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void FindChildsGOAndTex()
    {
        MeshFilter[] meshFilters;

        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i] != null)
            {
                meshFilters = gameObjects[i].GetComponentsInChildren<MeshFilter>();
                for (int j = 0; j < meshFilters.Length; j++)
                {
                    if (meshFilters[j].sharedMesh != null)
                    {
                        if (((textures[i] != null && meshFilters[j].gameObject == gameObjects[i]) || atlasManagerScript.IsAtlasedTransform(meshFilters[j].transform)) && meshFilters[j].transform.renderer == null)
                        {
                            meshFilters[j].gameObject.AddComponent<MeshRenderer>();
                        }

                        if (meshFilters[j].transform.renderer != null)
                        {
                            if (meshFilters[j].gameObject == gameObjects[i] && textures[i] != null)
                            {
                                GO.Add(meshFilters[j].gameObject);
                                tex.Add(textures[i]);
                                if (atlasManagerScript.IsAtlasedTransform(meshFilters[j].transform))
                                    atlasManagerScript.UpdateOldTexture(meshFilters[j].transform, textures[i]);
                                else
                                    atlasManagerScript.AddNewAtlased(meshFilters[j].transform, meshFilters[j].GetComponent<MeshFilter>().sharedMesh.uv, textures[i]);
                            }
                            else if (atlasManagerScript.IsAtlasedTransform(meshFilters[j].transform))
                            {
                                Texture oldTexture = atlasManagerScript.GetOldTexture(meshFilters[j].transform);
                                if (oldTexture != null)
                                {
                                    GO.Add(meshFilters[j].gameObject);
                                    tex.Add(oldTexture);
                                }
                                else
                                {
                                    atlasManagerScript.RemoveAtlasedObject(meshFilters[j].transform);
                                    AddWarning("'" + meshFilters[j].gameObject + "'" + "object did not find orjinal texture(before to Atlased Texture).");
                                }


                            }
                            else if (meshFilters[j].transform.renderer.sharedMaterial != null)
                            {
                                if (meshFilters[j].transform.renderer.sharedMaterial.mainTexture != null)
                                {
                                    GO.Add(meshFilters[j].gameObject);
                                    tex.Add(meshFilters[j].transform.renderer.sharedMaterial.mainTexture);
                                    atlasManagerScript.AddNewAtlased(meshFilters[j].transform, meshFilters[j].GetComponent<MeshFilter>().sharedMesh.uv, meshFilters[j].transform.renderer.sharedMaterial.mainTexture);
								}
                            }
                        }
                    }
                }
            }

        }
    }

    private void AddWarning(string newWarning)
    {
        if (warnings != null)
            warnings += "\n";
        warnings = warnings + newWarning;
    }

    private void ShowWarnings()
    {
        if (warnings != null)
            Debug.LogWarning(warnings);
        warnings = null;
    }
}