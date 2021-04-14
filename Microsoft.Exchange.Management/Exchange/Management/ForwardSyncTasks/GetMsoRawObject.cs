using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Get", "MsoRawObject", DefaultParameterSetName = "ExchangeIdentity")]
	public sealed class GetMsoRawObject : Task
	{
		private IRecipientSession GetRecipientSession(OrganizationId orgId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, null, false);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.RescopeToSubtree(sessionSettings), 99, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\GetMsoRawObject.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			tenantOrRootOrgRecipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
			return tenantOrRootOrgRecipientSession;
		}

		private IConfigurationSession GetOrgConfigurationSession(OrganizationId orgId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, null, false);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 116, "GetOrgConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\GetMsoRawObject.cs");
		}

		[Parameter(ParameterSetName = "ExchangeIdentity", ValueFromPipeline = true, Mandatory = true, Position = 0)]
		public RecipientOrOrganizationIdParameter Identity
		{
			get
			{
				return (RecipientOrOrganizationIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(ParameterSetName = "SyncObjectId", Mandatory = true)]
		public SyncObjectId ExternalObjectId
		{
			get
			{
				return (SyncObjectId)base.Fields["ExternalObjectID"];
			}
			set
			{
				base.Fields["ExternalObjectID"] = value;
			}
		}

		[Parameter(ParameterSetName = "SyncObjectId", Mandatory = true)]
		public string ServiceInstanceId
		{
			get
			{
				return (string)base.Fields["ServiceInstanceId"];
			}
			set
			{
				base.Fields["ServiceInstanceId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeBackLinks
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeBackLinks"] ?? false);
			}
			set
			{
				base.Fields["IncludeBackLinks"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeForwardLinks
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeForwardLinks"] ?? false);
			}
			set
			{
				base.Fields["IncludeForwardLinks"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int LinksResultSize
		{
			get
			{
				return (int)base.Fields["LinksResultSize"];
			}
			set
			{
				base.Fields["LinksResultSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter PopulateRawObject
		{
			get
			{
				return (SwitchParameter)(base.Fields["PopulateRawObject"] ?? false);
			}
			set
			{
				base.Fields["PopulateRawObject"] = value;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskBaseModuleFactory();
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			bool flag = true;
			try
			{
				if (this.ExternalObjectId != null)
				{
					this.ProcessMsoRawObject(this.ExternalObjectId, this.ServiceInstanceId);
				}
				else if (this.Identity.ResolvedSyncObjectId != null && this.Identity.ResolvedServiceInstanceId != null)
				{
					this.ProcessMsoRawObject(this.Identity.ResolvedSyncObjectId, this.Identity.ResolvedServiceInstanceId);
				}
				else
				{
					if (this.Identity.RecipientParameter != null)
					{
						OrganizationId orgId = this.Identity.RecipientParameter.ResolveOrganizationIdBasedOnIdentity(OrganizationId.ForestWideOrgId);
						IRecipientSession recipientSession = this.GetRecipientSession(orgId);
						IEnumerable<ADRecipient> objects = this.Identity.RecipientParameter.GetObjects<ADRecipient>(null, recipientSession);
						foreach (ADRecipient adrecipient in objects)
						{
							flag = false;
							if (adrecipient.ConfigurationUnit == null)
							{
								this.WriteError(new Exception(Strings.RecipientFromFirstOrganization(adrecipient.ToString())), ErrorCategory.ObjectNotFound, null, false);
							}
							else if (string.IsNullOrEmpty(adrecipient.ExternalDirectoryObjectId))
							{
								this.WriteError(new Exception(Strings.RecipientHasNoExternalId(adrecipient.ToString())), ErrorCategory.ObjectNotFound, null, false);
							}
							else
							{
								DirectoryObjectClass msoObjectClassForRecipient = this.GetMsoObjectClassForRecipient(adrecipient.RecipientType, adrecipient.RecipientTypeDetails);
								if (msoObjectClassForRecipient != DirectoryObjectClass.Company)
								{
									IConfigurationSession orgConfigurationSession = this.GetOrgConfigurationSession(adrecipient.OrganizationId);
									ExchangeConfigurationUnit exchangeConfigurationUnit = orgConfigurationSession.Read<ExchangeConfigurationUnit>(adrecipient.ConfigurationUnit);
									if (exchangeConfigurationUnit == null)
									{
										this.WriteError(new Exception(Strings.RecipientTenantNotFound(adrecipient.ToString())), ErrorCategory.ObjectNotFound, null, false);
									}
									else if (string.IsNullOrEmpty(exchangeConfigurationUnit.DirSyncServiceInstance))
									{
										this.WriteError(new Exception(Strings.RecipientWithTenantServiceInstanceNotSet(adrecipient.ToString())), ErrorCategory.ObjectNotFound, null, false);
									}
									else
									{
										this.ProcessMsoRawObject(new SyncObjectId(exchangeConfigurationUnit.ExternalDirectoryOrganizationId, adrecipient.ExternalDirectoryObjectId, msoObjectClassForRecipient), exchangeConfigurationUnit.DirSyncServiceInstance);
									}
								}
							}
						}
					}
					if (this.Identity.OrganizationParameter != null)
					{
						OrganizationId orgId2 = this.Identity.OrganizationParameter.ResolveOrganizationIdBasedOnIdentity(OrganizationId.ForestWideOrgId);
						IConfigurationSession orgConfigurationSession2 = this.GetOrgConfigurationSession(orgId2);
						IEnumerable<ExchangeConfigurationUnit> objects2 = this.Identity.OrganizationParameter.GetObjects<ExchangeConfigurationUnit>(null, orgConfigurationSession2);
						foreach (ExchangeConfigurationUnit exchangeConfigurationUnit2 in objects2)
						{
							flag = false;
							if (string.IsNullOrEmpty(exchangeConfigurationUnit2.DirSyncServiceInstance))
							{
								this.WriteError(new Exception(Strings.TenantServiceInstanceNotSet(exchangeConfigurationUnit2.ConfigurationUnit.ToString())), ErrorCategory.ObjectNotFound, null, false);
							}
							else
							{
								this.ProcessMsoRawObject(new SyncObjectId(exchangeConfigurationUnit2.ExternalDirectoryOrganizationId, exchangeConfigurationUnit2.ExternalDirectoryOrganizationId, DirectoryObjectClass.Company), exchangeConfigurationUnit2.DirSyncServiceInstance);
							}
						}
					}
					if (flag)
					{
						if (this.Identity.RecipientParameter != null)
						{
							this.WriteError(new Exception(Strings.ExchangeRecipientNotFound(this.Identity.RecipientParameter.ToString())), ErrorCategory.ObjectNotFound, null, false);
						}
						if (this.Identity.OrganizationParameter != null)
						{
							this.WriteError(new Exception(Strings.ExchangeTenantNotFound(this.Identity.OrganizationParameter.ToString())), ErrorCategory.ObjectNotFound, null, false);
						}
					}
				}
			}
			catch (CouldNotCreateMsoSyncServiceException exception)
			{
				this.WriteError(exception, ErrorCategory.ObjectNotFound, null, true);
			}
			catch (InvalidMsoSyncServiceResponseException exception2)
			{
				this.WriteError(exception2, ErrorCategory.InvalidOperation, null, false);
			}
			catch (Exception exception3)
			{
				this.WriteError(exception3, ErrorCategory.InvalidOperation, null, true);
			}
		}

		private DirectoryObjectClass GetMsoObjectClassForRecipient(RecipientType recipientType, RecipientTypeDetails recipientTypeDetails)
		{
			switch (recipientType)
			{
			case RecipientType.User:
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
				if ((recipientTypeDetails & RecipientTypeDetails.GroupMailbox) != RecipientTypeDetails.None)
				{
					return DirectoryObjectClass.Group;
				}
				return DirectoryObjectClass.User;
			case RecipientType.Contact:
			case RecipientType.MailContact:
				return DirectoryObjectClass.Contact;
			case RecipientType.Group:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
				return DirectoryObjectClass.Group;
			default:
				return DirectoryObjectClass.Company;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.service != null)
			{
				this.service.Dispose();
				this.service = null;
			}
			base.Dispose(disposing);
		}

		private void ProcessMsoRawObject(SyncObjectId syncObjectId, string serviceInstanceId)
		{
			if (this.service == null)
			{
				this.service = new MsoSyncService();
			}
			bool? allLinksCollected;
			DirectoryObjectsAndLinks msoRawObject = this.service.GetMsoRawObject(syncObjectId, serviceInstanceId, this.IncludeBackLinks.IsPresent, this.IncludeForwardLinks.IsPresent, base.Fields.IsModified("LinksResultSize") ? this.LinksResultSize : 1000, out allLinksCollected);
			if (msoRawObject.Objects.Length != 0)
			{
				MsoRawObject msoRawObject2 = new MsoRawObject(syncObjectId, serviceInstanceId, msoRawObject, allLinksCollected, this.PopulateRawObject.IsPresent);
				msoRawObject2.PopulateSyncObjectData();
				base.WriteObject(msoRawObject2);
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (msoRawObject.Errors.Length != 0)
			{
				foreach (DirectoryObjectError directoryObjectError in msoRawObject.Errors)
				{
					stringBuilder.Append(directoryObjectError.ErrorCode);
					stringBuilder.Append(";");
				}
			}
			else
			{
				stringBuilder.Append("no errors");
			}
			this.WriteError(new Exception(Strings.MsoObjectNotFound(syncObjectId.ToString(), stringBuilder.ToString())), ErrorCategory.ObjectNotFound, null, false);
		}

		private const int DefaultLinksResultSize = 1000;

		private MsoSyncService service;
	}
}
