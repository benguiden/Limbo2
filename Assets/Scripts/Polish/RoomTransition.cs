using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransition : MonoBehaviour {

    public RenderTexture transitionTexture;
    public RenderTexture blankTexture;
    public float pauseTime = 0.2f;
    public float transitionTime = 0.3f;
    public MeshRenderer meshRenderer;

    private Coroutine transitionCo;

    private void Awake() {
        transitionTexture = new RenderTexture (blankTexture);
        
    }

    public void ResetSize() {
        meshRenderer.transform.localScale = new Vector3 (Camera.main.orthographicSize * 2f * 1.7778f, Camera.main.orthographicSize * 2f, 1f);
    }

    public void Transition() {
        if (enabled) {
            if (transitionCo != null)
                StopCoroutine (transitionCo);

            transitionCo = StartCoroutine (ITransition ());
        }
    }

    private IEnumerator ITransition() {

        Camera newCamera = ((GameObject)Instantiate (Camera.main.gameObject, transform)).GetComponent<Camera> ();
        newCamera.transform.localPosition = Vector3.zero;
        newCamera.targetTexture = transitionTexture;

        yield return null;

        newCamera.enabled = false;
        newCamera.targetTexture = null;

        meshRenderer.gameObject.SetActive (true);
        float meshWidth = Camera.main.orthographicSize * 2f * 1.7778f;

        meshRenderer.transform.localPosition = new Vector3 (0f, 0f, 0.35f);
        meshRenderer.transform.localScale = new Vector3 (meshWidth, Camera.main.orthographicSize * 2f, 1f);

        meshRenderer.material.SetTextureOffset ("_MainTex", new Vector2 (0f, 0f));
        meshRenderer.material.mainTextureScale = new Vector2 (1f, 1f);

        
        meshRenderer.material.SetTextureOffset ("_MainTex", Vector2.zero);

        yield return new WaitForSeconds (pauseTime);
        
        float time = 0f;
        while (time < transitionTime) {
            time += Time.deltaTime;

            float theta = time / transitionTime;

            meshRenderer.transform.localPosition = new Vector3 (theta * meshWidth / 2f, 0f, 0.35f);
            meshRenderer.transform.localScale = new Vector3 (meshWidth - (theta * meshWidth), meshRenderer.transform.localScale.y, 1f);

            meshRenderer.material.SetTextureOffset ("_MainTex", new Vector2 (theta, 0f));
            meshRenderer.material.mainTextureScale = new Vector2 (1f - theta, 1f);
            yield return null;
        }

        meshRenderer.gameObject.SetActive (false);
        Destroy (newCamera.gameObject);
        transitionCo = null;
    }

}
