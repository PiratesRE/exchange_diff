using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoMetadata
	{
		public long Length { get; set; }

		public string ContentType { get; set; }
	}
}
