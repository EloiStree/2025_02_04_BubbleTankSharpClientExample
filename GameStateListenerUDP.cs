using System.Net.Sockets;
using System.Net;
using System.Text;

public class UdpQueueListener
{
    private  int m_port;
    private  Queue<string> m_messageQueue;
    private DateTime m_lastReceivedTime;
    private  object m_lock = new object();
    private bool m_isRunning;
    private bool m_useConsoleDebug;

    public UdpQueueListener(int port = 8001, bool useConsoleDebug=true)
    {
        m_useConsoleDebug = useConsoleDebug;
        m_port = port;
        m_messageQueue = new Queue<string>();
        m_lastReceivedTime = DateTime.UtcNow;
    }

    public void Start()
    {
        m_isRunning = true;
        Thread listenerThread = new Thread(Listen);
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    public void Stop()
    {
        m_isRunning = false;
    }

    public float m_secondsSinceLastTimeUpdate = 10;
    private void Listen()
    {
        using (UdpClient udpClient = new UdpClient(m_port))
        {
            udpClient.Client.ReceiveTimeout = 5000; 
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, m_port);

            while (m_isRunning)
            {
                try
                {
                    byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                    string receivedMessage = Encoding.UTF8.GetString(receivedBytes);

                    lock (m_lock)
                    {
                        m_messageQueue.Enqueue(receivedMessage);
                    }

                    DateTime now = DateTime.UtcNow;
                    if (now-m_lastReceivedTime > TimeSpan.FromSeconds(m_secondsSinceLastTimeUpdate))
                    {
                        Console.WriteLine("Exit Listener");
                        return;
                    }
                    if (m_useConsoleDebug)
    
                        Console.WriteLine($"Received: {receivedMessage}");
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"SocketException: {ex.Message}");
                }
            }
        }
    }

    public bool HasMessageInQueue()
    {
        lock (m_lock)
        {
            return m_messageQueue.Count > 0;
        }
    }
    public void GetNextMessage(out bool hasMessage,out  string message)
    {
        lock (m_lock)
        {
            hasMessage = m_messageQueue.Count > 0;
            if (hasMessage)
            {
                message = m_messageQueue.Dequeue();
            }
            else 
            {
                message = "";
            }
        }
    }

    public void KeepAlive()
    {
        lock (m_lock)
        {
            m_lastReceivedTime = DateTime.UtcNow;
        }
    }

    public DateTime GetLastReceivedTime()
    {
        lock (m_lock)
        {
            return m_lastReceivedTime;
        }
    }
}