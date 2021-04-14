using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal sealed class DeliveryAgentPerfCountersInstance : PerformanceCounterInstance
	{
		internal DeliveryAgentPerfCountersInstance(string instanceName, DeliveryAgentPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport DeliveryAgent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Messages Delivered Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Connections Completed Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.ConnectionsFailedTotal = new ExPerformanceCounter(base.CategoryName, "Connections Failed Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionsFailedTotal);
				ExPerformanceCounter exPerformanceCounter3 = null;
				exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Connections Failed Per Second", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(exPerformanceCounter3);
				this.CurrentConnectionCount = new ExPerformanceCounter(base.CategoryName, "Current Connection Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentConnectionCount);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Messages Deferred Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.MessagesDeferredTotal = new ExPerformanceCounter(base.CategoryName, "Messages Deferred Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.MessagesDeferredTotal);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Messages Failed Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.MessagesFailedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Failed Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.MessagesFailedTotal);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Recipient Deliveries Completed Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average Recipients Per Message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.RecipientDeliveriesCompletedTotal = new ExPerformanceCounter(base.CategoryName, "Recipient Deliveries Completed Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6,
					exPerformanceCounter7
				});
				list.Add(this.RecipientDeliveriesCompletedTotal);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Average Recipients per Message (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Message Bytes Sent Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "Average Messages Per Connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "Average Messages Per Connection (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				this.ConnectionsCompletedTotal = new ExPerformanceCounter(base.CategoryName, "Connections Completed Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2,
					exPerformanceCounter11
				});
				list.Add(this.ConnectionsCompletedTotal);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "Average Bytes Per Message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.MessageBytesSentTotal = new ExPerformanceCounter(base.CategoryName, "Message bytes sent total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter9,
					exPerformanceCounter12
				});
				list.Add(this.MessageBytesSentTotal);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "Average Bytes Per Message (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.MessagesDeliveredTotal = new ExPerformanceCounter(base.CategoryName, "Messages Delivered Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter8,
					exPerformanceCounter10,
					exPerformanceCounter13
				});
				list.Add(this.MessagesDeliveredTotal);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "Average Invocation Duration", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.InvocationDurationTotal = new ExPerformanceCounter(base.CategoryName, "Invocation Duration Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter14
				});
				list.Add(this.InvocationDurationTotal);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "Average Invocation Duration (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.InvocationTotal = new ExPerformanceCounter(base.CategoryName, "Invocation Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter15
				});
				list.Add(this.InvocationTotal);
				long num = this.MessagesDeliveredTotal.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter16 in list)
					{
						exPerformanceCounter16.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal DeliveryAgentPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport DeliveryAgent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Messages Delivered Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Connections Completed Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.ConnectionsFailedTotal = new ExPerformanceCounter(base.CategoryName, "Connections Failed Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionsFailedTotal);
				ExPerformanceCounter exPerformanceCounter3 = null;
				exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Connections Failed Per Second", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(exPerformanceCounter3);
				this.CurrentConnectionCount = new ExPerformanceCounter(base.CategoryName, "Current Connection Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentConnectionCount);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Messages Deferred Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.MessagesDeferredTotal = new ExPerformanceCounter(base.CategoryName, "Messages Deferred Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.MessagesDeferredTotal);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Messages Failed Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.MessagesFailedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Failed Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.MessagesFailedTotal);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Recipient Deliveries Completed Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average Recipients Per Message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.RecipientDeliveriesCompletedTotal = new ExPerformanceCounter(base.CategoryName, "Recipient Deliveries Completed Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6,
					exPerformanceCounter7
				});
				list.Add(this.RecipientDeliveriesCompletedTotal);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Average Recipients per Message (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Message Bytes Sent Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "Average Messages Per Connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "Average Messages Per Connection (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				this.ConnectionsCompletedTotal = new ExPerformanceCounter(base.CategoryName, "Connections Completed Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2,
					exPerformanceCounter11
				});
				list.Add(this.ConnectionsCompletedTotal);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "Average Bytes Per Message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.MessageBytesSentTotal = new ExPerformanceCounter(base.CategoryName, "Message bytes sent total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter9,
					exPerformanceCounter12
				});
				list.Add(this.MessageBytesSentTotal);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "Average Bytes Per Message (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.MessagesDeliveredTotal = new ExPerformanceCounter(base.CategoryName, "Messages Delivered Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter8,
					exPerformanceCounter10,
					exPerformanceCounter13
				});
				list.Add(this.MessagesDeliveredTotal);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "Average Invocation Duration", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.InvocationDurationTotal = new ExPerformanceCounter(base.CategoryName, "Invocation Duration Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter14
				});
				list.Add(this.InvocationDurationTotal);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "Average Invocation Duration (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.InvocationTotal = new ExPerformanceCounter(base.CategoryName, "Invocation Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter15
				});
				list.Add(this.InvocationTotal);
				long num = this.MessagesDeliveredTotal.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter16 in list)
					{
						exPerformanceCounter16.Close();
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

		public readonly ExPerformanceCounter MessagesDeliveredTotal;

		public readonly ExPerformanceCounter ConnectionsCompletedTotal;

		public readonly ExPerformanceCounter ConnectionsFailedTotal;

		public readonly ExPerformanceCounter CurrentConnectionCount;

		public readonly ExPerformanceCounter MessagesDeferredTotal;

		public readonly ExPerformanceCounter MessagesFailedTotal;

		public readonly ExPerformanceCounter RecipientDeliveriesCompletedTotal;

		public readonly ExPerformanceCounter MessageBytesSentTotal;

		public readonly ExPerformanceCounter InvocationTotal;

		public readonly ExPerformanceCounter InvocationDurationTotal;
	}
}
