using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EVENT_TYPE
{
    JUMP,
    ATTACK,
    MOVE
};

public interface IListener
{
    void OnEvent(EVENT_TYPE event_type, Component Sender, Object param = null);
}
