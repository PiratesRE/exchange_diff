using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class DagSelector
	{
		public DagSelector(int messageThresholdPerServer, double messageThresholdIncreaseFactor, int activeServersForDagToBeRoutable, int minimumSitesForDagToBeRoutable, bool logDiagnosticInfo, ITenantDagQuota tenantDagQuota, IEnumerable<DeliveryGroup> dags)
		{
			RoutingUtils.ThrowIfNull(tenantDagQuota, "tenantDagQuota");
			RoutingUtils.ThrowIfNull(dags, "dags");
			ArgumentValidator.ThrowIfZeroOrNegative("activeServersForDagToBeRoutable", activeServersForDagToBeRoutable);
			ArgumentValidator.ThrowIfZeroOrNegative("minimumSitesForDagToBeRoutable", minimumSitesForDagToBeRoutable);
			this.InitializeDagDictionary(messageThresholdPerServer, activeServersForDagToBeRoutable, minimumSitesForDagToBeRoutable, dags);
			this.messageThresholdIncreaseFactor = messageThresholdIncreaseFactor;
			this.logDiagnosticInfo = logDiagnosticInfo;
			this.tenantDagQuota = tenantDagQuota;
			this.tenantDagQuota.RefreshDagCount(this.dagsInOrder.Count);
			this.currentMessageThresholdMultiplier = 0;
		}

		public void IncrementMessagesDeliveredBasedOnMailbox(Guid dagId, Guid tenantId)
		{
			DagSelector.DagData dagData;
			if (this.dagDictionary.TryGetValue(dagId, out dagData))
			{
				dagData.IncrementMessagesDeliveredBasedOnMailbox();
			}
			this.tenantDagQuota.IncrementMessagesDeliveredToTenant(tenantId);
		}

		public bool TryGetDagDeliveryGroup(Guid externalOrganizationId, out DeliveryGroup dagDeliveryGroup)
		{
			if (this.dagsInOrder.Count == 0)
			{
				dagDeliveryGroup = null;
				return false;
			}
			int num = DagSelector.GetHashForGuid(externalOrganizationId) % this.dagsInOrder.Count;
			int dagCountForTenant = this.tenantDagQuota.GetDagCountForTenant(externalOrganizationId);
			int num2 = 0;
			for (;;)
			{
				int num3 = this.currentMessageThresholdMultiplier;
				double messageThresholdFactor = this.GetMessageThresholdFactor();
				if (this.TryGetDagDeliveryGroup(num, dagCountForTenant, messageThresholdFactor, true, out dagDeliveryGroup))
				{
					break;
				}
				if (this.TryGetDagDeliveryGroup((num + dagCountForTenant) % this.dagsInOrder.Count, this.dagsInOrder.Count - dagCountForTenant, messageThresholdFactor, false, out dagDeliveryGroup))
				{
					goto Block_3;
				}
				Interlocked.CompareExchange(ref this.currentMessageThresholdMultiplier, num3 + 1, num3);
				if (++num2 > 1000)
				{
					goto Block_4;
				}
			}
			this.tenantDagQuota.IncrementMessagesDeliveredToTenant(externalOrganizationId);
			return true;
			Block_3:
			this.tenantDagQuota.IncrementMessagesDeliveredToTenant(externalOrganizationId);
			return true;
			Block_4:
			throw new InvalidOperationException(string.Format("DagSelector.TryGetDagDeliveryGroup has iterated {0} times and not found a DAG that can accept messages", num2));
		}

		public bool TryGetDiagnosticInfo(bool verbose, DiagnosableParameters parameters, out XElement diagnosticInfo)
		{
			bool flag = verbose;
			if (!flag)
			{
				flag = parameters.Argument.Equals("DagSelector", StringComparison.InvariantCultureIgnoreCase);
			}
			if (flag)
			{
				diagnosticInfo = this.GetDiagnosticInfo();
			}
			else
			{
				diagnosticInfo = null;
			}
			return flag;
		}

		public void LogDiagnosticInfo()
		{
			if (this.logDiagnosticInfo)
			{
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_DagSelectorDiagnosticInfo, null, new object[]
				{
					this.GetDiagnosticInfo()
				});
			}
		}

		private static int GetHashForGuid(Guid tenantId)
		{
			return Math.Abs(tenantId.GetHashCode());
		}

		private void InitializeDagDictionary(int messageThreshold, int activeServersForDagToBeRoutable, int minimumSitesForDagToBeRoutable, IEnumerable<DeliveryGroup> dags)
		{
			this.dagDictionary = new Dictionary<Guid, DagSelector.DagData>();
			List<Guid> list = new List<Guid>();
			List<DeliveryGroup> list2 = null;
			foreach (DeliveryGroup deliveryGroup in dags)
			{
				DagSelector.DagData dagData = new DagSelector.DagData(messageThreshold, activeServersForDagToBeRoutable, minimumSitesForDagToBeRoutable, deliveryGroup);
				if (dagData.CanAcceptMessages)
				{
					if (TransportHelpers.AttemptAddToDictionary<Guid, DagSelector.DagData>(this.dagDictionary, deliveryGroup.NextHopGuid, dagData, new TransportHelpers.DiagnosticsHandler<Guid, DagSelector.DagData>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, DagSelector.DagData>)))
					{
						list.Add(deliveryGroup.NextHopGuid);
					}
				}
				else
				{
					RoutingUtils.AddItemToLazyList<DeliveryGroup>(deliveryGroup, ref list2);
				}
			}
			if (list2 != null)
			{
				ExEventLog eventLogger = RoutingDiag.EventLogger;
				ExEventLog.EventTuple tuple_InactiveDagsExcludedFromDagSelector = TransportEventLogConstants.Tuple_InactiveDagsExcludedFromDagSelector;
				string periodicKey = null;
				object[] array = new object[1];
				array[0] = string.Join(", ", from dag in list2
				select dag.Name);
				eventLogger.LogEvent(tuple_InactiveDagsExcludedFromDagSelector, periodicKey, array);
			}
			list.Sort();
			this.dagsInOrder = list;
		}

		private XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("DagSelector");
			xelement.SetAttributeValue("MessageThresholdFactor", this.messageThresholdIncreaseFactor);
			xelement.SetAttributeValue("MessageThresholdMultiplier", this.currentMessageThresholdMultiplier);
			XElement xelement2 = new XElement("DagData");
			xelement.Add(xelement2);
			for (int i = 0; i < this.dagsInOrder.Count; i++)
			{
				xelement2.Add(this.dagDictionary[this.dagsInOrder[i]].GetDiagnosticInfo(this.GetMessageThresholdFactor()));
			}
			return xelement;
		}

		private bool TryGetDagDeliveryGroup(int startOffset, int dagsForTenant, double messageThresholdFactor, bool randomizeStartOffset, out DeliveryGroup deliveryGroup)
		{
			int num = randomizeStartOffset ? ((startOffset + RoutingUtils.GetRandomNumber(dagsForTenant)) % this.dagsInOrder.Count) : startOffset;
			for (int i = 0; i < dagsForTenant; i++)
			{
				DagSelector.DagData dagData = this.dagDictionary[this.dagsInOrder[num]];
				if (dagData.IsUnderMessageThreshold(messageThresholdFactor))
				{
					deliveryGroup = dagData.DeliveryGroup;
					dagData.IncrementMessagesDeliveredBasedOnDagSelector();
					return true;
				}
				num = (num + 1) % this.dagsInOrder.Count;
			}
			deliveryGroup = null;
			return false;
		}

		private double GetMessageThresholdFactor()
		{
			return 1.0 + (double)this.currentMessageThresholdMultiplier * this.messageThresholdIncreaseFactor;
		}

		private const int TryGetDagDeliveryGroupMaxLoopCount = 1000;

		private ITenantDagQuota tenantDagQuota;

		private Dictionary<Guid, DagSelector.DagData> dagDictionary;

		private List<Guid> dagsInOrder;

		private int currentMessageThresholdMultiplier;

		private readonly double messageThresholdIncreaseFactor;

		private readonly bool logDiagnosticInfo;

		private class DagData
		{
			public DagData(int messageThresholdPerServer, int activeServersForDagToBeRoutable, int minimumSitesForDagToBeRoutable, DeliveryGroup dag)
			{
				this.DeliveryGroup = dag;
				this.messageThreshold = DagSelector.DagData.GetMessageThresholdBasedOnDag(dag, messageThresholdPerServer, activeServersForDagToBeRoutable, minimumSitesForDagToBeRoutable);
			}

			public DeliveryGroup DeliveryGroup { get; private set; }

			public bool CanAcceptMessages
			{
				get
				{
					return this.messageThreshold > 0;
				}
			}

			public void IncrementMessagesDeliveredBasedOnMailbox()
			{
				Interlocked.Increment(ref this.messagesDeliveredBasedOnMailbox);
			}

			public void IncrementMessagesDeliveredBasedOnDagSelector()
			{
				Interlocked.Increment(ref this.messagesDeliveredBasedOnDagSelector);
			}

			public bool IsUnderMessageThreshold(double multiplier)
			{
				return (double)(this.messagesDeliveredBasedOnMailbox + this.messagesDeliveredBasedOnDagSelector) < (double)this.messageThreshold * multiplier;
			}

			public XElement GetDiagnosticInfo(double messageThresholdFactor)
			{
				XElement xelement = new XElement("Dag");
				xelement.SetAttributeValue("Name", this.DeliveryGroup.Name);
				xelement.SetAttributeValue("Id", this.DeliveryGroup.NextHopGuid);
				xelement.SetAttributeValue("MessageThreshold", Math.Ceiling((double)this.messageThreshold * messageThresholdFactor));
				xelement.SetAttributeValue("MessagesDeliveredBasedOnMailbox", this.messagesDeliveredBasedOnMailbox);
				xelement.SetAttributeValue("MessagesDeliveredBasedOnDagSelector", this.messagesDeliveredBasedOnDagSelector);
				return xelement;
			}

			private static int GetMessageThresholdBasedOnDag(DeliveryGroup dag, int messageThresholdPerServer, int activeServersForDagToBeRoutable, int minimumSitesForDagToBeRoutable)
			{
				int num = 0;
				HashSet<Guid> hashSet = new HashSet<Guid>();
				foreach (RoutingServerInfo routingServerInfo in dag.AllServersNoFallback)
				{
					if (routingServerInfo.IsHubTransportActive)
					{
						num++;
						if (routingServerInfo.ADSite != null)
						{
							hashSet.Add(routingServerInfo.ADSite.ObjectGuid);
						}
					}
				}
				if (num < activeServersForDagToBeRoutable)
				{
					return 0;
				}
				if (hashSet.Count < minimumSitesForDagToBeRoutable)
				{
					return 0;
				}
				return messageThresholdPerServer * num;
			}

			private readonly int messageThreshold;

			private int messagesDeliveredBasedOnMailbox;

			private int messagesDeliveredBasedOnDagSelector;
		}
	}
}
