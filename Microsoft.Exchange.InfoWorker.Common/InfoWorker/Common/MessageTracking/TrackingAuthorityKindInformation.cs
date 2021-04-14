using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TrackingAuthorityKindInformation : Attribute
	{
		public int ExpectedConnectionLatencyMSec { get; set; }
	}
}
