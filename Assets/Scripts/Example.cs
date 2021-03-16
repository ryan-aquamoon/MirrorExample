using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Example : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnMoneyChanged))]
    public int money;

    void HelloWorld()
    {
        Debug.Log("Hello World");
    }

    void OnMoneyChanged(int oldVal, int newVal)
    {
        Debug.Log("New money is: " + newVal);
    }
}


