using System.Net.Sockets;

public class PushIntegerToGameUDP {

    public string m_ipAddress;
    public int m_port;
    public int m_playerIndex;
    private UdpClient m_udpClient = new UdpClient();

    public PushIntegerToGameUDP(string ipv4 ="127.0.0.1", int port=2504, int playerIndex=-10) {
        m_ipAddress = ipv4;
        m_port = port;
        m_playerIndex = playerIndex;
    }

    public void PushInteger(int integerValue) { 
    
        m_udpClient = new UdpClient();
        byte[] bytesIndex = BitConverter.GetBytes(m_playerIndex);
        byte[] bytesInteger = BitConverter.GetBytes(integerValue);
        byte[] bytesToSend = new byte[bytesIndex.Length + bytesInteger.Length];
        bytesIndex.CopyTo(bytesToSend, 0);
        bytesInteger.CopyTo(bytesToSend, bytesIndex.Length);
        m_udpClient.Send(bytesToSend, bytesToSend.Length, m_ipAddress, m_port);
        m_udpClient.Close();
    }
}
