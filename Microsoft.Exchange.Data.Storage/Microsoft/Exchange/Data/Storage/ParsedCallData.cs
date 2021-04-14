using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ParsedCallData
	{
		public SmtpAddress Mailbox { get; set; }

		public string DeviceId { get; set; }

		public string DeviceType { get; set; }

		public string SyncStateName { get; set; }

		public bool Metadata { get; set; }

		public bool FidMapping { get; set; }
	}
}
