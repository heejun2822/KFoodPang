public class Config
{
    public enum FoodType
    {
        Tteokbokki,
        Gyelanppang,
        Gimbap,
        Tteokkkochi,
        Bungeoppang,
    }

    public enum ItemType
    {
        Boom,
        Lightning,
    }

    public const int BLOCK_CAPACITY = 45;

    public const int MIN_CNT_TO_POP = 3;

    public const float CONNECTABLE_DISTANCE_FACTOR = 1.5f;

    public const float BOOM_RANGE_FACTOR = 2.0f;
}
