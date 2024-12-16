using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class FoodBlock : Block<Config.FoodType>
{
    private static readonly List<FoodBlock> s_SelectedBlocks = new();

    private static void ResetSelectedBlocks()
    {
        foreach (FoodBlock block in s_SelectedBlocks) block.ResetState();
        s_SelectedBlocks.Clear();
    }

    [SerializeField] private GameObject m_SelectionMark;
    [SerializeField] private GameObject m_ConnectionMark;

    public bool IsSelected { get; private set; }

    private FoodBlock m_ConnectedBlock;

    void FixedUpdate()
    {
        UpdateConnectionMark();
    }

    void OnMouseDown()
    {
        if (!Interactable) return;
        Select();
    }

    void OnMouseEnter()
    {
        if (!Interactable) return;
        if (IsSelected) return;
        if (Lightning.Selected != null)
        {
            Lightning.Selected.TryConnect(this);
            return;
        }
        if (s_SelectedBlocks.Count == 0) return;
        if (s_SelectedBlocks[^1].TryConnect(this)) Select();
    }

    async void OnMouseUp()
    {
        if (!Interactable) return;
        await TryPop();
        ResetSelectedBlocks();
    }

    private void Select()
    {
        IsSelected = true;
        m_SelectionMark.SetActive(true);
        s_SelectedBlocks.Add(this);

        DOTween.Sequence()
            .Append(m_SpritesContainer.DOScale(1.3f, 0.1f).SetEase(Ease.OutQuad))
            .Append(m_SpritesContainer.DOScale(1, 0.1f).SetEase(Ease.OutElastic));
    }

    private bool TryConnect(FoodBlock nextBlock)
    {
        if (OwnType != nextBlock.OwnType) return false;

        float sqrDistance = (nextBlock.transform.position - transform.position).sqrMagnitude;
        float maxSqrDistance = Mathf.Pow((Radius + nextBlock.Radius) * Config.CONNECTABLE_DISTANCE_FACTOR, 2);
        if (sqrDistance > maxSqrDistance) return false;

        m_ConnectedBlock = nextBlock;
        m_ConnectionMark.SetActive(true);
        UpdateConnectionMark();
        return true;
    }

    private async UniTask TryPop()
    {
        if (s_SelectedBlocks.Count < Config.CNT_TO_POP) return;
        await BlockManager.Instance.PopFoodBlocks(s_SelectedBlocks);
    }

    private void UpdateConnectionMark()
    {
        if (m_ConnectedBlock == null) return;

        Vector3 displacement = m_ConnectedBlock.transform.position - transform.position;
        float distance = displacement.magnitude;
        m_ConnectionMark.transform.localScale = new(distance, 1, 1);
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        m_ConnectionMark.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ResetState()
    {
        IsSelected = false;
        m_SelectionMark.SetActive(false);
        m_ConnectionMark.SetActive(false);
        m_ConnectedBlock = null;
    }
}
