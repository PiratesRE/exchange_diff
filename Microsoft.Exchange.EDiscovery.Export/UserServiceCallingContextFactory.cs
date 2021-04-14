using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class UserServiceCallingContextFactory : IServiceCallingContextFactory
	{
		public UserServiceCallingContextFactory(ICredentialHandler credentialHandler = null)
		{
			this.CredentialHandler = credentialHandler;
			this.cachedCredentials = new Dictionary<string, ICredentials>();
		}

		public ICredentialHandler CredentialHandler { get; set; }

		public IServiceCallingContext<DefaultBinding_Autodiscover> AutoDiscoverCallingContext
		{
			get
			{
				if (this.CredentialHandler == null)
				{
					throw new ArgumentNullException("CredentialHandler");
				}
				if (this.autoDiscoverCallingContext == null)
				{
					this.autoDiscoverCallingContext = new UserAutoDiscoverCallingContext(this.CredentialHandler, this.cachedCredentials);
				}
				return this.autoDiscoverCallingContext;
			}
		}

		public IServiceCallingContext<ExchangeServiceBinding> EwsCallingContext
		{
			get
			{
				if (this.CredentialHandler == null)
				{
					throw new ArgumentNullException("CredentialHandler");
				}
				if (this.ewsCallingContext == null)
				{
					this.ewsCallingContext = new UserEwsCallingContext(this.CredentialHandler, "MailboxSearch", this.cachedCredentials);
				}
				return this.ewsCallingContext;
			}
		}

		private readonly Dictionary<string, ICredentials> cachedCredentials;

		private IServiceCallingContext<DefaultBinding_Autodiscover> autoDiscoverCallingContext;

		private IServiceCallingContext<ExchangeServiceBinding> ewsCallingContext;
	}
}
