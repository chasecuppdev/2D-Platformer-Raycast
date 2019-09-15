using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxResponder
{
    void collidedWith(Collider2D hurtbox);
}
