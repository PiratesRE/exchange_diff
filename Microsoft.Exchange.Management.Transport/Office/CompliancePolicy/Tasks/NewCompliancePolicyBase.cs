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
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.Validators;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "CompliancePolicyBase", SupportsShouldProcess = true)]
	public abstract class NewCompliancePolicyBase : NewMultitenancyFixedNameSystemConfigurationObjectTask<PolicyStorage>
	{
		protected PsCompliancePolicyBase PsPolicyPresentationObject { get; set; }

		protected PolicyScenario Scenario { get; set; }

		protected Workload TenantWorkloadConfig
		{
			get
			{
				return Workload.Exchange | Workload.SharePoint;
			}
		}

		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return (string)base.Fields[ADObjectSchema.Name];
			}
			set
			{
				base.Fields[ADObjectSchema.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return (string)base.Fields[PsCompliancePolicyBaseSchema.Comment];
			}
			set
			{
				base.Fields[PsCompliancePolicyBaseSchema.Comment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				object obj = base.Fields[PsCompliancePolicyBaseSchema.Enabled];
				return obj == null || (bool)obj;
			}
			set
			{
				base.Fields[PsCompliancePolicyBaseSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExchangeBinding
		{
			get
			{
				return Utils.BindingParameterGetter(this.exchangeBindingParameter);
			}
			set
			{
				this.exchangeBindingParameter = Utils.BindingParameterSetter(value);
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> SharePointBinding
		{
			get
			{
				return Utils.BindingParameterGetter(this.sharePointBindingParameter);
			}
			set
			{
				this.sharePointBindingParameter = Utils.BindingParameterSetter(value);
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OneDriveBinding
		{
			get
			{
				return Utils.BindingParameterGetter(this.oneDriveBindingParameter);
			}
			set
			{
				this.oneDriveBindingParameter = Utils.BindingParameterSetter(value);
			}
		}

		protected override bool SkipWriteResult
		{
			get
			{
				return true;
			}
		}

		private protected MultiValuedProperty<BindingMetadata> InternalExchangeBindings { protected get; private set; }

		private protected MultiValuedProperty<BindingMetadata> InternalSharePointBindings { protected get; private set; }

		private protected MultiValuedProperty<BindingMetadata> InternalOneDriveBindings { protected get; private set; }

		public NewCompliancePolicyBase()
		{
		}

		protected NewCompliancePolicyBase(PolicyScenario scenario)
		{
			this.Scenario = scenario;
		}

		protected override IConfigurable PrepareDataObject()
		{
			PolicyStorage policyStorage = (PolicyStorage)base.PrepareDataObject();
			policyStorage.SetId(base.DataSession as IConfigurationSession, this.Name);
			policyStorage.MasterIdentity = Guid.NewGuid();
			policyStorage.Scenario = this.Scenario;
			return policyStorage;
		}

		protected override IConfigDataProvider CreateSession()
		{
			return PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForCmdlet(base.CreateSession() as IConfigurationSession, this.executionLogger);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
		}

		protected override void InternalValidate()
		{
			Utils.ThrowIfNotRunInEOP();
			base.InternalValidate();
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
			IEnumerable<PolicyStorage> source = Utils.LoadPolicyStorages(base.DataSession, this.Scenario);
			if (source.Count<PolicyStorage>() > 1000)
			{
				throw new CompliancePolicyCountExceedsLimitException(1000);
			}
			if (source.Any((PolicyStorage x) => x.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase)))
			{
				throw new CompliancePolicyAlreadyExistsException(this.Name);
			}
			this.ValidateBindingParameter();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.PsPolicyPresentationObject.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, true);
			base.InternalProcessRecord();
			foreach (BindingStorage bindingStorage in this.PsPolicyPresentationObject.StorageBindings)
			{
				base.DataSession.Save(bindingStorage);
				base.WriteVerbose(Strings.VerboseSaveBindingStorageObjects(bindingStorage.ToString(), this.PsPolicyPresentationObject.ToString()));
			}
			if (!base.HasErrors)
			{
				this.WriteResult();
			}
			PolicySettingStatusHelpers.CheckNotificationResultsAndUpdateStatus(this, (IConfigurationSession)base.DataSession, this.OnNotifyChanges());
			TaskLogger.LogExit();
		}

		protected virtual IEnumerable<ChangeNotificationData> OnNotifyChanges()
		{
			return AggregatedNotificationClients.NotifyChanges(this, (IConfigurationSession)base.DataSession, this.DataObject, this.executionLogger, base.GetType(), this.PsPolicyPresentationObject.StorageBindings, null);
		}

		internal static MultiValuedProperty<string> ValidateWideScopeBinding(IEnumerable<string> bindings, string wideScopeBindingName, string wideScopeBindingValue, Exception validationException)
		{
			if (NewCompliancePolicyBase.IsBindingEnabled(bindings, wideScopeBindingValue) && bindings.Count<string>() > 1)
			{
				throw validationException;
			}
			if (string.Compare(wideScopeBindingName, wideScopeBindingValue, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				List<string> list = new List<string>(bindings.Count<string>());
				list.AddRange(bindings.Select(delegate(string binding)
				{
					if (string.Compare(binding, wideScopeBindingName, StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						return binding;
					}
					return wideScopeBindingValue;
				}));
				return new MultiValuedProperty<string>(list);
			}
			return new MultiValuedProperty<string>(bindings);
		}

		internal static bool IsBindingEnabled(IEnumerable<string> bindings, string valueToCheck)
		{
			return bindings.Any((string b) => string.Compare(b, valueToCheck, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		internal static void SetBindingsSubWorkload(MultiValuedProperty<BindingMetadata> bindings, Workload subWorkload)
		{
			foreach (BindingMetadata bindingMetadata in bindings)
			{
				bindingMetadata.Workload = subWorkload;
			}
		}

		private void ValidateBindingParameter()
		{
			if ((this.TenantWorkloadConfig & Workload.Exchange) == Workload.None && this.ExchangeBinding.Any<string>())
			{
				throw new ExBindingWithoutExWorkloadException();
			}
			if ((this.TenantWorkloadConfig & Workload.SharePoint) == Workload.None && this.SharePointBinding.Any<string>())
			{
				throw new SpBindingWithoutSpWorkloadException();
			}
			this.InternalExchangeBindings = this.ValidateExchangeBindings(this.ExchangeBinding, 1000);
			this.InternalSharePointBindings = this.ValidateSharepointBindings(this.SharePointBinding, Workload.SharePoint, "Sharepoint", 100);
			this.InternalOneDriveBindings = this.ValidateSharepointBindings(this.OneDriveBinding, Workload.OneDriveForBusiness, "OneDriveBusiness", 100);
		}

		private MultiValuedProperty<BindingMetadata> ValidateExchangeBindings(IEnumerable<string> bindings, int maxCount)
		{
			base.WriteVerbose(Strings.VerboseValidatingExchangeBinding);
			MultiValuedProperty<BindingMetadata> multiValuedProperty = new MultiValuedProperty<BindingMetadata>();
			if (bindings.Any<string>())
			{
				List<string> list = (from binding in bindings
				where SourceValidator.IsWideScope(binding)
				select binding).ToList<string>();
				if (list.Any<string>())
				{
					throw new ExCannotContainWideScopeBindingsException(string.Join(", ", list));
				}
				this.ExchangeBinding = NewCompliancePolicyBase.ValidateWideScopeBinding(this.ExchangeBinding, "All", "All", new BindingCannotCombineAllWithIndividualBindingsException("Exchange"));
				ExchangeValidator exchangeValidator = this.CreateExchangeValidator(true, "Validating ExchangeBinding");
				multiValuedProperty = exchangeValidator.ValidateRecipients(this.ExchangeBinding);
				if (this.ExchangeBinding.Count<string>() > 1000)
				{
					throw new BindingCountExceedsLimitException("Exchange", 1000);
				}
			}
			NewCompliancePolicyBase.SetBindingsSubWorkload(multiValuedProperty, Workload.Exchange);
			return multiValuedProperty;
		}

		private MultiValuedProperty<BindingMetadata> ValidateSharepointBindings(IEnumerable<string> bindings, Workload subWorkload, string workloadName, int maxCount)
		{
			base.WriteVerbose(Strings.VerboseValidatingSharepointBinding(workloadName));
			MultiValuedProperty<BindingMetadata> multiValuedProperty = new MultiValuedProperty<BindingMetadata>();
			if (bindings.Any<string>())
			{
				bindings = NewCompliancePolicyBase.ValidateWideScopeBinding(bindings, "All", "All", new BindingCannotCombineAllWithIndividualBindingsException(workloadName));
				SharepointValidator sharepointValidator = this.CreateSharepointValidator(string.Format("Validating {0} Binding", workloadName));
				multiValuedProperty = sharepointValidator.ValidateLocations(bindings);
				if (multiValuedProperty.Count<BindingMetadata>() > maxCount)
				{
					throw new BindingCountExceedsLimitException(workloadName, maxCount);
				}
				NewCompliancePolicyBase.SetBindingsSubWorkload(multiValuedProperty, subWorkload);
			}
			return multiValuedProperty;
		}

		private ExchangeValidator CreateExchangeValidator(bool allowGroups, string logTag)
		{
			return ExchangeValidator.Create((IConfigurationSession)base.DataSession, (RecipientIdParameter recipientId, IRecipientSession recipientSession) => base.GetDataObject<ReducedRecipient>(recipientId, recipientSession, null, new LocalizedString?(Strings.ErrorUserObjectNotFound(recipientId.RawIdentity)), new LocalizedString?(Strings.ErrorUserObjectAmbiguous(recipientId.RawIdentity))) as ReducedRecipient, new Task.TaskErrorLoggingDelegate(base.WriteError), new Action<LocalizedString>(this.WriteWarning), (LocalizedString locString) => this.Force || base.ShouldContinue(locString), allowGroups, logTag, SourceValidator.Clients.NewCompliancePolicy, 0, this.executionLogger);
		}

		private SharepointValidator CreateSharepointValidator(string logTag)
		{
			return SharepointValidator.Create((IConfigurationSession)base.DataSession, base.ExchangeRunspaceConfig, new Task.TaskErrorLoggingDelegate(base.WriteError), new Action<LocalizedString>(this.WriteWarning), (LocalizedString locString) => this.Force || base.ShouldContinue(locString), logTag, SourceValidator.Clients.NewCompliancePolicy, 0, this.executionLogger);
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

		private MultiValuedProperty<string> exchangeBindingParameter;

		private MultiValuedProperty<string> sharePointBindingParameter;

		private MultiValuedProperty<string> oneDriveBindingParameter;

		protected ExecutionLog executionLogger = ExExecutionLog.CreateForCmdlet();
	}
}
