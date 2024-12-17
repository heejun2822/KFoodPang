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

    public const int CNT_TO_POP = 3;
    public const int CNT_TO_GET_BOOM = 7;

    public const float CONNECTABLE_DISTANCE_FACTOR = 1.5f;

    public const float BOOM_RANGE_FACTOR = 2.0f;

    // public const int TIME_LIMIT = 60;
    public const int TIME_LIMIT = 5;

    public const int COMBO_DURATION = (int)(2.5f * 1000);

    public const int BASIC_BLOCK_SCORE = 100;
}
