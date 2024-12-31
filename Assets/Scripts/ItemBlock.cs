using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class ItemBlock : Block<Config.ItemType>
{
    private static readonly List<FoodBlock> s_TargetBlocks = new();

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
        m_Item.Select();
    }

    async void OnMouseUp()
    {
        if (!Interactable) return;
        await TryPop();
        m_Item.ResetState();
    }

    public override void SetType(Config.ItemType type)
    {
        base.SetType(type);

        if (type == Config.ItemType.Boom) m_Item = m_Boom;
        else if (type == Config.ItemType.Lightning) m_Item = m_Lightning;
    }

    private async UniTask TryPop()
    {
        if (!m_Item.GetTargets(s_TargetBlocks)) return;
        await BlockManager.Instance.PopItemBlock(this, s_TargetBlocks);
    }
}
