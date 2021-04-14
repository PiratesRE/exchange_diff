using System;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class WacSecurityAccessToken : ISecurityAccessToken
	{
		internal WacSecurityAccessToken(string sid)
		{
			this.userSid = sid;
		}

		public string UserSid
		{
			get
			{
				return this.userSid;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public SidStringAndAttributes[] GroupSids
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public SidStringAndAttributes[] RestrictedGroupSids
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private readonly string userSid;
	}
}
