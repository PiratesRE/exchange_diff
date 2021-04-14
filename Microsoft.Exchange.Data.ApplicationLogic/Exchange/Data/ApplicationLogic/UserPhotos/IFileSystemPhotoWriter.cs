using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFileSystemPhotoWriter
	{
		void Write(string photoFullPath, int thumbprint, Stream photo);
	}
}
