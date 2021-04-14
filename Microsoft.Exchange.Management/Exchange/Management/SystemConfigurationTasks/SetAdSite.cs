using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "AdSite", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetAdSite : SetTopologySystemConfigurationObjectTask<AdSiteIdParameter, ADSite>
	{
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<AdSiteIdParameter> ResponsibleForSites
		{
			get
			{
				return (MultiValuedProperty<AdSiteIdParameter>)base.Fields["ResponsibleForSites"];
			}
			set
			{
				base.Fields["ResponsibleForSites"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAdSite(this.Identity.ToString());
			}
		}

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return true;
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 83, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Transport\\SetAdSite.cs");
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADSite dataObject = this.DataObject;
			if (dataObject != null)
			{
				if (dataObject.MinorPartnerId != -1 && dataObject.IsModified(ADSiteSchema.MinorPartnerId))
				{
					ADSite[] array = this.ConfigurationSession.Find<ADSite>(null, QueryScope.SubTree, null, null, 0);
					if (array != null && array.Length > 0)
					{
						foreach (ADSite adsite in array)
						{
							if (adsite.MinorPartnerId == dataObject.MinorPartnerId && !adsite.Id.Equals(dataObject.Id))
							{
								base.WriteError(new TaskException(Strings.ErrorMinorPartnerIdIsNotUnique(dataObject.MinorPartnerId.ToString())), (ErrorCategory)1000, null);
							}
						}
					}
				}
				if (base.Fields.IsModified("ResponsibleForSites"))
				{
					dataObject.ResponsibleForSites = base.ResolveIdParameterCollection<AdSiteIdParameter, ADSite, ADObjectId>(this.ResponsibleForSites, base.DataSession, this.RootId, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorSiteNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorSiteNotUnique), null, null);
				}
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private const string responsibleForSitesPropertyName = "ResponsibleForSites";
	}
}
