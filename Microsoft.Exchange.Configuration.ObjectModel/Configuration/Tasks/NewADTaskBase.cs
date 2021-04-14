using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewADTaskBase<TDataObject> : NewTenantADTaskBase<TDataObject> where TDataObject : ADObject, new()
	{
		protected virtual string ClonableTypeName
		{
			get
			{
				return NewADTaskBase<TDataObject>.clonableTypeName;
			}
		}

		protected virtual bool EnforceExchangeObjectVersion
		{
			get
			{
				return true;
			}
		}

		protected PSObject TemplateInstance
		{
			get
			{
				return (PSObject)base.Fields["TemplateInstance"];
			}
			set
			{
				if (!value.TypeNames.Contains(this.ClonableTypeName) && !value.TypeNames.Contains("Deserialized." + this.ClonableTypeName))
				{
					throw new ArgumentException(Strings.ErrorInvalidParameterType("TemplateInstance", this.ClonableTypeName));
				}
				base.Fields["TemplateInstance"] = value;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new ADObjectTaskModuleFactory();
		}

		protected virtual void StampDefaultValues(TDataObject dataObject)
		{
			dataObject.StampPersistableDefaultValues();
		}

		protected sealed override void InitializeDataObject(TDataObject dataObject)
		{
			this.StampDefaultValues(dataObject);
			dataObject.ResetChangeTracking();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TDataObject tdataObject = (TDataObject)((object)base.PrepareDataObject());
			if (this.TemplateInstance != null)
			{
				TDataObject tdataObject2 = Activator.CreateInstance<TDataObject>();
				this.InitializeDataObject(tdataObject2);
				tdataObject2.ProvisionalClone(new PSObjectWrapper(this.TemplateInstance));
				tdataObject2.CopyChangesFrom(tdataObject);
				tdataObject = tdataObject2;
			}
			if (base.CurrentOrganizationId != null)
			{
				tdataObject.OrganizationId = base.CurrentOrganizationId;
			}
			else
			{
				tdataObject.OrganizationId = base.ExecutingUserOrganizationId;
			}
			TaskLogger.LogExit();
			return tdataObject;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			TDataObject dataObject = this.DataObject;
			ADObjectId parent = dataObject.Id.Parent;
			ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(parent);
			if (!parent.Equals(ADSession.GetRootDomainNamingContext(adsessionSettings.GetAccountOrResourceForestFqdn())))
			{
				IDirectorySession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, adsessionSettings, 264, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\NewAdObjectTask.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = ((IDirectorySession)base.DataSession).UseConfigNC;
				ADRawEntry adrawEntry = tenantOrTopologyConfigurationSession.ReadADRawEntry(parent, new PropertyDefinition[]
				{
					ADObjectSchema.ExchangeVersion,
					ADObjectSchema.ObjectClass
				});
				if (adrawEntry == null)
				{
					if (string.IsNullOrEmpty(base.DomainController))
					{
						TDataObject dataObject2 = this.DataObject;
						base.WriteError(new TaskException(Strings.ErrorParentNotFound(dataObject2.Name, parent.ToString())), (ErrorCategory)1003, null);
					}
					else
					{
						TDataObject dataObject3 = this.DataObject;
						base.WriteError(new TaskException(Strings.ErrorParentNotFoundOnDomainController(dataObject3.Name, base.DomainController, parent.ToString(), parent.DomainId.ToString())), (ErrorCategory)1003, null);
					}
				}
				ExchangeObjectVersion exchangeObjectVersion = (ExchangeObjectVersion)adrawEntry[ADObjectSchema.ExchangeVersion];
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)adrawEntry[ADObjectSchema.ObjectClass];
				TDataObject dataObject4 = this.DataObject;
				if (dataObject4.ExchangeVersion.IsOlderThan(exchangeObjectVersion) && !multiValuedProperty.Contains(Organization.MostDerivedClass) && this.EnforceExchangeObjectVersion)
				{
					TDataObject dataObject5 = this.DataObject;
					string name = dataObject5.Name;
					TDataObject dataObject6 = this.DataObject;
					base.WriteError(new TaskException(Strings.ErrorParentHasNewerVersion(name, dataObject6.ExchangeVersion.ToString(), exchangeObjectVersion.ToString())), (ErrorCategory)1004, null);
				}
			}
			if (base.IsVerboseOn)
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetConfigurableObjectChangedProperties(this.DataObject));
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private static string clonableTypeName = typeof(TDataObject).FullName;
	}
}
