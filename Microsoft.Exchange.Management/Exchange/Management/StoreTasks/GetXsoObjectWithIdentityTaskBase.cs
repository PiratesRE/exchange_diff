using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	public abstract class GetXsoObjectWithIdentityTaskBase<TDataObject, TDirectoryObject> : GetRecipientObjectTask<MailboxIdParameter, TDirectoryObject> where TDataObject : IConfigurable, new() where TDirectoryObject : ADRecipient, new()
	{
		public GetXsoObjectWithIdentityTaskBase()
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

		protected virtual bool ShouldProcessArchive
		{
			get
			{
				return false;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			if (dataObject is ADUser || dataObject is ADSystemMailbox)
			{
				base.WriteResult(dataObject);
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			ExchangePrincipal exchangePrincipal = this.GetExchangePrincipal(dataObject);
			if (exchangePrincipal != null)
			{
				using (XsoMailboxDataProviderBase xsoMailboxDataProviderBase = (XsoMailboxDataProviderBase)this.CreateXsoMailboxDataProvider(exchangePrincipal, (base.ExchangeRunspaceConfig == null) ? null : base.ExchangeRunspaceConfig.SecurityAccessToken))
				{
					return xsoMailboxDataProviderBase.Read<TDataObject>(dataObject.Identity);
				}
			}
			return null;
		}

		private ExchangePrincipal GetExchangePrincipal(IConfigurable dataObject)
		{
			ADUser aduser = dataObject as ADUser;
			if (aduser != null)
			{
				if (!this.ShouldProcessArchive)
				{
					if (aduser.RecipientType == RecipientType.MailUser)
					{
						base.WriteError(new InvalidOperationException(Strings.RecipientTypeNotValid(this.Identity.ToString())), ErrorCategory.InvalidOperation, null);
					}
					return XsoStoreDataProviderBase.GetExchangePrincipalWithAdSessionSettingsForOrg(base.SessionSettings.CurrentOrganizationId, aduser);
				}
				if (aduser.ArchiveState != ArchiveState.None)
				{
					return ExchangePrincipal.FromMailboxGuid(base.SessionSettings, aduser.ArchiveGuid, RemotingOptions.AllowCrossSite | RemotingOptions.AllowCrossPremise, null);
				}
				base.WriteError(new InvalidOperationException(Strings.VerboseArchiveNotExistInStore(aduser.DisplayName)), ErrorCategory.InvalidOperation, null);
			}
			ADSystemMailbox adsystemMailbox = dataObject as ADSystemMailbox;
			if (adsystemMailbox != null)
			{
				return ExchangePrincipal.FromADSystemMailbox(base.SessionSettings, adsystemMailbox, ((ITopologyConfigurationSession)this.ConfigurationSession).FindServerByLegacyDN(adsystemMailbox.ServerLegacyDN));
			}
			return null;
		}

		internal abstract IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken);

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}
	}
}
