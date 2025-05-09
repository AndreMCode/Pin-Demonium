using UnityEngine;

public class GameEvent : MonoBehaviour
{
    // Game
    public const string TOGGLE_GAME_POPUP = "TOGGLE_GAME_POPUP";

    // UI
    public const string PLAYER_HEALTH_UPDATE = "PLAYER_HEALTH_UPDATE";
    public const string PLAYER_SCORE_UPDATE = "PLAYER_SCORE_UPDATE";
    public const string LEVEL_COMPLETE_TXT = "LEVEL_COMPLETE_TXT";
    public const string CARRY_OVER_SCORE = "CARRY_OVER_SCORE";
    public const string GET_READY_TXT = "GET_READY_TXT";
    public const string GAME_OVER_TXT = "GAME_OVER_TXT";
}
