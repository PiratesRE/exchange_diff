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
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Set", "ConnectSubscription", SupportsShouldProcess = true, DefaultParameterSetName = "FacebookParameterSet")]
	public sealed class SetConnectSubscription : SetSubscriptionBase<ConnectSubscriptionProxy>
	{
		[ValidateNotNull]
		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		public new AggregationSubscriptionIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
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

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "FacebookParameterSet")]
		[ValidateNotNullOrEmpty]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "LinkedInParameterSet")]
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

		public new string DisplayName
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		protected override AggregationSubscriptionType IdentityType
		{
			get
			{
				string parameterSetName;
				if ((parameterSetName = base.ParameterSetName) != null)
				{
					if (parameterSetName == "FacebookParameterSet")
					{
						return AggregationSubscriptionType.Facebook;
					}
					if (parameterSetName == "LinkedInParameterSet")
					{
						return AggregationSubscriptionType.LinkedIn;
					}
				}
				throw new InvalidOperationException();
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
						return Strings.SetFacebookSubscriptionConfirmation;
					}
					if (parameterSetName == "LinkedInParameterSet")
					{
						return Strings.SetLinkedInSubscriptionConfirmation;
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
			IConfigDataProvider result = base.CreateSession();
			this.InitializeMailboxPrincipal();
			return result;
		}

		private void InitializeMailboxPrincipal()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 221, "InitializeMailboxPrincipal", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Aggregation\\SetConnectSubscription.cs");
			ADUser user = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorUserNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(this.Mailbox.ToString())));
			this.mailboxPrincipal = ExchangePrincipal.FromADUser(tenantOrRootOrgRecipientSession.SessionSettings, user, RemotingOptions.AllowCrossSite);
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			base.StampChangesOn(dataObject);
			ConnectSubscriptionProxy connectSubscriptionProxy = dataObject as ConnectSubscriptionProxy;
			if (connectSubscriptionProxy == null)
			{
				return;
			}
			if (base.Fields.IsModified("AppAuthorizationCode"))
			{
				connectSubscriptionProxy.AppAuthorizationCode = this.AppAuthorizationCode;
			}
			if (base.Fields.IsModified("RedirectUri"))
			{
				connectSubscriptionProxy.RedirectUri = this.RedirectUri;
			}
			if (base.Fields.IsModified("RequestToken"))
			{
				connectSubscriptionProxy.RequestToken = this.RequestToken;
			}
			if (base.Fields.IsModified("RequestSecret"))
			{
				connectSubscriptionProxy.RequestSecret = this.RequestSecret;
			}
			if (base.Fields.IsModified("OAuthVerifier"))
			{
				connectSubscriptionProxy.OAuthVerifier = this.OAuthVerifier;
			}
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (!(parameterSetName == "FacebookParameterSet"))
				{
					if (!(parameterSetName == "LinkedInParameterSet"))
					{
						goto IL_ED;
					}
					this.providerImpl = new SetLinkedInSubscription();
				}
				else
				{
					this.providerImpl = new SetFacebookSubscription();
				}
				this.providerImpl.StampChangesOn(connectSubscriptionProxy);
				TaskLogger.LogExit();
				return;
			}
			IL_ED:
			throw new InvalidOperationException();
		}

		protected override bool SendAsCheckNeeded()
		{
			return false;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || ConnectSubscriptionTaskKnownExceptions.IsKnown(exception);
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			this.NotifyApps();
		}

		private void NotifyApps()
		{
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.mailboxPrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=SetConnectSubscription"))
			{
				this.providerImpl.NotifyApps(mailboxSession);
			}
		}

		private const string ClientInfoString = "Client=Management;Action=SetConnectSubscription";

		private const string FacebookParameterSet = "FacebookParameterSet";

		private const string LinkedInParameterSet = "LinkedInParameterSet";

		private ISetConnectSubscription providerImpl;

		private ExchangePrincipal mailboxPrincipal;
	}
}
