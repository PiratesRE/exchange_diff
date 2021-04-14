using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class CobaltStoreCleaner
	{
		private CobaltStoreCleaner(TimeSpan cleaningInterval, TimeSpan expirationTime)
		{
			this.cleaningInterval = cleaningInterval;
			this.expirationTime = expirationTime;
		}

		public static CobaltStoreCleaner GetInstance()
		{
			if (CobaltStoreCleaner.singleton == null)
			{
				lock (CobaltStoreCleaner.syncObject)
				{
					if (CobaltStoreCleaner.singleton == null)
					{
						CobaltStoreCleaner.singleton = new CobaltStoreCleaner(WacConfiguration.Instance.CobaltStoreCleanupInterval, WacConfiguration.Instance.CobaltStoreExpirationInterval);
					}
				}
			}
			return CobaltStoreCleaner.singleton;
		}

		internal static CobaltStoreCleaner GetTestInstance(TimeSpan cleaningInterval, TimeSpan expirationTime)
		{
			return new CobaltStoreCleaner(cleaningInterval, expirationTime);
		}

		public void Clean(IList<string> rootDirectories)
		{
			if (this.IsCleaningOverdue())
			{
				lock (CobaltStoreCleaner.syncObject)
				{
					if (this.IsCleaningOverdue())
					{
						SimulatedWebRequestContext.ExecuteWithoutUserContext("WAC.CleanCobaltStore", delegate(RequestDetailsLogger logger)
						{
							WacUtilities.SetEventId(logger, "WAC.CleanCobaltStore");
							foreach (string text in rootDirectories)
							{
								CobaltStoreCleaner.ValidatePath(text);
								try
								{
									this.CleanRootDirectory(text);
								}
								catch (DirectoryNotFoundException)
								{
								}
							}
						});
					}
				}
			}
		}

		private static void ValidatePath(string path)
		{
			if (!path.Contains("OwaCobalt"))
			{
				throw new InvalidOperationException("This path is not in the expected directory: " + path);
			}
		}

		private bool IsCleaningOverdue()
		{
			ExDateTime t = this.lastCleaning + this.cleaningInterval;
			return ExDateTime.UtcNow > t;
		}

		private void CleanRootDirectory(string rootDirectory)
		{
			string[] directories = Directory.GetDirectories(rootDirectory);
			foreach (string cobaltStoreDirectory in directories)
			{
				this.CleanCobaltStoreDirectory(cobaltStoreDirectory);
			}
		}

		private void CleanCobaltStoreDirectory(string cobaltStoreDirectory)
		{
			ExDateTime oldestFileTime = this.GetOldestFileTime(cobaltStoreDirectory);
			ExDateTime t = ExDateTime.UtcNow.Subtract(this.expirationTime);
			if (oldestFileTime < t)
			{
				CobaltStoreCleaner.ValidatePath(cobaltStoreDirectory);
				try
				{
					Directory.Delete(cobaltStoreDirectory, true);
				}
				catch (IOException)
				{
				}
			}
		}

		private ExDateTime GetOldestFileTime(string cobaltStoreDirectory)
		{
			ExDateTime exDateTime = ExDateTime.MinValue;
			string[] files = Directory.GetFiles(cobaltStoreDirectory, "*", SearchOption.AllDirectories);
			foreach (string path in files)
			{
				ExDateTime exDateTime2 = new ExDateTime(ExTimeZone.UtcTimeZone, File.GetLastAccessTimeUtc(path));
				if (exDateTime2 > exDateTime)
				{
					exDateTime = exDateTime2;
				}
			}
			return exDateTime;
		}

		private static readonly object syncObject = new object();

		private static CobaltStoreCleaner singleton;

		private readonly TimeSpan cleaningInterval;

		private readonly TimeSpan expirationTime;

		private ExDateTime lastCleaning = ExDateTime.MinValue;
	}
}
