using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public class DaylightTime
	{
		private DaylightTime()
		{
		}

		public DaylightTime(DateTime start, DateTime end, TimeSpan delta)
		{
			this.m_start = start;
			this.m_end = end;
			this.m_delta = delta;
		}

		public DateTime Start
		{
			get
			{
				return this.m_start;
			}
		}

		public DateTime End
		{
			get
			{
				return this.m_end;
			}
		}

		public TimeSpan Delta
		{
			get
			{
				return this.m_delta;
			}
		}

		internal DateTime m_start;

		internal DateTime m_end;

		internal TimeSpan m_delta;
	}
}
