using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDoor : InteractSystem
{
    public override void Interact()
    {
        base.Interact();
        Destroy(this.gameObject); // animation in the future
    }
}
