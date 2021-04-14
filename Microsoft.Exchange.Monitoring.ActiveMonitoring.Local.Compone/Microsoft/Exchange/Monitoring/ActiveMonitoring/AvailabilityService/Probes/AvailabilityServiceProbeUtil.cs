using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.AvailabilityService.Probes
{
	public static class AvailabilityServiceProbeUtil
	{
		static AvailabilityServiceProbeUtil()
		{
			AvailabilityServiceProbeUtil.KnownErrors[333.ToString()] = AvailabilityServiceProbeUtil.FailingComponent.XSO;
			AvailabilityServiceProbeUtil.KnownErrors["AutoDiscover Failed"] = AvailabilityServiceProbeUtil.FailingComponent.AutoD;
			AvailabilityServiceProbeUtil.KnownErrors[259.ToString()] = AvailabilityServiceProbeUtil.FailingComponent.Ignore;
			AvailabilityServiceProbeUtil.KnownErrors[261.ToString()] = AvailabilityServiceProbeUtil.FailingComponent.Ignore;
			AvailabilityServiceProbeUtil.KnownErrors[264.ToString()] = AvailabilityServiceProbeUtil.FailingComponent.Ignore;
			AvailabilityServiceProbeUtil.KnownErrors["No Active Copy Database On Mailbox"] = AvailabilityServiceProbeUtil.FailingComponent.Ignore;
			AvailabilityServiceProbeUtil.KnownErrors["Server In Maintenance"] = AvailabilityServiceProbeUtil.FailingComponent.Ignore;
			AvailabilityServiceProbeUtil.KnownErrors["Local Request Must Be ActiveCopy"] = AvailabilityServiceProbeUtil.FailingComponent.Ignore;
			AvailabilityServiceProbeUtil.KnownErrors["Server Too Busy"] = AvailabilityServiceProbeUtil.FailingComponent.EWS;
			AvailabilityServiceProbeUtil.KnownErrors[382.ToString()] = AvailabilityServiceProbeUtil.FailingComponent.EWS;
			AvailabilityServiceProbeUtil.KnownErrors[74.ToString()] = AvailabilityServiceProbeUtil.FailingComponent.EWS;
			AvailabilityServiceProbeUtil.KnownErrors["Probe Time Out"] = AvailabilityServiceProbeUtil.FailingComponent.Monitoring;
		}

		public const string NoActiveCopyDatabaseOnMailbox = "No Active Copy Database On Mailbox";

		public const string ServerInMaintenance = "Server In Maintenance";

		public const string LocalRequestMustBeActiveCopy = "Local Request Must Be ActiveCopy";

		public const string ProbeTimeOut = "Probe Time Out";

		public const string ServerTooBusy = "Server Too Busy";

		public const string AutoDFailed = "AutoDiscover Failed";

		public static Dictionary<string, AvailabilityServiceProbeUtil.FailingComponent> KnownErrors = new Dictionary<string, AvailabilityServiceProbeUtil.FailingComponent>();

		public enum FailingComponent
		{
			Unknown = 1,
			EWS,
			XSO,
			AutoD,
			Monitoring,
			AvailabilityService,
			Ignore
		}
	}
}
