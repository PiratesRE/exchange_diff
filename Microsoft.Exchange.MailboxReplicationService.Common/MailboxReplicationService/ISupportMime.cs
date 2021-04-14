using System;
using System.IO;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface ISupportMime
	{
		Stream GetMimeStream(MessageRec message, out PropValueData[] extraPropValues);

		ResourceHealthTracker RHTracker { get; }
	}
}
