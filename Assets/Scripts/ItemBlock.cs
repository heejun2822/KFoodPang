using System.Collections.Generic;

public class ItemBlock : Block<Config.ItemType>
{
    public static ItemBlock Selected { get; private set; }
    private static readonly List<FoodBlock> s_TargetBlocks = new();

    public static void TryPop()
    {
        if (Selected == null) return;

        if (Selected.GetTargets(s_TargetBlocks))
        {
            FoodBlock[] targetBlocks = new FoodBlock[s_TargetBlocks.Count];
            s_TargetBlocks.CopyTo(targetBlocks);
            BlockManager.Instance.PopItemBlock(Selected, targetBlocks).Forget();
        }
        else
        {
            Selected.ResetState();
        }
        Selected = null;
        s_TargetBlocks.Clear();
    }

    private Boom m_Boom;
    private Lightning m_Lightning;

    private IItem m_Item;

    protected override void Awake()
    {
        base.Awake();
        m_Boom = GetComponent<Boom>();
        m_Lightning = GetComponent<Lightning>();
    }

    void OnMouseDown()
    {
        if (!Interactable) return;
        Select();
    }

    void OnMouseUp()
    {
        if (!Interactable) return;
        TryPop();
    }

    public override void SetType(Config.ItemType type)
    {
        base.SetType(type);

        if (type == Config.ItemType.Boom) m_Item = m_Boom;
        else if (type == Config.ItemType.Lightning) m_Item = m_Lightning;
    }

    private void Select()
    {
        Selected = this;
        m_Item.Select();
    }

    public bool TryConnect(FoodBlock block)
    {
        return m_Item.TryConnect(block);
    }

    private bool GetTargets(List<FoodBlock> targets)
    {
        return m_Item.GetTargets(targets);
    }

    public void ResetState()
    {
        m_Item.ResetState();
    }
}
