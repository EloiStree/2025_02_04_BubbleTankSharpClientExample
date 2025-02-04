using System;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

// Example usage
public class Program
{
    public static void Main(string[] args)
    {
        int playerIndex = -10;
        string targetIpv4 = "192.168.1.18";
        int targetPortSend = 2504;
        int targetPortListen = 8001;
        int timeBetweenUpdates = 100;

        UdpQueueListener listener = new UdpQueueListener(targetPortListen,false);
        listener.Start();

        PushIntegerToGameUDP pusher = new PushIntegerToGameUDP(targetIpv4, targetPortSend, playerIndex);
        TankActionPusher tankAction = new TankActionPusher(pusher);
        STRUCT_IntegerPlayerInGameInfo yourPlayerInfo = new STRUCT_IntegerPlayerInGameInfo();
        yourPlayerInfo.m_playerIndex = playerIndex;
        List<STRUCT_IntegerPlayerInGameInfo> players = new List<STRUCT_IntegerPlayerInGameInfo>();
        YourTankCode yourTank = new YourTankCode();
        while (true)
        {
            Thread.Sleep(timeBetweenUpdates);
            listener.KeepAlive();
            while (listener.HasMessageInQueue())
            {
                listener.GetNextMessage(out bool hasMessage, out string m);
                if (hasMessage)
                {
                    ParseTextToListOfPlayers.PushInAsUtf8TextFromWeb(m, out players);
                    pusher.PushInteger(1080);
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].m_playerIndex == playerIndex)
                        {
                            yourPlayerInfo = players[i];
                            break;
                        }
                    }
                }
            }
            yourTank.Update(tankAction, in yourPlayerInfo, in players);            
        }
    }
}
