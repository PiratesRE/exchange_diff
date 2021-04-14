using System;

namespace System.Diagnostics.Tracing
{
	internal struct SessionMask
	{
		public SessionMask(SessionMask m)
		{
			this.m_mask = m.m_mask;
		}

		public SessionMask(uint mask = 0U)
		{
			this.m_mask = (mask & 15U);
		}

		public bool IsEqualOrSupersetOf(SessionMask m)
		{
			return (this.m_mask | m.m_mask) == this.m_mask;
		}

		public static SessionMask All
		{
			get
			{
				return new SessionMask(15U);
			}
		}

		public static SessionMask FromId(int perEventSourceSessionId)
		{
			return new SessionMask(1U << perEventSourceSessionId);
		}

		public ulong ToEventKeywords()
		{
			return (ulong)this.m_mask << 44;
		}

		public static SessionMask FromEventKeywords(ulong m)
		{
			return new SessionMask((uint)(m >> 44));
		}

		public bool this[int perEventSourceSessionId]
		{
			get
			{
				return ((ulong)this.m_mask & (ulong)(1L << (perEventSourceSessionId & 31))) > 0UL;
			}
			set
			{
				if (value)
				{
					this.m_mask |= 1U << perEventSourceSessionId;
					return;
				}
				this.m_mask &= ~(1U << perEventSourceSessionId);
			}
		}

		public static SessionMask operator |(SessionMask m1, SessionMask m2)
		{
			return new SessionMask(m1.m_mask | m2.m_mask);
		}

		public static SessionMask operator &(SessionMask m1, SessionMask m2)
		{
			return new SessionMask(m1.m_mask & m2.m_mask);
		}

		public static SessionMask operator ^(SessionMask m1, SessionMask m2)
		{
			return new SessionMask(m1.m_mask ^ m2.m_mask);
		}

		public static SessionMask operator ~(SessionMask m)
		{
			return new SessionMask(15U & ~m.m_mask);
		}

		public static explicit operator ulong(SessionMask m)
		{
			return (ulong)m.m_mask;
		}

		public static explicit operator uint(SessionMask m)
		{
			return m.m_mask;
		}

		private uint m_mask;

		internal const int SHIFT_SESSION_TO_KEYWORD = 44;

		internal const uint MASK = 15U;

		internal const uint MAX = 4U;
	}
}
