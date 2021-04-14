using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;
using Microsoft.Office.CompliancePolicy.Validators;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Set", "CompliancePolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public abstract class SetCompliancePolicyBase : SetSystemConfigurationObjectTask<PolicyIdParameter, PsCompliancePolicyBase, PolicyStorage>
	{
		private protected PolicyScenario Scenario { protected get; private set; }

		protected ExecutionLog ExecutionLogger
		{
			get
			{
				return this.executionLogger;
			}
		}

		protected PsCompliancePolicyBase PsPolicyPresentationObject { get; set; }

		protected bool IsRetryDistributionAllowed { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "RetryDistributionParameterSet", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override PolicyIdParameter Identity
		{
			get
			{
				return (PolicyIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string Comment
		{
			get
			{
				return (string)base.Fields["Comment"];
			}
			set
			{
				base.Fields["Comment"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields["Enabled"];
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> AddExchangeBinding
		{
			get
			{
				return Utils.BindingParameterGetter(base.Fields["AddExchangeBinding"]);
			}
			set
			{
				base.Fields["AddExchangeBinding"] = Utils.BindingParameterSetter(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> RemoveExchangeBinding
		{
			get
			{
				return Utils.BindingParameterGetter(base.Fields["RemoveExchangeBinding"]);
			}
			set
			{
				base.Fields["RemoveExchangeBinding"] = Utils.BindingParameterSetter(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> AddSharePointBinding
		{
			get
			{
				return Utils.BindingParameterGetter(base.Fields["AddSharePointBinding"]);
			}
			set
			{
				base.Fields["AddSharePointBinding"] = Utils.BindingParameterSetter(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> RemoveSharePointBinding
		{
			get
			{
				return Utils.BindingParameterGetter(base.Fields["RemoveSharePointBinding"]);
			}
			set
			{
				base.Fields["RemoveSharePointBinding"] = Utils.BindingParameterSetter(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> AddOneDriveBinding
		{
			get
			{
				return Utils.BindingParameterGetter(base.Fields["AddOneDriveBinding"]);
			}
			set
			{
				base.Fields["AddOneDriveBinding"] = Utils.BindingParameterSetter(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> RemoveOneDriveBinding
		{
			get
			{
				return Utils.BindingParameterGetter(base.Fields["RemoveOneDriveBinding"]);
			}
			set
			{
				base.Fields["RemoveOneDriveBinding"] = Utils.BindingParameterSetter(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "RetryDistributionParameterSet")]
		public SwitchParameter RetryDistribution
		{
			get
			{
				return (SwitchParameter)(base.Fields["RetryDistribution"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RetryDistribution"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return Utils.GetRootId(base.DataSession);
			}
		}

		private protected MultiValuedProperty<BindingMetadata> InternalAddExchangeBindings { protected get; private set; }

		private protected MultiValuedProperty<BindingMetadata> InternalRemoveExchangeBindings { protected get; private set; }

		private protected MultiValuedProperty<BindingMetadata> InternalAddSharePointBindings { protected get; private set; }

		private protected MultiValuedProperty<BindingMetadata> InternalRemoveSharePointBindings { protected get; private set; }

		private protected MultiValuedProperty<BindingMetadata> InternalAddOneDriveBindings { protected get; private set; }

		private protected MultiValuedProperty<BindingMetadata> InternalRemoveOneDriveBindings { protected get; private set; }

		protected override IConfigDataProvider CreateSession()
		{
			return PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForCmdlet(base.CreateSession() as IConfigurationSession, this.executionLogger);
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return true;
		}

		protected override bool IsObjectStateChanged()
		{
			if (!base.IsObjectStateChanged())
			{
				return this.PsPolicyPresentationObject.StorageBindings.Any((BindingStorage binding) => binding.ObjectState != ObjectState.Unchanged);
			}
			return true;
		}

		protected SetCompliancePolicyBase(PolicyScenario scenario)
		{
			this.Scenario = scenario;
		}

		protected override void InternalValidate()
		{
			Utils.ThrowIfNotRunInEOP();
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
			base.InternalValidate();
			if (this.DataObject.IsModified(ADObjectSchema.Name) && Utils.LoadPolicyStorages(base.DataSession, this.DataObject.Scenario).Any((PolicyStorage p) => p.Name.Equals((string)this.DataObject[ADObjectSchema.Name], StringComparison.InvariantCultureIgnoreCase)))
			{
				throw new CompliancePolicyAlreadyExistsException((string)this.DataObject[ADObjectSchema.Name]);
			}
			if (base.Fields.IsModified("Enabled") && this.Enabled)
			{
				IEnumerable<RuleStorage> enumerable = Utils.LoadRuleStoragesByPolicy(base.DataSession, this.DataObject, Utils.GetRootId(base.DataSession));
				foreach (RuleStorage ruleStorage in enumerable)
				{
					base.WriteVerbose(Strings.VerboseLoadRuleStorageObjectsForPolicy(ruleStorage.ToString(), this.DataObject.ToString()));
				}
				if (enumerable.Any((RuleStorage x) => !x.IsEnabled))
				{
					this.WriteWarning(Strings.WarningPolicyContainsDisabledRules(this.DataObject.Name));
				}
			}
			if (!this.RetryDistribution)
			{
				this.ValidateBindingParameter();
				return;
			}
			List<PolicyDistributionErrorDetails> source;
			DateTime? dateTime;
			PolicySettingStatusHelpers.GetPolicyDistributionStatus(this.DataObject, Utils.LoadBindingStoragesByPolicy(base.DataSession, this.DataObject), base.DataSession, out source, out dateTime);
			if (!source.Any((PolicyDistributionErrorDetails error) => error.ResultCode != UnifiedPolicyErrorCode.PolicySyncTimeout))
			{
				this.WriteWarning(Strings.ErrorCompliancePolicyHasNoObjectsToRetry(this.DataObject.Name));
				this.IsRetryDistributionAllowed = false;
				return;
			}
			this.IsRetryDistributionAllowed = true;
		}

		protected override IConfigurable ResolveDataObject()
		{
			PolicyStorage policyStorage = base.GetDataObjects<PolicyStorage>(this.Identity, base.DataSession, null).FirstOrDefault((PolicyStorage p) => p.Scenario == this.Scenario);
			if (policyStorage == null)
			{
				base.WriteError(new ErrorPolicyNotFoundException(this.Identity.ToString()), ErrorCategory.InvalidOperation, null);
			}
			return policyStorage;
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			PolicyStorage policyStorage = (PolicyStorage)dataObject;
			policyStorage.ResetChangeTracking(true);
			if (policyStorage.Mode == Mode.PendingDeletion && !this.RetryDistribution)
			{
				base.WriteError(new ErrorCompliancePolicyIsDeletedException(policyStorage.Name), ErrorCategory.InvalidOperation, null);
			}
			this.PsPolicyPresentationObject = new PsCompliancePolicyBase(policyStorage)
			{
				StorageBindings = Utils.LoadBindingStoragesByPolicy(base.DataSession, policyStorage)
			};
			foreach (BindingStorage bindingStorage in this.PsPolicyPresentationObject.StorageBindings)
			{
				base.WriteVerbose(Strings.VerboseLoadBindingStorageObjects(bindingStorage.ToString(), this.PsPolicyPresentationObject.Name));
			}
			this.PsPolicyPresentationObject.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			if (this.PsPolicyPresentationObject.ReadOnly)
			{
				throw new TaskRuleIsTooAdvancedToModifyException(this.PsPolicyPresentationObject.Name);
			}
			base.StampChangesOn(dataObject);
			this.ValidateBindingParameterBeforeMerge();
			this.CopyExplicitParameters();
			this.PsPolicyPresentationObject.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, false);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.RetryDistribution)
			{
				this.RetryPolicyDistribution();
			}
			else
			{
				foreach (BindingStorage bindingStorage in this.PsPolicyPresentationObject.StorageBindings)
				{
					if (bindingStorage.AppliedScopes.Any<ScopeStorage>())
					{
						base.DataSession.Save(bindingStorage);
						base.WriteVerbose(Strings.VerboseSaveBindingStorageObjects(bindingStorage.ToString(), this.PsPolicyPresentationObject.Name));
					}
					else
					{
						base.DataSession.Delete(bindingStorage);
						base.WriteVerbose(Strings.VerboseDeleteBindingStorageObjects(bindingStorage.ToString(), this.PsPolicyPresentationObject.Name));
					}
				}
				IEnumerable<RuleStorage> enumerable = Utils.LoadRuleStoragesByPolicy(base.DataSession, this.DataObject, this.RootId);
				foreach (RuleStorage ruleStorage in enumerable)
				{
					base.WriteVerbose(Strings.VerboseLoadRuleStorageObjectsForPolicy(ruleStorage.ToString(), this.DataObject.ToString()));
				}
				Utils.ThrowIfRulesInPolicyAreTooAdvanced(enumerable, this.DataObject, this, base.DataSession as IConfigurationSession);
				base.InternalProcessRecord();
				PolicySettingStatusHelpers.CheckNotificationResultsAndUpdateStatus(this, (IConfigurationSession)base.DataSession, this.OnNotifyChanges());
			}
			TaskLogger.LogExit();
		}

		protected virtual IEnumerable<ChangeNotificationData> OnNotifyChanges()
		{
			return AggregatedNotificationClients.NotifyChanges(this, (IConfigurationSession)base.DataSession, this.DataObject, this.executionLogger, base.GetType(), this.PsPolicyPresentationObject.StorageBindings, null);
		}

		protected virtual string OnNotifyRetryDistributionChanges(List<ChangeNotificationData> itemsToSync, Workload workload, bool isObjectLevelSync)
		{
			string text;
			return AggregatedNotificationClients.NotifyChangesByWorkload(this, (IConfigurationSession)base.DataSession, workload, from ols in itemsToSync
			select ols.CreateSyncChangeInfo(isObjectLevelSync), false, false, this.executionLogger, base.GetType(), out text);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
		}

		protected virtual void CopyExplicitParameters()
		{
			if (base.Fields.IsModified("Comment"))
			{
				this.PsPolicyPresentationObject.Comment = this.Comment;
			}
			if (base.Fields.IsModified("Enabled"))
			{
				this.PsPolicyPresentationObject.Enabled = this.Enabled;
			}
			Utils.MergeBindings(this.PsPolicyPresentationObject.ExchangeBinding, this.InternalAddExchangeBindings, this.InternalRemoveExchangeBindings, (this.PsPolicyPresentationObject.Workload & Workload.Exchange) != Workload.Exchange);
			if (this.PsPolicyPresentationObject.ExchangeBinding.Count<BindingMetadata>() > 1000)
			{
				throw new BindingCountExceedsLimitException("Exchange", 1000);
			}
			Utils.MergeBindings(this.PsPolicyPresentationObject.SharePointBinding, this.InternalAddSharePointBindings, this.InternalRemoveSharePointBindings, (this.PsPolicyPresentationObject.Workload & Workload.SharePoint) != Workload.SharePoint);
			if (this.PsPolicyPresentationObject.SharePointBinding.Count<BindingMetadata>() > 100)
			{
				throw new BindingCountExceedsLimitException("Sharepoint", 100);
			}
			Utils.MergeBindings(this.PsPolicyPresentationObject.OneDriveBinding, this.InternalAddOneDriveBindings, this.InternalRemoveOneDriveBindings, (this.PsPolicyPresentationObject.Workload & Workload.SharePoint) != Workload.SharePoint);
			if (this.PsPolicyPresentationObject.OneDriveBinding.Count<BindingMetadata>() > 100)
			{
				throw new BindingCountExceedsLimitException("Sharepoint", 100);
			}
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

		private void ValidateBindingParameterBeforeMerge()
		{
			if (this.AddExchangeBinding.Intersect(this.RemoveExchangeBinding, StringComparer.InvariantCultureIgnoreCase).Any<string>())
			{
				throw new AddRemoveExBindingsOverlappedException(string.Join(",", this.AddExchangeBinding.Intersect(this.RemoveExchangeBinding, StringComparer.InvariantCultureIgnoreCase)));
			}
			if (this.AddSharePointBinding.Intersect(this.RemoveSharePointBinding, StringComparer.InvariantCultureIgnoreCase).Any<string>())
			{
				throw new AddRemoveSpBindingsOverlappedException(string.Join(",", this.AddSharePointBinding.Intersect(this.RemoveSharePointBinding, StringComparer.InvariantCultureIgnoreCase)));
			}
			if (this.AddOneDriveBinding.Intersect(this.RemoveOneDriveBinding, StringComparer.InvariantCultureIgnoreCase).Any<string>())
			{
				throw new AddRemoveSpBindingsOverlappedException(string.Join(",", this.AddOneDriveBinding.Intersect(this.RemoveOneDriveBinding, StringComparer.InvariantCultureIgnoreCase)));
			}
			this.InternalAddExchangeBindings = new MultiValuedProperty<BindingMetadata>();
			this.InternalRemoveExchangeBindings = new MultiValuedProperty<BindingMetadata>();
			this.InternalAddSharePointBindings = new MultiValuedProperty<BindingMetadata>();
			this.InternalRemoveSharePointBindings = new MultiValuedProperty<BindingMetadata>();
			this.InternalAddOneDriveBindings = new MultiValuedProperty<BindingMetadata>();
			this.InternalRemoveOneDriveBindings = new MultiValuedProperty<BindingMetadata>();
			if (this.RemoveExchangeBinding.Any<string>())
			{
				this.ValidateRecipientsForRemove();
			}
			if (this.AddExchangeBinding.Any<string>())
			{
				List<string> list = (from b in this.PsPolicyPresentationObject.ExchangeBinding
				select b.ImmutableIdentity).ToList<string>();
				list.AddRange(this.AddExchangeBinding);
				this.AddExchangeBinding = NewCompliancePolicyBase.ValidateWideScopeBinding(this.AddExchangeBinding, "All", "All", new BindingCannotCombineAllWithIndividualBindingsException("Exchange"));
				this.ExpandGroupsAndValidateRecipientsForAdd();
			}
			this.InternalRemoveSharePointBindings = this.ValidateSharepointSitesForRemove(this.RemoveSharePointBinding);
			SetCompliancePolicyBase.ValidateAddedSharepointBinding(this.AddSharePointBinding, this.RemoveSharePointBinding, this.PsPolicyPresentationObject.SharePointBinding, "Sharepoint");
			this.InternalAddSharePointBindings = this.ValidateSharepointSitesForAdd(this.PsPolicyPresentationObject.SharePointBinding, this.InternalRemoveSharePointBindings, this.AddSharePointBinding, Workload.SharePoint);
			this.InternalRemoveOneDriveBindings = this.ValidateSharepointSitesForRemove(this.RemoveOneDriveBinding);
			SetCompliancePolicyBase.ValidateAddedSharepointBinding(this.AddOneDriveBinding, this.RemoveOneDriveBinding, this.PsPolicyPresentationObject.OneDriveBinding, "OneDriveBusiness");
			this.InternalAddOneDriveBindings = this.ValidateSharepointSitesForAdd(this.PsPolicyPresentationObject.OneDriveBinding, this.InternalRemoveOneDriveBindings, this.AddOneDriveBinding, Workload.OneDriveForBusiness);
		}

		internal static void ValidateAddedSharepointBinding(IEnumerable<string> addedBindings, IEnumerable<string> removedBindings, MultiValuedProperty<BindingMetadata> psObjectBindings, string subWorkloadName)
		{
			if (addedBindings.Any<string>())
			{
				addedBindings = NewCompliancePolicyBase.ValidateWideScopeBinding(addedBindings, "All", "All", new BindingCannotCombineAllWithIndividualBindingsException(subWorkloadName));
				if (NewCompliancePolicyBase.IsBindingEnabled(addedBindings, "All"))
				{
					psObjectBindings.Clear();
					return;
				}
				List<string> bindings = (from b in psObjectBindings
				select b.ImmutableIdentity).ToList<string>();
				if (NewCompliancePolicyBase.IsBindingEnabled(bindings, "All") && !NewCompliancePolicyBase.IsBindingEnabled(removedBindings, "All"))
				{
					throw new BindingCannotCombineAllWithIndividualBindingsException(subWorkloadName);
				}
			}
		}

		private void ValidateBindingParameter()
		{
			if ((this.PsPolicyPresentationObject.Workload & Workload.Exchange) == Workload.None && this.PsPolicyPresentationObject.ExchangeBinding.Any<BindingMetadata>())
			{
				throw new ExBindingWithoutExWorkloadException();
			}
			if ((this.PsPolicyPresentationObject.Workload & Workload.SharePoint) == Workload.None && this.PsPolicyPresentationObject.SharePointBinding.Any<BindingMetadata>())
			{
				throw new SpBindingWithoutSpWorkloadException();
			}
		}

		private void ExpandGroupsAndValidateRecipientsForAdd()
		{
			base.WriteVerbose(Strings.VerboseValidatingAddExchangeBinding);
			int existingRecipientsCount = SetCompliancePolicyBase.CalculateBindingCountAfterRemove(this.PsPolicyPresentationObject.ExchangeBinding, this.InternalRemoveExchangeBindings);
			ExchangeValidator exchangeValidator = this.CreateExchangeValidator(true, "Validating AddExchangeBinding", existingRecipientsCount);
			this.InternalAddExchangeBindings = exchangeValidator.ValidateRecipients(this.AddExchangeBinding);
		}

		private void ValidateRecipientsForRemove()
		{
			base.WriteVerbose(Strings.VerboseValidatingRemoveExchangeBinding);
			ExchangeValidator exchangeValidator = this.CreateExchangeValidator(false, "Validating RemoveExchangeBinding", 0);
			this.InternalRemoveExchangeBindings = exchangeValidator.ValidateRecipients(this.RemoveExchangeBinding);
		}

		private MultiValuedProperty<BindingMetadata> ValidateSharepointSitesForAdd(MultiValuedProperty<BindingMetadata> psObjectBindings, MultiValuedProperty<BindingMetadata> internalRemoveBindings, MultiValuedProperty<string> addBindingParameter, Workload subWorkload)
		{
			base.WriteVerbose(Strings.VerboseValidatingAddSharepointBinding);
			int existingSitesCount = SetCompliancePolicyBase.CalculateBindingCountAfterRemove(psObjectBindings, internalRemoveBindings);
			SharepointValidator sharepointValidator = this.CreateSharepointValidator("Validating AddSharepointBinding", existingSitesCount);
			MultiValuedProperty<BindingMetadata> multiValuedProperty = sharepointValidator.ValidateLocations(addBindingParameter);
			NewCompliancePolicyBase.SetBindingsSubWorkload(multiValuedProperty, subWorkload);
			return multiValuedProperty;
		}

		private MultiValuedProperty<BindingMetadata> ValidateSharepointSitesForRemove(IEnumerable<string> sitesToRemove)
		{
			base.WriteVerbose(Strings.VerboseValidatingRemoveSharepointBinding);
			SharepointValidator sharepointValidator = this.CreateSharepointValidator("Validating RemoveSharepointBinding", 0);
			return sharepointValidator.ValidateLocations(sitesToRemove);
		}

		private ExchangeValidator CreateExchangeValidator(bool allowGroups, string logTag, int existingRecipientsCount)
		{
			return ExchangeValidator.Create((IConfigurationSession)base.DataSession, (RecipientIdParameter recipientId, IRecipientSession recipientSession) => base.GetDataObject<ReducedRecipient>(recipientId, recipientSession, null, new LocalizedString?(Strings.ErrorUserObjectNotFound(recipientId.RawIdentity)), new LocalizedString?(Strings.ErrorUserObjectAmbiguous(recipientId.RawIdentity))) as ReducedRecipient, new Task.TaskErrorLoggingDelegate(base.WriteError), new Action<LocalizedString>(this.WriteWarning), (LocalizedString locString) => this.Force || base.ShouldContinue(locString), allowGroups, logTag, SourceValidator.Clients.SetCompliancePolicy, existingRecipientsCount, this.executionLogger);
		}

		private void RetryPolicyDistribution()
		{
			if (!this.IsRetryDistributionAllowed)
			{
				base.WriteVerbose(Strings.VerboseRetryDistributionNotApplicable);
				return;
			}
			IEnumerable<Workload> enumerable = new List<Workload>
			{
				Workload.Exchange,
				Workload.SharePoint
			};
			Dictionary<Workload, List<ChangeNotificationData>> dictionary = this.GenerateSyncsForFailedPolicies(enumerable);
			Dictionary<Workload, List<ChangeNotificationData>> dictionary2 = this.GenerateSyncsForFailedBindings();
			Dictionary<Workload, List<ChangeNotificationData>> dictionary3 = this.GenerateSyncsForFailedRules(enumerable);
			foreach (Workload workload in enumerable)
			{
				List<ChangeNotificationData> list = new List<ChangeNotificationData>();
				List<ChangeNotificationData> list2 = new List<ChangeNotificationData>();
				if (dictionary.ContainsKey(workload))
				{
					list.AddRange(dictionary[workload]);
				}
				if (dictionary3.ContainsKey(workload))
				{
					list.AddRange(dictionary3[workload]);
				}
				if (dictionary2.ContainsKey(workload))
				{
					list2.AddRange(dictionary2[workload]);
				}
				if (list.Any<ChangeNotificationData>())
				{
					this.WriteSyncVerbose(list, workload, true);
					this.HandleNotificationErrors(this.OnNotifyRetryDistributionChanges(list, workload, true), list);
				}
				if (list2.Any<ChangeNotificationData>())
				{
					this.WriteSyncVerbose(list2, workload, false);
					this.HandleNotificationErrors(this.OnNotifyRetryDistributionChanges(list2, workload, false), list2);
				}
			}
		}

		private void WriteSyncVerbose(List<ChangeNotificationData> syncData, Workload workload, bool isObjectLevelSync)
		{
			base.WriteVerbose(Strings.VerboseRetryDistributionNotifyingWorkload(workload.ToString(), isObjectLevelSync ? "object" : "delta"));
			foreach (ChangeNotificationData changeNotificationData in syncData)
			{
				base.WriteVerbose(Strings.VerboseRetryDistributionNotificationDetails(changeNotificationData.Id.ToString(), changeNotificationData.ChangeType.ToString(), changeNotificationData.ObjectType.ToString()));
			}
		}

		private Dictionary<Workload, List<ChangeNotificationData>> GenerateSyncsForFailedPolicies(IEnumerable<Workload> workloads)
		{
			IEnumerable<UnifiedPolicySettingStatus> syncStatuses = PolicySettingStatusHelpers.LoadSyncStatuses(base.DataSession, Utils.GetUniversalIdentity(this.DataObject), typeof(PolicyStorage).Name);
			return SetCompliancePolicyBase.GenerateSyncs(syncStatuses, workloads, base.DataSession, this.DataObject, ConfigurationObjectType.Policy);
		}

		private Dictionary<Workload, List<ChangeNotificationData>> GenerateSyncsForFailedRules(IEnumerable<Workload> workloads)
		{
			Dictionary<Workload, List<ChangeNotificationData>> dictionary = new Dictionary<Workload, List<ChangeNotificationData>>();
			IList<RuleStorage> list = Utils.LoadRuleStoragesByPolicy(base.DataSession, this.DataObject, Utils.GetRootId(base.DataSession));
			foreach (RuleStorage storageObject in list)
			{
				IEnumerable<UnifiedPolicySettingStatus> syncStatuses = PolicySettingStatusHelpers.LoadSyncStatuses(base.DataSession, Utils.GetUniversalIdentity(storageObject), typeof(RuleStorage).Name);
				Dictionary<Workload, List<ChangeNotificationData>> dictionary2 = SetCompliancePolicyBase.GenerateSyncs(syncStatuses, workloads, base.DataSession, storageObject, ConfigurationObjectType.Rule);
				foreach (Workload key in dictionary2.Keys)
				{
					if (dictionary.ContainsKey(key))
					{
						dictionary[key].AddRange(dictionary2[key]);
					}
					else
					{
						dictionary[key] = dictionary2[key];
					}
				}
			}
			return dictionary;
		}

		private Dictionary<Workload, List<ChangeNotificationData>> GenerateSyncsForFailedBindings()
		{
			Dictionary<Workload, List<ChangeNotificationData>> dictionary = new Dictionary<Workload, List<ChangeNotificationData>>();
			using (IEnumerator<BindingStorage> enumerator = this.PsPolicyPresentationObject.StorageBindings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BindingStorage bindingStorage = enumerator.Current;
					IEnumerable<UnifiedPolicySettingStatus> source = PolicySettingStatusHelpers.LoadSyncStatuses(base.DataSession, Utils.GetUniversalIdentity(bindingStorage), typeof(BindingStorage).Name);
					bool flag = false;
					if (source.Any((UnifiedPolicySettingStatus s) => SetCompliancePolicyBase.HasDistributionFailed(bindingStorage, s)))
					{
						flag = true;
						bindingStorage.PolicyVersion = CombGuidGenerator.NewGuid();
						if (!dictionary.ContainsKey(bindingStorage.Workload))
						{
							dictionary[bindingStorage.Workload] = new List<ChangeNotificationData>();
						}
						dictionary[bindingStorage.Workload].Add(AggregatedNotificationClients.CreateChangeData(bindingStorage.Workload, bindingStorage));
					}
					List<ChangeNotificationData> list = this.GenerateSyncsForFailedScopes(bindingStorage.AppliedScopes);
					list.AddRange(this.GenerateSyncsForFailedScopes(bindingStorage.RemovedScopes));
					if (list.Any<ChangeNotificationData>())
					{
						if (!dictionary.ContainsKey(bindingStorage.Workload))
						{
							dictionary[bindingStorage.Workload] = new List<ChangeNotificationData>();
						}
						dictionary[bindingStorage.Workload].AddRange(list);
						if (!flag)
						{
							flag = true;
							bindingStorage.PolicyVersion = CombGuidGenerator.NewGuid();
							base.DataSession.Save(bindingStorage);
							dictionary[bindingStorage.Workload].Add(AggregatedNotificationClients.CreateChangeData(bindingStorage.Workload, bindingStorage));
						}
					}
					if (flag)
					{
						base.DataSession.Save(bindingStorage);
					}
				}
			}
			return dictionary;
		}

		private List<ChangeNotificationData> GenerateSyncsForFailedScopes(IEnumerable<ScopeStorage> scopeStorages)
		{
			List<ChangeNotificationData> list = new List<ChangeNotificationData>();
			foreach (ScopeStorage scopeStorage in scopeStorages)
			{
				IEnumerable<UnifiedPolicySettingStatus> enumerable = PolicySettingStatusHelpers.LoadSyncStatuses(base.DataSession, Utils.GetUniversalIdentity(scopeStorage), typeof(ScopeStorage).Name);
				foreach (UnifiedPolicySettingStatus status in enumerable)
				{
					if (SetCompliancePolicyBase.HasDistributionFailed(scopeStorage, status))
					{
						scopeStorage.PolicyVersion = CombGuidGenerator.NewGuid();
						list.Add(AggregatedNotificationClients.CreateChangeData(scopeStorage.Workload, scopeStorage));
					}
				}
			}
			return list;
		}

		private static Dictionary<Workload, List<ChangeNotificationData>> GenerateSyncs(IEnumerable<UnifiedPolicySettingStatus> syncStatuses, IEnumerable<Workload> workloads, IConfigDataProvider dataSession, UnifiedPolicyStorageBase storageObject, ConfigurationObjectType objectType)
		{
			Dictionary<Workload, List<ChangeNotificationData>> dictionary = new Dictionary<Workload, List<ChangeNotificationData>>();
			if (syncStatuses.Any((UnifiedPolicySettingStatus s) => SetCompliancePolicyBase.HasDistributionFailed(storageObject, s)))
			{
				storageObject.PolicyVersion = CombGuidGenerator.NewGuid();
				dataSession.Save(storageObject);
				foreach (Workload workload in workloads)
				{
					dictionary[workload] = new List<ChangeNotificationData>
					{
						AggregatedNotificationClients.CreateChangeData(workload, storageObject)
					};
				}
			}
			return dictionary;
		}

		private void HandleNotificationErrors(string notificationError, IEnumerable<ChangeNotificationData> syncData)
		{
			if (!string.IsNullOrEmpty(notificationError))
			{
				base.WriteWarning(notificationError);
				AggregatedNotificationClients.SetNotificationResults(syncData, notificationError);
				PolicySettingStatusHelpers.CheckNotificationResultsAndUpdateStatus(this, (IConfigurationSession)base.DataSession, syncData);
			}
		}

		private SharepointValidator CreateSharepointValidator(string logTag, int existingSitesCount)
		{
			return SharepointValidator.Create((IConfigurationSession)base.DataSession, base.ExchangeRunspaceConfig, new Task.TaskErrorLoggingDelegate(base.WriteError), new Action<LocalizedString>(this.WriteWarning), (LocalizedString locString) => this.Force || base.ShouldContinue(locString), logTag, SourceValidator.Clients.SetCompliancePolicy, existingSitesCount, this.executionLogger);
		}

		private static int CalculateBindingCountAfterRemove(MultiValuedProperty<BindingMetadata> existingBinding, MultiValuedProperty<BindingMetadata> removeBinding)
		{
			return existingBinding.Count - (from item in existingBinding
			select item.ImmutableIdentity).Intersect(from item in removeBinding
			select item.ImmutableIdentity, StringComparer.OrdinalIgnoreCase).Count<string>();
		}

		private static bool HasDistributionFailed(UnifiedPolicyStorageBase storageObject, UnifiedPolicySettingStatus status)
		{
			return status.ObjectVersion == storageObject.PolicyVersion && status.ErrorCode != 0;
		}

		private const string RetryDistributionParameterSet = "RetryDistributionParameterSet";

		protected ExecutionLog executionLogger = ExExecutionLog.CreateForCmdlet();
	}
}
