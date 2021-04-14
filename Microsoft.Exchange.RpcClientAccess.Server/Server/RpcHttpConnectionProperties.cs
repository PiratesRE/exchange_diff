using System;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class RpcHttpConnectionProperties
	{
		public RpcHttpConnectionProperties(string clientIp, string serverTarget, string sessionCookie, string[] requestIds)
		{
			this.clientIp = clientIp;
			this.serverTarget = serverTarget;
			this.sessionCookie = sessionCookie;
			this.requestIds = requestIds;
		}

		public string ClientIp
		{
			get
			{
				return this.clientIp;
			}
		}

		public string ServerTarget
		{
			get
			{
				return this.serverTarget;
			}
		}

		public string SessionCookie
		{
			get
			{
				return this.sessionCookie;
			}
		}

		public string[] RequestIds
		{
			get
			{
				return this.requestIds;
			}
		}

		private readonly string clientIp;

		private readonly string serverTarget;

		private readonly string sessionCookie;

		private readonly string[] requestIds;
	}
}
