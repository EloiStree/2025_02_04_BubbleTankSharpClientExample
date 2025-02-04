public class TankActionPusher : I_TankAction
{
    private PushIntegerToGameUDP m_push;
    public TankActionPusher(PushIntegerToGameUDP push) {
        m_push = push;
    }
    public void Fire()
    {
        m_push.PushInteger(1032);
        m_push.PushInteger(2032);
    }
    public void Ping()
    {
        m_push.PushInteger(1080);
        m_push.PushInteger(2080);
    }

    private float m_horizontalAxis = 0;
    private float m_verticalAxis = 0;

    public void SetMoveDownUpPercentage(float percentage11)
    {
        m_verticalAxis = percentage11;
        ParseAndPush();
    }

    public void SetRotationLeftRightPercentage(float percentage11)
    {
    
        m_horizontalAxis = percentage11;
        ParseAndPush();
    }

    public int m_joystickPreviousValue = 0;

    public void ParseAndPush() {
            int value = 1800000000;
            ParsePercent11To99(m_horizontalAxis, out int valueHoriziontal);
            ParsePercent11To99(m_verticalAxis, out int valueVertical);
            value += valueVertical;
            value += valueHoriziontal * 100;
            value += valueVertical * 10000;
            value += valueHoriziontal * 1000000;
            m_joystickPreviousValue = value;
            m_push.PushInteger(value);
            
        }

        private void ParsePercent11To99(float percent11, out int value99)
        {
            if (percent11 == 0)
            {
                value99 = 0;
                return;
            }
            percent11 += 1f;
            percent11 /= 2f;
            percent11 *= 98f;
            if (percent11 > 98f)
            {
                percent11 = 98f;
            }
            else if (percent11 < 0f)
            {
                percent11 = 0f;
            }
            value99 = 1 + (int)(percent11);

        }
        public void TurnLeft() { 
            SetRotationLeftRightPercentage(-1);
    
        }
        public void TurnRight() { 
            SetRotationLeftRightPercentage(1);
        }
        public void MoveForward() { 
            SetMoveDownUpPercentage(1);
        }

        public void MoveBackward() { 
            SetMoveDownUpPercentage(-1);
        }
        public void StopMoving() { 
            SetMoveDownUpPercentage(0);
            SetRotationLeftRightPercentage(0);
        }
    }
