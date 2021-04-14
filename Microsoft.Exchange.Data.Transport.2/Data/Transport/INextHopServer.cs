using System;
using System.Net;

namespace Microsoft.Exchange.Data.Transport
{
	internal interface INextHopServer
	{
		bool IsIPAddress { get; }

		IPAddress Address { get; }

		string Fqdn { get; }

		bool IsFrontendAndHubColocatedServer { get; }
	}
}
