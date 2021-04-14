using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class UserAgent
	{
		internal UserAgent(string category, string type, string source, string protocol)
		{
			if (category != null)
			{
				this.category = category;
			}
			if (type != null)
			{
				this.type = type;
			}
			if (source != null)
			{
				this.source = source;
			}
			if (protocol != null)
			{
				this.protocol = protocol;
			}
		}

		internal static UserAgent Parse(string userAgentString)
		{
			string[] array = userAgentString.Split(UserAgent.Delimiters, 5, StringSplitOptions.None);
			if (array.Length < 4)
			{
				UserAgent.SecurityTracer.TraceDebug<string, int, object>(0L, "{0}: User agent string {1} has {2} parts. Expected at least 4.", userAgentString, array.Length, TraceContext.Get());
				return null;
			}
			return new UserAgent(array[0], array[1], array[2], array[3]);
		}

		internal string Category
		{
			get
			{
				return this.category;
			}
		}

		internal string Type
		{
			get
			{
				return this.type;
			}
		}

		internal string Source
		{
			get
			{
				return this.source;
			}
		}

		internal string Protocol
		{
			get
			{
				return this.protocol;
			}
		}

		internal string Version
		{
			get
			{
				return UserAgent.VersionInfo.FileVersion;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.category,
				"/",
				this.type,
				"/",
				this.source,
				"/",
				this.protocol,
				"/",
				this.Version
			});
		}

		private const int MaximumComponents = 5;

		internal const string ProxyCategory = "ASProxy";

		internal const string AutoDiscoverCategory = "ASAutoDiscover";

		internal const string CrossSiteType = "CrossSite";

		internal const string CrossForestType = "CrossForest";

		internal const string ScpSource = "Directory";

		internal const string DnsSource = "EmailDomain";

		internal const string ASAutoDiscoverUserAgentPrefix = "ASAutoDiscover/CrossForest";

		private static readonly FileVersionInfo VersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

		private static readonly Microsoft.Exchange.Diagnostics.Trace SecurityTracer = ExTraceGlobals.SecurityTracer;

		private readonly string category = string.Empty;

		private readonly string type = string.Empty;

		private readonly string source = string.Empty;

		private readonly string protocol = string.Empty;

		internal static readonly char[] Delimiters = new char[]
		{
			'/'
		};
	}
}
