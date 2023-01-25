using ObjectPoolCP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DeadZone: MonoBehaviour
{
    #region Editor Gizmo
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new(1f, 0f, 0f, 0.4f);
        Gizmos.DrawCube(this.transform.position, this.transform.localScale);
    }
#endif
    #endregion

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(nameof(Ingredient)))
        {
            collision.TryGetComponent<Ingredient>(out Ingredient ingred);

            if (!ingred.IsCliked)
            {
                GameObject instVfx = ingred.IngredData.delVfx;
                PoolCp.Inst.BringObjectCp(instVfx).transform.position = collision.transform.position;
                PoolCp.Inst.DestoryObjectCp(ingred.gameObject);
            }
        }
    }
}
