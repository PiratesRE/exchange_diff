using System;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal class ProbeRecord
	{
		public EdgeSyncCredential Credential;

		public long LastProbedTime;

		public bool Confirmed;
	}
}
