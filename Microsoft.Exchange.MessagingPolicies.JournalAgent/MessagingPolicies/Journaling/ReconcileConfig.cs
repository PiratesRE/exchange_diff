using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class ReconcileConfig
	{
		public static ReconcileConfig GetInstance(OrganizationId organizationId)
		{
			if (organizationId != OrganizationId.ForestWideOrgId)
			{
				ReconcileConfig reconcileConfig = new ReconcileConfig();
				reconcileConfig.InitializeTenantConfiguration(organizationId);
				return reconcileConfig;
			}
			if (ReconcileConfig.instance == null || ReconcileConfig.instance.IsOutdated())
			{
				lock (ReconcileConfig.lockObject)
				{
					if (ReconcileConfig.instance == null || ReconcileConfig.instance.IsOutdated())
					{
						ReconcileConfig reconcileConfig2 = new ReconcileConfig();
						if (reconcileConfig2.InitializeEnterpriseConfiguration())
						{
							ReconcileConfig.instance = reconcileConfig2;
						}
						else
						{
							ExTraceGlobals.JournalingTracer.TraceError(0L, "Reconciliation Configuration could not be loaded");
						}
					}
				}
			}
			return ReconcileConfig.instance;
		}

		public Dictionary<string, ReconciliationAccountConfig> ReconciliationAccounts
		{
			get
			{
				return this.reconciliationAccounts;
			}
		}

		public bool IsReconcileMailbox(string mailboxSmtpAddress)
		{
			return this.mailboxesDictionary.ContainsKey(mailboxSmtpAddress);
		}

		private bool IsOutdated()
		{
			return this.validUntil < DateTime.UtcNow;
		}

		private bool InitializeEnterpriseConfiguration()
		{
			Exception ex = null;
			try
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId.ForestWideOrgId), 331, "InitializeEnterpriseConfiguration", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\Journaling\\Agent\\ReconcileConfig.cs");
				ADPagedReader<JournalingReconciliationAccount> adpagedReader = tenantOrTopologyConfigurationSession.FindAllPaged<JournalingReconciliationAccount>();
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 338, "InitializeEnterpriseConfiguration", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\Journaling\\Agent\\ReconcileConfig.cs");
				foreach (JournalingReconciliationAccount journalingReconciliationAccount in adpagedReader)
				{
					ReconciliationAccountConfig reconciliationAccountConfig = ReconciliationAccountConfig.Create(tenantOrRootOrgRecipientSession, journalingReconciliationAccount);
					this.reconciliationAccounts[journalingReconciliationAccount.Guid.ToString()] = reconciliationAccountConfig;
					if (reconciliationAccountConfig.Mailboxes == null || reconciliationAccountConfig.Mailboxes.Length == 0)
					{
						throw new JournalingConfigurationLoadException(string.Format("No mailboxes for reconciliation account object: {0}", journalingReconciliationAccount.Id));
					}
					foreach (string text in reconciliationAccountConfig.Mailboxes)
					{
						if (string.IsNullOrEmpty(text) || !SmtpAddress.IsValidSmtpAddress(text) || SmtpAddress.NullReversePath.ToString() == text)
						{
							throw new JournalingConfigurationLoadException(string.Format("Journaling reconciliation account AD object has empty mailbox: {0}", journalingReconciliationAccount.Id));
						}
						this.mailboxesDictionary[text] = true;
					}
				}
			}
			catch (DataValidationException ex2)
			{
				ex = ex2;
			}
			catch (TransientException ex3)
			{
				ex = ex3;
			}
			catch (JournalingConfigurationLoadException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.JournalingTracer.TraceError<Exception>((long)this.GetHashCode(), "Failed to load reconciliation configuration, exception: {0}", ex);
				return false;
			}
			return true;
		}

		private bool InitializeTenantConfiguration(OrganizationId organizationId)
		{
			ReconciliationAccountPerTenantSettings reconciliationAccountPerTenantSettings;
			if (!Components.Configuration.TryGetReconciliationAccounts(organizationId, out reconciliationAccountPerTenantSettings))
			{
				ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "Failed to load reconciliation configuration");
				return false;
			}
			ReconciliationAccountPerTenantSettings.ReconciliationAccountData[] accountDataArray = reconciliationAccountPerTenantSettings.AccountDataArray;
			foreach (ReconciliationAccountPerTenantSettings.ReconciliationAccountData reconciliationAccountData in accountDataArray)
			{
				string key = reconciliationAccountData.Guid.ToString();
				this.reconciliationAccounts[key] = new ReconciliationAccountConfig(reconciliationAccountData.Mailboxes);
				foreach (string text in reconciliationAccountData.Mailboxes)
				{
					if (string.IsNullOrEmpty(text) || !SmtpAddress.IsValidSmtpAddress(text) || SmtpAddress.NullReversePath.ToString() == text)
					{
						ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Journaling reconciliation account AD object has an invalid mailbox:'{0}'", text);
					}
					this.mailboxesDictionary[text] = true;
				}
			}
			return true;
		}

		private static object lockObject = new object();

		private static ReconcileConfig instance = null;

		private static TimeSpan refreshInterval = TimeSpan.FromHours(4.0);

		private Dictionary<string, ReconciliationAccountConfig> reconciliationAccounts = new Dictionary<string, ReconciliationAccountConfig>(10, StringComparer.OrdinalIgnoreCase);

		private Dictionary<string, bool> mailboxesDictionary = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

		private DateTime validUntil = DateTime.UtcNow + ReconcileConfig.refreshInterval;
	}
}
