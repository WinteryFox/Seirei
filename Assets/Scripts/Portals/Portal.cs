using UnityEngine;

namespace Portals {
    public class Portal : MonoBehaviour {
        public Portal link;
        public MeshRenderer screen;
        private Camera playerCamera;
        private Camera portalCamera;
        private RenderTexture renderTarget;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        //private static readonly int PortalTexture = Shader.PropertyToID("_PortalTexture");

        private void Awake() {
            playerCamera = Camera.main;
            portalCamera = GetComponentInChildren<Camera>();
            portalCamera.enabled = false;
        }

        public void render() {
            screen.enabled = false;
        
            if (renderTarget == null || renderTarget.width != Screen.width || renderTarget.height != Screen.height) {
                if (renderTarget != null)
                    renderTarget.Release();
                renderTarget = new RenderTexture(Screen.width, Screen.height, 0);
                portalCamera.targetTexture = renderTarget;
                link.screen.material.SetTexture(MainTex, renderTarget);
            }
        
            var m = transform.localToWorldMatrix * link.transform.worldToLocalMatrix * playerCamera.transform.localToWorldMatrix;
            portalCamera.transform.SetPositionAndRotation (m.GetColumn (3), m.rotation);
            portalCamera.Render();

            screen.enabled = true;
        }
    }
}
