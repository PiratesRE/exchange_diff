using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Partner;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class CatServer : ExtendedRoutingSmtpServer
	{
		private CatServer(IReadOnlyMailItem currentMailItem, AcceptedDomainCollection acceptedDomains)
		{
			if (acceptedDomains == null)
			{
				throw new ArgumentNullException("acceptedDomains");
			}
			this.currentMailItem = currentMailItem;
			this.acceptedDomains = acceptedDomains;
		}

		public override string Name
		{
			get
			{
				return CatServer.serverName;
			}
		}

		public override Version Version
		{
			get
			{
				return this.serverVersion;
			}
		}

		public override IPPermission IPPermission
		{
			get
			{
				return CatServer.allowDenyList;
			}
		}

		public override AddressBook AddressBook
		{
			get
			{
				if (this.addressBook == null)
				{
					lock (this.addressBookCreationLock)
					{
						if (this.addressBook == null)
						{
							this.addressBook = new AddressBookImpl();
						}
					}
				}
				return this.addressBook;
			}
		}

		public override AcceptedDomainCollection AcceptedDomains
		{
			get
			{
				return this.acceptedDomains;
			}
		}

		public override RemoteDomainCollection RemoteDomains
		{
			get
			{
				return this.remoteDomains;
			}
		}

		public static CatServer GetInstance(IReadOnlyMailItem currentMailItem, AcceptedDomainCollection acceptedDomains)
		{
			return new CatServer(currentMailItem, acceptedDomains);
		}

		public override void SubmitMessage(EmailMessage message)
		{
			SubmitHelper.CreateTransportMailItemAndSubmit(this.currentMailItem, message, CatServer.serverName, this.serverVersion, base.AssociatedAgent.Name);
		}

		public void SubmitMessage(EmailMessage message, OrganizationId organizationId, Guid externalOrgId = default(Guid))
		{
			SubmitHelper.CreateTransportMailItemAndSubmit(this.currentMailItem, message, CatServer.serverName, this.serverVersion, base.AssociatedAgent.Name, organizationId, externalOrgId, false);
		}

		public override void TrackAgentInfo(string agentName, string groupName, List<KeyValuePair<string, string>> data)
		{
			QueuedMessageEventSource queuedMessageEventSource = base.AssociatedAgent.Session.CurrentEventSource as QueuedMessageEventSource;
			if (queuedMessageEventSource == null)
			{
				throw new InvalidOperationException("Not invoked from a routing agent");
			}
			queuedMessageEventSource.TrackAgentInfo(agentName, groupName, data);
		}

		private static readonly string serverName = Dns.GetHostName();

		private static IPPermission allowDenyList = new IPPermissionImpl();

		private IReadOnlyMailItem currentMailItem;

		private volatile AddressBookImpl addressBook;

		private object addressBookCreationLock = new object();

		private AcceptedDomainCollection acceptedDomains;

		private RemoteDomainCollection remoteDomains = Components.AgentComponent.RemoteDomains;

		private Version serverVersion = Components.Configuration.LocalServer.TransportServer.AdminDisplayVersion;
	}
}
