using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("New", "ConnectSubscription", SupportsShouldProcess = true, DefaultParameterSetName = "FacebookParameterSet")]
	public sealed class NewConnectSubscription : NewSubscriptionBase<ConnectSubscriptionProxy>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		public new MailboxIdParameter Mailbox
		{
			get
			{
				return base.Mailbox;
			}
			set
			{
				base.Mailbox = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "FacebookParameterSet")]
		public SwitchParameter Facebook
		{
			get
			{
				if (base.Fields["Facebook"] == null)
				{
					return new SwitchParameter(false);
				}
				return (SwitchParameter)base.Fields["Facebook"];
			}
			set
			{
				base.Fields["Facebook"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "FacebookParameterSet")]
		public string AppAuthorizationCode
		{
			get
			{
				return (string)base.Fields["AppAuthorizationCode"];
			}
			set
			{
				base.Fields["AppAuthorizationCode"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "FacebookParameterSet")]
		public string RedirectUri
		{
			get
			{
				return (string)base.Fields["RedirectUri"];
			}
			set
			{
				base.Fields["RedirectUri"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "LinkedInParameterSet")]
		public SwitchParameter LinkedIn
		{
			get
			{
				if (base.Fields["LinkedIn"] == null)
				{
					return new SwitchParameter(false);
				}
				return (SwitchParameter)base.Fields["LinkedIn"];
			}
			set
			{
				base.Fields["LinkedIn"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "LinkedInParameterSet")]
		[ValidateNotNullOrEmpty]
		public string RequestToken
		{
			get
			{
				return (string)base.Fields["RequestToken"];
			}
			set
			{
				base.Fields["RequestToken"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "LinkedInParameterSet")]
		[ValidateNotNullOrEmpty]
		public string RequestSecret
		{
			get
			{
				return (string)base.Fields["RequestSecret"];
			}
			set
			{
				base.Fields["RequestSecret"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "LinkedInParameterSet")]
		[ValidateNotNullOrEmpty]
		public string OAuthVerifier
		{
			get
			{
				return (string)base.Fields["OAuthVerifier"];
			}
			set
			{
				base.Fields["OAuthVerifier"] = value;
			}
		}

		public new string Name
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public new string DisplayName
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public new SmtpAddress EmailAddress
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string parameterSetName;
				if ((parameterSetName = base.ParameterSetName) != null)
				{
					if (parameterSetName == "FacebookParameterSet")
					{
						return Strings.CreateFacebookSubscriptionConfirmation;
					}
					if (parameterSetName == "LinkedInParameterSet")
					{
						return Strings.CreateLinkedInSubscriptionConfirmation;
					}
				}
				throw new InvalidOperationException();
			}
		}

		protected override AggregationType AggregationType
		{
			get
			{
				return AggregationType.PeopleConnection;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			this.InitializeMailboxPrincipal();
			return base.CreateSession();
		}

		protected override IConfigurable PrepareDataObject()
		{
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (!(parameterSetName == "FacebookParameterSet"))
				{
					if (!(parameterSetName == "LinkedInParameterSet"))
					{
						goto IL_40;
					}
					this.providerImpl = new NewLinkedInSubscription();
				}
				else
				{
					this.providerImpl = new NewFacebookSubscription();
				}
				base.Name = this.providerImpl.SubscriptionName;
				base.DisplayName = this.providerImpl.SubscriptionDisplayName;
				base.EmailAddress = SmtpAddress.NullReversePath;
				return this.PrepareSubscription((ConnectSubscriptionProxy)base.PrepareDataObject());
			}
			IL_40:
			throw new InvalidOperationException();
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			this.InitializeFolderAndNotifyApps();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || ConnectSubscriptionTaskKnownExceptions.IsKnown(exception);
		}

		private IConfigurable PrepareSubscription(ConnectSubscriptionProxy subscription)
		{
			subscription.AppAuthorizationCode = this.AppAuthorizationCode;
			subscription.RedirectUri = this.RedirectUri;
			subscription.RequestToken = this.RequestToken;
			subscription.RequestSecret = this.RequestSecret;
			subscription.OAuthVerifier = this.OAuthVerifier;
			IConfigurable result;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.mailboxPrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=NewConnectSubscription"))
			{
				result = this.providerImpl.PrepareSubscription(mailboxSession, subscription);
			}
			return result;
		}

		private void InitializeFolderAndNotifyApps()
		{
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.mailboxPrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=NewConnectSubscription"))
			{
				this.providerImpl.InitializeFolderAndNotifyApps(mailboxSession, this.DataObject);
			}
		}

		private void InitializeMailboxPrincipal()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 311, "InitializeMailboxPrincipal", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Aggregation\\NewConnectSubscription.cs");
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorUserNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(this.Mailbox.ToString())));
			ADSessionSettings adSettings = ADSessionSettings.RescopeToOrganization(base.SessionSettings, aduser.OrganizationId, true);
			this.mailboxPrincipal = ExchangePrincipal.FromADUser(adSettings, aduser, RemotingOptions.AllowCrossSite);
		}

		private const string ClientInfoString = "Client=Management;Action=NewConnectSubscription";

		private const string FacebookParameterSet = "FacebookParameterSet";

		private const string LinkedInParameterSet = "LinkedInParameterSet";

		private INewConnectSubscription providerImpl;

		private ExchangePrincipal mailboxPrincipal;
	}
}
