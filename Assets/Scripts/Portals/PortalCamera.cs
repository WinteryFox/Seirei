using UnityEngine;

namespace Portals {
    public class PortalCamera : MonoBehaviour {
        private Portal[] portals;

        private void Awake() {
            portals = FindObjectsOfType<Portal>();
        }

        private void OnPreCull() {
            foreach (var portal in portals)
                portal.render();
        }
    }
}
