using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IAirSyncAttachments
	{
		IEnumerable<AirSyncAttachmentInfo> Attachments { get; }
	}
}
