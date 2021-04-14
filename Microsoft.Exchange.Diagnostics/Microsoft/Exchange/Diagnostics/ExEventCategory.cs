using System;

namespace Microsoft.Exchange.Diagnostics
{
	[CLSCompliant(true)]
	[Serializable]
	public class ExEventCategory
	{
		public ExEventCategory(string name, int number, ExEventLog.EventLevel level)
		{
			this.m_name = name;
			this.m_number = number;
			this.m_level = level;
		}

		public ExEventCategory()
		{
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		public int Number
		{
			get
			{
				return this.m_number;
			}
			set
			{
				this.m_number = value;
			}
		}

		public ExEventLog.EventLevel EventLevel
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_level = value;
			}
		}

		private string m_name;

		private int m_number = -1;

		private ExEventLog.EventLevel m_level = ExEventLog.EventLevel.Expert;
	}
}
