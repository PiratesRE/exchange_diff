using System;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Partner;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpReceiveServer : ExtendedSmtpServer
	{
		public static string ServerName
		{
			get
			{
				if (SmtpReceiveServer.serverName == null)
				{
					lock (SmtpReceiveServer.syncRoot)
					{
						if (SmtpReceiveServer.serverName == null)
						{
							SmtpReceiveServer.serverName = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;
						}
					}
				}
				return SmtpReceiveServer.serverName;
			}
		}

		public override string Name
		{
			get
			{
				return SmtpReceiveServer.ServerName;
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
				return SmtpReceiveServer.ipPermission;
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
							this.addressBook = new AddressBookImpl(this.isMemberOfResolver);
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

		public override void SubmitMessage(EmailMessage message)
		{
			if (this.TransportMailItem != null)
			{
				SubmitHelper.CreateTransportMailItemAndSubmit(this.TransportMailItem, message, SmtpReceiveServer.ServerName, this.serverVersion, base.AssociatedAgent.Name, null, this.TransportMailItem.ExternalOrganizationId, false);
				return;
			}
			SubmitHelper.CreateNewTransportMailItemAndSubmit(message, SmtpReceiveServer.ServerName, this.serverVersion, base.AssociatedAgent.Name, default(Guid), null, null, false);
		}

		private TransportMailItem TransportMailItem
		{
			get
			{
				if (this.smtpInSession != null)
				{
					return this.smtpInSession.TransportMailItem;
				}
				return this.sessionState.TransportMailItem;
			}
		}

		public static SmtpReceiveServer FromSmtpInSession(ISmtpInSession smtpInSession, AcceptedDomainCollection acceptedDomains, RemoteDomainCollection remoteDomains, Version serverVersion)
		{
			return new SmtpReceiveServer(smtpInSession, acceptedDomains, remoteDomains, serverVersion);
		}

		public static SmtpReceiveServer FromSmtpInSessionState(SmtpInSessionState sessionState, AcceptedDomainCollection acceptedDomains, RemoteDomainCollection remoteDomains, Version serverVersion, IIsMemberOfResolver<RoutingAddress> isMemberOfResolver)
		{
			return new SmtpReceiveServer(sessionState, acceptedDomains, remoteDomains, serverVersion, isMemberOfResolver);
		}

		private SmtpReceiveServer(ISmtpInSession smtpInSession, AcceptedDomainCollection acceptedDomains, RemoteDomainCollection remoteDomains, Version serverVersion)
		{
			ArgumentValidator.ThrowIfNull("smtpInSession", smtpInSession);
			ArgumentValidator.ThrowIfNull("acceptedDomains", acceptedDomains);
			ArgumentValidator.ThrowIfNull("remoteDomains", remoteDomains);
			ArgumentValidator.ThrowIfNull("serverVersion", serverVersion);
			this.smtpInSession = smtpInSession;
			this.acceptedDomains = acceptedDomains;
			this.remoteDomains = remoteDomains;
			this.serverVersion = serverVersion;
			this.isMemberOfResolver = smtpInSession.IsMemberOfResolver;
		}

		private SmtpReceiveServer(SmtpInSessionState sessionState, AcceptedDomainCollection acceptedDomains, RemoteDomainCollection remoteDomains, Version serverVersion, IIsMemberOfResolver<RoutingAddress> isMemberOfResolver)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("acceptedDomains", acceptedDomains);
			ArgumentValidator.ThrowIfNull("remoteDomains", remoteDomains);
			ArgumentValidator.ThrowIfNull("serverVersion", serverVersion);
			ArgumentValidator.ThrowIfNull("isMemberOfResolver", isMemberOfResolver);
			this.sessionState = sessionState;
			this.acceptedDomains = acceptedDomains;
			this.remoteDomains = remoteDomains;
			this.serverVersion = serverVersion;
			this.isMemberOfResolver = isMemberOfResolver;
		}

		private static readonly object syncRoot = new object();

		private static string serverName;

		private static readonly IPPermission ipPermission = new IPPermissionImpl();

		private volatile AddressBookImpl addressBook;

		private readonly object addressBookCreationLock = new object();

		private readonly AcceptedDomainCollection acceptedDomains;

		private readonly RemoteDomainCollection remoteDomains;

		private readonly Version serverVersion;

		private readonly IIsMemberOfResolver<RoutingAddress> isMemberOfResolver;

		private readonly ISmtpInSession smtpInSession;

		private readonly SmtpInSessionState sessionState;
	}
}
