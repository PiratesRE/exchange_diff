using System;
using System.Net;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.Smtp
{
	internal class NextHopFqdn : INextHopServer
	{
		public NextHopFqdn(string fqdn, bool isFrontendAndHubColocatedServer)
		{
			this.fqdn = fqdn;
			this.isFrontendAndHubColocatedServer = isFrontendAndHubColocatedServer;
		}

		bool INextHopServer.IsIPAddress
		{
			get
			{
				return false;
			}
		}

		IPAddress INextHopServer.Address
		{
			get
			{
				throw new InvalidOperationException("INextHopServer.Address must not be requested from NextHopFqdn objects");
			}
		}

		string INextHopServer.Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		bool INextHopServer.IsFrontendAndHubColocatedServer
		{
			get
			{
				return this.isFrontendAndHubColocatedServer;
			}
		}

		private readonly string fqdn;

		private readonly bool isFrontendAndHubColocatedServer;
	}
}
