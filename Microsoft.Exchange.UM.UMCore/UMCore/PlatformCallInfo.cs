using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PlatformCallInfo
	{
		public abstract string CallId { get; }

		public abstract PlatformTelephonyAddress CalledParty { get; }

		public abstract PlatformTelephonyAddress CallingParty { get; }

		public abstract ReadOnlyCollection<PlatformDiversionInfo> DiversionInfo { get; }

		public abstract string FromTag { get; }

		public abstract ReadOnlyCollection<PlatformSignalingHeader> RemoteHeaders { get; }

		public abstract string RemoteUserAgent { get; }

		public abstract PlatformSipUri RequestUri { get; }

		public abstract string ToTag { get; }

		public abstract X509Certificate RemoteCertificate { get; }

		public abstract string ApplicationAor { get; }

		public abstract string RemoteMatchedFQDN { get; set; }

		public abstract IPAddress RemotePeer { get; }

		public abstract bool IsInbound { get; }

		public abstract bool IsServiceRequest { get; }
	}
}
