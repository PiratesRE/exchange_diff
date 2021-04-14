using System;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class DialInInformation
	{
		public DialInRegions DialInRegions { get; set; }

		public string ExternalDirectoryUri { get; set; }

		public string InternalDirectoryUri { get; set; }

		public bool IsAudioConferenceProviderEnabled { get; set; }

		public string ParticipantPassCode { get; set; }

		public string[] TollFreeNumbers { get; set; }

		public string TollNumber { get; set; }
	}
}
