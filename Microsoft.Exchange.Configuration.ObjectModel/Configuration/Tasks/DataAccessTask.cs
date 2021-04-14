using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class DataAccessTask<TDataObject> : Task where TDataObject : IConfigurable, new()
	{
		protected DataAccessTask()
		{
			this.optionalData = new OptionalIdentityData();
		}

		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		protected NetworkCredential NetCredential
		{
			get
			{
				return this.netCredential;
			}
			set
			{
				this.netCredential = value;
			}
		}

		protected ADObjectId CurrentOrgContainerId
		{
			get
			{
				if (base.CurrentOrganizationId != null && base.CurrentOrganizationId.ConfigurationUnit != null)
				{
					return base.CurrentOrganizationId.ConfigurationUnit;
				}
				return this.RootOrgContainerId;
			}
		}

		internal ADObjectId RootOrgContainerId
		{
			get
			{
				if (this.rootOrgId == null)
				{
					this.rootOrgId = ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, string.IsNullOrEmpty(this.DomainController) ? null : this.NetCredential);
				}
				return this.rootOrgId;
			}
		}

		internal ADSessionSettings OrgWideSessionSettings
		{
			get
			{
				if (this.orgWideSessionSettings == null || !this.orgWideSessionSettings.CurrentOrganizationId.Equals(base.CurrentOrganizationId))
				{
					ADSessionSettings adsessionSettings = this.orgWideSessionSettings;
					this.orgWideSessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, false);
					if (adsessionSettings != null)
					{
						this.orgWideSessionSettings.IsSharedConfigChecked = adsessionSettings.IsSharedConfigChecked;
						this.orgWideSessionSettings.IsRedirectedToSharedConfig = adsessionSettings.IsRedirectedToSharedConfig;
					}
				}
				return this.orgWideSessionSettings;
			}
		}

		internal virtual IConfigurationSession ConfigurationSession
		{
			get
			{
				if (this.configurationSession == null || this.ShouldChangeScope(this.configurationSession))
				{
					IConfigurationSession oldSession = this.configurationSession;
					this.configurationSession = this.CreateConfigurationSession(this.OrgWideSessionSettings);
					ADSession.CopySettableSessionPropertiesAndSettings(oldSession, this.configurationSession);
				}
				return this.configurationSession;
			}
		}

		internal IRecipientSession TenantGlobalCatalogSession
		{
			get
			{
				if (this.tenantGlobalCatalogSession == null || this.ShouldChangeScope(this.tenantGlobalCatalogSession))
				{
					IRecipientSession oldSession = this.tenantGlobalCatalogSession;
					this.tenantGlobalCatalogSession = this.CreateTenantGlobalCatalogSession(this.OrgWideSessionSettings);
					ADSession.CopySettableSessionPropertiesAndSettings(oldSession, this.tenantGlobalCatalogSession);
				}
				return this.tenantGlobalCatalogSession;
			}
		}

		internal ITopologyConfigurationSession GlobalConfigSession
		{
			get
			{
				if (this.globalConfigSession == null)
				{
					this.CreateGlobalConfigSession();
				}
				return this.globalConfigSession;
			}
		}

		internal ITopologyConfigurationSession PartitionConfigSession
		{
			get
			{
				if (this.partitionOrRootOrgGlobalConfigSession == null || (base.CurrentOrganizationId.PartitionId != null && this.partitionOrRootOrgGlobalConfigSession.SessionSettings.PartitionId != null && !base.CurrentOrganizationId.PartitionId.Equals(this.partitionOrRootOrgGlobalConfigSession.SessionSettings.PartitionId)))
				{
					ITopologyConfigurationSession oldSession = this.partitionOrRootOrgGlobalConfigSession;
					this.CreateCurrentPartitionTopologyConfigSession();
					ADSession.CopySettableSessionPropertiesAndSettings(oldSession, this.partitionOrRootOrgGlobalConfigSession);
				}
				return this.partitionOrRootOrgGlobalConfigSession;
			}
		}

		internal ITopologyConfigurationSession RootOrgGlobalConfigSession
		{
			get
			{
				if (this.rootOrgGlobalConfigSession == null)
				{
					this.CreateRootOrgConfigSession();
				}
				return this.rootOrgGlobalConfigSession;
			}
		}

		internal ITopologyConfigurationSession ReadWriteRootOrgGlobalConfigSession
		{
			get
			{
				if (this.readWriteRootOrgGlobalConfigSession == null)
				{
					this.CreateReadWriteRootOrgConfigSession();
				}
				return this.readWriteRootOrgGlobalConfigSession;
			}
		}

		internal IRecipientSession PartitionOrRootOrgGlobalCatalogSession
		{
			get
			{
				if (this.partitionOrRootOrgGlobalCatalogSession == null || (base.CurrentOrganizationId.PartitionId != null && this.partitionOrRootOrgGlobalCatalogSession.SessionSettings.PartitionId != null && !base.CurrentOrganizationId.PartitionId.Equals(this.partitionOrRootOrgGlobalCatalogSession.SessionSettings.PartitionId)))
				{
					IRecipientSession oldSession = this.partitionOrRootOrgGlobalCatalogSession;
					this.CreatePartitionAllTenantsOrRootOrgGlobalCatalogSession();
					ADSession.CopySettableSessionPropertiesAndSettings(oldSession, this.partitionOrRootOrgGlobalCatalogSession);
				}
				return this.partitionOrRootOrgGlobalCatalogSession;
			}
		}

		internal IRootOrganizationRecipientSession RootOrgGlobalCatalogSession
		{
			get
			{
				if (this.rootOrgGlobalCatalogSession == null)
				{
					this.CreateRootOrgGlobalCatalogSession();
				}
				return this.rootOrgGlobalCatalogSession;
			}
		}

		internal virtual IRecipientSession CreateTenantGlobalCatalogSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings == null)
			{
				throw new ArgumentNullException("sessionSettings");
			}
			bool flag = TaskHelper.ShouldPassDomainControllerToSession(this.DomainController, sessionSettings);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(flag ? this.DomainController : null, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(this.DomainController) ? null : (flag ? this.NetCredential : null), sessionSettings, 375, "CreateTenantGlobalCatalogSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			if (!tenantOrRootOrgRecipientSession.IsReadConnectionAvailable())
			{
				tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 384, "CreateTenantGlobalCatalogSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			}
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			return tenantOrRootOrgRecipientSession;
		}

		internal virtual IConfigurationSession CreateConfigurationSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings == null)
			{
				throw new ArgumentNullException("sessionSettings");
			}
			bool flag = TaskHelper.ShouldPassDomainControllerToSession(this.DomainController, sessionSettings);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(flag ? this.DomainController : null, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(this.DomainController) ? null : (flag ? this.NetCredential : null), sessionSettings, 419, "CreateConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
		}

		private void CreateGlobalConfigSession()
		{
			bool flag = TaskHelper.ShouldPassDomainControllerToSession(this.DomainController, ADSessionSettings.FromRootOrgScopeSet());
			this.globalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(flag ? this.DomainController : null, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(this.DomainController) ? null : (flag ? this.NetCredential : null), ADSessionSettings.FromRootOrgScopeSet(), 433, "CreateGlobalConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
		}

		private void CreatePartitionAllTenantsOrRootOrgGlobalCatalogSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(base.CurrentOrganizationId);
			bool flag = TaskHelper.ShouldPassDomainControllerToSession(this.DomainController, sessionSettings);
			this.partitionOrRootOrgGlobalCatalogSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(flag ? this.DomainController : null, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(this.DomainController) ? null : (flag ? this.NetCredential : null), sessionSettings, 450, "CreatePartitionAllTenantsOrRootOrgGlobalCatalogSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			if (!this.partitionOrRootOrgGlobalCatalogSession.IsReadConnectionAvailable())
			{
				this.partitionOrRootOrgGlobalCatalogSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 461, "CreatePartitionAllTenantsOrRootOrgGlobalCatalogSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			}
			this.partitionOrRootOrgGlobalCatalogSession.UseGlobalCatalog = true;
		}

		private void CreateCurrentPartitionTopologyConfigSession()
		{
			ADSessionSettings sessionSettings = OrganizationId.ForestWideOrgId.Equals(base.CurrentOrganizationId) ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromAccountPartitionRootOrgScopeSet(base.CurrentOrganizationId.PartitionId);
			bool flag = TaskHelper.ShouldPassDomainControllerToSession(this.DomainController, sessionSettings);
			this.partitionOrRootOrgGlobalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(flag ? this.DomainController : null, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(this.DomainController) ? null : (flag ? this.NetCredential : null), sessionSettings, 479, "CreateCurrentPartitionTopologyConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			if (!this.partitionOrRootOrgGlobalConfigSession.IsReadConnectionAvailable())
			{
				this.partitionOrRootOrgGlobalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 488, "CreateCurrentPartitionTopologyConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			}
			this.partitionOrRootOrgGlobalConfigSession.UseGlobalCatalog = true;
		}

		private void CreateRootOrgGlobalCatalogSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			bool flag = TaskHelper.ShouldPassDomainControllerToSession(this.DomainController, sessionSettings);
			this.rootOrgGlobalCatalogSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(flag ? this.DomainController : null, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(this.DomainController) ? null : (flag ? this.NetCredential : null), sessionSettings, 504, "CreateRootOrgGlobalCatalogSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			if (!this.rootOrgGlobalCatalogSession.IsReadConnectionAvailable())
			{
				this.rootOrgGlobalCatalogSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 515, "CreateRootOrgGlobalCatalogSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			}
			this.rootOrgGlobalCatalogSession.UseGlobalCatalog = true;
		}

		private void CreateReadWriteRootOrgConfigSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			bool flag = TaskHelper.ShouldPassDomainControllerToSession(this.DomainController, sessionSettings);
			this.readWriteRootOrgGlobalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(flag ? this.DomainController : null, false, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(this.DomainController) ? null : (flag ? this.NetCredential : null), sessionSettings, 532, "CreateReadWriteRootOrgConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			if (!this.readWriteRootOrgGlobalConfigSession.IsReadConnectionAvailable())
			{
				this.readWriteRootOrgGlobalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 541, "CreateReadWriteRootOrgConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			}
			this.readWriteRootOrgGlobalConfigSession.UseGlobalCatalog = true;
		}

		private void CreateRootOrgConfigSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			bool flag = TaskHelper.ShouldPassDomainControllerToSession(this.DomainController, sessionSettings);
			this.rootOrgGlobalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(flag ? this.DomainController : null, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(this.DomainController) ? null : (flag ? this.NetCredential : null), sessionSettings, 558, "CreateRootOrgConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			if (!this.rootOrgGlobalConfigSession.IsReadConnectionAvailable())
			{
				this.rootOrgGlobalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 567, "CreateRootOrgConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
			}
			this.rootOrgGlobalConfigSession.UseGlobalCatalog = true;
		}

		protected virtual bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return false;
		}

		protected virtual void ResolveCurrentOrgIdBasedOnIdentity(IIdentityParameter identity)
		{
			if (this.ShouldSupportPreResolveOrgIdBasedOnIdentity() && base.CurrentOrganizationId != null && base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				ADIdParameter adidParameter = identity as ADIdParameter;
				if (adidParameter != null)
				{
					OrganizationId organizationId = adidParameter.ResolveOrganizationIdBasedOnIdentity(base.ExecutingUserOrganizationId);
					if (organizationId != null && !organizationId.Equals(base.CurrentOrganizationId))
					{
						this.SetCurrentOrganizationWithScopeSet(organizationId);
					}
				}
			}
		}

		protected void SetCurrentOrganizationWithScopeSet(OrganizationId orgId)
		{
			if (orgId != null && !orgId.Equals(OrganizationId.ForestWideOrgId))
			{
				base.CurrentTaskContext.ScopeSet = ScopeSet.ResolveUnderScope(orgId, base.CurrentTaskContext.ScopeSet);
			}
			base.CurrentOrganizationId = orgId;
		}

		private bool ShouldChangeScope(IDirectorySession session)
		{
			return base.CurrentOrganizationId != null && session.SessionSettings.CurrentOrganizationId != null && !base.CurrentOrganizationId.Equals(session.SessionSettings.CurrentOrganizationId);
		}

		protected IConfigDataProvider DataSession
		{
			get
			{
				return this.dataSession;
			}
		}

		protected virtual ObjectId RootId
		{
			get
			{
				return null;
			}
		}

		protected OptionalIdentityData OptionalIdentityData
		{
			get
			{
				return this.optionalData;
			}
		}

		protected abstract IConfigDataProvider CreateSession();

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.dataSession = this.CreateSession();
			if (this.dataSession != null)
			{
				IDirectorySession directorySession = this.dataSession as IDirectorySession;
				if (directorySession != null && directorySession.SessionSettings != null)
				{
					directorySession.SessionSettings.ExecutingUserIdentityName = base.ExecutingUserIdentityName;
				}
			}
		}

		protected void Validate(TDataObject dataObject)
		{
			ValidationError[] array = dataObject.Validate();
			if (array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					this.WriteError(new DataValidationException(array[i]), (ErrorCategory)1003, dataObject.Identity, array.Length - 1 == i);
				}
			}
		}

		protected IConfigurable GetDataObject(IIdentityParameter id)
		{
			return this.GetDataObject<TDataObject>(id, this.DataSession, this.RootId, null, null, null);
		}

		protected IEnumerable<TDataObject> GetDataObjects(IIdentityParameter id, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			return this.GetDataObjects<TDataObject>(id, this.DataSession, this.RootId, optionalData, out notFoundReason);
		}

		protected IEnumerable<TDataObject> GetDataObjects(IIdentityParameter id)
		{
			LocalizedString? localizedString;
			return this.GetDataObjects<TDataObject>(id, this.DataSession, this.RootId, null, out localizedString);
		}

		protected IConfigurable GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError) where TObject : IConfigurable, new()
		{
			return this.GetDataObject<TObject>(id, session, rootID, null, notFoundError, multipleFoundError, (ExchangeErrorCategory)0);
		}

		protected IConfigurable GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, OptionalIdentityData optionalData, LocalizedString? notFoundError, LocalizedString? multipleFoundError) where TObject : IConfigurable, new()
		{
			return this.GetDataObject<TObject>(id, session, rootID, optionalData, notFoundError, multipleFoundError, (ExchangeErrorCategory)0);
		}

		protected IEnumerable<TObject> GetDataObjects<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID) where TObject : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetDataObjects<TObject>(id, session, rootID, null, out localizedString);
		}

		protected IEnumerable<TObject> GetDataObjects<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where TObject : IConfigurable, new()
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			notFoundReason = null;
			base.WriteVerbose(TaskVerboseStringHelper.GetFindByIdParameterVerboseString(id, session ?? this.DataSession, typeof(TObject), rootID));
			IEnumerable<TObject> objects;
			try
			{
				objects = id.GetObjects<TObject>(rootID, session ?? this.DataSession, optionalData, out notFoundReason);
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(session ?? this.DataSession));
			}
			return objects;
		}

		internal void VerifyIsWithinScopes(IDirectorySession session, ADObject adObject, bool isModification, DataAccessTask<TDataObject>.ADObjectOutOfScopeString adObjectOutOfScopeString)
		{
			ADScopeException ex;
			if (!session.TryVerifyIsWithinScopes(adObject, isModification, out ex))
			{
				base.WriteError(new InvalidOperationException(adObjectOutOfScopeString(adObject.Identity.ToString(), (ex == null) ? string.Empty : ex.Message), ex), ErrorCategory.InvalidOperation, adObject.Identity);
			}
		}

		protected override void TranslateException(ref Exception e, out ErrorCategory category)
		{
			base.TranslateException(ref e, out category);
			ErrorCategory exceptionErrorCategory = (ErrorCategory)RecipientTaskHelper.GetExceptionErrorCategory(e);
			if (exceptionErrorCategory != ErrorCategory.NotSpecified)
			{
				category = exceptionErrorCategory;
				return;
			}
			category = (ErrorCategory)DataAccessHelper.ResolveExceptionErrorCategory(e);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception);
		}

		protected LocalizedString GetErrorMessageObjectNotFound(string id, string type, string source)
		{
			if (source == null)
			{
				if (id != null)
				{
					return Strings.ErrorManagementObjectNotFound(id);
				}
				return Strings.ErrorManagementObjectNotFoundByType(type);
			}
			else
			{
				if (id != null)
				{
					return Strings.ErrorManagementObjectNotFoundWithSource(id, source);
				}
				return Strings.ErrorManagementObjectNotFoundWithSourceByType(type, source);
			}
		}

		protected virtual IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return dataObject;
		}

		protected void VerifyValues<TType>(TType[] allowedTypes, TType value)
		{
			this.VerifyValues<TType>(allowedTypes, new TType[]
			{
				value
			});
		}

		protected void VerifyValues<TType>(TType[] allowedTypes, TType[] values)
		{
			if (allowedTypes == null)
			{
				throw new ArgumentNullException("allowedTypes");
			}
			if (values == null)
			{
				return;
			}
			List<TType> list = new List<TType>();
			foreach (TType ttype in values)
			{
				if (-1 == Array.IndexOf<TType>(allowedTypes, ttype))
				{
					list.Add(ttype);
				}
			}
			if (list.Count > 0)
			{
				throw new TaskException(Strings.ErrorUnsupportedValues(this.FormatMultiValuedProperty(list), this.FormatMultiValuedProperty(allowedTypes)));
			}
		}

		protected void UnderscopeDataSession(OrganizationId orgId)
		{
			IDirectorySession session = (IDirectorySession)this.dataSession;
			this.dataSession = (IConfigDataProvider)TaskHelper.UnderscopeSessionToOrganization(session, orgId, this.RehomeDataSession);
		}

		internal void RebindDataSessionToDataObjectPartitionRidMasterIncludeRetiredTenants(PartitionId partitionId)
		{
			string ridMasterName = ForestTenantRelocationsCache.GetRidMasterName(partitionId);
			if (this.DomainController != null)
			{
				string value = this.DomainController.ToString().Split(new char[]
				{
					'.'
				})[0] + ".";
				if (!ridMasterName.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					ForestTenantRelocationsCache.Reset();
					base.WriteError(new InvalidOperationException(Strings.ErrorMustWriteToRidMaster(ridMasterName)), ErrorCategory.InvalidOperation, ridMasterName);
				}
			}
			ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			adsessionSettings.RetiredTenantModificationAllowed = true;
			this.dataSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ridMasterName, false, ConsistencyMode.PartiallyConsistent, adsessionSettings, 1125, "RebindDataSessionToDataObjectPartitionRidMasterIncludeRetiredTenants", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\DataAccessTask.cs");
		}

		protected virtual bool RehomeDataSession
		{
			get
			{
				return true;
			}
		}

		protected string FormatMultiValuedProperty(IList mvp)
		{
			return MultiValuedPropertyBase.FormatMultiValuedProperty(mvp);
		}

		protected IConfigurable GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory errorCategory) where TObject : IConfigurable, new()
		{
			return this.GetDataObject<TObject>(id, session, rootID, null, notFoundError, multipleFoundError, errorCategory);
		}

		protected MultiValuedProperty<TResult> ResolveIdParameterCollection<TIdParameter, TObject, TResult>(IEnumerable<TIdParameter> idParameters, IConfigDataProvider session, ObjectId rootId, OptionalIdentityData optionalData) where TIdParameter : IIdentityParameter where TObject : IConfigurable, new()
		{
			return this.ResolveIdParameterCollection<TIdParameter, TObject, TResult>(idParameters, session, rootId, optionalData, (ExchangeErrorCategory)0, null, null, null, null);
		}

		protected MultiValuedProperty<TResult> ResolveIdParameterCollection<TIdParameter, TObject, TResult>(IEnumerable<TIdParameter> idParameters, IConfigDataProvider session, ObjectId rootId, OptionalIdentityData optionalData, ExchangeErrorCategory errorCategory, Func<IIdentityParameter, LocalizedString> parameterToNotFoundError, Func<IIdentityParameter, LocalizedString> parameterToMultipleFoundError, Func<IConfigurable, TResult> convertToResult, Func<IConfigurable, IConfigurable> validateObject) where TIdParameter : IIdentityParameter where TObject : IConfigurable, new()
		{
			MultiValuedProperty<TIdParameter> multiValuedProperty = idParameters as MultiValuedProperty<TIdParameter>;
			MultiValuedProperty<TResult> result;
			if (multiValuedProperty != null && multiValuedProperty.IsChangesOnlyCopy)
			{
				Hashtable hashtable = new Hashtable();
				if (multiValuedProperty.Added.Length > 0)
				{
					IEnumerable<TResult> value = this.ResolveIdList<TObject, TResult>(multiValuedProperty.Added, session, rootId, optionalData, errorCategory, parameterToNotFoundError, parameterToMultipleFoundError, convertToResult, validateObject);
					hashtable.Add("Add", value);
				}
				if (multiValuedProperty.Removed.Length > 0)
				{
					IEnumerable<TResult> value2 = this.ResolveIdList<TObject, TResult>(multiValuedProperty.Removed, session, rootId, optionalData, errorCategory, parameterToNotFoundError, parameterToMultipleFoundError, convertToResult, null);
					hashtable.Add("Remove", value2);
				}
				result = new MultiValuedProperty<TResult>(hashtable);
			}
			else
			{
				IEnumerable<TResult> value3 = this.ResolveIdList<TObject, TResult>(idParameters, session, rootId, optionalData, errorCategory, parameterToNotFoundError, parameterToMultipleFoundError, convertToResult, validateObject);
				result = new MultiValuedProperty<TResult>(value3);
			}
			return result;
		}

		private IEnumerable<TResult> ResolveIdList<TObject, TResult>(IEnumerable idParameters, IConfigDataProvider session, ObjectId rootId, OptionalIdentityData optionalData, ExchangeErrorCategory errorCategory, Func<IIdentityParameter, LocalizedString> parameterToNotFoundError, Func<IIdentityParameter, LocalizedString> parameterToMultipleFoundError, Func<IConfigurable, TResult> convertToResult, Func<IConfigurable, IConfigurable> validateObject) where TObject : IConfigurable, new()
		{
			Func<IConfigurable, TResult> func = null;
			Func<IConfigurable, IConfigurable> func2 = null;
			Dictionary<TResult, IIdentityParameter> dictionary = new Dictionary<TResult, IIdentityParameter>();
			if (idParameters != null)
			{
				if (convertToResult == null)
				{
					if (func == null)
					{
						func = ((IConfigurable obj) => (TResult)((object)obj.Identity));
					}
					convertToResult = func;
				}
				if (validateObject == null)
				{
					if (func2 == null)
					{
						func2 = ((IConfigurable obj) => obj);
					}
					validateObject = func2;
				}
				foreach (object obj2 in idParameters)
				{
					IIdentityParameter identityParameter = (IIdentityParameter)obj2;
					LocalizedString? notFoundError = (parameterToNotFoundError == null) ? null : new LocalizedString?(parameterToNotFoundError(identityParameter));
					LocalizedString? multipleFoundError = (parameterToMultipleFoundError == null) ? null : new LocalizedString?(parameterToMultipleFoundError(identityParameter));
					IConfigurable arg = this.GetDataObject<TObject>(identityParameter, session, rootId, notFoundError, multipleFoundError);
					arg = validateObject(arg);
					TResult tresult = convertToResult(arg);
					if (dictionary.ContainsKey(tresult))
					{
						throw new ManagementObjectDuplicateException(Strings.ErrorDuplicateManagementObjectFound(dictionary[tresult], identityParameter, tresult));
					}
					dictionary.Add(tresult, identityParameter);
				}
			}
			return dictionary.Keys;
		}

		protected IConfigurable GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, OptionalIdentityData optionalData, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory errorCategory) where TObject : IConfigurable, new()
		{
			IConfigurable result = null;
			LocalizedString? localizedString;
			IEnumerable<TObject> dataObjects = this.GetDataObjects<TObject>(id, session, rootID, optionalData, out localizedString);
			Exception ex = null;
			using (IEnumerator<TObject> enumerator = dataObjects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (enumerator.MoveNext())
					{
						ex = new ManagementObjectAmbiguousException(multipleFoundError ?? Strings.ErrorManagementObjectAmbiguous(id.ToString()));
					}
				}
				else if (notFoundError != null)
				{
					LocalizedString message;
					if (localizedString != null)
					{
						LocalizedString? localizedString2 = notFoundError;
						string notFound = (localizedString2 != null) ? localizedString2.GetValueOrDefault() : null;
						LocalizedString? localizedString3 = localizedString;
						message = Strings.ErrorNotFoundWithReason(notFound, (localizedString3 != null) ? localizedString3.GetValueOrDefault() : null);
					}
					else
					{
						message = notFoundError.Value;
					}
					ex = new ManagementObjectNotFoundException(message);
				}
				else
				{
					ex = new ManagementObjectNotFoundException(localizedString ?? this.GetErrorMessageObjectNotFound(id.ToString(), typeof(TObject).ToString(), (this.DataSession != null) ? this.DataSession.Source : null));
				}
			}
			if (ex != null)
			{
				if (errorCategory != (ExchangeErrorCategory)0)
				{
					RecipientTaskHelper.SetExceptionErrorCategory(ex, errorCategory);
				}
				throw ex;
			}
			return result;
		}

		private IConfigDataProvider dataSession;

		private OptionalIdentityData optionalData;

		private NetworkCredential netCredential;

		private ADSessionSettings orgWideSessionSettings;

		private ADObjectId rootOrgId;

		private IConfigurationSession configurationSession;

		private IRecipientSession tenantGlobalCatalogSession;

		private ITopologyConfigurationSession globalConfigSession;

		private ITopologyConfigurationSession partitionOrRootOrgGlobalConfigSession;

		private ITopologyConfigurationSession rootOrgGlobalConfigSession;

		private ITopologyConfigurationSession readWriteRootOrgGlobalConfigSession;

		private IRecipientSession partitionOrRootOrgGlobalCatalogSession;

		private IRootOrganizationRecipientSession rootOrgGlobalCatalogSession;

		internal delegate LocalizedString ADObjectOutOfScopeString(string objectName, string reason);
	}
}
