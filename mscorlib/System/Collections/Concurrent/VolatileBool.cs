using System;

namespace System.Collections.Concurrent
{
	internal struct VolatileBool
	{
		public VolatileBool(bool value)
		{
			this.m_value = value;
		}

		public volatile bool m_value;
	}
}
