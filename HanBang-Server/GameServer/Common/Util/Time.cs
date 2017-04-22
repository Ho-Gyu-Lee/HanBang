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
        private DateTime m_PrevTime;

        public double DeltaTime { get; private set; }

        public Time()
        {
            m_PrevTime = DateTime.Now;
        }

        public void Update()
        {
            m_NowTime = DateTime.Now;
            TimeSpan frameTime = m_NowTime - m_PrevTime;
            m_PrevTime = m_NowTime;

            DeltaTime = frameTime.TotalMilliseconds / 1000;
        }
    }
}
