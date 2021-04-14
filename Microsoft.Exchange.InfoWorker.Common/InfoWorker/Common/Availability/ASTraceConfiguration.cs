using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ASTraceConfiguration
	{
		private ASTraceConfiguration()
		{
			this.exTraceConfiguration = ExTraceConfiguration.Instance;
			this.exTraceConfigVersion = this.exTraceConfiguration.Version;
			this.filteredMailboxes = ASTraceConfiguration.GetStringFilterList(this.exTraceConfiguration, "UserDN");
			this.filteredRequesters = ASTraceConfiguration.GetStringFilterList(this.exTraceConfiguration, "WindowsIdentity");
		}

		public static ASTraceConfiguration Instance
		{
			get
			{
				if (ASTraceConfiguration.instance.exTraceConfigVersion != ExTraceConfiguration.Instance.Version)
				{
					lock (ASTraceConfiguration.staticSyncObject)
					{
						if (ASTraceConfiguration.instance.exTraceConfigVersion != ExTraceConfiguration.Instance.Version)
						{
							ASTraceConfiguration.instance = new ASTraceConfiguration();
						}
					}
				}
				return ASTraceConfiguration.instance;
			}
		}

		public List<string> FilteredMailboxes
		{
			get
			{
				return this.filteredMailboxes;
			}
		}

		public List<string> FilteredRequesters
		{
			get
			{
				return this.filteredRequesters;
			}
		}

		public bool FilteredTracingEnabled
		{
			get
			{
				return this.exTraceConfiguration.PerThreadTracingConfigured;
			}
		}

		private static List<string> GetStringFilterList(ExTraceConfiguration configuration, string filterKey)
		{
			List<string> result;
			if (!configuration.CustomParameters.TryGetValue(filterKey, out result))
			{
				return new List<string>();
			}
			return result;
		}

		public const string MailboxDNFilterKey = "UserDN";

		public const string RequesterWindowsIndentityFilterKey = "WindowsIdentity";

		private static object staticSyncObject = new object();

		private static ASTraceConfiguration instance = new ASTraceConfiguration();

		private ExTraceConfiguration exTraceConfiguration;

		private int exTraceConfigVersion;

		private List<string> filteredMailboxes;

		private List<string> filteredRequesters;
	}
}
