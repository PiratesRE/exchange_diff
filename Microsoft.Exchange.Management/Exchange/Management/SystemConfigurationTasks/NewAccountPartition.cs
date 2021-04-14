using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "AccountPartition", SupportsShouldProcess = true, DefaultParameterSetName = "Trust")]
	public sealed class NewAccountPartition : NewSystemConfigurationObjectTask<AccountPartition>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Trust")]
		[Parameter(Mandatory = true, ParameterSetName = "Secondary")]
		public Fqdn Trust
		{
			get
			{
				return (Fqdn)base.Fields["Trust"];
			}
			set
			{
				base.Fields["Trust"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "LocalForest")]
		public SwitchParameter LocalForest
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

		[Parameter(Mandatory = false, ParameterSetName = "Trust")]
		[Parameter(Mandatory = false, ParameterSetName = "LocalForest")]
		public SwitchParameter EnabledForProvisioning
		{
			get
			{
				return (SwitchParameter)(base.Fields["EnabledForProvisioning"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["EnabledForProvisioning"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Secondary")]
		public SwitchParameter Secondary
		{
			get
			{
				return (SwitchParameter)(base.Fields["Secondary"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Secondary"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAccountPartition(base.Name);
			}
		}

		protected override void InternalValidate()
		{
			this.adTrust = null;
			if (this.Trust != null)
			{
				this.adTrust = NewAccountPartition.ResolveAndValidateForestTrustForADDomain(this.Trust, new Task.ErrorLoggerDelegate(base.WriteError), (IConfigurationSession)base.DataSession);
			}
			if (this.LocalForest)
			{
				NewAccountPartition.VerifyNoOtherPartitionIsLocalForest(base.GlobalConfigSession, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			base.InternalValidate();
			bool hasErrors = base.HasErrors;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (AccountPartition)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			this.DataObject.SetId(configurationSession.GetOrgContainerId().GetChildId(AccountPartition.AccountForestContainerName).GetChildId(this.DataObject.Name));
			this.DataObject.TrustedDomain = ((this.Trust == null) ? null : this.adTrust.Id);
			this.DataObject.IsLocalForest = this.LocalForest;
			this.DataObject.EnabledForProvisioning = this.EnabledForProvisioning;
			this.DataObject.IsSecondary = this.Secondary;
			return this.DataObject;
		}

		internal static ADDomainTrustInfo ResolveAndValidateForestTrustForADDomain(Fqdn domainFqdn, Task.ErrorLoggerDelegate errorDelegate, IConfigurationSession configSession)
		{
			if (domainFqdn == null)
			{
				throw new ArgumentNullException("domainFqdn");
			}
			if (errorDelegate == null)
			{
				throw new ArgumentNullException("errorDelegate");
			}
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			ADForest localForest = ADForest.GetLocalForest();
			ADDomainTrustInfo[] array = localForest.FindTrustRelationshipsForDomain(domainFqdn);
			if (array == null || array.Length == 0)
			{
				errorDelegate(new ManagementObjectNotFoundException(Strings.ErrorManagementObjectNotFound(domainFqdn.ToString())), ExchangeErrorCategory.Client, null);
			}
			else if (array.Length != 1)
			{
				errorDelegate(new ManagementObjectAmbiguousException(Strings.ErrorManagementObjectAmbiguous(domainFqdn.ToString())), ExchangeErrorCategory.Client, null);
			}
			ADDomainTrustInfo addomainTrustInfo = array[0];
			AccountPartition[] array2 = configSession.Find<AccountPartition>(configSession.GetOrgContainerId().GetChildId(AccountPartition.AccountForestContainerName), QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.Equal, AccountPartitionSchema.TrustedDomainLink, addomainTrustInfo.DistinguishedName), null, 1);
			if (array2 != null && array2.Length != 0)
			{
				errorDelegate(new LocalizedException(Strings.ErrorTrustAlreadyInUse(addomainTrustInfo.Name, array2[0].Name)), ExchangeErrorCategory.Client, null);
			}
			return addomainTrustInfo;
		}

		internal static void VerifyNoOtherPartitionIsLocalForest(ITopologyConfigurationSession topologySession, Task.ErrorLoggerDelegate errorDelegate)
		{
			AccountPartition[] array = topologySession.FindAllAccountPartitions();
			AccountPartition accountPartition = null;
			foreach (AccountPartition accountPartition2 in array)
			{
				if (accountPartition2.IsLocalForest)
				{
					accountPartition = accountPartition2;
					break;
				}
			}
			if (accountPartition != null)
			{
				errorDelegate(new LocalizedException(Strings.ErrorOnlyOnePartitionCanBeLocalForest(accountPartition.Name)), ExchangeErrorCategory.Client, null);
			}
		}

		private ADDomainTrustInfo adTrust;
	}
}
