using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;
using Microsoft.Office.CompliancePolicy.Validators;
using Microsoft.SharePoint.Client;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal class Utils
	{
		internal static bool ValidateContentDateParameter(DateTime? contentDateFrom, DateTime? contentDateTo)
		{
			return contentDateFrom == null || contentDateTo == null || contentDateFrom.Value < contentDateTo.Value;
		}

		internal static void ValidateNotForestWideOrganization(OrganizationId organizationId)
		{
			if (Globals.IsMicrosoftHostedOnly && organizationId == OrganizationId.ForestWideOrgId)
			{
				throw new ForestWideOrganizationException();
			}
		}

		internal static void ThrowIfNotRunInEOP()
		{
			if (!ExPolicyConfigProvider.IsFFOOnline)
			{
				throw new ErrorOnlyAllowInEopException();
			}
		}

		internal static void ValidateWorkloadParameter(Workload workload)
		{
			if (workload != Workload.Exchange && workload != Workload.SharePoint && workload != (Workload.Exchange | Workload.SharePoint))
			{
				throw new InvalidCompliancePolicyWorkloadException();
			}
		}

		internal static void ThrowIfRulesInPolicyAreTooAdvanced(IEnumerable<RuleStorage> ruleStorages, PolicyStorage policyStorage, Task task, IConfigurationSession datasession)
		{
			foreach (PsComplianceRuleBase psComplianceRuleBase in from x in ruleStorages
			select new PsComplianceRuleBase(x))
			{
				psComplianceRuleBase.PopulateTaskProperties(task, datasession);
				if (psComplianceRuleBase.ReadOnly)
				{
					throw new RulesInPolicyIsTooAdvancedToModifyException(policyStorage.Name, psComplianceRuleBase.Name);
				}
			}
		}

		internal static void ValidateDataClassification(IEnumerable<Hashtable> hashtables)
		{
			ArgumentValidator.ThrowIfNull("hashtables", hashtables);
			foreach (Hashtable hashtable in hashtables)
			{
				IEnumerable<string> source = hashtable.Keys.Cast<string>();
				if (!source.Contains("name", StringComparer.InvariantCultureIgnoreCase))
				{
					throw new SensitiveInformationDoesNotContainIdException();
				}
				using (IEnumerator<string> enumerator2 = (from key in source
				where !PsContentContainsSensitiveInformationPredicate.CmdletParameterNameToEngineKeyMapping.Keys.Contains(key, StringComparer.InvariantCultureIgnoreCase)
				select key).GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						string invalidParameter = enumerator2.Current;
						throw new InvalidSensitiveInformationParameterNameException(invalidParameter);
					}
				}
			}
		}

		internal static void ValidateAccessScopeIsPredicate(AccessScope? accessScope)
		{
			if (accessScope != null && !Utils.SupporttedAccessScopes.Contains(accessScope.Value))
			{
				throw new InvalidAccessScopeIsPredicateException();
			}
		}

		internal static BindingStorage CreateNewBindingStorage(ADObjectId tenantId, Workload workload, Guid policyId)
		{
			string text = workload.ToString() + policyId.ToString();
			BindingStorage bindingStorage = new BindingStorage
			{
				MasterIdentity = Guid.NewGuid(),
				Name = text,
				PolicyId = policyId,
				Workload = workload
			};
			bindingStorage[ADObjectSchema.OrganizationalUnitRoot] = tenantId;
			bindingStorage.SetId(tenantId.GetDescendantId(PolicyStorage.PoliciesContainer).GetChildId(policyId.ToString()).GetChildId(text));
			return bindingStorage;
		}

		internal static void MergeBindings(MultiValuedProperty<BindingMetadata> bindings, MultiValuedProperty<BindingMetadata> addedBindings, MultiValuedProperty<BindingMetadata> removedBindings, bool forceClear)
		{
			ArgumentValidator.ThrowIfNull("bindings", bindings);
			ArgumentValidator.ThrowIfNull("addedBindings", addedBindings);
			ArgumentValidator.ThrowIfNull("removedBindings", removedBindings);
			if (forceClear && bindings.Any<BindingMetadata>())
			{
				bindings.Clear();
				return;
			}
			using (MultiValuedProperty<BindingMetadata>.Enumerator enumerator = removedBindings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BindingMetadata item = enumerator.Current;
					BindingMetadata bindingMetadata = bindings.FirstOrDefault((BindingMetadata p) => p.ImmutableIdentity == item.ImmutableIdentity);
					if (bindingMetadata != null)
					{
						bindings.Remove(bindingMetadata);
					}
				}
			}
			using (MultiValuedProperty<BindingMetadata>.Enumerator enumerator2 = addedBindings.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					BindingMetadata item = enumerator2.Current;
					BindingMetadata bindingMetadata2 = bindings.FirstOrDefault((BindingMetadata p) => p.ImmutableIdentity == item.ImmutableIdentity);
					if (bindingMetadata2 == null)
					{
						bindings.Add(item);
					}
					else if (!string.Equals(bindingMetadata2.DisplayName, item.DisplayName, StringComparison.InvariantCulture) || !string.Equals(bindingMetadata2.Name, item.Name, StringComparison.InvariantCultureIgnoreCase))
					{
						int index = bindings.IndexOf(bindingMetadata2);
						bindings[index] = item;
					}
				}
			}
		}

		internal static void PopulateScopeStorages(BindingStorage bindingStorage, MultiValuedProperty<BindingMetadata> scopes)
		{
			ArgumentValidator.ThrowIfNull("bindingStorage", bindingStorage);
			ArgumentValidator.ThrowIfNull("scopes", scopes);
			if (scopes.Changed)
			{
				object[] removed = scopes.Removed;
				for (int i = 0; i < removed.Length; i++)
				{
					BindingMetadata removedScope = (BindingMetadata)removed[i];
					ScopeStorage scopeStorage = bindingStorage.AppliedScopes.Find((ScopeStorage item) => string.Equals(BindingMetadata.FromStorage(item.Scope).ImmutableIdentity, removedScope.ImmutableIdentity, StringComparison.OrdinalIgnoreCase));
					scopeStorage.Mode = Mode.PendingDeletion;
					scopeStorage.PolicyVersion = CombGuidGenerator.NewGuid();
				}
				object[] added = scopes.Added;
				for (int j = 0; j < added.Length; j++)
				{
					BindingMetadata addedScope = (BindingMetadata)added[j];
					ScopeStorage scopeStorage2 = bindingStorage.AppliedScopes.Find((ScopeStorage item) => string.Equals(BindingMetadata.FromStorage(item.Scope).ImmutableIdentity, addedScope.ImmutableIdentity, StringComparison.OrdinalIgnoreCase));
					if (scopeStorage2 == null)
					{
						Guid objectGuid = Guid.NewGuid();
						scopeStorage2 = new ScopeStorage();
						scopeStorage2[ADObjectSchema.OrganizationalUnitRoot] = bindingStorage.OrganizationalUnitRoot;
						scopeStorage2.Name = objectGuid.ToString();
						scopeStorage2.SetId(new ADObjectId(PolicyStorage.PoliciesContainer.GetChildId(scopeStorage2.Name).DistinguishedName, objectGuid));
						bindingStorage.AppliedScopes.Add(scopeStorage2);
					}
					scopeStorage2.Mode = Mode.Enforce;
					scopeStorage2.Scope = BindingMetadata.ToStorage(addedScope);
					scopeStorage2.PolicyVersion = CombGuidGenerator.NewGuid();
				}
				bindingStorage.PolicyVersion = CombGuidGenerator.NewGuid();
				scopes.ResetChangeTracking();
			}
		}

		internal static MultiValuedProperty<BindingMetadata> GetScopesFromStorage(BindingStorage bindingStorage)
		{
			ArgumentValidator.ThrowIfNull("bindingStorage", bindingStorage);
			MultiValuedProperty<BindingMetadata> multiValuedProperty = new MultiValuedProperty<BindingMetadata>();
			if (bindingStorage.AppliedScopes.Any<ScopeStorage>())
			{
				foreach (ScopeStorage scopeStorage in bindingStorage.AppliedScopes)
				{
					if (scopeStorage.Mode == Mode.Enforce)
					{
						multiValuedProperty.TryAdd(BindingMetadata.FromStorage(scopeStorage.Scope));
					}
				}
			}
			multiValuedProperty.ResetChangeTracking();
			return multiValuedProperty;
		}

		internal static Guid GetUniversalIdentity(UnifiedPolicyStorageBase storageObject)
		{
			ArgumentValidator.ThrowIfNull("storageObject", storageObject);
			if (!(storageObject.MasterIdentity == Guid.Empty))
			{
				return storageObject.MasterIdentity;
			}
			return storageObject.Guid;
		}

		internal static IList<BindingStorage> LoadBindingStoragesByPolicy(IConfigDataProvider dataSession, UnifiedPolicyStorageBase policyStorage)
		{
			ArgumentValidator.ThrowIfNull("dataSession", dataSession);
			ArgumentValidator.ThrowIfNull("policyStorage", policyStorage);
			Guid universalIdentity = Utils.GetUniversalIdentity(policyStorage);
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ExPolicyConfigProvider.IsFFOOnline ? UnifiedPolicyStorageBaseSchema.ContainerProp : BindingStorageSchema.PolicyId, universalIdentity);
			return dataSession.Find<BindingStorage>(filter, null, false, null).Cast<BindingStorage>().ToList<BindingStorage>();
		}

		internal static ObjectId GetRootId(IConfigDataProvider dataSession)
		{
			ExPolicyConfigProvider exPolicyConfigProvider = dataSession as ExPolicyConfigProvider;
			if (exPolicyConfigProvider != null)
			{
				return exPolicyConfigProvider.GetPolicyConfigContainer(null);
			}
			return null;
		}

		internal static IList<RuleStorage> LoadRuleStoragesByPolicy(IConfigDataProvider dataSession, PolicyStorage policyStorage, ObjectId rootId)
		{
			Guid policyId = Utils.GetUniversalIdentity(policyStorage);
			return (from RuleStorage x in dataSession.Find<RuleStorage>(new ComparisonFilter(ComparisonOperator.Equal, RuleStorageSchema.ParentPolicyId, policyId), rootId, true, null)
			where x.ParentPolicyId.Equals(policyId)
			select x).ToList<RuleStorage>();
		}

		internal static IList<PolicyStorage> LoadPolicyStorages(IConfigDataProvider dataSession, PolicyScenario scenarioType)
		{
			return (from PolicyStorage s in dataSession.Find<PolicyStorage>(null, Utils.GetRootId(dataSession), false, null)
			where s.Scenario == scenarioType
			select s).ToList<PolicyStorage>();
		}

		internal static void RemovePolicyStorageBase(IConfigDataProvider dataSession, WriteVerboseDelegate writeVerboseDelegate, IEnumerable<UnifiedPolicyStorageBase> policyStorageBases)
		{
			ArgumentValidator.ThrowIfNull("dataSession", dataSession);
			ArgumentValidator.ThrowIfNull("writeVerboseDelegate", writeVerboseDelegate);
			if (policyStorageBases != null)
			{
				foreach (UnifiedPolicyStorageBase unifiedPolicyStorageBase in policyStorageBases)
				{
					writeVerboseDelegate(Strings.VerboseDeletePolicyStorageBaseObject(unifiedPolicyStorageBase.Name, unifiedPolicyStorageBase.GetType().Name));
					dataSession.Delete(unifiedPolicyStorageBase);
				}
			}
		}

		internal static bool ExecutingUserIsForestWideAdmin(Task task)
		{
			return task.ExecutingUserOrganizationId == null || task.ExecutingUserOrganizationId.Equals(OrganizationId.ForestWideOrgId);
		}

		internal static MultiValuedProperty<string> BindingParameterGetter(object bindingParameter)
		{
			if (bindingParameter != null)
			{
				return (MultiValuedProperty<string>)bindingParameter;
			}
			return MultiValuedProperty<string>.Empty;
		}

		internal static MultiValuedProperty<string> BindingParameterSetter(MultiValuedProperty<string> value)
		{
			return value ?? MultiValuedProperty<string>.Empty;
		}

		internal static ADUser GetUserObjectByExternalDirectoryObjectId(string externalDirectoryObjectId, IConfigurationSession configurationSession)
		{
			if (string.IsNullOrWhiteSpace(externalDirectoryObjectId))
			{
				return null;
			}
			if (!ExPolicyConfigProvider.IsFFOOnline)
			{
				return Utils.GetRecipientSession(configurationSession).FindADUserByExternalDirectoryObjectId(externalDirectoryObjectId);
			}
			IEnumerable<ADUser> source = UserIdParameter.Parse(externalDirectoryObjectId).GetObjects<ADUser>(null, Utils.GetRecipientSession(configurationSession)).ToArray<ADUser>();
			if (source.Count<ADUser>() == 1)
			{
				return source.First<ADUser>();
			}
			return null;
		}

		internal static ADUser GetUserObjectByObjectId(ADObjectId objectId, IConfigurationSession configurationSession)
		{
			if (objectId == null)
			{
				return null;
			}
			return Utils.GetRecipientSession(configurationSession).FindADUserByObjectId(objectId);
		}

		internal static IRecipientSession GetRecipientSession(IConfigurationSession configurationSession)
		{
			OrganizationId organizationId = configurationSession.GetOrgContainer().OrganizationId;
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromExternalDirectoryOrganizationId(new Guid(organizationId.ToExternalDirectoryOrganizationId())), 803, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\UnifiedPolicy\\Utils.cs");
		}

		internal static string ConvertObjectIdentityInFfo(string identity)
		{
			if (!string.IsNullOrWhiteSpace(identity) && ExPolicyConfigProvider.IsFFOOnline)
			{
				string[] array = identity.Split(new string[]
				{
					string.Format("/{0}/", "Microsoft Exchange Hosted Organizations"),
					"/Configuration/"
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Count<string>() == 3)
				{
					return string.Format("{0}\\{1}", Regex.Unescape(array[1]), Regex.Unescape(array[2]));
				}
			}
			return identity;
		}

		internal static void RedactBinding(BindingMetadata binding, bool redactImmutableId)
		{
			binding.DisplayName = SuppressingPiiData.Redact(binding.DisplayName);
			binding.Name = SuppressingPiiData.Redact(binding.Name);
			if (redactImmutableId)
			{
				binding.ImmutableIdentity = SuppressingPiiData.Redact(binding.ImmutableIdentity);
			}
		}

		internal static PiiMap GetSessionPiiMap(ExchangeRunspaceConfiguration config)
		{
			if (config != null && config.PiiMapId != null)
			{
				return PiiMapManager.Instance.GetOrAdd(config.PiiMapId);
			}
			return null;
		}

		internal static void WrapSharePointCsomCall(Uri siteUrl, ICredentials credentials, Action<ClientContext> doWork)
		{
			try
			{
				using (ClientContext clientContext = new ClientContext(siteUrl))
				{
					clientContext.Credentials = credentials;
					clientContext.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs args)
					{
						args.WebRequestExecutor.RequestHeaders.Add(HttpRequestHeader.Authorization, "Bearer");
						args.WebRequestExecutor.WebRequest.PreAuthenticate = true;
					};
					doWork(clientContext);
				}
			}
			catch (CommunicationException ex)
			{
				throw new SpCsomCallException(Strings.ErrorSharePointCallFailed(ex.Message), ex);
			}
			catch (ClientRequestException ex2)
			{
				throw new SpCsomCallException(Strings.ErrorSharePointCallFailed(ex2.Message), ex2);
			}
			catch (ServerException ex3)
			{
				throw new SpCsomCallException(Strings.ErrorSharePointCallFailed(ex3.Message), ex3);
			}
			catch (TimeoutException ex4)
			{
				throw new SpCsomCallException(Strings.ErrorSharePointCallFailed(ex4.Message), ex4);
			}
			catch (IOException ex5)
			{
				throw new SpCsomCallException(Strings.ErrorSharePointCallFailed(ex5.Message), ex5);
			}
			catch (WebException ex6)
			{
				throw new SpCsomCallException(Strings.ErrorSharePointCallFailed(ex6.Message), ex6);
			}
			catch (FormatException ex7)
			{
				throw new SpCsomCallException(Strings.ErrorSharePointCallFailed(ex7.Message), ex7);
			}
		}

		internal static bool TryGetMockCsomProvider(out ISharepointCsomProvider mockCsomProvider)
		{
			mockCsomProvider = null;
			string stringFromConfig = Utils.GetStringFromConfig("MockCsomProviderType");
			string stringFromConfig2 = Utils.GetStringFromConfig("MockCsomProviderAssemblyPath");
			if (!string.IsNullOrWhiteSpace(stringFromConfig) && File.Exists(stringFromConfig2))
			{
				string stringFromConfig3 = Utils.GetStringFromConfig("MockSharepointSites");
				Assembly assembly = Assembly.LoadFrom(stringFromConfig2);
				mockCsomProvider = (ISharepointCsomProvider)Activator.CreateInstance(assembly.GetType(stringFromConfig, true), new object[]
				{
					stringFromConfig3
				});
				return true;
			}
			return false;
		}

		internal static bool GetBoolFromConfig(string key, bool defaultValue = false)
		{
			bool result;
			if (!bool.TryParse(Utils.GetStringFromConfig(key), out result))
			{
				result = defaultValue;
			}
			return result;
		}

		internal static int GetIntFromConfig(string key, int defaultValue)
		{
			int result;
			if (!int.TryParse(Utils.GetStringFromConfig(key), out result))
			{
				result = defaultValue;
			}
			return result;
		}

		internal static string GetStringFromConfig(string key)
		{
			string result = null;
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
			if (configuration != null)
			{
				KeyValueConfigurationElement keyValueConfigurationElement = configuration.AppSettings.Settings[key];
				if (keyValueConfigurationElement != null && !string.IsNullOrEmpty(keyValueConfigurationElement.Value))
				{
					result = keyValueConfigurationElement.Value;
				}
			}
			return result;
		}

		internal const string NameParameterName = "Name";

		internal const string PolicyParameterName = "Policy";

		internal const string AddExchangeBindingParameterName = "AddExchangeBinding";

		internal const string RemoveExchangeBindingParameterName = "RemoveExchangeBinding";

		internal const string AddSharePointBindingParameterName = "AddSharePointBinding";

		internal const string RemoveSharePointBindingParameterName = "RemoveSharePointBinding";

		internal const string AddOneDriveBindingParameterName = "AddOneDriveBinding";

		internal const string RemoveOneDriveBindingParameterName = "RemoveOneDriveBinding";

		internal const string HoldContentParameterName = "HoldContent";

		internal const string ContentDateFromParameterName = "ContentDateFrom";

		internal const string ContentDateToParameterName = "ContentDateTo";

		internal const string ContentMatchQueryParameterName = "ContentMatchQuery";

		internal const string CommentParameterName = "Comment";

		internal const string EnabledParameterName = "Enabled";

		internal const string PasswordRequiredName = "PasswordRequired";

		internal const string ForceDeletionParameterName = "ForceDeletion";

		internal const string AuditOperationParameterName = "AuditOperation";

		internal const string TargetGroupsParameterName = "TargetGroups";

		internal const string ExclusionListParameterName = "ExclusionList";

		internal const string ForceParameterName = "Force";

		internal const string FullSyncParameterName = "FullSync";

		internal const string RetryDistributionParameterName = "RetryDistribution";

		internal const string UpdateObjectIdParameterName = "UpdateObjectId";

		internal const string DeleteObjectIdParameterName = "DeleteObjectId";

		internal const string ObjectTypeParameterName = "ObjectType";

		internal const string WorkloadParameterName = "Workload";

		internal const string PolicyCenterSiteOwnerParameterName = "PolicyCenterSiteOwner";

		internal const string LoadOnlyParameterName = "LoadOnly";

		internal const string ForceInitializeParameterName = "ForceInitialize";

		internal const string LoadOnlyParameterSetName = "LoadOnly";

		internal const string InitializeParameterSetName = "Initialize";

		internal const string DisabledParameterName = "Disabled";

		internal const string HoldDurationDisplayHintParameterName = "HoldDurationDisplayHint";

		internal const string ContentContainsSensitiveInformationParameterName = "ContentContainsSensitiveInformation";

		internal const string ContentPropertyContainsWordsParameterName = "ContentPropertyContainsWords";

		internal const string AccessScopeIsParameterName = "AccessScopeIs";

		internal const string BlockAccessParameterName = "BlockAccess";

		internal const string MockCsomProviderTypeKey = "MockCsomProviderType";

		internal const string MockCsomProviderAssemblyPathKey = "MockCsomProviderAssemblyPath";

		internal const string MockSharepointSitesKey = "MockSharepointSites";

		internal const int CompliancePolicyCountLimit = 1000;

		internal const int ExBindingCountLimit = 1000;

		internal const int SpBindingCountLimit = 100;

		internal static readonly string[] MockSharepointSitesSeparator = new string[]
		{
			"|"
		};

		internal static readonly IEnumerable<Type> KnownExceptions = new Type[]
		{
			typeof(CompliancePolicyAlreadyExistsException),
			typeof(ComplianceRuleAlreadyExistsException),
			typeof(PolicyAndIdentityParameterUsedTogetherException),
			typeof(CompliancePolicyCountExceedsLimitException),
			typeof(InvalidCompliancePolicyWorkloadException),
			typeof(InvalidCompliancePolicyBindingException),
			typeof(MulipleComplianceRulesFoundInPolicyException),
			typeof(InvalidComplianceRulePredicateException),
			typeof(InvalidComplianceRuleActionException),
			typeof(BindingCannotCombineAllWithIndividualBindingsException),
			typeof(ForestWideOrganizationException),
			typeof(TroubleshootingCmdletException),
			typeof(SpCsomCallException),
			typeof(ErrorOnlyAllowInEopException),
			typeof(ErrorInvalidPolicyCenterSiteOwnerException),
			typeof(TaskObjectIsTooAdvancedException),
			typeof(StoragePermanentException),
			typeof(StorageTransientException),
			typeof(SensitiveInformationCmdletException)
		};

		internal static readonly IEnumerable<AccessScope> SupporttedAccessScopes = new AccessScope[]
		{
			AccessScope.Internal,
			AccessScope.Internal | AccessScope.External
		};

		public static class BindingParameters
		{
			internal const string All = "All";
		}

		public static class WorkloadNames
		{
			internal const string Exchange = "Exchange";

			internal const string Sharepoint = "Sharepoint";

			internal const string OneDriveBusiness = "OneDriveBusiness";
		}
	}
}
