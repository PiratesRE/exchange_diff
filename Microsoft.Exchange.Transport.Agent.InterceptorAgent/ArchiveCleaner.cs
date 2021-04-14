using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal class ArchiveCleaner : IDisposable
	{
		public ArchiveCleaner(string archivePath, TimeSpan maximumArchivedItemAge, int maximumArchivedItemCount, long maximumArchiveSize)
		{
			this.archivePath = archivePath;
			this.maximumArchivedItemAge = maximumArchivedItemAge;
			this.maximumArchivedItemCount = maximumArchivedItemCount;
			this.maximumArchiveSize = maximumArchiveSize;
		}

		public void StartMonitoring(TimeSpan cleanupInterval)
		{
			if (this.timer != null)
			{
				this.timer.Change(cleanupInterval, cleanupInterval);
				return;
			}
			this.timer = new Timer(new TimerCallback(this.CleanupProcedure), null, cleanupInterval, cleanupInterval);
		}

		public void StopMonitoring()
		{
			if (this.timer != null)
			{
				this.timer.Change(-1, -1);
			}
		}

		public void Dispose()
		{
			if (this.timer != null)
			{
				this.StopMonitoring();
				this.timer.Dispose();
			}
		}

		private static void ReduceTotalArchiveSize(List<ArchiveCleaner.ArchivedItem> items, long targetSize)
		{
			long num = 0L;
			int num2 = 0;
			while (num2 < items.Count && (num += items[num2].Size) <= targetSize)
			{
				num2++;
			}
			for (int i = num2; i < items.Count; i++)
			{
				items[i].Delete();
			}
			items.RemoveRange(num2, items.Count - num2);
		}

		private static void ReduceTotalArchivedItemsCount(List<ArchiveCleaner.ArchivedItem> items, int targetCount)
		{
			if (items.Count <= targetCount)
			{
				return;
			}
			for (int i = targetCount; i < items.Count; i++)
			{
				items[i].Delete();
			}
			items.RemoveRange(targetCount, items.Count - targetCount);
		}

		private static void DeleteOldArchivedItems(IEnumerable<ArchiveCleaner.ArchivedItem> items, TimeSpan maximumAge)
		{
			foreach (ArchiveCleaner.ArchivedItem archivedItem in items)
			{
				archivedItem.DeleteIfOlderThan(maximumAge);
			}
		}

		private static long ScanDirectory(string path, out List<ArchiveCleaner.ArchivedItem> items)
		{
			ConcurrentBag<ArchiveCleaner.ArchivedItem> concurrentBag = new ConcurrentBag<ArchiveCleaner.ArchivedItem>();
			long result = ArchiveCleaner.ScanDirectory(path, concurrentBag);
			List<ArchiveCleaner.ArchivedItem> list;
			items = (list = concurrentBag.ToList<ArchiveCleaner.ArchivedItem>());
			list.Sort((ArchiveCleaner.ArchivedItem item1, ArchiveCleaner.ArchivedItem item2) => item1.CreationTime.CompareTo(item2.CreationTime));
			return result;
		}

		private static long ScanDirectory(string path, ConcurrentBag<ArchiveCleaner.ArchivedItem> concurrentBag)
		{
			long totalSize = 0L;
			foreach (string text in Directory.GetFiles(path, "*.eml"))
			{
				long length = new FileInfo(text).Length;
				concurrentBag.Add(new ArchiveCleaner.ArchivedItem(text, length));
				Interlocked.Add(ref totalSize, length);
			}
			string[] directories = Directory.GetDirectories(path);
			Parallel.For<long>(0, directories.Length, () => 0L, delegate(int i, ParallelLoopState loop, long subtotal)
			{
				if ((File.GetAttributes(directories[i]) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
				{
					subtotal += ArchiveCleaner.ScanDirectory(directories[i], concurrentBag);
					return subtotal;
				}
				return 0L;
			}, delegate(long x)
			{
				Interlocked.Add(ref totalSize, x);
			});
			return totalSize;
		}

		private static void DeleteEmptySubDirectories(string path)
		{
			foreach (string path2 in Directory.GetDirectories(path))
			{
				ArchiveCleaner.DeleteDirectoryIfEmpty(path2);
			}
		}

		private static void DeleteDirectoryIfEmpty(string path)
		{
			foreach (string path2 in Directory.GetDirectories(path))
			{
				ArchiveCleaner.DeleteDirectoryIfEmpty(path2);
			}
			if (Directory.GetDirectories(path).Length == 0 && Directory.GetFiles(path).Length == 0)
			{
				try
				{
					Directory.Delete(path, false);
				}
				catch (Exception arg)
				{
					ExTraceGlobals.InterceptorAgentTracer.TraceError<string, Exception>((long)typeof(ArchiveCleaner.ArchivedItem).GetHashCode(), "Error deleting directory '{0}': {1}", path, arg);
				}
			}
		}

		private void CleanupProcedure(object state)
		{
			using (Mutex mutex = new Mutex(false, "ArchiveCleanerMutex_FBA1B140EAEA42C286369027B6169594"))
			{
				if (mutex.WaitOne(0))
				{
					ExTraceGlobals.InterceptorAgentTracer.TraceInformation(0, (long)this.GetHashCode(), "Starting interceptor archive cleaunup. Path: {0}; Max item age: {1}; Max item count: {2}; Max archive size: {3}", new object[]
					{
						this.archivePath,
						this.maximumArchivedItemAge,
						this.maximumArchivedItemCount,
						this.maximumArchiveSize
					});
					try
					{
						List<ArchiveCleaner.ArchivedItem> list;
						long num = ArchiveCleaner.ScanDirectory(this.archivePath, out list);
						if (this.maximumArchiveSize > 0L && num > this.maximumArchiveSize)
						{
							ArchiveCleaner.ReduceTotalArchiveSize(list, this.maximumArchiveSize);
						}
						if (this.maximumArchivedItemCount > 0 && list.Count > this.maximumArchivedItemCount)
						{
							ArchiveCleaner.ReduceTotalArchivedItemsCount(list, this.maximumArchivedItemCount);
						}
						ArchiveCleaner.DeleteOldArchivedItems(list, this.maximumArchivedItemAge);
						ArchiveCleaner.DeleteEmptySubDirectories(this.archivePath);
					}
					catch (Exception arg)
					{
						ExTraceGlobals.InterceptorAgentTracer.TraceError<Exception>((long)this.GetHashCode(), "Error while cleaning interceptor archive: {0}", arg);
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
		}

		private const string CleanupProcMutexName = "ArchiveCleanerMutex_FBA1B140EAEA42C286369027B6169594";

		private readonly string archivePath;

		private readonly TimeSpan maximumArchivedItemAge;

		private readonly int maximumArchivedItemCount;

		private readonly long maximumArchiveSize;

		private Timer timer;

		private class ArchivedItem
		{
			public ArchivedItem(string path, long size)
			{
				this.Path = path;
				this.Size = size;
				this.CreationTime = File.GetCreationTimeUtc(path);
			}

			public string Path { get; private set; }

			public long Size { get; private set; }

			public DateTime CreationTime { get; private set; }

			public void DeleteIfOlderThan(TimeSpan age)
			{
				if (DateTime.UtcNow - this.CreationTime >= age)
				{
					this.Delete();
				}
			}

			public void Delete()
			{
				try
				{
					FileAttributes fileAttributes = File.GetAttributes(this.Path);
					if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
					{
						fileAttributes &= ~FileAttributes.ReadOnly;
						File.SetAttributes(this.Path, fileAttributes);
					}
					File.Delete(this.Path);
				}
				catch (Exception arg)
				{
					ExTraceGlobals.InterceptorAgentTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Error deleting file '{0}': {1}", this.Path, arg);
				}
			}
		}
	}
}
