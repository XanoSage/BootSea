using UnityEngine;
using System.Collections;

public class A
{
    public int i = 5;
    public int j = 6;
}

public class B: A
{
    public int g = 10;
}
public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	A a=new A();
	    B b= new B();
	    b.i = 0;
	    b.j = 0;
	    //b = a as B;*/
	    a = b;
        //Debug.Log(	a.i.ToString());

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
