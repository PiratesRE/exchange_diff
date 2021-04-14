using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver.Shared
{
	internal class StoreDriverServer : SmtpServer
	{
		protected StoreDriverServer(OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			this.organizationId = organizationId;
		}

		public override string Name
		{
			get
			{
				return StoreDriverServer.serverName;
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
				return StoreDriverServer.allowDenyList;
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
				if (this.acceptedDomains == null)
				{
					PerTenantAcceptedDomainTable acceptedDomainTable = Components.Configuration.GetAcceptedDomainTable(this.organizationId);
					this.acceptedDomains = acceptedDomainTable.AcceptedDomainTable;
				}
				return this.acceptedDomains;
			}
		}

		public override RemoteDomainCollection RemoteDomains
		{
			get
			{
				if (this.remoteDomains == null)
				{
					PerTenantRemoteDomainTable remoteDomainTable = Components.Configuration.GetRemoteDomainTable(this.organizationId);
					this.remoteDomains = remoteDomainTable.RemoteDomainTable;
				}
				return this.remoteDomains;
			}
		}

		public static StoreDriverServer GetInstance(OrganizationId organizationId)
		{
			return new StoreDriverServer(organizationId);
		}

		public override void SubmitMessage(EmailMessage message)
		{
			throw new NotImplementedException();
		}

		public virtual void SubmitMessage(IReadOnlyMailItem originalMailItem, EmailMessage message, OrganizationId organizationId, Guid externalOrganizationId, bool suppressDSNs)
		{
			throw new NotImplementedException();
		}

		public virtual void SubmitMailItem(TransportMailItem mailItem, bool suppressDSNs)
		{
			throw new NotImplementedException();
		}

		private static string serverName = Dns.GetHostName();

		private static IPPermission allowDenyList = new IPPermissionImpl();

		private volatile AddressBookImpl addressBook;

		private object addressBookCreationLock = new object();

		private OrganizationId organizationId;

		private AcceptedDomainCollection acceptedDomains;

		private RemoteDomainCollection remoteDomains;

		private Version serverVersion = Components.Configuration.LocalServer.TransportServer.AdminDisplayVersion;
	}
}
