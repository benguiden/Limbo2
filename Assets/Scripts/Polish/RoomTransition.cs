using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class RoomTransition : MonoBehaviour {

    public RenderTexture transitionTexture;
    public RenderTexture blankTexture;
    public float pauseTime = 0.2f;
    public float transitionTime = 0.3f;
    public MeshRenderer meshRenderer;
    
    private Coroutine transitionCo;
    private PostProcessingBehaviour post;

    private void Awake() {
        transitionTexture = new RenderTexture (blankTexture);
        post = GetComponent<PostProcessingBehaviour> ();
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

        ChromaticAberrationModel.Settings chromeSettings = new ChromaticAberrationModel.Settings ();
        MotionBlurModel.Settings blurSettings = new MotionBlurModel.Settings ();

        float time = 0f;
        while (time < transitionTime) {
            time += Time.deltaTime;

            float theta = time / transitionTime;

            meshRenderer.transform.localPosition = new Vector3 (theta * meshWidth / 2f, 0f, 0.35f);
            meshRenderer.transform.localScale = new Vector3 (meshWidth - (theta * meshWidth), meshRenderer.transform.localScale.y, 1f);

            meshRenderer.material.SetTextureOffset ("_MainTex", new Vector2 (theta, 0f));
            meshRenderer.material.mainTextureScale = new Vector2 (1f - theta, 1f);

            chromeSettings.intensity = Mathf.Sin (theta * Mathf.PI);
            post.profile.chromaticAberration.settings = chromeSettings;

            blurSettings.frameBlending = Mathf.Sin (theta * Mathf.PI);
            post.profile.motionBlur.settings = blurSettings;

            yield return null;
        }
        
        chromeSettings.intensity = 0f;
        post.profile.chromaticAberration.settings = chromeSettings;

        blurSettings.frameBlending = 0f;
        post.profile.motionBlur.settings = blurSettings;

        meshRenderer.gameObject.SetActive (false);
        Destroy (newCamera.gameObject);
        transitionCo = null;
    }

}
