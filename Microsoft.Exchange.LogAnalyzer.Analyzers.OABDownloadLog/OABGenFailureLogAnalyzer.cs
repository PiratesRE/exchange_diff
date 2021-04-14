using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.LogAnalyzer.Extensions.OABDownloadLog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.LogAnalyzer.Analyzers.OABDownloadLog
{
	public sealed class OABGenFailureLogAnalyzer : OABDownloadLogAnalyzer
	{
		public OABGenFailureLogAnalyzer(IJob job) : base(job)
		{
			string name = base.GetType().Name;
			base.TimeUpdatePeriod = Configuration.GetConfigTimeSpan(name + "RecurrenceInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
			this.monitoringInterval = Configuration.GetConfigInt(name + "MonitoringInterval", 5, 24, 24);
			this.lastRequestedTimeThreshold = Configuration.GetConfigInt(name + "LastRequestedTimeThreshold", 1, 14, 14);
			this.lastTouchedTimeThreshold = Configuration.GetConfigInt(name + "LastTouchedTimeThreshold", 1, 48, 8);
			this.noOfRequestsThreshold = Configuration.GetConfigInt(name + "NoOfRequestsThreshold", 1, 5, 3);
			this.staleRequestTimeThreshold = Configuration.GetConfigInt(name + "StaleRequestTimeThreshold", 1, 2, 2);
			this.tenantsOutOfSLA = new List<OABTenantInfo>();
			this.whiteListedTenants = new List<string>();
			this.GetConfigs("OABDownloadAnalyzerConfiguration.xml");
			this.whiteListedTenantStatus = new List<OrganizationStatus>
			{
				OrganizationStatus.Active,
				OrganizationStatus.Invalid
			};
		}

		public override void OnLogLine(OnLogLineSource source, OnLogLineArgs args)
		{
			OABDownloadLogLine oabdownloadLogLine = (OABDownloadLogLine)args.LogLine;
			string organization = oabdownloadLogLine.Organization;
			DateTime d;
			DateTime dateTime;
			if (this.IsCustomerScenario(organization) && DateTime.TryParse(oabdownloadLogLine.LastRequestedTime, out d) && DateTime.TryParse(oabdownloadLogLine.LastTouchedTime, out dateTime))
			{
				OABTenantInfo oabtenantInfo = this.tenantsOutOfSLA.Find((OABTenantInfo x) => x.Organization == organization);
				if (this.whiteListedTenantStatus.Contains(oabdownloadLogLine.OrgStatus) && d.AddDays((double)this.lastRequestedTimeThreshold) > DateTime.UtcNow && dateTime.AddHours((double)this.lastTouchedTimeThreshold) < DateTime.UtcNow && oabdownloadLogLine.Timestamp - d > TimeSpan.FromDays((double)this.staleRequestTimeThreshold) && !oabdownloadLogLine.IsAddressListDeleted)
				{
					if (oabtenantInfo == null)
					{
						oabtenantInfo = new OABTenantInfo(organization, oabdownloadLogLine, TimeSpan.FromHours((double)this.monitoringInterval));
						this.tenantsOutOfSLA.Add(oabtenantInfo);
					}
					oabtenantInfo.NoOfRequests++;
					return;
				}
				if (oabtenantInfo != null)
				{
					this.tenantsOutOfSLA.Remove(oabtenantInfo);
				}
			}
		}

		public override void OnTimeUpdate(OnTimeUpdateSource source, OnTimeUpdateArgs args)
		{
			DateTime currentTime = args.Timestamp;
			this.tenantsOutOfSLA.RemoveAll((OABTenantInfo x) => x.LogLine.Timestamp.AddHours((double)this.monitoringInterval) < currentTime);
			foreach (OABTenantInfo oabtenantInfo in this.tenantsOutOfSLA)
			{
				if (oabtenantInfo.IsAlert(currentTime, this.noOfRequestsThreshold, base.TimeUpdatePeriod))
				{
					TriggerHandler.Trigger(OABGenFailureLogAnalyzer.EventName.OABGenTenantOutOfSLA.ToString(), new object[]
					{
						oabtenantInfo.Organization,
						this.lastTouchedTimeThreshold,
						oabtenantInfo.LogLine.LastTouchedTime,
						oabtenantInfo.LogLine.LastRequestedTime
					});
					oabtenantInfo.LastAlertTime = currentTime;
				}
			}
		}

		private static DateTime TryParseDateTime(string str, DateTime defaultValue)
		{
			DateTime result;
			if (DateTime.TryParse(str, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private void GetConfigs(string config)
		{
			string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), config);
			if (!File.Exists(path))
			{
				throw new ArgumentException(string.Format("Couldn't find the {0} file in the current directory", config));
			}
			using (XmlReader xmlReader = XmlReader.Create(File.OpenRead(path)))
			{
				XDocument xdocument = XDocument.Load(xmlReader);
				this.whiteListedTenants = (from x in xdocument.Descendants("WhiteListedTenants").Descendants("Tenants")
				select x.Value).ToList<string>();
			}
		}

		private bool IsCustomerScenario(string organization)
		{
			if (organization == null)
			{
				return false;
			}
			foreach (string value in this.whiteListedTenants)
			{
				if (organization.Contains(value))
				{
					return false;
				}
			}
			return true;
		}

		public const string ConfigurationFile = "OABDownloadAnalyzerConfiguration.xml";

		private readonly int monitoringInterval;

		private readonly int lastTouchedTimeThreshold;

		private readonly int lastRequestedTimeThreshold;

		private readonly int noOfRequestsThreshold;

		private readonly int staleRequestTimeThreshold;

		private List<string> whiteListedTenants;

		private List<OABTenantInfo> tenantsOutOfSLA;

		private List<OrganizationStatus> whiteListedTenantStatus;

		public enum EventName
		{
			OABGenTenantOutOfSLA
		}

		private static class ConfigurationElements
		{
			public const string WhiteListedTenants = "WhiteListedTenants";

			public const string Tenants = "Tenants";
		}
	}
}
