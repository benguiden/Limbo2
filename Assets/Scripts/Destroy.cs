using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("DestroyAfter", 1.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void DestroyAfter() {
        Destroy(this.gameObject);
    }
}
