using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IAggregateSession
	{
		MbxReadMode MbxReadMode { get; set; }

		BackendWriteMode BackendWriteMode { get; set; }
	}
}
