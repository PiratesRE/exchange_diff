using System;

namespace Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe
{
	internal class MapiSubmitMonitoringSystemProbeFactory : MapiSubmitSystemProbeFactory
	{
		private MapiSubmitMonitoringSystemProbeFactory()
		{
		}

		public static MapiSubmitMonitoringSystemProbeFactory CreateInstance()
		{
			return new MapiSubmitMonitoringSystemProbeFactory();
		}

		public override IMapiMessageSubmitter MakeMapiMessageSubmitter()
		{
			return MapiMessageSubmitter.CreateInstance();
		}
	}
}
