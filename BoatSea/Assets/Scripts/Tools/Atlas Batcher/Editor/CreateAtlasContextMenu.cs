using UnityEditor;
using UnityEngine;

public class CreateAtlasContextMenu : MonoBehaviour
{
    [MenuItem("Assets/Create/3D Atlas")]
    public static void Create()
    {
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (System.IO.Path.GetExtension (path) != "") 
		{
			path = path.Replace (System.IO.Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		Material material = new Material(Shader.Find("Diffuse"));
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New 3D Atlas.mat");
 
		AssetDatabase.CreateAsset (material, assetPathAndName);
 
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = material;
		
        CreateAtlas ca = (CreateAtlas)ScriptableWizard.DisplayWizard("Build Atlases", typeof(CreateAtlas), "Create", "Assign textures");
        ca.Show();
		ca.atlasMaterial = material;
	}
}