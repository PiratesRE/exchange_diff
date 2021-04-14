using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IContentProperty : IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		Stream Body { get; }

		long Size { get; }

		bool IsIrmErrorMessage { get; }

		Stream GetData(BodyType type, long truncationSize, out long estimatedDataSize, out IEnumerable<AirSyncAttachmentInfo> attachments);

		BodyType GetNativeType();

		void PreProcessProperty();

		void PostProcessProperty();
	}
}
