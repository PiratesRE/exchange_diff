using System;
using System.Management.Automation;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "LinkedUser", SupportsShouldProcess = true)]
	public sealed class NewLinkedUser : NewGeneralRecipientObjectTask<ADUser>
	{
		[Parameter(Mandatory = true)]
		public string UserPrincipalName
		{
			get
			{
				return this.DataObject.UserPrincipalName;
			}
			set
			{
				this.DataObject.UserPrincipalName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UserIdParameter LinkedMasterAccount
		{
			get
			{
				return (UserIdParameter)base.Fields["LinkedMasterAccount"];
			}
			set
			{
				base.Fields["LinkedMasterAccount"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LinkedDomainController
		{
			get
			{
				return (string)base.Fields["LinkedDomainController"];
			}
			set
			{
				base.Fields["LinkedDomainController"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential LinkedCredential
		{
			get
			{
				return (PSCredential)base.Fields["LinkedCredential"];
			}
			set
			{
				base.Fields["LinkedCredential"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<X509Identifier> CertificateSubject
		{
			get
			{
				return this.DataObject.CertificateSubject;
			}
			set
			{
				this.DataObject.CertificateSubject = value;
			}
		}

		private new string ExternalDirectoryObjectId
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewUser(base.Name.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.LinkedMasterAccount != null)
			{
				if (string.IsNullOrEmpty(this.LinkedDomainController))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMissLinkedDomainController), ErrorCategory.InvalidArgument, base.Name);
				}
				try
				{
					NetworkCredential userForestCredential = (this.LinkedCredential == null) ? null : this.LinkedCredential.GetNetworkCredential();
					this.linkedUserSid = MailboxTaskHelper.GetAccountSidFromAnotherForest(this.LinkedMasterAccount, this.LinkedDomainController, userForestCredential, base.GlobalConfigSession, new MailboxTaskHelper.GetUniqueObject(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
				}
				catch (PSArgumentException exception)
				{
					base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, this.LinkedCredential);
				}
			}
		}

		protected override void PrepareRecipientObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(user);
			user.RecipientTypeDetails = RecipientTypeDetails.LinkedUser;
			user.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.LinkedUser);
			if (this.LinkedMasterAccount != null)
			{
				user.MasterAccountSid = this.linkedUserSid;
			}
			RecipientTaskHelper.IsUserPrincipalNameUnique(base.TenantGlobalCatalogSession, user, user.UserPrincipalName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			NewLinkedUser.ValidateCertificateSubject(this.CertificateSubject, OrganizationId.ForestWideOrgId.Equals(base.CurrentOrganizationId) ? null : base.CurrentOrganizationId.PartitionId, null, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		internal static void ValidateCertificateSubject(MultiValuedProperty<X509Identifier> certificateSubjects, PartitionId partitionId, ADObjectId excludeObjectId, Task.TaskErrorLoggingDelegate errorLogger)
		{
			if (errorLogger == null)
			{
				throw new ArgumentNullException("errorLogger");
			}
			ADSessionSettings sessionSettings;
			if (partitionId != null)
			{
				sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			}
			else
			{
				sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 220, "ValidateCertificateSubject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\LinkedUser\\NewLinkedUser.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			if (certificateSubjects != null && certificateSubjects.Count != 0)
			{
				foreach (X509Identifier x509Identifier in certificateSubjects)
				{
					QueryFilter queryFilter = ADUser.GetCertificateMatchFilter(x509Identifier);
					if (excludeObjectId != null)
					{
						queryFilter = new AndFilter(new QueryFilter[]
						{
							queryFilter,
							new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, excludeObjectId)
						});
					}
					ADRecipient[] array = tenantOrRootOrgRecipientSession.Find(null, QueryScope.SubTree, queryFilter, null, 1);
					if (array != null && array.Length > 0)
					{
						errorLogger(new RecipientTaskException(Strings.ErrorDuplicateCertificateSubject(x509Identifier.ToString())), ErrorCategory.InvalidArgument, excludeObjectId);
					}
				}
			}
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			ADUser aduser = (ADUser)result;
			if (null != aduser.MasterAccountSid)
			{
				aduser.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(aduser.MasterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				aduser.ResetChangeTracking();
			}
			base.WriteResult(new LinkedUser(aduser));
			TaskLogger.LogExit();
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(LinkedUser).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return User.FromDataObject((ADUser)dataObject);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private SecurityIdentifier linkedUserSid;
	}
}
