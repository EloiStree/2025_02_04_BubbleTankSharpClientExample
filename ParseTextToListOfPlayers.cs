public class ParseTextToListOfPlayers { 

    public static void PushInAsUtf8TextFromWeb(
        string textReceived,
        out List<STRUCT_IntegerPlayerInGameInfo> playerStateInGames)
    {

        playerStateInGames = new List<STRUCT_IntegerPlayerInGameInfo>();
        //`PLAYER_ID:PLAYER_LOBBY_ID:TEAM_ID:POSITION_X:POSITION_Y:POSITIONZ:ROTATION_X:ROTATION_Y:ROTATION_Z:SCALE:FLAT_XZ_ANGLE`

        string[] lines = textReceived.Split('\n');
        foreach (var line in lines)
        {
            string[] values = line.Trim().Split(':');
            if (values.Length == 11)
            {
                if (!int.TryParse(values[0], out int playerIndex))
                {
                    continue;
                }

                STRUCT_IntegerPlayerInGameInfo playerStateInGame = new STRUCT_IntegerPlayerInGameInfo();
                playerStateInGame.m_playerIndex = int.Parse(values[0]);
                playerStateInGame.m_playerNetworkIndex = int.Parse(values[1]);
                playerStateInGame.m_playerTeamIndex = int.Parse(values[2]);
                float xfromMM = int.Parse(values[3]) / 1000f;
                float yfromMM = int.Parse(values[4]) / 1000f;
                float zfromMM = int.Parse(values[5]) / 1000f;
                float xEuleurFromMM = int.Parse(values[6]) / 1000f;
                float yEuleurFromMM = int.Parse(values[7]) / 1000f;
                float zEuleurFromMM = int.Parse(values[8]) / 1000f;
                float scaleFromMM = int.Parse(values[9]) / 1000f;
                float flatAngleXZFromMM = int.Parse(values[10]) / 1000f;
                playerStateInGame.m_positionX= xfromMM;
                playerStateInGame.m_positionY = yfromMM;
                playerStateInGame.m_positionZ = zfromMM;
                playerStateInGame.m_rotationEulerX = xEuleurFromMM;
                playerStateInGame.m_rotationEulerY = yEuleurFromMM;
                playerStateInGame.m_rotationEulerZ = zEuleurFromMM;
                playerStateInGame.m_playerScale = (int)scaleFromMM;
                playerStateInGame.m_flatAngleXZ = (int)flatAngleXZFromMM;
                long timeInSecondsSince1970UTC = (System.DateTime.UtcNow.Ticks - new System.DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond;
                playerStateInGame.m_timestampReceived = timeInSecondsSince1970UTC;
                playerStateInGames.Add(playerStateInGame);
            }
        }
    }
}
