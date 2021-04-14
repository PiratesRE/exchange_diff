using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoRequestLogFactory
	{
		public PhotoRequestLogFactory(PhotosConfiguration configuration, string build)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNullOrEmpty("build", build);
			this.build = build;
			this.configuration = configuration;
		}

		public PhotoRequestLog Create()
		{
			if (PhotoRequestLogFactory.logInstance == null)
			{
				lock (PhotoRequestLogFactory.SyncLock)
				{
					if (PhotoRequestLogFactory.logInstance == null)
					{
						PhotoRequestLogFactory.logInstance = new PhotoRequestLog(this.configuration, this.GetLogFilenamePrefix(), this.build);
					}
				}
			}
			return PhotoRequestLogFactory.logInstance;
		}

		private string GetLogFilenamePrefix()
		{
			return ApplicationName.Current.Name;
		}

		private static readonly object SyncLock = new object();

		private static PhotoRequestLog logInstance;

		private readonly PhotosConfiguration configuration;

		private readonly string build;
	}
}
