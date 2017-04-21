using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Common.Util
{
    class Time
    {
        private DateTime m_NowTime;
        private DateTime m_CurrentTime;

        public double DeltaTime { get; private set; }

        public Time()
        {
            m_NowTime = DateTime.Now;
        }

        public void Update()
        {
            m_CurrentTime = DateTime.Now;
            TimeSpan frameTime = m_NowTime - m_CurrentTime;

            DeltaTime = frameTime.TotalMilliseconds / 1000;
        }
    }
}
