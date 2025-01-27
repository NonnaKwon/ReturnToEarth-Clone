using UnityEngine;

public class InGameUI : BaseUI
{
    public Transform followTarget;
    public Vector3 followOffset;

    public override void LocalUpdate()
    {

    }

    private void LateUpdate()
    {
        if (followTarget != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(followTarget.position) + followOffset;
        }
    }

    public void Close()
    {
        Manager.UI.CloseInGameUI();
    }
}
