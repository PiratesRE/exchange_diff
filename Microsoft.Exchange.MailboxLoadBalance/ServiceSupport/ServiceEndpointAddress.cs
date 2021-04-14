using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ServiceEndpointAddress
	{
		public ServiceEndpointAddress(string endpointSuffix)
		{
			this.endpointSuffix = endpointSuffix;
		}

		public virtual string GetAddress(string serverName)
		{
			return string.Format("net.tcp://{0}/{1}", serverName, this.endpointSuffix);
		}

		public virtual Uri[] GetBaseUris()
		{
			return Array<Uri>.Empty;
		}

		private readonly string endpointSuffix;
	}
}
