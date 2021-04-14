using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotosConfiguration
	{
		public PhotosConfiguration(string exchangeInstallPath, TimeSpan photoFileTimeToLive)
		{
			if (string.IsNullOrEmpty(exchangeInstallPath))
			{
				throw new ArgumentNullException("exchangeInstallPath");
			}
			this.exchangeInstallPath = exchangeInstallPath;
			this.photoFileTimeToLive = photoFileTimeToLive;
			this.photosRootDirectoryPath = PhotosConfiguration.ComputePhotosRootDirectory(exchangeInstallPath);
			this.garbageCollectionLoggingPath = PhotosConfiguration.ComputeGarbageCollectionLoggingPath(exchangeInstallPath);
			this.photoRequestLoggingPath = PhotosConfiguration.ComputePhotoRequestLoggingPath(exchangeInstallPath);
		}

		public PhotosConfiguration(string exchangeInstallPath) : this(exchangeInstallPath, PhotosConfiguration.DefaultPhotoFileTimeToLive)
		{
		}

		public UserPhotoSize PhotoSizeToUploadToAD
		{
			get
			{
				return UserPhotoSize.HR64x64;
			}
		}

		public string ExchangeInstallPath
		{
			get
			{
				return this.exchangeInstallPath;
			}
		}

		public TimeSpan PhotoFileTimeToLive
		{
			get
			{
				return this.photoFileTimeToLive;
			}
		}

		public ICollection<UserPhotoSize> SizesToCacheOnFileSystem
		{
			get
			{
				return PhotosConfiguration.DefaultSizesToCacheOnFileSystem;
			}
		}

		public TimeSpan UserAgentCacheTimeToLive
		{
			get
			{
				return PhotosConfiguration.DefaultUserAgentCacheTimeToLive;
			}
		}

		public TimeSpan UserAgentCacheTimeToLiveNotFound
		{
			get
			{
				return PhotosConfiguration.DefaultUserAgentCacheTimeToLiveNotFound;
			}
		}

		public string PhotoServiceEndpointRelativeToEwsWithLeadingSlash
		{
			get
			{
				return "/s/GetUserPhoto";
			}
		}

		public TimeSpan GarbageCollectionInterval
		{
			get
			{
				return PhotosConfiguration.DefaultGarbageCollectionInterval;
			}
		}

		public TimeSpan LastAccessGarbageThreshold
		{
			get
			{
				return PhotosConfiguration.DefaultLastAccessGarbageThreshold;
			}
		}

		public string GarbageCollectionLoggingPath
		{
			get
			{
				return this.garbageCollectionLoggingPath;
			}
		}

		public TimeSpan GarbageCollectionLogFileMaxAge
		{
			get
			{
				return PhotosConfiguration.DefaultGarbageCollectionLogFileMaxAge;
			}
		}

		public long GarbageCollectionLogDirectoryMaxSize
		{
			get
			{
				return 104857600L;
			}
		}

		public long GarbageCollectionLogFileMaxSize
		{
			get
			{
				return 10485760L;
			}
		}

		public string PhotoRequestLoggingPath
		{
			get
			{
				return this.photoRequestLoggingPath;
			}
		}

		public TimeSpan PhotoRequestLogFileMaxAge
		{
			get
			{
				return PhotosConfiguration.DefaultPhotoRequestLogFileMaxAge;
			}
		}

		public long PhotoRequestLogDirectoryMaxSize
		{
			get
			{
				return 104857600L;
			}
		}

		public long PhotoRequestLogFileMaxSize
		{
			get
			{
				return 10485760L;
			}
		}

		public string PhotosRootDirectoryFullPath
		{
			get
			{
				return this.photosRootDirectoryPath;
			}
		}

		public int OutgoingPhotoRequestTimeoutMilliseconds
		{
			get
			{
				return 5000;
			}
		}

		private static string ComputePhotosRootDirectory(string exchangeInstallPath)
		{
			return Path.Combine(exchangeInstallPath, "ClientAccess\\photos");
		}

		private static string ComputeGarbageCollectionLoggingPath(string exchangeInstallPath)
		{
			return Path.Combine(exchangeInstallPath, "Logging\\PhotoGarbageCollector");
		}

		private static string ComputePhotoRequestLoggingPath(string exchangeInstallPath)
		{
			return Path.Combine(exchangeInstallPath, "Logging\\photos");
		}

		internal const string GarbageCollectionLoggingComponentName = "PhotoGarbageCollector";

		internal const string PhotoRequestLoggingComponentName = "photos";

		private const UserPhotoSize DefaultPhotoSizeToUploadToAD = UserPhotoSize.HR64x64;

		private const string DefaultPhotoServiceEndpointRelativeToEwsWithLeadingSlash = "/s/GetUserPhoto";

		private const string PhotosPathRelativeToInstallPath = "ClientAccess\\photos";

		private const long DefaultGarbageCollectionLogDirectoryMaxSize = 104857600L;

		private const long DefaultGarbageCollectionLogFileMaxSize = 10485760L;

		private const long DefaultPhotoRequestLogDirectoryMaxSize = 104857600L;

		private const long DefaultPhotoRequestLogFileMaxSize = 10485760L;

		private const int DefaultOutgoingPhotoRequestTimeoutMilliseconds = 5000;

		private static readonly UserPhotoSize[] DefaultSizesToCacheOnFileSystem = new UserPhotoSize[]
		{
			UserPhotoSize.HR96x96,
			UserPhotoSize.HR648x648
		};

		private static readonly TimeSpan DefaultPhotoFileTimeToLive = TimeSpan.FromDays(7.0);

		private static readonly TimeSpan DefaultUserAgentCacheTimeToLive = TimeSpan.FromDays(3.0);

		private static readonly TimeSpan DefaultUserAgentCacheTimeToLiveNotFound = TimeSpan.FromDays(1.0);

		private static readonly TimeSpan DefaultGarbageCollectionInterval = TimeSpan.FromDays(7.0);

		private static readonly TimeSpan DefaultLastAccessGarbageThreshold = TimeSpan.FromDays(7.0);

		private static readonly TimeSpan DefaultGarbageCollectionLogFileMaxAge = TimeSpan.FromDays(90.0);

		private static readonly TimeSpan DefaultPhotoRequestLogFileMaxAge = TimeSpan.FromDays(30.0);

		private readonly string exchangeInstallPath;

		private readonly TimeSpan photoFileTimeToLive;

		private readonly string photosRootDirectoryPath;

		private readonly string garbageCollectionLoggingPath;

		private readonly string photoRequestLoggingPath;
	}
}
