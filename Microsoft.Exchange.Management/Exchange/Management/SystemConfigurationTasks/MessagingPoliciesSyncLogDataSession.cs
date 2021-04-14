using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class MessagingPoliciesSyncLogDataSession : IConfigDataProvider
	{
		public MessagingPoliciesSyncLogDataSession(IConfigDataProvider dataSession, string rulesCollectionName = null, string policiesCollectionName = null)
		{
			if (dataSession == null)
			{
				throw new ArgumentNullException("dataSession");
			}
			this.dataSession = dataSession;
			this.rulesCollectionName = (rulesCollectionName ?? Utils.RuleCollectionNameFromRole());
			this.policiesCollectionName = (policiesCollectionName ?? DlpUtils.TenantDlpPoliciesCollectionName);
		}

		internal string LastUsedDc
		{
			get
			{
				IConfigurationSession configurationSession = this.dataSession as IConfigurationSession;
				if (configurationSession == null)
				{
					return null;
				}
				return configurationSession.LastUsedDc;
			}
		}

		internal ADSessionSettings SessionSettings
		{
			get
			{
				IConfigurationSession configurationSession = this.dataSession as IConfigurationSession;
				if (configurationSession == null)
				{
					return null;
				}
				return configurationSession.SessionSettings;
			}
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			return this.dataSession.Read<T>(identity);
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			return this.dataSession.Find<T>(filter, rootId, deepSearch, sortBy);
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			return this.dataSession.FindPaged<T>(filter, rootId, deepSearch, sortBy, pageSize);
		}

		void IConfigDataProvider.Save(IConfigurable instance)
		{
			if (!TenantSettingSyncLogGenerator.Instance.Enabled)
			{
				this.dataSession.Save(instance);
				return;
			}
			if (instance is TransportRule)
			{
				this.SaveTransportRule((TransportRule)instance);
				return;
			}
			if (instance is ADComplianceProgram)
			{
				this.SaveDlpPolicy((ADComplianceProgram)instance);
				return;
			}
			this.dataSession.Save(instance);
		}

		void IConfigDataProvider.Delete(IConfigurable instance)
		{
			this.dataSession.Delete(instance);
			Guid value;
			if (TenantSettingSyncLogGenerator.Instance.Enabled && (instance is TransportRule || instance is ADComplianceProgram) && this.GetExternalDirectoryOrganizationIdToLog((ADObject)instance, out value))
			{
				TenantSettingSyncLogGenerator.Instance.LogChangesForDelete((ADObject)instance, new Guid?(value));
			}
		}

		string IConfigDataProvider.Source
		{
			get
			{
				return this.dataSession.Source;
			}
		}

		private void SaveTransportRule(TransportRule instance)
		{
			bool flag = MessagingPoliciesSyncLogDataSession.IsNameNewOrChangedForTenantScopedObject(instance);
			Guid empty = Guid.Empty;
			bool flag2 = false;
			if (instance.OrganizationId != OrganizationId.ForestWideOrgId && instance.IsChanged(TransportRuleSchema.Xml))
			{
				flag2 = true;
				if (!instance.Guid.Equals(Guid.Empty))
				{
					TransportRule transportRuleByGuid = DlpUtils.GetTransportRuleByGuid(this.dataSession, this.rulesCollectionName, instance.Guid);
					TransportRule transportRule = (TransportRule)TransportRuleParser.Instance.GetRule(transportRuleByGuid.Xml);
					transportRule.TryGetDlpPolicyId(out empty);
				}
			}
			this.dataSession.Save(instance);
			if (flag || flag2)
			{
				TransportRule transportRuleByName = DlpUtils.GetTransportRuleByName(this.dataSession, this.rulesCollectionName, instance.Name);
				Guid empty2 = Guid.Empty;
				List<KeyValuePair<string, object>> list = null;
				if (transportRuleByName != null)
				{
					TransportRule transportRule2 = (TransportRule)TransportRuleParser.Instance.GetRule(transportRuleByName.Xml);
					transportRule2.TryGetDlpPolicyId(out empty2);
					if (!empty2.Equals(empty))
					{
						flag = true;
						list = new List<KeyValuePair<string, object>>();
						list.Add(new KeyValuePair<string, object>("DLPPolicyId", empty2));
					}
				}
				Guid value;
				if (flag && this.GetExternalDirectoryOrganizationIdToLog(transportRuleByName, out value))
				{
					TenantSettingSyncLogGenerator.Instance.LogChangesForSave(transportRuleByName, new Guid?(value), new Guid?(transportRuleByName.ImmutableId), list);
				}
			}
		}

		private void SaveDlpPolicy(ADComplianceProgram instance)
		{
			bool flag = MessagingPoliciesSyncLogDataSession.IsNameNewOrChangedForTenantScopedObject(instance);
			this.dataSession.Save(instance);
			if (flag)
			{
				ADComplianceProgram dlpPolicyByName = DlpUtils.GetDlpPolicyByName(this.dataSession, this.policiesCollectionName, instance.Name);
				Guid value;
				if (this.GetExternalDirectoryOrganizationIdToLog(dlpPolicyByName, out value))
				{
					TenantSettingSyncLogGenerator.Instance.LogChangesForSave(dlpPolicyByName, new Guid?(value), new Guid?(dlpPolicyByName.ImmutableId), null);
				}
			}
		}

		private static bool IsNameNewOrChangedForTenantScopedObject(ADObject instance)
		{
			return instance.OrganizationId != OrganizationId.ForestWideOrgId && (instance.IsChanged(ADObjectSchema.Id) || instance.IsChanged(ADObjectSchema.RawName));
		}

		private bool GetExternalDirectoryOrganizationIdToLog(ADObject instance, out Guid externalDirectoryOrganizationId)
		{
			externalDirectoryOrganizationId = Guid.Empty;
			if (instance != null && instance.OrganizationId != OrganizationId.ForestWideOrgId)
			{
				ADOperationResult externalDirectoryOrganizationId2 = SystemConfigurationTasksHelper.GetExternalDirectoryOrganizationId(this.dataSession, instance.OrganizationId, out externalDirectoryOrganizationId);
				TenantSettingSyncLogGenerator.Instance.AddEventLogOnADException(externalDirectoryOrganizationId2);
				return externalDirectoryOrganizationId2.Succeeded && externalDirectoryOrganizationId != Guid.Empty;
			}
			return false;
		}

		private IConfigDataProvider dataSession;

		private readonly string rulesCollectionName;

		private readonly string policiesCollectionName;
	}
}
