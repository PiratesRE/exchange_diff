using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class TenantDagQuota : ITenantDagQuota
	{
		public TenantDagQuota(int dagsPerTenant, int messagesPerDag, double historyWeight, bool logDiagnosticInfo)
		{
			ArgumentValidator.ThrowIfOutOfRange<int>("dagsPerTenant", dagsPerTenant, 1, int.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<int>("messagesPerDag", messagesPerDag, 1, int.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<double>("historyWeight", historyWeight, 0.0, 1.0);
			this.defaultDagsPerTenant = dagsPerTenant;
			this.messagesPerDag = messagesPerDag;
			this.historyWeight = historyWeight;
			this.logDiagnosticInfo = logDiagnosticInfo;
			this.tenantDataDictionary = new ConcurrentDictionary<Guid, TenantDagQuota.TenantData>();
		}

		public void RefreshDagCount(int dagsAvailable)
		{
			this.LogDiagnosticInfo();
			List<Guid> list = new List<Guid>();
			foreach (KeyValuePair<Guid, TenantDagQuota.TenantData> keyValuePair in this.tenantDataDictionary)
			{
				TenantDagQuota.TenantData value = keyValuePair.Value;
				if (value.DagsPerTenant <= this.defaultDagsPerTenant && value.MessageCount == 0)
				{
					list.Add(keyValuePair.Key);
				}
				int num = this.ComputeDagCount(value);
				value.DagsPerTenant = ((num > dagsAvailable) ? dagsAvailable : num);
				value.ResetMessageCount();
			}
			foreach (Guid key in list)
			{
				TenantDagQuota.TenantData tenantData;
				if (this.tenantDataDictionary.TryGetValue(key, out tenantData) && tenantData.MessageCount == 0)
				{
					this.tenantDataDictionary.TryRemove(key, out tenantData);
				}
			}
		}

		public int GetDagCountForTenant(Guid tenantId)
		{
			TenantDagQuota.TenantData tenantData;
			if (this.tenantDataDictionary.TryGetValue(tenantId, out tenantData))
			{
				return tenantData.DagsPerTenant;
			}
			return this.defaultDagsPerTenant;
		}

		public void IncrementMessagesDeliveredToTenant(Guid tenantId)
		{
			TenantDagQuota.TenantData tenantData;
			if (!this.tenantDataDictionary.TryGetValue(tenantId, out tenantData))
			{
				this.tenantDataDictionary.TryAdd(tenantId, new TenantDagQuota.TenantData(this.defaultDagsPerTenant, 1));
				return;
			}
			tenantData.IncrementMessageCount();
		}

		public bool TryGetDiagnosticInfo(bool verbose, DiagnosableParameters parameters, out XElement diagnosticInfo)
		{
			bool flag = verbose;
			if (!flag)
			{
				flag = parameters.Argument.Equals("TenantDagQuota", StringComparison.InvariantCultureIgnoreCase);
			}
			if (flag)
			{
				diagnosticInfo = this.GetDiagnosticInfo();
				return true;
			}
			if (parameters.Argument.IndexOf("tenant:", StringComparison.OrdinalIgnoreCase) != -1)
			{
				diagnosticInfo = this.GetTenantDiagnosticInfo(parameters.Argument.Substring(7));
				return true;
			}
			diagnosticInfo = null;
			return false;
		}

		private static XElement GetTenantDiagnosticInfo(Guid tenantId, TenantDagQuota.TenantData tenantData)
		{
			XElement xelement = new XElement("Tenant");
			xelement.SetAttributeValue("Id", tenantId);
			xelement.SetAttributeValue("DagsForTenant", tenantData.DagsPerTenant);
			xelement.SetAttributeValue("MessageCount", tenantData.MessageCount);
			return xelement;
		}

		private XElement GetDiagnosticInfo()
		{
			XElement xelement;
			XElement tenantDagQuotaDiagnosticInfo = this.GetTenantDagQuotaDiagnosticInfo(out xelement);
			foreach (KeyValuePair<Guid, TenantDagQuota.TenantData> keyValuePair in this.tenantDataDictionary)
			{
				xelement.Add(TenantDagQuota.GetTenantDiagnosticInfo(keyValuePair.Key, keyValuePair.Value));
			}
			return tenantDagQuotaDiagnosticInfo;
		}

		private void LogDiagnosticInfo()
		{
			if (this.logDiagnosticInfo)
			{
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_TenantDagQuotaDiagnosticInfo, null, new object[]
				{
					this.GetDiagnosticInfo()
				});
			}
		}

		private XElement GetTenantDagQuotaDiagnosticInfo(out XElement tenantsElement)
		{
			XElement xelement = new XElement("TenantDagQuota");
			xelement.SetAttributeValue("DefaultDagsPerTenant", this.defaultDagsPerTenant);
			xelement.SetAttributeValue("MessagesPerDag", this.messagesPerDag);
			xelement.SetAttributeValue("WeightForHistory", this.historyWeight);
			tenantsElement = new XElement("TenantData");
			xelement.Add(tenantsElement);
			return xelement;
		}

		private XElement GetTenantDiagnosticInfo(string tenantIdString)
		{
			Guid guid;
			if (!Guid.TryParse(tenantIdString, out guid))
			{
				return new XElement("Error", string.Format("Invalid tenant id {0} passed as argument. A Guid is expected.", tenantIdString));
			}
			TenantDagQuota.TenantData tenantData;
			if (this.tenantDataDictionary.TryGetValue(guid, out tenantData))
			{
				XElement xelement;
				XElement tenantDagQuotaDiagnosticInfo = this.GetTenantDagQuotaDiagnosticInfo(out xelement);
				xelement.Add(TenantDagQuota.GetTenantDiagnosticInfo(guid, tenantData));
				return tenantDagQuotaDiagnosticInfo;
			}
			return new XElement("Error", string.Format("Tenant with id {0} not present in TenantDagQuota.", guid));
		}

		private int ComputeDagCount(TenantDagQuota.TenantData tenantData)
		{
			double num = (double)tenantData.DagsPerTenant * this.historyWeight;
			double num2 = (double)tenantData.MessageCount * (1.0 - this.historyWeight) / (double)this.messagesPerDag;
			return Math.Max(1, (int)Math.Floor(num + num2));
		}

		private readonly int defaultDagsPerTenant;

		private readonly int messagesPerDag;

		private readonly double historyWeight;

		private readonly bool logDiagnosticInfo;

		private ConcurrentDictionary<Guid, TenantDagQuota.TenantData> tenantDataDictionary;

		private class TenantData
		{
			public TenantData(int dagsPerTenant, int messageCount)
			{
				this.DagsPerTenant = dagsPerTenant;
				this.messageCount = messageCount;
			}

			public int DagsPerTenant { get; set; }

			public int MessageCount
			{
				get
				{
					return this.messageCount;
				}
			}

			public void IncrementMessageCount()
			{
				Interlocked.Increment(ref this.messageCount);
			}

			public void ResetMessageCount()
			{
				Interlocked.Exchange(ref this.messageCount, 0);
			}

			private int messageCount;
		}
	}
}
