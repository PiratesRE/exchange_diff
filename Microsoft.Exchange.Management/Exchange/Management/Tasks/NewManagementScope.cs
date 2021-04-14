using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks.ManagementScopeExtensions;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "ManagementScope", SupportsShouldProcess = true, DefaultParameterSetName = "RecipientFilter")]
	public sealed class NewManagementScope : NewMultitenancySystemConfigurationObjectTask<ManagementScope>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewManagementScope(this.DataObject.ScopeRestrictionType.ToString(), this.DataObject.Filter, (this.DataObject.RecipientRoot == null) ? "<null>" : this.DataObject.RecipientRoot.ToString());
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RecipientFilter")]
		public string RecipientRestrictionFilter
		{
			get
			{
				return (string)base.Fields["RecipientRestrictionFilter"];
			}
			set
			{
				base.Fields["RecipientRestrictionFilter"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RecipientFilter")]
		public OrganizationalUnitIdParameter RecipientRoot
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["RecipientRoot"];
			}
			set
			{
				base.Fields["RecipientRoot"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ServerFilter")]
		public string ServerRestrictionFilter
		{
			get
			{
				return (string)base.Fields["ServerRestrictionFilter"];
			}
			set
			{
				base.Fields["ServerRestrictionFilter"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ServerList")]
		public ServerIdParameter[] ServerList
		{
			get
			{
				return (ServerIdParameter[])base.Fields["ServerList"];
			}
			set
			{
				base.Fields["ServerList"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DatabaseList")]
		public DatabaseIdParameter[] DatabaseList
		{
			get
			{
				return (DatabaseIdParameter[])base.Fields["DatabaseList"];
			}
			set
			{
				base.Fields["DatabaseList"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DatabaseFilter")]
		public string DatabaseRestrictionFilter
		{
			get
			{
				return (string)base.Fields["DatabaseRestrictionFilter"];
			}
			set
			{
				base.Fields["DatabaseRestrictionFilter"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "PartnerFilter")]
		public string PartnerDelegatedTenantRestrictionFilter
		{
			get
			{
				return (string)base.Fields["PartnerDelegatedTenantRestrictionFilter"];
			}
			set
			{
				base.Fields["PartnerDelegatedTenantRestrictionFilter"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DatabaseFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "RecipientFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "ServerFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "ServerList")]
		[Parameter(Mandatory = false, ParameterSetName = "DatabaseList")]
		public SwitchParameter Exclusive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Exclusive"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Exclusive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ServerList")]
		[Parameter(Mandatory = false, ParameterSetName = "RecipientFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "ServerFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "DatabaseFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "DatabaseList")]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (ManagementScope)base.PrepareDataObject();
			if (this.ServerRestrictionFilter != null || this.ServerList != null)
			{
				this.DataObject.ScopeRestrictionType = ScopeRestrictionType.ServerScope;
				if (this.ServerList != null && this.ServerList.Length > 0)
				{
					this.DataObject.ServerFilter = this.BuildAndVerifyObjectListFilter<Server>(this.ServerList, "ServerList", new NewManagementScope.SingleParameterLocString(Strings.ServerNamesMustBeValid), new NewManagementScope.SingleParameterLocString(Strings.ServerNamesMustBeUnique), new NewManagementScope.SingleParameterLocString(Strings.ServerListMustBeValid));
				}
				else
				{
					this.ValidateAndSetFilterOnDataObject("ServerRestrictionFilter", this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			else if (this.PartnerDelegatedTenantRestrictionFilter != null)
			{
				this.DataObject.ScopeRestrictionType = ScopeRestrictionType.PartnerDelegatedTenantScope;
				this.ValidateAndSetFilterOnDataObject("PartnerDelegatedTenantRestrictionFilter", this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			else if (this.RecipientRestrictionFilter != null)
			{
				this.DataObject.ScopeRestrictionType = ScopeRestrictionType.RecipientScope;
				this.ValidateAndSetFilterOnDataObject("RecipientRestrictionFilter", this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			else if (this.DatabaseRestrictionFilter != null || this.DatabaseList != null)
			{
				this.DataObject.ScopeRestrictionType = ScopeRestrictionType.DatabaseScope;
				if (this.DatabaseList != null && this.DatabaseList.Length > 0)
				{
					this.DataObject.DatabaseFilter = this.BuildAndVerifyObjectListFilter<Database>(this.DatabaseList, "DatabaseList", new NewManagementScope.SingleParameterLocString(Strings.DatabaseNamesMustBeValid), new NewManagementScope.SingleParameterLocString(Strings.DatabaseNamesMustBeUnique), new NewManagementScope.SingleParameterLocString(Strings.DatabaseListMustBeValid));
				}
				else
				{
					this.ValidateAndSetFilterOnDataObject("DatabaseRestrictionFilter", this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				this.DataObject.SetExchangeVersion(ManagementScopeSchema.ExchangeManagementScope2010_SPVersion);
			}
			if (this.Exclusive.ToBool())
			{
				this.DataObject.Exclusive = true;
			}
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			this.DataObject.RecipientRoot = null;
			if (this.RecipientRoot != null)
			{
				this.DataObject.RecipientRoot = this.GetADObjectIdFromOrganizationalUnitIdParameter(configurationSession, this.RecipientRoot);
			}
			if (!base.HasErrors)
			{
				ADObjectId rootContainerId = ManagementScopeIdParameter.GetRootContainerId(configurationSession);
				this.DataObject.SetId(rootContainerId.GetChildId(base.Name));
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		private ADObjectId GetADObjectIdFromOrganizationalUnitIdParameter(IConfigurationSession configurationSession, OrganizationalUnitIdParameter root)
		{
			bool useConfigNC = configurationSession.UseConfigNC;
			bool useGlobalCatalog = configurationSession.UseGlobalCatalog;
			ADObjectId id;
			try
			{
				configurationSession.UseConfigNC = false;
				configurationSession.UseGlobalCatalog = true;
				IConfigurable configurable = (ADConfigurationObject)base.GetDataObject<ExchangeOrganizationalUnit>(root, configurationSession, base.OrganizationId.OrganizationalUnit, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(root.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(root.ToString())));
				id = ((ADObject)configurable).Id;
			}
			finally
			{
				configurationSession.UseConfigNC = useConfigNC;
				configurationSession.UseGlobalCatalog = useGlobalCatalog;
			}
			return id;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if ((this.ServerList != null || this.ServerRestrictionFilter != null || this.PartnerDelegatedTenantRestrictionFilter != null || this.DatabaseList != null || this.DatabaseRestrictionFilter != null) && base.CurrentOrganizationId != OrganizationId.ForestWideOrgId && base.CurrentOrganizationId.ConfigurationUnit != base.RootOrgContainerId)
			{
				base.WriteError(new ArgumentException(Strings.ServerDatabaseAndPartnerScopesAreOnlyAllowedInTopOrg(base.CurrentOrganizationId.ToString())), ErrorCategory.InvalidArgument, null);
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			ManagementScope[] array = configurationSession.FindSimilarManagementScope(this.DataObject);
			if (array.Length > 0)
			{
				base.WriteError(new ArgumentException(Strings.SimilarScopeFound(array[0].Name)), ErrorCategory.InvalidArgument, null);
			}
			if (ScopeRestrictionType.DatabaseScope == this.DataObject.ScopeRestrictionType)
			{
				this.WriteWarning(Strings.WarningDatabaseScopeCreationApplicableOnlyInSP);
			}
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			if (this.Exclusive && !this.Force && !base.ShouldContinue(Strings.ConfirmCreatingExclusiveScope(this.DataObject.Identity.ToString())))
			{
				TaskLogger.LogExit();
				return;
			}
			if (!this.Force && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private string BuildAndVerifyObjectListFilter<TConfigObject>(ADIdParameter[] adParameterList, string parameterName, NewManagementScope.SingleParameterLocString misspelledObjectNamesString, NewManagementScope.SingleParameterLocString duplicateObjectNamesString, NewManagementScope.SingleParameterLocString parsingExceptionString) where TConfigObject : ADLegacyVersionableObject, new()
		{
			string[] array = new string[adParameterList.Length];
			for (int i = 0; i < adParameterList.Length; i++)
			{
				array[i] = adParameterList[i].ToString();
			}
			Result<TConfigObject>[] array2 = base.GlobalConfigSession.ReadMultipleLegacyObjects<TConfigObject>(array);
			ComparisonFilter[] array3 = new ComparisonFilter[array2.Length];
			StringBuilder stringBuilder = null;
			StringBuilder stringBuilder2 = null;
			List<TConfigObject> list = new List<TConfigObject>();
			for (int j = 0; j < array2.Length; j++)
			{
				TConfigObject data = array2[j].Data;
				ProviderError error = array2[j].Error;
				if (error == ProviderError.NotFound)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append("\r\n");
					stringBuilder.Append(array[j].ToString());
				}
				else if (error == null && data != null)
				{
					if (list.Contains(data))
					{
						if (stringBuilder2 == null)
						{
							stringBuilder2 = new StringBuilder();
						}
						stringBuilder2.Append("\r\n");
						stringBuilder2.Append(data.Name);
					}
					else
					{
						array3[j] = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, data.DistinguishedName);
						list.Add(data);
					}
				}
			}
			if (stringBuilder != null)
			{
				base.WriteError(new ArgumentException(misspelledObjectNamesString(stringBuilder.ToString()), parameterName), ErrorCategory.InvalidArgument, null);
			}
			if (stringBuilder2 != null)
			{
				base.WriteError(new ArgumentException(duplicateObjectNamesString(stringBuilder2.ToString()), parameterName), ErrorCategory.InvalidArgument, null);
			}
			string result = string.Empty;
			QueryFilter queryFilter;
			if (array3.Length == 1)
			{
				queryFilter = array3[0];
			}
			else
			{
				queryFilter = new OrFilter(array3);
			}
			try
			{
				result = queryFilter.GenerateInfixString(FilterLanguage.Monad);
			}
			catch (ParsingException ex)
			{
				base.WriteError(new ArgumentException(parsingExceptionString(ex.Message), parameterName, ex), ErrorCategory.InvalidArgument, null);
			}
			return result;
		}

		private delegate LocalizedString SingleParameterLocString(string message);
	}
}
