using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    void Start()
    {
        if (isLocalPlayer)
            CmdRandomPos();
    }

    [Command]
    void CmdRandomPos()
    {
        transform.position = new Vector2(Random.Range(-7.5f, 7.5f), Random.Range(-2.5f, 2.5f));
        RpcRandomPos(transform.position);
    }

    [ClientRpc]
    void RpcRandomPos(Vector2 pos)
    {
        transform.position = pos;
    }

}
