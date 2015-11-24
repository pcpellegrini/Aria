using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {

	public float velocidade = 0f;

	private Renderer textura;

	//private Vector2 loop = new Vector2(1, 1);

	// Use this for initialization
	void Start () {
		textura = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		textura.material.mainTextureOffset = textura.material.mainTextureOffset + new Vector2(velocidade, velocidade) * (Time.deltaTime/2);

		if (textura.material.mainTextureOffset.x >= 1) {
			textura.material.mainTextureOffset = new Vector2(0, 0);
		}
	}
}
