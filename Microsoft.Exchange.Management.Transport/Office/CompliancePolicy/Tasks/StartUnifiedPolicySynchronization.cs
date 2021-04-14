using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Start", "UnifiedPolicySynchronization", DefaultParameterSetName = "SyncByType")]
	public sealed class StartUnifiedPolicySynchronization : GetMultitenancySystemConfigurationObjectTask<PolicyIdParameter, PolicyStorage>
	{
		[Parameter(Mandatory = true, ParameterSetName = "SyncDeleteObject")]
		[Parameter(Mandatory = false, ParameterSetName = "SyncByType")]
		[Parameter(Mandatory = true, ParameterSetName = "SyncUpdateObject")]
		public MultiValuedProperty<ConfigurationObjectType> ObjectType
		{
			get
			{
				if (base.Fields["ObjectType"] != null)
				{
					return (MultiValuedProperty<ConfigurationObjectType>)base.Fields["ObjectType"];
				}
				return new MultiValuedProperty<ConfigurationObjectType>(StartUnifiedPolicySynchronization.SupporttedObjectTypes);
			}
			set
			{
				base.Fields["ObjectType"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SyncByType")]
		public SwitchParameter FullSync
		{
			get
			{
				return (SwitchParameter)(base.Fields["FullSync"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["FullSync"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SyncUpdateObject")]
		public MultiValuedProperty<string> UpdateObjectId
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["UpdateObjectId"];
			}
			set
			{
				base.Fields["UpdateObjectId"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SyncDeleteObject")]
		public MultiValuedProperty<Guid> DeleteObjectId
		{
			get
			{
				return (MultiValuedProperty<Guid>)base.Fields["DeleteObjectId"];
			}
			set
			{
				base.Fields["DeleteObjectId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Workload Workload
		{
			get
			{
				if (base.Fields["Workload"] != null)
				{
					return (Workload)base.Fields["Workload"];
				}
				return Workload.Exchange | Workload.SharePoint;
			}
			set
			{
				base.Fields["Workload"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			Utils.ThrowIfNotRunInEOP();
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
			if (base.Fields.IsModified("Workload"))
			{
				Utils.ValidateWorkloadParameter(this.Workload);
			}
			if (!base.ParameterSetName.Equals("SyncDeleteObject") && !base.ParameterSetName.Equals("SyncUpdateObject"))
			{
				if (base.Fields.IsModified("ObjectType"))
				{
					if (this.ObjectType.Any((ConfigurationObjectType x) => !StartUnifiedPolicySynchronization.SupporttedObjectTypes.Contains(x)))
					{
						throw new InvalidDeltaSyncAndFullSyncTypeException(string.Join(",", from x in StartUnifiedPolicySynchronization.SupporttedObjectTypes
						select x.ToString()));
					}
				}
				return;
			}
			if (this.ObjectType.Count != 1)
			{
				throw new MultipleObjectTypeForObjectLevelSyncException(string.Join(",", from x in StartUnifiedPolicySynchronization.SupporttedObjectTypesForObjectSync
				select x.ToString()));
			}
			if (!StartUnifiedPolicySynchronization.SupporttedObjectTypesForObjectSync.Contains(this.ObjectType.First<ConfigurationObjectType>()))
			{
				throw new InvalidObjectSyncTypeException(string.Join(",", from x in StartUnifiedPolicySynchronization.SupporttedObjectTypesForObjectSync
				select x.ToString()));
			}
			this.objectSyncGuids = (base.ParameterSetName.Equals("SyncDeleteObject") ? this.DeleteObjectId : this.ValidateUpdateObjectSyncParameters());
		}

		protected override void InternalProcessRecord()
		{
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (!(parameterSetName == "SyncByType"))
				{
					if (!(parameterSetName == "SyncUpdateObject"))
					{
						if (!(parameterSetName == "SyncDeleteObject"))
						{
							goto IL_124;
						}
						goto IL_D1;
					}
				}
				else
				{
					using (MultiValuedProperty<ConfigurationObjectType>.Enumerator enumerator = this.ObjectType.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ConfigurationObjectType objectType = enumerator.Current;
							this.syncChangeInfos.Add(new SyncChangeInfo(objectType));
						}
						goto IL_124;
					}
				}
				using (IEnumerator<Guid> enumerator2 = this.objectSyncGuids.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Guid value = enumerator2.Current;
						this.syncChangeInfos.Add(new SyncChangeInfo(this.ObjectType.First<ConfigurationObjectType>(), ChangeType.Update, null, new Guid?(value)));
					}
					goto IL_124;
				}
				IL_D1:
				foreach (Guid value2 in this.DeleteObjectId)
				{
					this.syncChangeInfos.Add(new SyncChangeInfo(this.ObjectType.First<ConfigurationObjectType>(), ChangeType.Delete, null, new Guid?(value2)));
				}
			}
			IL_124:
			this.SendNotification();
		}

		private void SendNotification()
		{
			foreach (object obj in Enum.GetValues(typeof(Workload)))
			{
				Workload workload = (Workload)obj;
				if ((this.Workload & workload) != Workload.None)
				{
					string identity;
					string text = AggregatedNotificationClients.NotifyChangesByWorkload(this, base.DataSession as IConfigurationSession, workload, this.syncChangeInfos, this.FullSync.ToBool(), true, this.executionLogger, base.GetType(), out identity);
					if (string.IsNullOrEmpty(text))
					{
						this.WriteResult(new PsUnifiedPolicyNotification(workload, identity, this.syncChangeInfos, this.FullSync.ToBool()));
					}
					else
					{
						base.WriteWarning(text);
					}
				}
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
		}

		protected override IConfigDataProvider CreateSession()
		{
			return PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForCmdlet(base.CreateSession() as IConfigurationSession, this.executionLogger);
		}

		private IList<Guid> ValidateUpdateObjectSyncParameters()
		{
			IList<Guid> list = new List<Guid>();
			foreach (string text in this.UpdateObjectId)
			{
				switch (this.ObjectType.First<ConfigurationObjectType>())
				{
				case ConfigurationObjectType.Policy:
				{
					PolicyStorage storageObject = (PolicyStorage)base.GetDataObject<PolicyStorage>(new PolicyIdParameter(text), base.DataSession, null, new LocalizedString?(Strings.ErrorPolicyNotFound(text)), new LocalizedString?(Strings.ErrorPolicyNotUnique(text)));
					list.Add(Utils.GetUniversalIdentity(storageObject));
					break;
				}
				case ConfigurationObjectType.Rule:
				{
					RuleStorage storageObject2 = (RuleStorage)base.GetDataObject<RuleStorage>(new ComplianceRuleIdParameter(text), base.DataSession, null, new LocalizedString?(Strings.ErrorRuleNotFound(text)), new LocalizedString?(Strings.ErrorRuleNotUnique(text)));
					list.Add(Utils.GetUniversalIdentity(storageObject2));
					break;
				}
				default:
					throw new NotImplementedException();
				}
			}
			return list;
		}

		protected override void InternalStateReset()
		{
			this.DisposePolicyConfigProvider();
			base.InternalStateReset();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.DisposePolicyConfigProvider();
		}

		private void DisposePolicyConfigProvider()
		{
			PolicyConfigProvider policyConfigProvider = base.DataSession as PolicyConfigProvider;
			if (policyConfigProvider != null)
			{
				policyConfigProvider.Dispose();
			}
		}

		private ExecutionLog executionLogger = ExExecutionLog.CreateForCmdlet();

		private static readonly List<ConfigurationObjectType> SupporttedObjectTypes = new List<ConfigurationObjectType>
		{
			ConfigurationObjectType.Policy,
			ConfigurationObjectType.Rule,
			ConfigurationObjectType.Binding
		};

		private static readonly List<ConfigurationObjectType> SupporttedObjectTypesForObjectSync = new List<ConfigurationObjectType>
		{
			ConfigurationObjectType.Policy,
			ConfigurationObjectType.Rule
		};

		private readonly List<SyncChangeInfo> syncChangeInfos = new List<SyncChangeInfo>();

		private IList<Guid> objectSyncGuids = new List<Guid>();
	}
}
