using System;
using System.Collections.Generic;
using Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ServerToServerCallingContextFactory : IServiceCallingContextFactory
	{
		public ServerToServerCallingContextFactory(IDictionary<Uri, string> remoteUrls)
		{
			this.remoteUrls = (remoteUrls ?? new Dictionary<Uri, string>());
		}

		public ICredentialHandler CredentialHandler
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotSupportedException("ServerToServerCallingContextFactory shouldn't need the credential handler.");
			}
		}

		public IServiceCallingContext<DefaultBinding_Autodiscover> AutoDiscoverCallingContext
		{
			get
			{
				if (this.autoDiscoveryCallingContext == null)
				{
					this.autoDiscoveryCallingContext = new ServerToServerAutoDiscoveryCallingContext(this.remoteUrls);
				}
				return this.autoDiscoveryCallingContext;
			}
		}

		public IServiceCallingContext<ExchangeServiceBinding> EwsCallingContext
		{
			get
			{
				if (this.ewsCallingContext == null)
				{
					this.ewsCallingContext = new ServerToServerEwsCallingContext(this.remoteUrls);
				}
				return this.ewsCallingContext;
			}
		}

		private readonly IDictionary<Uri, string> remoteUrls;

		private IServiceCallingContext<ExchangeServiceBinding> ewsCallingContext;

		private IServiceCallingContext<DefaultBinding_Autodiscover> autoDiscoveryCallingContext;
	}
}
