using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoFileEnumerator
	{
		public PhotoFileEnumerator(PhotosConfiguration configuration, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.tracer = tracer;
			this.configuration = configuration;
		}

		public IEnumerable<FileInfo> Enumerate()
		{
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "Photo file enumerator: enumerating at {0}", this.configuration.PhotosRootDirectoryFullPath);
			if (!Directory.Exists(this.configuration.PhotosRootDirectoryFullPath))
			{
				return Array<FileInfo>.Empty;
			}
			return new DirectoryInfo(this.configuration.PhotosRootDirectoryFullPath).EnumerateFiles("*.jpg", SearchOption.AllDirectories);
		}

		private const string PhotoFilePattern = "*.jpg";

		private readonly ITracer tracer;

		private readonly PhotosConfiguration configuration;
	}
}
