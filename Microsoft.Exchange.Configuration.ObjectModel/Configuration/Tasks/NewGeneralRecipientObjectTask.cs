using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewGeneralRecipientObjectTask<TDataObject> : NewRecipientObjectTaskBase<TDataObject> where TDataObject : ADRecipient, new()
	{
		protected ADObjectId RecipientContainerId
		{
			get
			{
				return this.containerId;
			}
		}

		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.Name;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.Name = value;
			}
		}

		[Parameter]
		public string DisplayName
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.DisplayName;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.DisplayName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["OrganizationalUnit"];
			}
			set
			{
				base.Fields["OrganizationalUnit"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public string ExternalDirectoryObjectId
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.ExternalDirectoryObjectId;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.ExternalDirectoryObjectId = value;
			}
		}

		public RecipientIdParameter SoftDeletedObject
		{
			get
			{
				return (RecipientIdParameter)base.Fields["SoftDeletedObject"];
			}
			set
			{
				base.Fields["SoftDeletedObject"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (!string.IsNullOrEmpty(this.ExternalDirectoryObjectId))
			{
				ITenantRecipientSession tenantRecipientSession = base.TenantGlobalCatalogSession as ITenantRecipientSession;
				if (tenantRecipientSession != null)
				{
					bool useGlobalCatalog = tenantRecipientSession.UseGlobalCatalog;
					tenantRecipientSession.UseGlobalCatalog = false;
					Result<ADRawEntry>[] array = null;
					try
					{
						array = tenantRecipientSession.FindByExternalDirectoryObjectIds(new string[]
						{
							this.ExternalDirectoryObjectId
						}, true, new ADPropertyDefinition[]
						{
							DeletedObjectSchema.LastKnownParent
						});
					}
					finally
					{
						tenantRecipientSession.UseGlobalCatalog = useGlobalCatalog;
					}
					if (array != null)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i].Error != ProviderError.NotFound || array[i].Data != null)
							{
								ADObjectId adobjectId = null;
								if (array[i].Error == null)
								{
									adobjectId = (ADObjectId)array[i].Data[DeletedObjectSchema.LastKnownParent];
								}
								if (array[i].Error != null || adobjectId == null || (adobjectId.DomainId != null && !adobjectId.IsDescendantOf(ADSession.GetDeletedObjectsContainer(adobjectId.DomainId))))
								{
									base.ThrowTerminatingError(new DuplicateExternalDirectoryObjectIdException(this.Name, this.ExternalDirectoryObjectId), ExchangeErrorCategory.Client, null);
								}
							}
						}
					}
				}
			}
			bool useConfigNC = this.ConfigurationSession.UseConfigNC;
			bool useGlobalCatalog2 = this.ConfigurationSession.UseGlobalCatalog;
			this.ConfigurationSession.UseConfigNC = false;
			this.ConfigurationSession.UseGlobalCatalog = true;
			IConfigurationSession cfgSession = this.ConfigurationSession;
			if (!cfgSession.IsReadConnectionAvailable())
			{
				cfgSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, this.ConfigurationSession.SessionSettings, 623, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\NewAdObjectTask.cs");
				cfgSession.UseGlobalCatalog = true;
				cfgSession.UseConfigNC = false;
			}
			try
			{
				ExchangeOrganizationalUnit exchangeOrganizationalUnit = null;
				if (this.OrganizationalUnit != null)
				{
					exchangeOrganizationalUnit = base.ProvisioningCache.TryAddAndGetGlobalDictionaryValue<ExchangeOrganizationalUnit, string>(CannedProvisioningCacheKeys.OrganizationalUnitDictionary, this.OrganizationalUnit.RawIdentity, () => (ExchangeOrganizationalUnit)this.GetDataObject<ExchangeOrganizationalUnit>(this.OrganizationalUnit, cfgSession, (this.CurrentOrganizationId != null) ? this.CurrentOrganizationId.OrganizationalUnit : null, null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(this.OrganizationalUnit.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(this.OrganizationalUnit.ToString()))));
				}
				if (exchangeOrganizationalUnit != null)
				{
					this.containerId = exchangeOrganizationalUnit.Id;
				}
				else if (base.CurrentOrganizationId != null && base.CurrentOrganizationId.OrganizationalUnit != null)
				{
					this.containerId = base.CurrentOrganizationId.OrganizationalUnit;
				}
				else
				{
					string defaultOUForRecipient = RecipientTaskHelper.GetDefaultOUForRecipient(base.ServerSettings.RecipientViewRoot);
					if (string.IsNullOrEmpty(defaultOUForRecipient))
					{
						base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorCannotDiscoverDefaultOrganizationUnitForRecipient), ExchangeErrorCategory.Client, null);
					}
					exchangeOrganizationalUnit = (ExchangeOrganizationalUnit)base.GetDataObject<ExchangeOrganizationalUnit>(new OrganizationalUnitIdParameter(defaultOUForRecipient), cfgSession, null, null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(defaultOUForRecipient)), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(defaultOUForRecipient)), ExchangeErrorCategory.Client);
					this.containerId = exchangeOrganizationalUnit.Id;
				}
				if (exchangeOrganizationalUnit != null)
				{
					RecipientTaskHelper.IsOrgnizationalUnitInOrganization(cfgSession, base.CurrentOrganizationId, exchangeOrganizationalUnit, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			finally
			{
				this.ConfigurationSession.UseConfigNC = useConfigNC;
				this.ConfigurationSession.UseGlobalCatalog = useGlobalCatalog2;
			}
		}

		protected override void PrepareRecipientObject(TDataObject dataObject)
		{
			dataObject.SetId(this.RecipientContainerId.GetChildId(this.Name));
		}

		private ADObjectId containerId;
	}
}
