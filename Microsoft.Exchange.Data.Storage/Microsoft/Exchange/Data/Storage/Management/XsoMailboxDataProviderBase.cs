using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class XsoMailboxDataProviderBase : XsoStoreDataProviderBase
	{
		public MailboxSession MailboxSession
		{
			get
			{
				return (MailboxSession)base.StoreSession;
			}
			private set
			{
				base.StoreSession = value;
			}
		}

		public ADUser MailboxOwner { get; protected set; }

		public XsoMailboxDataProviderBase(ADSessionSettings adSessionSettings, ADUser mailboxOwner, string action) : this(ExchangePrincipal.FromADUser(adSessionSettings, mailboxOwner, RemotingOptions.AllowCrossSite), action)
		{
			this.MailboxOwner = mailboxOwner;
		}

		public XsoMailboxDataProviderBase(ADSessionSettings adSessionSettings, ADUser mailboxOwner, ISecurityAccessToken userToken, string action) : this(XsoStoreDataProviderBase.GetExchangePrincipalWithAdSessionSettingsForOrg(adSessionSettings.CurrentOrganizationId, mailboxOwner), userToken, action)
		{
			this.MailboxOwner = mailboxOwner;
		}

		public XsoMailboxDataProviderBase(ExchangePrincipal mailboxOwner, string action)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				Util.ThrowOnNullArgument(mailboxOwner, "mailboxOwner");
				Util.ThrowOnNullOrEmptyArgument(action, "action");
				this.MailboxSession = MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, string.Format("Client=Management;Action={0};Privilege:ActAsAdmin", action));
				disposeGuard.Success();
			}
		}

		protected XsoMailboxDataProviderBase(ExchangePrincipal mailboxOwner, ISecurityAccessToken userToken, string action)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				Util.ThrowOnNullArgument(mailboxOwner, "mailboxOwner");
				Util.ThrowOnNullOrEmptyArgument(action, "action");
				if (userToken == null)
				{
					this.MailboxSession = MailboxSession.Open(mailboxOwner, new WindowsPrincipal(WindowsIdentity.GetCurrent()), CultureInfo.InvariantCulture, string.Format("Client=Management;Action={0}", action));
				}
				else
				{
					try
					{
						using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(userToken, AuthzFlags.AuthzSkipTokenGroups))
						{
							clientSecurityContext.SetSecurityAccessToken(userToken);
							this.MailboxSession = MailboxSession.Open(mailboxOwner, clientSecurityContext, CultureInfo.InvariantCulture, string.Format("Client=Management;Action={0}", action));
						}
					}
					catch (AuthzException ex)
					{
						throw new AccessDeniedException(new LocalizedString(ex.Message));
					}
				}
				disposeGuard.Success();
			}
		}

		public XsoMailboxDataProviderBase(MailboxSession session) : base(session)
		{
		}

		public XsoMailboxDataProviderBase()
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				string stackTrace = Environment.StackTrace;
				if (!stackTrace.Contains("Internal.Exchange.Management.OWAOptionTasks.XsoDriverUnitTest") && !stackTrace.Contains("Internal.Exchange.Test.Data.Storage.DisposeSuite") && !stackTrace.Contains("Internal.Exchange.Migration.MigrationUnitTests"))
				{
					throw new InvalidOperationException(string.Format("The default constructor is used only to help test code right now. Current stack trace is: {0}", stackTrace));
				}
				disposeGuard.Success();
			}
		}
	}
}
