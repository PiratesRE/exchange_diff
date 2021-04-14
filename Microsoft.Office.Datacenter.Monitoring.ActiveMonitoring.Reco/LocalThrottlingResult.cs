using System;
using System.Xml.Linq;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	internal class LocalThrottlingResult
	{
		internal bool IsPassed { get; set; }

		internal RecoveryActionHelper.RecoveryActionEntrySerializable MostRecentEntry { get; set; }

		internal int MinimumMinutes { get; set; }

		internal int TotalInOneHour { get; set; }

		internal int MaxAllowedInOneHour { get; set; }

		internal int TotalInOneDay { get; set; }

		internal int MaxAllowedInOneDay { get; set; }

		internal bool IsThrottlingInProgress { get; set; }

		internal bool IsRecoveryInProgress { get; set; }

		internal string ChecksFailed { get; set; }

		internal DateTime TimeToRetryAfter { get; set; }

		internal string ToXml(bool isForce = false)
		{
			if (!isForce && this.xml != null)
			{
				return this.xml;
			}
			XElement xelement = new XElement("LocalThrottlingResult", new object[]
			{
				new XAttribute("IsPassed", this.IsPassed),
				new XAttribute("MinimumMinutes", this.MinimumMinutes),
				new XAttribute("TotalInOneHour", this.TotalInOneHour),
				new XAttribute("MaxAllowedInOneHour", this.MaxAllowedInOneHour),
				new XAttribute("TotalInOneDay", this.TotalInOneDay),
				new XAttribute("MaxAllowedInOneDay", this.MaxAllowedInOneDay),
				new XAttribute("IsThrottlingInProgress", this.IsThrottlingInProgress),
				new XAttribute("IsRecoveryInProgress", this.IsRecoveryInProgress),
				new XAttribute("ChecksFailed", this.ChecksFailed),
				new XAttribute("TimeToRetryAfter", this.TimeToRetryAfter.ToString("o"))
			});
			if (this.MostRecentEntry != null)
			{
				xelement.Add(new XElement("MostRecentEntry", new object[]
				{
					new XAttribute("Requester", this.MostRecentEntry.RequestorName),
					new XAttribute("StartTime", this.MostRecentEntry.StartTime),
					new XAttribute("EndTime", this.MostRecentEntry.EndTime),
					new XAttribute("State", this.MostRecentEntry.State),
					new XAttribute("Result", this.MostRecentEntry.Result)
				}));
			}
			this.xml = xelement.ToString();
			return this.xml;
		}

		private string xml;
	}
}
