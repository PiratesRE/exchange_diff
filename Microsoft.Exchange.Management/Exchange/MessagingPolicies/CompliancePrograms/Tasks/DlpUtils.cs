using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal static class DlpUtils
	{
		public static Version MaxSupportedVersion
		{
			get
			{
				return new Version("15.0.3.2");
			}
		}

		public static IEnumerable<ADComplianceProgram> GetInstalledTenantDlpPolicies(IConfigDataProvider dataSession, string name)
		{
			Guid guid;
			if (Guid.TryParse(name, out guid))
			{
				IList<ADComplianceProgram> list = (from x in DlpUtils.GetDlpPolicies(dataSession, DlpUtils.TenantDlpPoliciesCollectionName, null)
				where x.ImmutableId.Equals(guid)
				select x).ToList<ADComplianceProgram>();
				if (!list.Any<ADComplianceProgram>())
				{
					list = DlpUtils.GetDlpPolicies(dataSession, DlpUtils.TenantDlpPoliciesCollectionName, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid)).ToList<ADComplianceProgram>();
				}
				if (list.Any<ADComplianceProgram>())
				{
					return list;
				}
			}
			return DlpUtils.GetDlpPolicies(dataSession, DlpUtils.TenantDlpPoliciesCollectionName, new TextFilter(ADObjectSchema.Name, name, MatchOptions.FullString, MatchFlags.Default));
		}

		public static IEnumerable<ADComplianceProgram> GetInstalledTenantDlpPolicies(IConfigDataProvider dataSession)
		{
			return DlpUtils.GetDlpPolicies(dataSession, DlpUtils.TenantDlpPoliciesCollectionName, null);
		}

		public static IEnumerable<ADComplianceProgram> GetOutOfBoxDlpTemplates(IConfigDataProvider dataSession, string name)
		{
			Guid guid;
			QueryFilter filter;
			if (Guid.TryParse(name, out guid))
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid);
			}
			else
			{
				filter = new TextFilter(ADObjectSchema.Name, name, MatchOptions.FullString, MatchFlags.Default);
			}
			return DlpUtils.GetDlpPolicies(dataSession, DlpUtils.OutOfBoxDlpPoliciesCollectionName, filter);
		}

		public static IEnumerable<ADComplianceProgram> GetOutOfBoxDlpTemplates(IConfigDataProvider dataSession)
		{
			return DlpUtils.GetDlpPolicies(dataSession, DlpUtils.OutOfBoxDlpPoliciesCollectionName, null);
		}

		public static void SaveOutOfBoxDlpTemplates(IConfigDataProvider dataSession, IEnumerable<DlpPolicyTemplateMetaData> dlpTemplates)
		{
			ADComplianceProgramCollection dlpPolicyCollection = DlpUtils.GetDlpPolicyCollection(dataSession, DlpUtils.OutOfBoxDlpPoliciesCollectionName);
			foreach (DlpPolicyTemplateMetaData dlpPolicyTemplateMetaData in dlpTemplates)
			{
				ADComplianceProgram adcomplianceProgram = dlpPolicyTemplateMetaData.ToAdObject();
				adcomplianceProgram.OrganizationId = dlpPolicyCollection.OrganizationId;
				adcomplianceProgram.SetId(dlpPolicyCollection.Id.GetChildId(dlpPolicyTemplateMetaData.Name));
				dataSession.Save(adcomplianceProgram);
			}
		}

		public static void DeleteOutOfBoxDlpPolicies(IConfigDataProvider dataSession)
		{
			List<ADComplianceProgram> list = DlpUtils.GetOutOfBoxDlpTemplates(dataSession).ToList<ADComplianceProgram>();
			list.ForEach(new Action<ADComplianceProgram>(dataSession.Delete));
		}

		public static void DeleteOutOfBoxDlpPolicy(IConfigDataProvider dataSession, string templateName)
		{
			ADComplianceProgram instance = DlpUtils.GetOutOfBoxDlpTemplates(dataSession, templateName).FirstOrDefault<ADComplianceProgram>();
			dataSession.Delete(instance);
		}

		public static void AddTenantDlpPolicy(IConfigDataProvider dataSession, DlpPolicyMetaData dlpPolicy, string organizationParameterValue, CmdletRunner cmdletRunner, out IEnumerable<PSObject> results)
		{
			results = null;
			ADComplianceProgram adcomplianceProgram = dlpPolicy.ToAdObject();
			ADComplianceProgramCollection dlpPolicyCollection = DlpUtils.GetDlpPolicyCollection(dataSession, DlpUtils.TenantDlpPoliciesCollectionName);
			adcomplianceProgram.OrganizationId = dlpPolicyCollection.OrganizationId;
			adcomplianceProgram.SetId(dlpPolicyCollection.Id.GetChildId(dlpPolicy.Name));
			dataSession.Save(adcomplianceProgram);
			IEnumerable<string> enumerable = Utils.AddOrganizationScopeToCmdlets(dlpPolicy.PolicyCommands, organizationParameterValue);
			string domainController = null;
			ADSessionSettings sessionSettings = null;
			MessagingPoliciesSyncLogDataSession messagingPoliciesSyncLogDataSession = dataSession as MessagingPoliciesSyncLogDataSession;
			if (messagingPoliciesSyncLogDataSession != null)
			{
				domainController = messagingPoliciesSyncLogDataSession.LastUsedDc;
				sessionSettings = messagingPoliciesSyncLogDataSession.SessionSettings;
			}
			try
			{
				foreach (string cmdlet in enumerable)
				{
					cmdletRunner.RunCmdlet(cmdlet, true);
				}
			}
			catch (ParseException e)
			{
				DlpUtils.HandleScriptExecutionError(adcomplianceProgram, DlpUtils.GetErrorHandlingDataSession(domainController, sessionSettings, dataSession), e);
			}
			catch (RuntimeException e2)
			{
				DlpUtils.HandleScriptExecutionError(adcomplianceProgram, DlpUtils.GetErrorHandlingDataSession(domainController, sessionSettings, dataSession), e2);
			}
			catch (CmdletExecutionException e3)
			{
				DlpUtils.HandleScriptExecutionError(adcomplianceProgram, DlpUtils.GetErrorHandlingDataSession(domainController, sessionSettings, dataSession), e3);
			}
		}

		private static void HandleScriptExecutionError(ADComplianceProgram adDlpPolicy, IConfigDataProvider dataSession, Exception e)
		{
			DlpUtils.DeleteEtrsByDlpPolicy(adDlpPolicy.ImmutableId, dataSession);
			dataSession.Delete(adDlpPolicy);
			throw new DlpPolicyScriptExecutionException(e.Message);
		}

		private static IConfigDataProvider GetErrorHandlingDataSession(string domainController, ADSessionSettings sessionSettings, IConfigDataProvider dataSession)
		{
			if (!string.IsNullOrEmpty(domainController) && sessionSettings != null)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainController, false, ConsistencyMode.IgnoreInvalid, sessionSettings, 321, "GetErrorHandlingDataSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\DlpPolicy\\Utils.cs");
				return new MessagingPoliciesSyncLogDataSession(tenantOrTopologyConfigurationSession, null, null);
			}
			return dataSession;
		}

		public static bool TryGetTransportRules(IConfigDataProvider dataSession, out IEnumerable<TransportRule> rules, out string error)
		{
			string text = Utils.RuleCollectionNameFromRole();
			QueryFilter filter = new TextFilter(ADObjectSchema.Name, text, MatchOptions.FullString, MatchFlags.Default);
			TransportRuleCollection[] array = (TransportRuleCollection[])dataSession.Find<TransportRuleCollection>(filter, null, true, null);
			if (array.Any<TransportRuleCollection>())
			{
				rules = dataSession.FindPaged<TransportRule>(null, array[0].Id, false, null, 0);
				error = null;
				return true;
			}
			rules = null;
			error = string.Format("Unable to find rule collection {0}. Tenant is not provisioned for Exchange Transport Rules.", text);
			return false;
		}

		internal static IEnumerable<Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule> GetTransportRules(IConfigDataProvider dataSession, Func<Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule, bool> selector)
		{
			ADRuleStorageManager adruleStorageManager;
			IEnumerable<TransportRuleHandle> transportRuleHandles = DlpUtils.GetTransportRuleHandles(dataSession, out adruleStorageManager);
			IEnumerable<Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule> source = from ruleHandle in transportRuleHandles
			select Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule.CreateFromInternalRule(TransportRulePredicate.GetAvailablePredicateMappings(), TransportRuleAction.GetAvailableActionMappings(), ruleHandle.Rule, ruleHandle.AdRule.Priority, ruleHandle.AdRule);
			return source.Where(selector);
		}

		public static List<string> GetEtrsForDlpPolicy(Guid dlpGuid, IConfigDataProvider dataSession)
		{
			ADRuleStorageManager adruleStorageManager;
			IEnumerable<TransportRuleHandle> transportRuleHandles = DlpUtils.GetTransportRuleHandles(dataSession, out adruleStorageManager);
			IEnumerable<Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule> source = from ruleHandle in transportRuleHandles
			select Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule.CreateFromInternalRule(TransportRulePredicate.GetAvailablePredicateMappings(), TransportRuleAction.GetAvailableActionMappings(), ruleHandle.Rule, ruleHandle.AdRule.Priority, ruleHandle.AdRule);
			return (from rule in source
			where rule.DlpPolicyId == dlpGuid
			select rule.ToCmdlet()).ToList<string>();
		}

		internal static IEnumerable<TransportRuleHandle> GetTransportRuleHandles(IConfigDataProvider dataSession, out ADRuleStorageManager ruleStorageManager)
		{
			ruleStorageManager = new ADRuleStorageManager(Utils.RuleCollectionNameFromRole(), dataSession);
			ruleStorageManager.LoadRuleCollection();
			return ruleStorageManager.GetRuleHandles();
		}

		internal static TransportRule GetTransportRuleByName(IConfigDataProvider dataSession, string collectionName, string ruleName)
		{
			return DlpUtils.GetTransportRuleUnParsed(dataSession, collectionName, new TextFilter(ADObjectSchema.Name, ruleName, MatchOptions.FullString, MatchFlags.Default));
		}

		internal static TransportRule GetTransportRuleByGuid(IConfigDataProvider dataSession, string collectionName, Guid objectGuid)
		{
			return DlpUtils.GetTransportRuleUnParsed(dataSession, collectionName, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, objectGuid));
		}

		internal static ADComplianceProgram GetDlpPolicyByName(IConfigDataProvider dataSession, string collectionName, string name)
		{
			QueryFilter filter = new TextFilter(ADObjectSchema.Name, name, MatchOptions.FullString, MatchFlags.Default);
			return DlpUtils.GetDlpPolicies(dataSession, collectionName, filter).FirstOrDefault<ADComplianceProgram>();
		}

		public static void DeleteEtrsByDlpPolicy(Guid dlpGuid, IConfigDataProvider dataSession)
		{
			IEnumerable<TransportRule> enumerable;
			string message;
			if (!DlpUtils.TryGetTransportRules(dataSession, out enumerable, out message))
			{
				throw new InvalidOperationException(message);
			}
			foreach (TransportRule transportRule in enumerable)
			{
				TransportRule transportRule2 = (TransportRule)TransportRuleParser.Instance.GetRule(transportRule.Xml);
				Guid guid;
				if (transportRule2.TryGetDlpPolicyId(out guid) && guid.Equals(dlpGuid))
				{
					dataSession.Delete(transportRule);
				}
			}
		}

		private static IEnumerable<ADComplianceProgram> GetDlpPolicies(IConfigDataProvider dataSession, string collectionName, QueryFilter filter)
		{
			ADComplianceProgramCollection dlpPolicyCollection = DlpUtils.GetDlpPolicyCollection(dataSession, collectionName);
			return dataSession.FindPaged<ADComplianceProgram>(filter, dlpPolicyCollection.Id, false, null, 0);
		}

		private static ADComplianceProgramCollection GetDlpPolicyCollection(IConfigDataProvider dataSession, string collectionName)
		{
			QueryFilter filter = new TextFilter(ADObjectSchema.Name, collectionName, MatchOptions.FullString, MatchFlags.Default);
			ADComplianceProgramCollection[] array = (ADComplianceProgramCollection[])dataSession.Find<ADComplianceProgramCollection>(filter, null, true, null);
			if (array.Length != 1)
			{
				throw new DlpPolicyInvalidCollectionException();
			}
			return array[0];
		}

		private static TransportRule GetTransportRuleUnParsed(IConfigDataProvider dataSession, string collectionName, QueryFilter filter)
		{
			ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager(collectionName, dataSession);
			adruleStorageManager.LoadRuleCollectionWithoutParsing(filter);
			TransportRule result = null;
			if (adruleStorageManager.Count > 0)
			{
				adruleStorageManager.GetRuleWithoutParsing(0, out result);
			}
			return result;
		}

		internal static IConfigDataProvider CreateOrgSession(string domainController)
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 552, "CreateOrgSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\DlpPolicy\\Utils.cs");
		}

		internal static IEnumerable<DlpPolicyTemplateMetaData> LoadDlpPolicyTemplates(byte[] data)
		{
			IEnumerable<DlpPolicyTemplateMetaData> result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				result = DlpPolicyParser.ParseDlpPolicyTemplates(memoryStream);
			}
			return result;
		}

		internal static IEnumerable<DlpPolicyMetaData> LoadDlpPolicyInstances(byte[] data)
		{
			IEnumerable<DlpPolicyMetaData> result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				result = DlpPolicyParser.ParseDlpPolicyInstances(memoryStream);
			}
			return result;
		}

		internal static DlpPolicyTemplateMetaData LoadOutOfBoxDlpTemplate(Fqdn domainController, string templateName)
		{
			IConfigDataProvider dataSession = DlpUtils.CreateOrgSession(domainController);
			ADComplianceProgram adcomplianceProgram = DlpUtils.GetOutOfBoxDlpTemplates(dataSession, templateName).FirstOrDefault<ADComplianceProgram>();
			if (adcomplianceProgram != null)
			{
				return DlpPolicyParser.ParseDlpPolicyTemplate(adcomplianceProgram.TransportRulesXml);
			}
			return null;
		}

		internal static Tuple<RuleState, RuleMode> DlpStateToRuleState(DlpPolicyState state)
		{
			switch (state)
			{
			case DlpPolicyState.Disabled_Audit:
				return new Tuple<RuleState, RuleMode>(RuleState.Disabled, RuleMode.Audit);
			case DlpPolicyState.Disabled_AuditAndNotify:
				return new Tuple<RuleState, RuleMode>(RuleState.Disabled, RuleMode.AuditAndNotify);
			case DlpPolicyState.Disabled_Enforce:
				return new Tuple<RuleState, RuleMode>(RuleState.Disabled, RuleMode.Enforce);
			case DlpPolicyState.Enabled_Audit:
				return new Tuple<RuleState, RuleMode>(RuleState.Enabled, RuleMode.Audit);
			case DlpPolicyState.Enabled_AuditAndNotify:
				return new Tuple<RuleState, RuleMode>(RuleState.Enabled, RuleMode.AuditAndNotify);
			case DlpPolicyState.Enabled_Enforce:
				return new Tuple<RuleState, RuleMode>(RuleState.Enabled, RuleMode.Enforce);
			default:
				return new Tuple<RuleState, RuleMode>(RuleState.Disabled, RuleMode.Enforce);
			}
		}

		internal static DlpPolicyState RuleStateToDlpState(RuleState state, RuleMode mode)
		{
			if (state == RuleState.Disabled && mode == RuleMode.Audit)
			{
				return DlpPolicyState.Disabled_Audit;
			}
			if (state == RuleState.Disabled && mode == RuleMode.AuditAndNotify)
			{
				return DlpPolicyState.Disabled_AuditAndNotify;
			}
			if (state == RuleState.Disabled && mode == RuleMode.Enforce)
			{
				return DlpPolicyState.Disabled_Enforce;
			}
			if (state == RuleState.Enabled && mode == RuleMode.Audit)
			{
				return DlpPolicyState.Enabled_Audit;
			}
			if (state == RuleState.Enabled && mode == RuleMode.AuditAndNotify)
			{
				return DlpPolicyState.Enabled_AuditAndNotify;
			}
			if (state == RuleState.Enabled && mode == RuleMode.Enforce)
			{
				return DlpPolicyState.Enabled_Enforce;
			}
			return DlpPolicyState.Disabled_Audit;
		}

		internal static ILookup<string, Microsoft.Exchange.MessagingPolicies.Rules.Rule> GetDataClassificationsInUse(IConfigDataProvider tenantSession, IEnumerable<string> dataClassificationIds, IEqualityComparer<string> dataClassificationIdComparer = null)
		{
			ArgumentValidator.ThrowIfNull("tenantSession", tenantSession);
			ArgumentValidator.ThrowIfNull("dataClassificationIds", dataClassificationIds);
			if (!dataClassificationIds.Any<string>())
			{
				return Enumerable.Empty<Microsoft.Exchange.MessagingPolicies.Rules.Rule>().ToLookup((Microsoft.Exchange.MessagingPolicies.Rules.Rule rule) => null);
			}
			ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager(Utils.RuleCollectionNameFromRole(), tenantSession);
			adruleStorageManager.LoadRuleCollection();
			return DlpUtils.GetDataClassificationsReferencedByRuleCollection(adruleStorageManager.GetRuleCollection(), dataClassificationIds, dataClassificationIdComparer);
		}

		internal static ILookup<string, Microsoft.Exchange.MessagingPolicies.Rules.Rule> GetDataClassificationsReferencedByRuleCollection(IEnumerable<Microsoft.Exchange.MessagingPolicies.Rules.Rule> ruleCollection, IEnumerable<string> dataClassificationIds, IEqualityComparer<string> dataClassificationIdComparer = null)
		{
			ArgumentValidator.ThrowIfNull("dataClassificationIds", dataClassificationIds);
			if (!dataClassificationIds.Any<string>() || ruleCollection == null || !ruleCollection.Any<Microsoft.Exchange.MessagingPolicies.Rules.Rule>())
			{
				return Enumerable.Empty<Microsoft.Exchange.MessagingPolicies.Rules.Rule>().ToLookup((Microsoft.Exchange.MessagingPolicies.Rules.Rule rule) => null);
			}
			HashSet<string> dataClassificationIdsSet = (dataClassificationIdComparer != null) ? new HashSet<string>(dataClassificationIds, dataClassificationIdComparer) : new HashSet<string>(dataClassificationIds);
			return (from dataClassificationIdAndTransportRuleAssociation in ruleCollection.SelectMany(delegate(Microsoft.Exchange.MessagingPolicies.Rules.Rule transportRule)
			{
				SupplementalData supplementalData = new SupplementalData();
				transportRule.GetSupplementalData(supplementalData);
				Dictionary<string, string> dictionary = supplementalData.Get("DataClassification");
				return dictionary.Keys;
			}, (Microsoft.Exchange.MessagingPolicies.Rules.Rule transportRule, string dataClassificationId) => new
			{
				ReferencedDataClassificationId = dataClassificationId,
				ReferencingRule = transportRule
			})
			where dataClassificationIdsSet.Contains(dataClassificationIdAndTransportRuleAssociation.ReferencedDataClassificationId)
			select dataClassificationIdAndTransportRuleAssociation).ToLookup(dataClassificationIdAndTransportRuleAssociation => dataClassificationIdAndTransportRuleAssociation.ReferencedDataClassificationId, dataClassificationIdAndTransportRuleAssociation => dataClassificationIdAndTransportRuleAssociation.ReferencingRule);
		}

		public static string OutOfBoxDlpPoliciesCollectionName = "MailflowPolicyTemplatesRtm";

		public static string TenantDlpPoliciesCollectionName = "InstalledMailflowPoliciesRtm";
	}
}
