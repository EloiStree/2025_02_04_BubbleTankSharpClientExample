
using System;
using System.Numerics;


public class PlayersUtility { 

    public static bool IsPlayerDeath(in STRUCT_IntegerPlayerInGameInfo player, in List<STRUCT_IntegerPlayerInGameInfo> players) {
        foreach (var p in players) {
            if (p.m_playerIndex == player.m_playerIndex) {
                return false;
            }
        }
        return true;
    }
}
public class YourTankCode {
    public ulong m_frame = 0;


    public float m_angleWanted = -180;
    public Vector3 m_target= new Vector3(0,0,0);

    public void Print(string message) { 
    
        Console.WriteLine(message);
    }


    public void Update(in I_TankAction tankAction,
        in STRUCT_IntegerPlayerInGameInfo tankInfo, 
        in List<STRUCT_IntegerPlayerInGameInfo> players)
    {
        if (players.Count == 0)
        {
            Console.WriteLine("No players");
            return;
        }

       if (PlayersUtility.IsPlayerDeath(tankInfo, players)) {
            Console.WriteLine("Player is dead");
            return;
        }


        foreach (var player in players)
        {
            if (player.m_playerIndex != tankInfo.m_playerIndex && player.m_playerTeamIndex != tankInfo.m_playerTeamIndex)
            {
                m_target = new Vector3(player.m_positionX, player.m_positionY, player.m_positionZ);
                Console.WriteLine("Target: " + player.m_playerIndex+" : "+m_target);
                break;
            }
        }

        m_frame++;
        Console.WriteLine($"Update,{m_frame}: {tankInfo.m_playerIndex} / {players.Count}");
        Console.WriteLine($"Player Angle:" + tankInfo.m_flatAngleXZ);

        tankAction.Fire();
        tankAction.Ping();
        Vector3 playerPosition = new Vector3(tankInfo.m_positionX, tankInfo.m_positionY, tankInfo.m_positionZ);
        Console.WriteLine("Position: " + playerPosition);

        // Ensure the angle is in radians
        Vector3 playerDirection, targetDirection;
        Vector3Utility.GetPlayerDirection(tankInfo, out playerDirection);
        Vector3Utility.GetTargetDirection(playerPosition, m_target, out targetDirection);
        Print("Direction : " + playerDirection);


        float signedAngleBetween = Vector3Utility.SignAngleBetweenTwoVector3(playerDirection, targetDirection);
        Print("Signed Angle: " + signedAngleBetween);

        tankAction.SetMoveDownUpPercentage(1f);
        // Decide turn direction
        if (signedAngleBetween > 0)
        {
            tankAction.TurnRight();
        }
        else
        {
            tankAction.TurnLeft();
        }


        Console.WriteLine("Direction: " + playerDirection);
        Thread.Sleep(200);
        tankAction.TurnLeft();


    }


    private void TestCommands(I_TankAction tankAction)
    {
        Print("Fire");
        tankAction.Fire();
        Thread.Sleep(1000);
        Print("Ping");
        tankAction.Ping();
        Thread.Sleep(1000);
        Print("Turn Right");
        tankAction.TurnRight();
        tankAction.MoveForward();
        Thread.Sleep(1000);
        Print("Turn Left");
        tankAction.TurnLeft();
        tankAction.MoveBackward();
        Thread.Sleep(1000);
    }
}

public class Vector3Utility {


    public static void GetPlayerDirection(STRUCT_IntegerPlayerInGameInfo tankInfo, out Vector3 playerDirection)
    {
        float angleRadians = (float)(tankInfo.m_flatAngleXZ * Math.PI / 180.0);
        playerDirection = new Vector3((float)Math.Cos(angleRadians), 0, (float)Math.Sin(angleRadians));
        playerDirection = Vector3.Normalize(playerDirection);
    }

    public static void GetTargetDirection( Vector3 playerPosition, Vector3 targetPositoin, out Vector3 direction)
    {
        direction = Vector3.Normalize(targetPositoin- playerPosition);
    }

    public static float SignAngleBetweenTwoVector3(Vector3 from, Vector3 to)
    {
        // Ensure the vectors are normalized
        from = Vector3.Normalize(from);
        to = Vector3.Normalize(to);

        // Calculate the dot product to get the cosine of the angle
        float dot = Vector3.Dot(from, to);

        // Calculate the cross product to determine the direction of rotation
        Vector3 cross = Vector3.Cross(from, to);

        // Calculate the magnitude of the cross product to get the sine of the angle
        float crossMagnitude = cross.Length();

        // Calculate the angle in radians using Atan2
        float angle = MathF.Atan2(crossMagnitude, dot);

        // Determine the sign of the angle based on the cross product's y-component
        // (assuming the rotation is around the Y-axis)
        if (cross.Y < 0)
        {
            angle = -angle;
        }

        return angle;
    }


}