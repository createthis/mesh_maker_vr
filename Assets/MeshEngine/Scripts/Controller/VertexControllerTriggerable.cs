using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreateThis.VR.UI.Interact;

namespace MeshEngine.Controller {
    public class VertexControllerTriggerable : Triggerable {
        public override void OnTriggerDown(Transform controller, int controllerIndex) {
            base.OnTriggerDown(controller, controllerIndex);
            GetComponent<VertexController>().TriggerDown(controller, controllerIndex);
        }
    }
}