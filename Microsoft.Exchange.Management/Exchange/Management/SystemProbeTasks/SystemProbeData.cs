using System;

namespace Microsoft.Exchange.Management.SystemProbeTasks
{
	[Serializable]
	public class SystemProbeData
	{
		public Guid Guid
		{
			get
			{
				return this.guid;
			}
			set
			{
				this.guid = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
			set
			{
				this.timeStamp = value;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		private Guid guid;

		private DateTime timeStamp;

		private string text;
	}
}
