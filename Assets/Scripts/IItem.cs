using System.Collections.Generic;

public interface IItem
{
    public void Select();
    public bool TryConnect(FoodBlock block);
    public bool GetTargets(List<FoodBlock> targets);
    public void ResetState();
}
