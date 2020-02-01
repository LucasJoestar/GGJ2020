using EnhancedEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Warp : MonoBehaviour
{
    #region Fields / Properties
    /**********************
     *****   FIELDS   *****
     *********************/

    [SerializeField]
    private bool                isXWarp =                   true;

    [SerializeField]
    private bool                isXAndYChanged =            false;


    [SerializeField]
    private new BoxCollider2D   collider =                  null;

    [SerializeField]
    private Transform           teleportTransform =         null;
    #endregion

    #region Methods
    public void TryToTeleport(MyPlayercontroller _myPlayer)
    {
        if (isXWarp)
        {
            // Teleport if exit map
            if (Mathf.Sign(transform.position.x - _myPlayer.transform.position.x) == Mathf.Sign(transform.position.x - teleportTransform.position.x)) return;

            _myPlayer.transform.position = new Vector3(teleportTransform.position.x, isXAndYChanged ? teleportTransform.position.y : _myPlayer.transform.position.y, _myPlayer.transform.position.z);
            _myPlayer.Flip();
        }
        else
        {
            // Teleport if exit map
            if (Mathf.Sign(transform.position.y - _myPlayer.transform.position.y) == Mathf.Sign(transform.position.y - teleportTransform.position.y)) return;

            _myPlayer.transform.position = new Vector3(_myPlayer.transform.position.x, teleportTransform.transform.position.y, _myPlayer.transform.position.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (!teleportTransform) return;
        Gizmos.color = isXWarp ? isXAndYChanged ? SuperColor.Purple.GetColor() : SuperColor.Turquoise.GetColor() : SuperColor.Pumpkin.GetColor();
        Gizmos.DrawCube(teleportTransform.position, Vector3.one * .5f);

        if (!collider) return;
        Gizmos.color = isXWarp? isXAndYChanged ? SuperColor.Purple.GetColor(.25f) : SuperColor.Turquoise.GetColor(.25f) : SuperColor.Pumpkin.GetColor(.25f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }
    #endregion
}
