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

    public enum AudioId
    {
        None = 0,
        BGM_Title = 100,
        BGM_InGame = 101,
        BGM_Fever = 102,
        SFX_ButtonClicked = 200,
        SFX_BlockSelected = 201,
        SFX_BlockPoped = 202,
        SFX_Boom = 203,
        SFX_Lightning = 204,
    }

    public const int BLOCK_CAPACITY = 45;

    public const int CNT_TO_POP = 3;
    public const int CNT_TO_GET_BOOM = 7;
    public const int COMBO_TO_GET_LIGHTNING = 7;

    public const float CONNECTABLE_DISTANCE_FACTOR = 1.5f;

    public const float BOOM_RANGE_FACTOR = 2.0f;

    public const int TIME_LIMIT = 60;

    public const float COMBO_DURATION = 2.5f;

    public const int BASIC_BLOCK_SCORE = 100;

    public const float FEVER_TIME_SCORE_FACTOR = 2.0f;

    public const float FEVER_GAUGE_PER_BLOCK = 1;

    public const int MAX_FEVER_GAUGE = 40;

    public const int FEVER_TIME_DURATION = 10;

    public const float SHAKING_IMPULSE = 6;
}
