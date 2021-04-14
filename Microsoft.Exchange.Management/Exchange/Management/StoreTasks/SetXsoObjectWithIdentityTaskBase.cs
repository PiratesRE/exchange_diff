using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	public abstract class SetXsoObjectWithIdentityTaskBase<TDataObject> : SetRecipientObjectTask<MailboxIdParameter, TDataObject, ADUser> where TDataObject : IConfigurable, new()
	{
		public SetXsoObjectWithIdentityTaskBase()
		{
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "Identity")]
		public override MailboxIdParameter Identity
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

		internal override IConfigurationSession CreateConfigurationSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings.ConfigScopes == ConfigScopes.TenantLocal)
			{
				return base.CreateConfigurationSession(ADSessionSettings.RescopeToSubtree(sessionSettings));
			}
			return base.CreateConfigurationSession(sessionSettings);
		}

		internal override IRecipientSession CreateTenantGlobalCatalogSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings.ConfigScopes == ConfigScopes.TenantLocal)
			{
				return base.CreateTenantGlobalCatalogSession(ADSessionSettings.RescopeToSubtree(sessionSettings));
			}
			return base.CreateTenantGlobalCatalogSession(sessionSettings);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			using (XsoMailboxDataProviderBase xsoMailboxDataProviderBase = (XsoMailboxDataProviderBase)this.CreateXsoMailboxDataProvider(XsoStoreDataProviderBase.GetExchangePrincipalWithAdSessionSettingsForOrg(base.SessionSettings.CurrentOrganizationId, this.DataObject), (base.ExchangeRunspaceConfig == null) ? null : base.ExchangeRunspaceConfig.SecurityAccessToken))
			{
				TDataObject tdataObject = (TDataObject)((object)xsoMailboxDataProviderBase.Read<TDataObject>(this.DataObject.Identity));
				if (tdataObject == null)
				{
					tdataObject = this.GetDefaultConfiguration();
				}
				this.StampChangesOnXsoObject(tdataObject);
				this.SaveXsoObject(xsoMailboxDataProviderBase, tdataObject);
			}
			TaskLogger.LogExit();
		}

		protected virtual void StampChangesOnXsoObject(IConfigurable dataObject)
		{
			if (this.Instance != null)
			{
				dataObject.CopyChangesFrom(this.Instance);
			}
		}

		protected virtual void SaveXsoObject(IConfigDataProvider provider, IConfigurable dataObject)
		{
			provider.Save(dataObject);
		}

		protected virtual TDataObject GetDefaultConfiguration()
		{
			throw new ManagementObjectNotFoundException(Strings.ErrorObjectNotFound(this.Identity.ToString()));
		}

		internal abstract IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken);

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}
	}
}
