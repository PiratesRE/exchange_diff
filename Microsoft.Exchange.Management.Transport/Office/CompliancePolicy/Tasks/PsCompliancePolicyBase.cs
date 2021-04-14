using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public class PsCompliancePolicyBase : ADPresentationObject
	{
		public PsCompliancePolicyBase()
		{
			this.InitializeBindings();
		}

		public PsCompliancePolicyBase(PolicyStorage policyStorage) : base(policyStorage)
		{
			this.InitializeBindings();
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return PsCompliancePolicyBase.schema;
			}
		}

		internal IList<BindingStorage> StorageBindings
		{
			get
			{
				return this.storageBindings;
			}
			set
			{
				this.storageBindings = value;
			}
		}

		public MultiValuedProperty<BindingMetadata> ExchangeBinding { get; set; }

		public MultiValuedProperty<BindingMetadata> SharePointBinding { get; set; }

		public MultiValuedProperty<BindingMetadata> OneDriveBinding { get; set; }

		public Workload Workload
		{
			get
			{
				return (Workload)this[PsCompliancePolicyBaseSchema.Workload];
			}
			set
			{
				this[PsCompliancePolicyBaseSchema.Workload] = value;
			}
		}

		public Guid ObjectVersion
		{
			get
			{
				return (Guid)this[PsCompliancePolicyBaseSchema.ObjectVersion];
			}
		}

		internal Guid MasterIdentity
		{
			get
			{
				return (Guid)this[PsComplianceRuleBaseSchema.MasterIdentity];
			}
			set
			{
				this[PsComplianceRuleBaseSchema.MasterIdentity] = value;
			}
		}

		public string CreatedBy { get; protected set; }

		public string LastModifiedBy { get; protected set; }

		public bool ReadOnly { get; internal set; }

		public string ExternalIdentity
		{
			get
			{
				if (this.MasterIdentity != Guid.Empty)
				{
					return this.MasterIdentity.ToString();
				}
				return string.Empty;
			}
		}

		public string Comment
		{
			get
			{
				return (string)this[PsCompliancePolicyBaseSchema.Comment];
			}
			set
			{
				this[PsCompliancePolicyBaseSchema.Comment] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[PsCompliancePolicyBaseSchema.Enabled];
			}
			set
			{
				this[PsCompliancePolicyBaseSchema.Enabled] = value;
			}
		}

		public Mode Mode
		{
			get
			{
				return (Mode)this[PsCompliancePolicyBaseSchema.Mode];
			}
			set
			{
				this[PsCompliancePolicyBaseSchema.Mode] = value;
			}
		}

		public PolicyApplyStatus DistributionStatus { get; set; }

		public MultiValuedProperty<PolicyDistributionErrorDetails> DistributionResults { get; set; }

		public DateTime? LastStatusUpdateTime { get; set; }

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal virtual void PopulateTaskProperties(Task task, IConfigurationSession configurationSession)
		{
			this.InitializeBindings();
			foreach (BindingStorage bindingStorage in this.StorageBindings)
			{
				switch (bindingStorage.Workload)
				{
				case Workload.Exchange:
					this.ExchangeBinding = Utils.GetScopesFromStorage(bindingStorage);
					break;
				case Workload.SharePoint:
				{
					MultiValuedProperty<BindingMetadata> scopesFromStorage = Utils.GetScopesFromStorage(bindingStorage);
					MultiValuedProperty<BindingMetadata> multiValuedProperty = new MultiValuedProperty<BindingMetadata>(PsCompliancePolicyBase.GetBindingsBySubWorkload(scopesFromStorage, Workload.SharePoint));
					multiValuedProperty.SetIsReadOnly(false, null);
					if (multiValuedProperty.Any<BindingMetadata>())
					{
						this.SharePointBinding = multiValuedProperty;
					}
					else
					{
						multiValuedProperty = new MultiValuedProperty<BindingMetadata>(PsCompliancePolicyBase.GetBindingsBySubWorkload(scopesFromStorage, Workload.OneDriveForBusiness));
						multiValuedProperty.SetIsReadOnly(false, null);
						if (multiValuedProperty.Any<BindingMetadata>())
						{
							this.OneDriveBinding = scopesFromStorage;
						}
					}
					break;
				}
				default:
					this.ReadOnly = true;
					this.ExchangeBinding.Clear();
					this.SharePointBinding.Clear();
					this.OneDriveBinding.Clear();
					break;
				}
			}
			PolicyStorage policyStorage = base.DataObject as PolicyStorage;
			ADUser userObjectByExternalDirectoryObjectId = Utils.GetUserObjectByExternalDirectoryObjectId(policyStorage.CreatedBy, configurationSession);
			ADUser userObjectByExternalDirectoryObjectId2 = Utils.GetUserObjectByExternalDirectoryObjectId(policyStorage.LastModifiedBy, configurationSession);
			this.CreatedBy = ((!Utils.ExecutingUserIsForestWideAdmin(task) && userObjectByExternalDirectoryObjectId != null) ? userObjectByExternalDirectoryObjectId.DisplayName : policyStorage.CreatedBy);
			this.LastModifiedBy = ((!Utils.ExecutingUserIsForestWideAdmin(task) && userObjectByExternalDirectoryObjectId2 != null) ? userObjectByExternalDirectoryObjectId2.DisplayName : policyStorage.LastModifiedBy);
		}

		internal static IList<BindingMetadata> GetBindingsBySubWorkload(MultiValuedProperty<BindingMetadata> bindings, Workload subWorkload)
		{
			return (from s in bindings
			where s.Workload == subWorkload
			select s).ToList<BindingMetadata>();
		}

		internal virtual void UpdateStorageProperties(Task task, IConfigurationSession configurationSession, bool isNewPolicy)
		{
			PolicyStorage policyStorage = base.DataObject as PolicyStorage;
			Guid universalIdentity = Utils.GetUniversalIdentity(policyStorage);
			if (!Utils.ExecutingUserIsForestWideAdmin(task))
			{
				ADObjectId objectId;
				task.TryGetExecutingUserId(out objectId);
				ADUser userObjectByObjectId = Utils.GetUserObjectByObjectId(objectId, configurationSession);
				if (userObjectByObjectId != null)
				{
					policyStorage.LastModifiedBy = userObjectByObjectId.ExternalDirectoryObjectId;
					if (isNewPolicy)
					{
						policyStorage.CreatedBy = userObjectByObjectId.ExternalDirectoryObjectId;
					}
				}
			}
			this.UpdateWorkloadStorageBinding(universalIdentity, Workload.Exchange, this.ExchangeBinding, new MulipleExBindingObjectDetectedException());
			this.UpdateSharepointStorageBinding(universalIdentity, Workload.SharePoint, this.SharePointBinding, new MulipleSpBindingObjectDetectedException());
			this.UpdateSharepointStorageBinding(universalIdentity, Workload.OneDriveForBusiness, this.OneDriveBinding, new MulipleSpBindingObjectDetectedException());
		}

		internal void SuppressPiiData(PiiMap piiMap)
		{
			base.Name = (SuppressingPiiProperty.TryRedact(ADObjectSchema.Name, base.Name, piiMap) as string);
			if (this.ExchangeBinding != null)
			{
				foreach (BindingMetadata binding in this.ExchangeBinding)
				{
					Utils.RedactBinding(binding, false);
				}
			}
			if (this.SharePointBinding != null)
			{
				foreach (BindingMetadata binding2 in this.SharePointBinding)
				{
					Utils.RedactBinding(binding2, true);
				}
			}
			if (this.OneDriveBinding != null)
			{
				foreach (BindingMetadata binding3 in this.OneDriveBinding)
				{
					Utils.RedactBinding(binding3, true);
				}
			}
			if (this.DistributionResults != null)
			{
				foreach (PolicyDistributionErrorDetails policyDistributionErrorDetails in this.DistributionResults)
				{
					policyDistributionErrorDetails.Redact();
				}
			}
		}

		private void UpdateWorkloadStorageBinding(Guid universalIdentity, Workload workload, MultiValuedProperty<BindingMetadata> scopes, Exception mulipleStorageObjectsException)
		{
			ExAssert.RetailAssert(workload != Workload.SharePoint, "UpdateWorkloadBinding called for Sharepoint workload.");
			if (this.StorageBindings.Count((BindingStorage x) => x.Workload == workload) > 1)
			{
				throw mulipleStorageObjectsException;
			}
			BindingStorage bindingStorage = this.StorageBindings.FirstOrDefault((BindingStorage x) => x.Workload == workload);
			if (bindingStorage == null && scopes.Any<BindingMetadata>())
			{
				bindingStorage = Utils.CreateNewBindingStorage(base.OrganizationalUnitRoot, workload, universalIdentity);
				this.StorageBindings.Add(bindingStorage);
			}
			if (bindingStorage != null)
			{
				Utils.PopulateScopeStorages(bindingStorage, scopes);
			}
		}

		private void UpdateSharepointStorageBinding(Guid universalIdentity, Workload subWorkload, MultiValuedProperty<BindingMetadata> scopes, Exception mulipleStorageObjectsException)
		{
			ExAssert.RetailAssert(subWorkload == Workload.SharePoint || subWorkload == Workload.OneDriveForBusiness, "UpdateSharepointStorageBinding called for non-Sharepoint workload.");
			if (this.StorageBindings.Count((BindingStorage x) => x.Workload == Workload.SharePoint) > 2)
			{
				throw mulipleStorageObjectsException;
			}
			BindingStorage bindingStorageForSubWorkload = this.GetBindingStorageForSubWorkload(this.StorageBindings, subWorkload, universalIdentity, scopes.Any<BindingMetadata>());
			if (bindingStorageForSubWorkload != null)
			{
				Utils.PopulateScopeStorages(bindingStorageForSubWorkload, scopes);
			}
		}

		private BindingStorage GetBindingStorageForSubWorkload(IEnumerable<BindingStorage> bindingContainers, Workload subWorkload, Guid universalIdentity, bool createNewIfNotFound)
		{
			IEnumerable<BindingStorage> enumerable = from s in bindingContainers
			where s.Workload == Workload.SharePoint
			select s;
			BindingStorage bindingStorage3 = (from bindingStorage in enumerable
			where bindingStorage.Scopes.Any<string>()
			select bindingStorage).FirstOrDefault((BindingStorage bindingStorage) => BindingMetadata.FromStorage(bindingStorage.Scopes.First<string>()).Workload == subWorkload);
			if (bindingStorage3 != null)
			{
				return bindingStorage3;
			}
			foreach (BindingStorage bindingStorage2 in enumerable)
			{
				if (!bindingStorage2.Scopes.Any<string>())
				{
					bindingStorage3 = bindingStorage2;
					return bindingStorage3;
				}
			}
			if (createNewIfNotFound)
			{
				bindingStorage3 = Utils.CreateNewBindingStorage(base.OrganizationalUnitRoot, Workload.SharePoint, universalIdentity);
				this.StorageBindings.Add(bindingStorage3);
			}
			return bindingStorage3;
		}

		private void InitializeBindings()
		{
			this.ExchangeBinding = new MultiValuedProperty<BindingMetadata>();
			this.SharePointBinding = new MultiValuedProperty<BindingMetadata>();
			this.OneDriveBinding = new MultiValuedProperty<BindingMetadata>();
		}

		private static PsCompliancePolicyBaseSchema schema = ObjectSchema.GetInstance<PsCompliancePolicyBaseSchema>();

		private IList<BindingStorage> storageBindings = new List<BindingStorage>();
	}
}
