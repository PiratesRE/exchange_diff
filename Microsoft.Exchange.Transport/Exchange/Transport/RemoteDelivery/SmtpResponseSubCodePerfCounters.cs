using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal static class SmtpResponseSubCodePerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (SmtpResponseSubCodePerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in SmtpResponseSubCodePerfCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchangeTransport Queuing Errors";

		public static readonly ExPerformanceCounter Zero = new ExPerformanceCounter("MSExchangeTransport Queuing Errors", "X.0.X Other or Undefined Status", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter One = new ExPerformanceCounter("MSExchangeTransport Queuing Errors", "X.1.X Addressing Status", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Two = new ExPerformanceCounter("MSExchangeTransport Queuing Errors", "X.2.X Mailbox Status", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Three = new ExPerformanceCounter("MSExchangeTransport Queuing Errors", "X.3.X Mail System Status", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Four = new ExPerformanceCounter("MSExchangeTransport Queuing Errors", "X.4.X Network and Routing Status", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Five = new ExPerformanceCounter("MSExchangeTransport Queuing Errors", "X.5.X Mail Delivery Protocol Status", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Six = new ExPerformanceCounter("MSExchangeTransport Queuing Errors", "X.6.X Message Content or Media Status", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Seven = new ExPerformanceCounter("MSExchangeTransport Queuing Errors", "X.7.X Security or Policy Status", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			SmtpResponseSubCodePerfCounters.Zero,
			SmtpResponseSubCodePerfCounters.One,
			SmtpResponseSubCodePerfCounters.Two,
			SmtpResponseSubCodePerfCounters.Three,
			SmtpResponseSubCodePerfCounters.Four,
			SmtpResponseSubCodePerfCounters.Five,
			SmtpResponseSubCodePerfCounters.Six,
			SmtpResponseSubCodePerfCounters.Seven
		};
	}
}
