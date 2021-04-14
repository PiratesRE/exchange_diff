using System;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class ClientInfo
	{
		public string ApplicationId { get; set; }

		public string ClientVersion { get; set; }

		public string ClientMachineName { get; set; }

		public string ClientProcessName { get; set; }

		public DateTime ConnectTime { get; set; }

		public override string ToString()
		{
			return string.Format("[ApplicationId:{0}, ClientVersion:{1}, ClientMachineName:{2}, ClientProcessName:{3}, ConnectTime:{4}]", new object[]
			{
				this.ApplicationId,
				this.ClientVersion,
				this.ClientMachineName,
				this.ClientProcessName,
				this.ConnectTime
			});
		}
	}
}
