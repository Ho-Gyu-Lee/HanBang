using System;

namespace GameServer.Common.Util
{
    public class Singleton<T> where T : class, new()
    {
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new T();
                }
                return m_Instance;
            }
        }
    }

    public class ThreadSafeSingleton<T> where T : class, new()
    {
        private static volatile T m_Instance;
        private static object m_SyncRoot = new Object();

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (m_SyncRoot)
                    {
                        if (m_Instance == null)
                            m_Instance = new T();
                    }
                }

                return m_Instance;
            }
        }
    }
}
