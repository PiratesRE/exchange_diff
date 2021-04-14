using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security;
using System.Security.AccessControl;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "UserAccount", SupportsShouldProcess = true)]
	public sealed class InstallUserAccount : NewADTaskBase<ADUser>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewUser(this.Name.ToString());
			}
		}

		[Parameter(Mandatory = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string LastName
		{
			get
			{
				return this.DataObject.LastName;
			}
			set
			{
				this.DataObject.LastName = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Domain
		{
			get
			{
				return (string)base.Fields["Domain"];
			}
			set
			{
				base.Fields["Domain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter LogonEnabled { get; set; }

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, ConfigScopes.TenantSubTree, 101, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\InstallUserAccount.cs");
			tenantOrRootOrgRecipientSession.LinkResolutionServer = ADSession.GetCurrentConfigDC(tenantOrRootOrgRecipientSession.SessionSettings.GetAccountOrResourceForestFqdn());
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = false;
			return tenantOrRootOrgRecipientSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			ADForest localForest = ADForest.GetLocalForest();
			if (base.Fields.IsModified("Domain"))
			{
				this.adDomain = localForest.FindDomainByFqdn(this.Domain);
				if (this.adDomain == null)
				{
					base.WriteError(new DomainNotFoundException(this.Domain), ErrorCategory.InvalidArgument, null);
				}
			}
			else
			{
				this.adDomain = localForest.FindLocalDomain();
			}
			string defaultOUForRecipient = RecipientTaskHelper.GetDefaultOUForRecipient(this.adDomain.Id);
			if (string.IsNullOrEmpty(defaultOUForRecipient))
			{
				base.WriteError(new ArgumentException(Strings.UsersContainerNotFound(this.adDomain.Fqdn, WellKnownGuid.UsersWkGuid)), ErrorCategory.InvalidArgument, null);
			}
			this.containerId = new ADObjectId(NativeHelpers.DistinguishedNameFromCanonicalName(defaultOUForRecipient));
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.PrepareDataObject();
			aduser.SetId(this.containerId.GetChildId(this.Name));
			if (string.IsNullOrEmpty(aduser.UserPrincipalName))
			{
				aduser.UserPrincipalName = aduser.Name + "@" + this.adDomain.Fqdn;
			}
			if (string.IsNullOrEmpty(aduser.SamAccountName))
			{
				aduser.SamAccountName = "SM_" + Guid.NewGuid().ToString("N").Substring(0, 17);
			}
			TaskLogger.LogExit();
			return aduser;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ADUser dataObject = this.DataObject;
				IRecipientSession recipientSession = (IRecipientSession)base.DataSession;
				recipientSession.Save(dataObject);
				ADUser aduser = (ADUser)base.DataSession.Read<ADUser>(dataObject.Identity);
				if (aduser == null)
				{
					throw new LocalizedException(Strings.ErrorReadingUpdatedUserFromAD(dataObject.OriginatingServer, recipientSession.LastUsedDc));
				}
				aduser.UserAccountControl = UserAccountControlFlags.None;
				if (this.LogonEnabled)
				{
					using (SecureString randomPassword = MailboxTaskUtilities.GetRandomPassword(this.Name, aduser.SamAccountName))
					{
						recipientSession.SetPassword(aduser, randomPassword);
						goto IL_98;
					}
				}
				aduser.UserAccountControl |= UserAccountControlFlags.AccountDisabled;
				IL_98:
				aduser.UserAccountControl |= UserAccountControlFlags.NormalAccount;
				this.DataObject = aduser;
				base.InternalProcessRecord();
			}
			catch (ADObjectAlreadyExistsException ex)
			{
				base.WriteVerbose(Strings.UserCreateFailed(this.Name, ex.Message.ToString()));
			}
			LocalizedString localizedString = LocalizedString.Empty;
			try
			{
				base.WriteVerbose(Strings.VerboseGrantingEoaFullAccessOnMailbox(this.DataObject.Identity.ToString()));
				ADGroup adgroup = base.RootOrgGlobalCatalogSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.EoaWkGuid, base.GlobalConfigSession.ConfigurationNamingContext.ToDNString());
				if (adgroup == null)
				{
					localizedString = Strings.ErrorGroupNotFound(WellKnownGuid.EoaWkGuid.ToString());
				}
				else
				{
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, (IDirectorySession)base.DataSession, this.DataObject.Id, new ActiveDirectoryAccessRule[]
					{
						new ActiveDirectoryAccessRule(adgroup.Sid, ActiveDirectoryRights.GenericAll, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All)
					});
				}
			}
			catch (ADTransientException ex2)
			{
				localizedString = ex2.LocalizedString;
			}
			catch (ADOperationException ex3)
			{
				localizedString = ex3.LocalizedString;
			}
			catch (SecurityDescriptorAccessDeniedException ex4)
			{
				localizedString = ex4.LocalizedString;
			}
			if (LocalizedString.Empty != localizedString)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorGrantingEraFullAccessOnMailbox(this.DataObject.Identity.ToString(), localizedString)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		private const string paramDomain = "Domain";

		private ADObjectId containerId;

		private ADDomain adDomain;
	}
}
