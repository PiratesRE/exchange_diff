using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "OAuthConnectivity", SupportsShouldProcess = true)]
	public sealed class TestOAuthConnectivity : DataAccessTask<ADUser>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter AppOnly { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter UseCachedToken { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter ReloadConfig { get; set; }

		[Parameter(Mandatory = true)]
		public ModServiceType Service
		{
			get
			{
				return (ModServiceType)base.Fields["Service"];
			}
			set
			{
				base.Fields["Service"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public Uri TargetUri
		{
			get
			{
				return (Uri)base.Fields["TargetUri"];
			}
			set
			{
				base.Fields["TargetUri"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OrganizationDomain
		{
			get
			{
				return (string)base.Fields["OrganizationDomain"];
			}
			set
			{
				base.Fields["OrganizationDomain"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.ExecutingUserOrganizationId, base.ExecutingUserOrganizationId, true), 115, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Tasks\\TestOAuthConnectivity.cs");
		}

		protected override void InternalProcessRecord()
		{
			ADUser aduser = null;
			string empty = string.Empty;
			ResultType type = ResultType.Success;
			if (this.Mailbox != null)
			{
				aduser = (ADUser)base.GetDataObject(this.Mailbox);
				if (aduser == null && !this.AppOnly)
				{
					base.ThrowTerminatingError(new MailboxUserNotFoundException(this.Mailbox.ToString()), ErrorCategory.ObjectNotFound, null);
				}
			}
			if (this.AppOnly)
			{
				if (this.Mailbox == null && string.IsNullOrEmpty(this.OrganizationDomain))
				{
					base.ThrowTerminatingError(new NoUserOrOrganiztionProvidedException(), ErrorCategory.ObjectNotFound, null);
				}
				if (this.Service == ModServiceType.EWS)
				{
					base.ThrowTerminatingError(new EwsNotSupportedException(), ErrorCategory.NotEnabled, null);
				}
			}
			else if (this.Mailbox == null)
			{
				base.ThrowTerminatingError(new MailboxParameterMissingException(), ErrorCategory.ObjectNotFound, null);
			}
			switch (this.Service)
			{
			case ModServiceType.EWS:
				type = TestOAuthConnectivityHelper.SendExchangeOAuthRequest(aduser, this.OrganizationDomain, this.TargetUri, out empty, this.AppOnly, this.UseCachedToken, this.ReloadConfig);
				break;
			case ModServiceType.AutoD:
				type = TestOAuthConnectivityHelper.SendAutodiscoverOAuthRequest(aduser, this.OrganizationDomain, this.TargetUri, out empty, this.AppOnly, this.UseCachedToken, this.ReloadConfig);
				break;
			case ModServiceType.Generic:
				type = TestOAuthConnectivityHelper.SendGenericOAuthRequest(aduser, this.OrganizationDomain, this.TargetUri, out empty, this.AppOnly, this.UseCachedToken, this.ReloadConfig);
				break;
			}
			ValidationResultNode sendToPipeline = new ValidationResultNode(Strings.TestApiCallUnderOauthTask(this.Service.ToString()), new LocalizedString(empty), type);
			base.WriteObject(sendToPipeline);
		}
	}
}
