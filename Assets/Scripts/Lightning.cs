using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour, IItem
{
    public static Lightning Selected { get; private set; }

    [SerializeField] private GameObject m_SelectionMark;
    [SerializeField] private GameObject m_ConnectionMark;
    [SerializeField] private GameObject m_TargetSelectionMark;

    private FoodBlock m_Connected;

    void FixedUpdate()
    {
        UpdateConnectionMark();
    }

    public void Select()
    {
        Selected = this;
        m_SelectionMark.SetActive(true);
    }

    public bool TryConnect(FoodBlock block)
    {
        if (m_Connected != null) return false;

        m_Connected = block;
        m_ConnectionMark.SetActive(true);
        UpdateConnectionMark();
        return true;
    }

    public bool GetTargets(List<FoodBlock> targets)
    {
        if (m_Connected == null) return false;

        targets.Clear();
        BlockManager.Instance.ForEachBlocks(block => {
            FoodBlock foodBlock = block as FoodBlock;
            if (foodBlock == null) return;
            if (foodBlock.OwnType != m_Connected.OwnType) return;
            targets.Add(foodBlock);
        });
        return true;
    }

    public void ResetState()
    {
        Selected = null;
        m_SelectionMark.SetActive(false);
        m_ConnectionMark.SetActive(false);
        m_Connected = null;
    }

    private void UpdateConnectionMark()
    {
        if (m_Connected == null) return;

        Vector3 displacement = m_Connected.transform.position - transform.position;
        float distance = displacement.magnitude;
        m_ConnectionMark.transform.localScale = new(distance, 1, 1);
        m_TargetSelectionMark.transform.localScale = new(1 / distance, 1, 1);
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        m_ConnectionMark.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
