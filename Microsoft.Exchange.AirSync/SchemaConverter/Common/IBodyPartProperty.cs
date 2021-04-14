using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IBodyPartProperty : IProperty
	{
		string Preview { get; }

		Stream GetData(BodyType type, long truncationSize, out long estimatedDataSize, out IEnumerable<AirSyncAttachmentInfo> attachments);
	}
}
