using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal sealed class MSExchangeTransportApprovalInstance : PerformanceCounterInstance
	{
		internal MSExchangeTransportApprovalInstance(string instanceName, MSExchangeTransportApprovalInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Approval Framework")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Number of Initiation Messages/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.InitiationMessages = new ExPerformanceCounter(base.CategoryName, "Number of Initiation Messages", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.InitiationMessages);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Number of Decision Messages/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.DecisionMessages = new ExPerformanceCounter(base.CategoryName, "Number of Decision Messages", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.DecisionMessages);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Decision Messages Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.DecisionUsed = new ExPerformanceCounter(base.CategoryName, "Decision Messages Processed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.DecisionUsed);
				this.DecisionInitiationMessageSearchTimeMilliseconds = new ExPerformanceCounter(base.CategoryName, "Total Time to Find Initiation Message in Milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DecisionInitiationMessageSearchTimeMilliseconds);
				this.NdrOofInitiationMessageSearchTimeMilliseconds = new ExPerformanceCounter(base.CategoryName, "Total time to find initiation message to corelate NDRs or OOFs in milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NdrOofInitiationMessageSearchTimeMilliseconds);
				this.TotalSearchesForInitiationBasedOnNdrAndOof = new ExPerformanceCounter(base.CategoryName, "Number of searches based on NDR and OOF", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSearchesForInitiationBasedOnNdrAndOof);
				this.TotalNdrOofHandled = new ExPerformanceCounter(base.CategoryName, "Number of NDRs and OOFs processed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalNdrOofHandled);
				this.TotalNdrOofUpdated = new ExPerformanceCounter(base.CategoryName, "Number of NDR and OOF that result in updates", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalNdrOofUpdated);
				long num = this.InitiationMessages.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MSExchangeTransportApprovalInstance(string instanceName) : base(instanceName, "MSExchange Approval Framework")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Number of Initiation Messages/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.InitiationMessages = new ExPerformanceCounter(base.CategoryName, "Number of Initiation Messages", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.InitiationMessages);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Number of Decision Messages/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.DecisionMessages = new ExPerformanceCounter(base.CategoryName, "Number of Decision Messages", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.DecisionMessages);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Decision Messages Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.DecisionUsed = new ExPerformanceCounter(base.CategoryName, "Decision Messages Processed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.DecisionUsed);
				this.DecisionInitiationMessageSearchTimeMilliseconds = new ExPerformanceCounter(base.CategoryName, "Total Time to Find Initiation Message in Milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DecisionInitiationMessageSearchTimeMilliseconds);
				this.NdrOofInitiationMessageSearchTimeMilliseconds = new ExPerformanceCounter(base.CategoryName, "Total time to find initiation message to corelate NDRs or OOFs in milliseconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NdrOofInitiationMessageSearchTimeMilliseconds);
				this.TotalSearchesForInitiationBasedOnNdrAndOof = new ExPerformanceCounter(base.CategoryName, "Number of searches based on NDR and OOF", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSearchesForInitiationBasedOnNdrAndOof);
				this.TotalNdrOofHandled = new ExPerformanceCounter(base.CategoryName, "Number of NDRs and OOFs processed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalNdrOofHandled);
				this.TotalNdrOofUpdated = new ExPerformanceCounter(base.CategoryName, "Number of NDR and OOF that result in updates", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalNdrOofUpdated);
				long num = this.InitiationMessages.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter InitiationMessages;

		public readonly ExPerformanceCounter DecisionMessages;

		public readonly ExPerformanceCounter DecisionUsed;

		public readonly ExPerformanceCounter DecisionInitiationMessageSearchTimeMilliseconds;

		public readonly ExPerformanceCounter NdrOofInitiationMessageSearchTimeMilliseconds;

		public readonly ExPerformanceCounter TotalSearchesForInitiationBasedOnNdrAndOof;

		public readonly ExPerformanceCounter TotalNdrOofHandled;

		public readonly ExPerformanceCounter TotalNdrOofUpdated;
	}
}
