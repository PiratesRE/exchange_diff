using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal sealed class InformationStoreServiceRunningCondition : Condition
	{
		public InformationStoreServiceRunningCondition(string computerName)
		{
			this.computerName = computerName;
		}

		public override bool Verify()
		{
			bool flag = WmiWrapper.IsServiceRunning(this.computerName, "MSExchangeIS");
			TaskLogger.Trace("InformationStoreServiceRunningCondition.Verify() returns {0}: <Server '{1}'>", new object[]
			{
				flag,
				this.computerName
			});
			return flag;
		}

		private readonly string computerName;
	}
}
