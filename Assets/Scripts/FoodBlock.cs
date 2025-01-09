using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FoodBlock : Block<Config.FoodType>
{
    private static readonly List<FoodBlock> s_SelectedBlocks = new();

    public static void TryPop()
    {
        if (s_SelectedBlocks.Count >= Config.CNT_TO_POP)
        {
            FoodBlock[] selectedBlocks = new FoodBlock[s_SelectedBlocks.Count];
            s_SelectedBlocks.CopyTo(selectedBlocks);
            BlockManager.Instance.PopFoodBlocks(selectedBlocks).Forget();
        }
        else
        {
            foreach (FoodBlock block in s_SelectedBlocks) block.ResetState();
        }
        s_SelectedBlocks.Clear();
    }

    [SerializeField] private GameObject m_SelectionMark;
    [SerializeField] private GameObject m_ConnectionMark;

    public bool IsSelected { get; set; }

    private FoodBlock m_ConnectedBlock;

    private Tween m_BouncingTween;

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
        if (ItemBlock.Selected != null)
        {
            ItemBlock.Selected.TryConnect(this);
            return;
        }
        if (s_SelectedBlocks.Count == 0) return;
        if (s_SelectedBlocks[^1].TryConnect(this)) Select();
    }

    void OnMouseUp()
    {
        if (!Interactable) return;
        TryPop();
    }

    private void Select()
    {
        IsSelected = true;
        m_SelectionMark.SetActive(true);
        s_SelectedBlocks.Add(this);
        Bounce();

        AudioManager.Instance.PlaySfx(Config.AudioId.SFX_BlockSelected);
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

    private void Bounce()
    {
        m_BouncingTween ??= DOTween.Sequence()
            .Append(m_SpritesContainer.DOScale(1.3f, 0.1f).SetEase(Ease.OutQuad))
            .Append(m_SpritesContainer.DOScale(1, 0.1f).SetEase(Ease.OutElastic))
            .SetAutoKill(false);
        m_BouncingTween.Restart();
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

    public void ResetState()
    {
        IsSelected = false;
        m_SelectionMark.SetActive(false);
        m_ConnectionMark.SetActive(false);
        m_ConnectedBlock = null;
    }
}
