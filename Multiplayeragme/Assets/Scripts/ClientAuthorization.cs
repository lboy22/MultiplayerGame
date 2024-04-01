using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

[DisallowMultipleComponent]
public class ClientAuthorization : NetworkTransform
{
    // This is a script that allows the player to move freely as allowed by the player movement).
    // This measure of allowing is not the best for securty measures, but for this class this is sufficient.
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}

