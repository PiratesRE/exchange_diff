using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFileSystemPhotoReader
	{
		PhotoMetadata Read(string photoFullPath, Stream output);

		int ReadThumbprint(string photoFullPath);

		DateTime GetLastModificationTimeUtc(string photoFullPath);
	}
}
