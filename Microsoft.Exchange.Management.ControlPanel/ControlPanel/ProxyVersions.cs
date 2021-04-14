using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ProxyVersions
	{
		public ProxyVersions(string rootDirectory)
		{
			string[] directories = Directory.GetDirectories(rootDirectory);
			IEnumerable<Version> collection = from folderPath in directories
			let folderName = Path.GetFileName(folderPath)
			where char.IsDigit(folderName[0])
			select new Version(folderName);
			this.validProxyVersions = new HashSet<Version>(collection);
			this.MaxVersion = new Version(((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(typeof(ProxyVersions).Assembly, typeof(AssemblyFileVersionAttribute))).Version);
			this.MinVersion = new Version(14, 0, 634, 0);
		}

		public bool Contains(Version version)
		{
			return this.validProxyVersions.Contains(version);
		}

		public bool IsCompatible(Version internalApplicationVersion, TextWriter decisionLogWriter)
		{
			if (OutboundProxySession.Factory.ProxyToLocalHost)
			{
				return true;
			}
			DecisionLogger decisionLogger = new DecisionLogger(decisionLogWriter)
			{
				{
					internalApplicationVersion <= this.MaxVersion,
					Strings.ProxyServiceConditionGreaterVersion(internalApplicationVersion, this.MaxVersion)
				},
				{
					internalApplicationVersion >= this.MinVersion,
					Strings.ProxyServiceConditionLesserVersion(internalApplicationVersion, this.MinVersion)
				},
				{
					this.Contains(internalApplicationVersion),
					Strings.ProxyServiceConditionInstalledVersion(internalApplicationVersion)
				}
			};
			return decisionLogger.Decision;
		}

		private readonly ICollection<Version> validProxyVersions;

		public readonly Version MinVersion;

		public readonly Version MaxVersion;
	}
}
