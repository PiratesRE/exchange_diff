using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class RusAkamaiPackagesVerifierProbe : RusPublishingPipelineBase
	{
		private TimeSpan AllowedMaxPublishingIntervalTimeSpan { get; set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.TraceDebug("RusAkamaiPackagesVerifierProbe started.");
			this.AllowedMaxPublishingIntervalTimeSpan = base.GetTimeSpanExtensionAttributeFromXml(base.Definition.ExtensionAttributes, "//RusAkamaiPackagesVerifierProbeParam", "AllowedMaxPublishingIntervalTimeSpan", RusAkamaiPackagesVerifierProbe.defaultAllowedPublishingInterval, RusAkamaiPackagesVerifierProbe.minimumAllowedPublishingInterval, RusAkamaiPackagesVerifierProbe.maximumAllowedPublishingInterval);
			base.RusEngineName = base.GetExtensionAttributeStringFromXml(base.Definition.ExtensionAttributes, "//RusAkamaiPackagesVerifierProbeParam", "EngineName", true);
			base.Platforms = base.GetExtensionAttributeStringFromXml(base.Definition.ExtensionAttributes, "//RusAkamaiPackagesVerifierProbeParam", "Platforms", true).Split(new char[]
			{
				','
			});
			base.TraceDebug(string.Format("[AllowedMaxPublishingIntervalTimeSpan: {0}, EngineName: {1}, Platforms: {2}]", this.AllowedMaxPublishingIntervalTimeSpan, base.RusEngineName, string.Join(",", base.Platforms)));
			bool flag = false;
			List<RusEngine> list = new List<RusEngine>();
			foreach (string text in base.Platforms)
			{
				base.RusEngine = new RusEngine(base.RusEngineName, text);
				DateTime forefrontdlManifestCreatedTimeInUtc = base.RusEngine.ForefrontdlManifestCreatedTimeInUtc;
				DateTime utcNow = DateTime.UtcNow;
				base.TraceDebug(string.Format("[{0} platform manifest published time (in utc) to Akamai: {1}, CurrentUTCTime: {2}]", text, forefrontdlManifestCreatedTimeInUtc, utcNow));
				if (base.AreManifestFilesOutOfSync(utcNow, forefrontdlManifestCreatedTimeInUtc, this.AllowedMaxPublishingIntervalTimeSpan))
				{
					flag = true;
					list.Add(base.RusEngine);
				}
			}
			base.TraceDebug(string.Format("[Has any engine platform exceeded publishing threshold: {0}]", flag));
			if (flag)
			{
				string arg = string.Join("\n", (from rusEngine in list
				select rusEngine.Platform + ", " + rusEngine.EngineName).ToArray<string>());
				string errorMsg = string.Format("Following Platform Engines did not publish to Akamai beyond their max allowed publishing timespan [{0}]. {1}", this.AllowedMaxPublishingIntervalTimeSpan, arg);
				base.LogTraceErrorAndThrowApplicationException(errorMsg);
			}
			base.TraceDebug("RusAkamaiPackagesVerifierProbe finished with success.");
		}

		private const string ProbeParamXmlNode = "//RusAkamaiPackagesVerifierProbeParam";

		private const string AllowedMaxPublishingIntervalTimeSpanXmlAttribute = "AllowedMaxPublishingIntervalTimeSpan";

		private static TimeSpan defaultAllowedPublishingInterval = TimeSpan.FromHours(24.0);

		private static TimeSpan minimumAllowedPublishingInterval = TimeSpan.FromHours(1.0);

		private static TimeSpan maximumAllowedPublishingInterval = TimeSpan.FromDays(3.0);
	}
}
