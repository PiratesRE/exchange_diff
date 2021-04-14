using System;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class MonitoringServiceBasicCmdletResult
	{
		public MonitoringServiceBasicCmdletResult()
		{
			this.result = MonitoringServiceBasicCmdletResultEnum.Undefined;
		}

		public MonitoringServiceBasicCmdletResult(MonitoringServiceBasicCmdletResultEnum result)
		{
			this.result = result;
		}

		public MonitoringServiceBasicCmdletResultEnum Value
		{
			get
			{
				return this.result;
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			switch (this.result)
			{
			case MonitoringServiceBasicCmdletResultEnum.Undefined:
				text = "Undefined";
				break;
			case MonitoringServiceBasicCmdletResultEnum.Success:
				text = "Success";
				break;
			case MonitoringServiceBasicCmdletResultEnum.Failure:
				text = "Failure";
				break;
			}
			return text;
		}

		private MonitoringServiceBasicCmdletResultEnum result;
	}
}
