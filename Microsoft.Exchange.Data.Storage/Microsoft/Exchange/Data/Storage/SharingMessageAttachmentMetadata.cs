using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SharingMessageAttachmentMetadata
	{
		public const string ContentType = "application/x-sharing-metadata-xml";

		public const string ContentDisposition = "sharing_metadata.xml";
	}
}
