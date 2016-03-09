using UnityEngine;
using UnityEditor;

public class FlipVertices : ScriptableWizard
{
    [MenuItem("Tools/Flip")]
    static void Flip()
    {
        FlipVertices ca = (FlipVertices)DisplayWizard("Flip", typeof(FlipVertices), "Flip");
        ca.Show();
    }
	
	[SerializeField]
	Transform transform;
	
	[SerializeField]
	Vector3 _flipScale = Vector3.one;

	void OnEnable()
    {
		if (Selection.activeGameObject)
			transform = Selection.activeGameObject.transform;
    }
	
    void OnWizardCreate()
    {
		if (_flipScale != Vector3.one)
			FlipAllChilds(transform);
    }

	void FlipAllChilds(Transform root)
	{
		foreach(Transform child in root)
		{
			FlipTransform(child, _flipScale);
			FlipAllChilds(child);
		}
	}

	public void FlipTransform(Transform transform, Vector3 scale)
	{
		if (!(transform.GetComponent<MeshFilter>() && transform.GetComponent<MeshFilter>().mesh))
		{
			Debug.LogWarning(transform.name + " has no MeshFilter/mesh");
			return;
		}
		
		Mesh _mesh = transform.GetComponent<MeshFilter>().mesh;

		// Vertices flipping
		_mesh.vertices = ApplyFlipToTransform(transform, _mesh.vertices, _flipScale);

		// Flipping outside
		int[] triangles = _mesh.triangles;
		System.Array.Reverse(triangles);

		_mesh.triangles = triangles;
		
		EditorUtility.SetDirty(transform);
	}

	// Returns right vertices
	Vector3[] ApplyFlipToTransform(Transform tm, Vector3[] vectors, Vector3 scale)
	{
		for(int i=0; i<vectors.Length; i++)
		{
			Vector3 localSpace = vectors[i];
			Vector3 worldSpace = tm.TransformPoint(localSpace);
			Vector3 parentSpace = tm.parent.InverseTransformPoint(worldSpace);

			parentSpace.x *= scale.x;
			parentSpace.y *= scale.y;
			parentSpace.z *= scale.z;

			worldSpace = tm.parent.TransformPoint(parentSpace); 
			vectors[i] = tm.InverseTransformPoint(worldSpace);
		}

		return vectors;
	}
}
