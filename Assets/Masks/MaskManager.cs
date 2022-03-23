using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour {
    public List<Mask> masks;
    
    void Start() {
        
    }

    void Update() {
        
    }
}

public interface Mask {
    public void useAbility();
}

public class FoxMask : Mask {
    public void useAbility () {
        
    }
}
