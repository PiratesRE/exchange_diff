using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CacheCleaner : IUMAsyncComponent
	{
		public static IUMAsyncComponent Instance
		{
			get
			{
				return CacheCleaner.instance;
			}
		}

		public AutoResetEvent StoppedEvent
		{
			get
			{
				return this.syncEvent;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.initialized;
			}
		}

		public string Name
		{
			get
			{
				return "Local Disk Cache Cleaner";
			}
		}

		private string CleanupSentinelPath
		{
			get
			{
				string path = Path.Combine(Utils.GetExchangeDirectory(), "UnifiedMessaging");
				return Path.Combine(path, "CacheCleanup.bin");
			}
		}

		internal static void TouchUpDirectory(DirectoryInfo dir)
		{
			try
			{
				Directory.SetLastAccessTimeUtc(dir.FullName, (DateTime)ExDateTime.UtcNow);
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (IOException)
			{
			}
		}

		public void StartNow(StartupStage stage)
		{
			if (stage == StartupStage.WPActivation)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheCleaner.StartNow", new object[0]);
				this.cleanupTimer = new Timer(new TimerCallback(this.CleanupProcedure), null, this.GetTimeToCleanup(), this.cleanupInterval);
			}
			this.initialized = true;
		}

		public void StopAsync()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheCleaner.StopAsync", new object[0]);
			lock (this.lockObj)
			{
				this.shuttingDown = true;
				if (!this.cleaning)
				{
					this.syncEvent.Set();
				}
			}
		}

		public void CleanupAfterStopped()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheCleaner.CleanupAfterStopped", new object[0]);
			if (this.cleanupTimer != null)
			{
				this.cleanupTimer.Dispose();
			}
			if (this.syncEvent != null)
			{
				this.syncEvent.Close();
			}
		}

		internal void TestHookSetCleanupParameters(TimeSpan interval, string consumerName, ulong limit)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheCleaner.TestHookSetCleanupParameters", new object[0]);
			foreach (CacheCleaner.IDiskCacheConsumer diskCacheConsumer in this.cacheConsumers)
			{
				if (diskCacheConsumer.Name.Equals(consumerName, StringComparison.InvariantCultureIgnoreCase))
				{
					diskCacheConsumer.CacheSizeLimit = limit;
				}
			}
			this.cleanupInterval = interval;
			this.cleanupTimer.Change(interval, interval);
		}

		private static bool TrySafeDeleteDirectory(DateTime utcLastUsed, string directoryPath)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, null, "TrySafeDeleteDirectory", new object[0]);
			bool result = false;
			try
			{
				string fullName = Directory.GetParent(directoryPath).FullName;
				string text = Path.Combine(fullName, Guid.NewGuid().ToString());
				Directory.Move(directoryPath, text);
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, null, "Moved '{0}' to '{1}'", new object[]
				{
					directoryPath,
					text
				});
				Directory.SetLastAccessTimeUtc(text, utcLastUsed);
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, null, "Updated access time", new object[0]);
				Directory.Delete(text, true);
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, null, "Deleted", new object[0]);
				result = true;
			}
			catch (IOException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, null, "TrySafeDeleteDirectory exception='{0}'", new object[]
				{
					ex
				});
			}
			return result;
		}

		private void CleanupProcedure(object state)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheCleaner.CleanupProcedure", new object[0]);
			lock (this.lockObj)
			{
				this.cleaning = true;
			}
			this.bailedEarly = false;
			try
			{
				foreach (CacheCleaner.IDiskCacheConsumer diskCacheConsumer in this.cacheConsumers)
				{
					List<CacheCleaner.LRUInformation> lru = null;
					if (this.shuttingDown)
					{
						this.bailedEarly = true;
						break;
					}
					ulong num = diskCacheConsumer.CustomCleanupAndBuildLRU(out lru);
					if (num > diskCacheConsumer.CacheSizeLimit)
					{
						this.LruKick(lru, num, diskCacheConsumer.CacheSizeLimit);
					}
				}
				if (!this.bailedEarly)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "updating sentinel", new object[0]);
					using (File.Create(this.CleanupSentinelPath))
					{
					}
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Cleanup procedure encountered an exception='{0}'", new object[]
				{
					ex
				});
			}
			catch (IOException ex2)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Cleanup procedure encountered an exception='{0}'", new object[]
				{
					ex2
				});
			}
			finally
			{
				lock (this.lockObj)
				{
					this.cleaning = false;
					this.bailedEarly = false;
					if (this.shuttingDown)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Cleanup procedure setting shutdown flag", new object[0]);
						this.syncEvent.Set();
					}
				}
			}
		}

		private void LruKick(List<CacheCleaner.LRUInformation> lru, ulong totalBytes, ulong cacheLimitBytes)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheCleaner.LruKick", new object[0]);
			if (this.shuttingDown)
			{
				this.bailedEarly = true;
				return;
			}
			lru.Sort();
			ulong num = totalBytes;
			ulong num2 = (ulong)(cacheLimitBytes * 0.8);
			foreach (CacheCleaner.LRUInformation lruinformation in lru)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "TargetBytes='{0}', UpdatedBytes='{1}'", new object[]
				{
					num2,
					num
				});
				if (this.shuttingDown)
				{
					this.bailedEarly = true;
					break;
				}
				if (num < num2)
				{
					break;
				}
				if (CacheCleaner.TrySafeDeleteDirectory((DateTime)lruinformation.UtcLastUsed, lruinformation.Path))
				{
					num -= lruinformation.Bytes;
				}
			}
		}

		private TimeSpan GetTimeToCleanup()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheCleaner.GetTimeToCleanup", new object[0]);
			DateTime creationTimeUtc = File.GetCreationTimeUtc(this.CleanupSentinelPath);
			TimeSpan timeSpan = this.cleanupInterval - (DateTime.UtcNow - creationTimeUtc);
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = TimeSpan.FromMinutes(1.0);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "TimeToCleanup='{0}'", new object[]
			{
				timeSpan
			});
			return timeSpan;
		}

		public const string SentinelFileExtension = ".delete";

		private static CacheCleaner instance = new CacheCleaner();

		private bool initialized;

		private bool shuttingDown;

		private bool cleaning;

		private bool bailedEarly;

		private TimeSpan cleanupInterval = TimeSpan.FromHours(24.0);

		private AutoResetEvent syncEvent = new AutoResetEvent(false);

		private object lockObj = new object();

		private Timer cleanupTimer;

		private CacheCleaner.IDiskCacheConsumer[] cacheConsumers = new CacheCleaner.IDiskCacheConsumer[]
		{
			new CacheCleaner.CustomPromptDiskCacheConsumer(),
			new CacheCleaner.LargeGrammarsDiskCacheConsumer()
		};

		internal class LRUInformation : IComparable
		{
			public LRUInformation(string path, ulong bytes, ExDateTime utcLastUsed)
			{
				this.Path = path;
				this.Bytes = bytes;
				this.UtcLastUsed = utcLastUsed;
			}

			public string Path { get; set; }

			public ulong Bytes { get; set; }

			public ExDateTime UtcLastUsed { get; set; }

			public int CompareTo(object obj)
			{
				CacheCleaner.LRUInformation lruinformation = obj as CacheCleaner.LRUInformation;
				if (lruinformation != null)
				{
					return ExDateTime.Compare(lruinformation.UtcLastUsed, this.UtcLastUsed);
				}
				throw new ArgumentException("Object is not LRUInformation");
			}
		}

		private interface IDiskCacheConsumer
		{
			ulong CustomCleanupAndBuildLRU(out List<CacheCleaner.LRUInformation> lru);

			ulong CacheSizeLimit { get; set; }

			string Name { get; }
		}

		private class CustomPromptDiskCacheConsumer : CacheCleaner.IDiskCacheConsumer
		{
			public ulong CacheSizeLimit
			{
				get
				{
					return this.cacheSizeLimit;
				}
				set
				{
					this.cacheSizeLimit = value;
				}
			}

			public string Name
			{
				get
				{
					return base.GetType().Name;
				}
			}

			public CustomPromptDiskCacheConsumer()
			{
				this.cachePath = Path.Combine(Utils.GetExchangeDirectory(), "UnifiedMessaging\\Prompts\\Cache");
				this.cacheSizeLimit = 17179869184UL;
			}

			public ulong CustomCleanupAndBuildLRU(out List<CacheCleaner.LRUInformation> lru)
			{
				lru = new List<CacheCleaner.LRUInformation>();
				ulong num = 0UL;
				DirectoryInfo directoryInfo = new DirectoryInfo(this.cachePath);
				if (directoryInfo.Exists)
				{
					DirectoryInfo[] directories = directoryInfo.GetDirectories();
					foreach (DirectoryInfo directoryInfo2 in directories)
					{
						DirectoryInfo[] directories2 = directoryInfo2.GetDirectories();
						DateTime t = DateTime.MinValue;
						CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "computing newest directory", new object[0]);
						foreach (DirectoryInfo directoryInfo3 in directories2)
						{
							if (directoryInfo3.CreationTimeUtc > t)
							{
								t = directoryInfo3.CreationTimeUtc;
							}
						}
						CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "removing aged directories", new object[0]);
						foreach (DirectoryInfo directoryInfo4 in directories2)
						{
							if (directoryInfo4.CreationTimeUtc >= t || !CacheCleaner.TrySafeDeleteDirectory(directoryInfo4.LastAccessTimeUtc, directoryInfo4.FullName))
							{
								ulong num2 = 0UL;
								foreach (FileInfo fileInfo in directoryInfo4.GetFiles())
								{
									num += (ulong)fileInfo.Length;
									num2 += (ulong)fileInfo.Length;
								}
								lru.Add(new CacheCleaner.LRUInformation(directoryInfo4.FullName, num2, new ExDateTime(ExTimeZone.UtcTimeZone, directoryInfo4.LastAccessTimeUtc)));
							}
						}
					}
				}
				return num;
			}

			private readonly string cachePath;

			private ulong cacheSizeLimit;
		}

		internal class LargeGrammarsDiskCacheConsumer : CacheCleaner.IDiskCacheConsumer
		{
			public ulong CacheSizeLimit
			{
				get
				{
					return this.cacheSizeLimit;
				}
				set
				{
					this.cacheSizeLimit = value;
				}
			}

			public string Name
			{
				get
				{
					return base.GetType().Name;
				}
			}

			public LargeGrammarsDiskCacheConsumer()
			{
				this.cachePath = Path.Combine(Utils.GetExchangeDirectory(), "UnifiedMessaging\\grammars");
				this.cacheSizeLimit = 9223372036854775807UL;
			}

			public ulong CustomCleanupAndBuildLRU(out List<CacheCleaner.LRUInformation> lru)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "Entering LargeGrammarsDiskCacheConsumer.CustomCleanupAndBuildLRU", new object[0]);
				lru = new List<CacheCleaner.LRUInformation>();
				DirectoryInfo directoryInfo = new DirectoryInfo(this.cachePath);
				if (directoryInfo.Exists)
				{
					DirectoryInfo[] directories = directoryInfo.GetDirectories("Cache", SearchOption.AllDirectories);
					foreach (DirectoryInfo directoryInfo2 in directories)
					{
						DirectoryInfo[] directories2 = directoryInfo2.GetDirectories();
						foreach (DirectoryInfo directoryInfo3 in directories2)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "Checking tenant dir='{0}'", new object[]
							{
								directoryInfo3.FullName
							});
							FileInfo[] files = directoryInfo3.GetFiles("*.delete");
							foreach (FileInfo fileInfo in files)
							{
								if (DateTime.UtcNow - fileInfo.CreationTimeUtc > CacheCleaner.LargeGrammarsDiskCacheConsumer.CompiledGrammarExpiration)
								{
									string fullName = fileInfo.FullName;
									CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "Sentinel file='{0}' has expired", new object[]
									{
										fullName
									});
									string text = fullName.Substring(0, fullName.Length - ".delete".Length);
									CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "Deleting sentinel file='{0}' and grammarFile='{1}'", new object[]
									{
										fullName,
										text
									});
									File.Delete(fullName);
									if (File.Exists(text))
									{
										File.Delete(text);
									}
								}
							}
						}
					}
				}
				return 0UL;
			}

			private static readonly TimeSpan CompiledGrammarExpiration = TimeSpan.FromDays(1.0);

			private readonly string cachePath;

			private ulong cacheSizeLimit;
		}
	}
}
