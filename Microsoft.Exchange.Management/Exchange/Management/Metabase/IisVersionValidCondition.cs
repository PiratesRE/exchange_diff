using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[Serializable]
	internal sealed class IisVersionValidCondition : Condition
	{
		public IisVersionValidCondition(string serverName)
		{
			this.ServerName = serverName;
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
			set
			{
				this.serverName = value;
			}
		}

		public override bool Verify()
		{
			TaskLogger.LogEnter();
			bool result = IisUtility.IsSupportedIisVersion(this.ServerName);
			TaskLogger.LogExit();
			return result;
		}

		private string serverName;
	}
}
