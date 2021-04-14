using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class SmtpAvailabilityPerfCountersInstance : PerformanceCounterInstance
	{
		internal SmtpAvailabilityPerfCountersInstance(string instanceName, SmtpAvailabilityPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, SmtpAvailabilityPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalConnections = new ExPerformanceCounter(base.CategoryName, "Total Connections", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalConnections);
				this.AvailabilityPercentage = new ExPerformanceCounter(base.CategoryName, "% Availability", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AvailabilityPercentage);
				this.ActivityPercentage = new ExPerformanceCounter(base.CategoryName, "% Activity", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityPercentage);
				this.FailuresDueToMaxInboundConnectionLimit = new ExPerformanceCounter(base.CategoryName, "% Failures Due To MaxInboundConnectionLimit", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToMaxInboundConnectionLimit);
				this.FailuresDueToWLIDDown = new ExPerformanceCounter(base.CategoryName, "% Failures Due To WLID Down", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToWLIDDown);
				this.FailuresDueToADDown = new ExPerformanceCounter(base.CategoryName, "% Failures Due To Active Directory Down", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToADDown);
				this.FailuresDueToBackPressure = new ExPerformanceCounter(base.CategoryName, "% Failures Due To Back Pressure", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToBackPressure);
				this.FailuresDueToIOExceptions = new ExPerformanceCounter(base.CategoryName, "% Failures Due To IO Exceptions", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToIOExceptions);
				this.FailuresDueToTLSErrors = new ExPerformanceCounter(base.CategoryName, "% Failures Due To TLS Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToTLSErrors);
				this.FailuresDueToMaxLocalLoopCount = new ExPerformanceCounter(base.CategoryName, "Failures Due to Maximum Local Loop Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToMaxLocalLoopCount);
				this.LoopingMessagesLastHour = new ExPerformanceCounter(base.CategoryName, "Looping Messages Last Hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LoopingMessagesLastHour);
				long num = this.TotalConnections.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal SmtpAvailabilityPerfCountersInstance(string instanceName) : base(instanceName, SmtpAvailabilityPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalConnections = new ExPerformanceCounter(base.CategoryName, "Total Connections", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalConnections);
				this.AvailabilityPercentage = new ExPerformanceCounter(base.CategoryName, "% Availability", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AvailabilityPercentage);
				this.ActivityPercentage = new ExPerformanceCounter(base.CategoryName, "% Activity", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityPercentage);
				this.FailuresDueToMaxInboundConnectionLimit = new ExPerformanceCounter(base.CategoryName, "% Failures Due To MaxInboundConnectionLimit", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToMaxInboundConnectionLimit);
				this.FailuresDueToWLIDDown = new ExPerformanceCounter(base.CategoryName, "% Failures Due To WLID Down", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToWLIDDown);
				this.FailuresDueToADDown = new ExPerformanceCounter(base.CategoryName, "% Failures Due To Active Directory Down", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToADDown);
				this.FailuresDueToBackPressure = new ExPerformanceCounter(base.CategoryName, "% Failures Due To Back Pressure", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToBackPressure);
				this.FailuresDueToIOExceptions = new ExPerformanceCounter(base.CategoryName, "% Failures Due To IO Exceptions", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToIOExceptions);
				this.FailuresDueToTLSErrors = new ExPerformanceCounter(base.CategoryName, "% Failures Due To TLS Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToTLSErrors);
				this.FailuresDueToMaxLocalLoopCount = new ExPerformanceCounter(base.CategoryName, "Failures Due to Maximum Local Loop Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailuresDueToMaxLocalLoopCount);
				this.LoopingMessagesLastHour = new ExPerformanceCounter(base.CategoryName, "Looping Messages Last Hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LoopingMessagesLastHour);
				long num = this.TotalConnections.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
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

		public readonly ExPerformanceCounter TotalConnections;

		public readonly ExPerformanceCounter AvailabilityPercentage;

		public readonly ExPerformanceCounter ActivityPercentage;

		public readonly ExPerformanceCounter FailuresDueToMaxInboundConnectionLimit;

		public readonly ExPerformanceCounter FailuresDueToWLIDDown;

		public readonly ExPerformanceCounter FailuresDueToADDown;

		public readonly ExPerformanceCounter FailuresDueToBackPressure;

		public readonly ExPerformanceCounter FailuresDueToIOExceptions;

		public readonly ExPerformanceCounter FailuresDueToTLSErrors;

		public readonly ExPerformanceCounter FailuresDueToMaxLocalLoopCount;

		public readonly ExPerformanceCounter LoopingMessagesLastHour;
	}
}
