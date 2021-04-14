using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;

namespace Microsoft.Search.Platform.Parallax.DataLoad
{
	internal sealed class FileChangesAccumulator : IDisposable
	{
		internal FileChangesAccumulator(string directoryPath, string filter, int accumulationTimeoutInMs, bool processSubdirectories = false)
		{
			if (directoryPath == null)
			{
				throw new ArgumentNullException("directoryPath");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			this.changesListAccessGate = new object();
			this.filter = filter;
			this.directoryPath = directoryPath;
			this.processSubdirectories = processSubdirectories;
			this.accumulatedFileChanges = new List<FileChangesAccumulator.Change>();
			this.fileChangeAccumulationTimer = new Timer((double)accumulationTimeoutInMs)
			{
				AutoReset = false
			};
			this.fileChangeAccumulationTimer.Elapsed += this.ApplyFileChanges;
			this.fileWatcher = new FileSystemWatcher(this.directoryPath, this.filter)
			{
				IncludeSubdirectories = this.processSubdirectories
			};
			this.fileWatcher.Changed += this.LoadNewFile;
			this.fileWatcher.Created += this.LoadNewFile;
			this.fileWatcher.Deleted += this.FileDeleted;
			this.fileWatcher.Renamed += this.FileRenamed;
			this.fileWatcher.Error += this.FileWatcherError;
			this.fileWatcher.EnableRaisingEvents = true;
		}

		internal event EventHandler<IEnumerable<string>> ChangesAccumulated;

		internal event EventHandler<Exception> ErrorDetected;

		public void Dispose()
		{
			this.fileWatcher.Dispose();
			this.fileChangeAccumulationTimer.Dispose();
		}

		private void OnChangesAccumulated(IEnumerable<string> changedFiles)
		{
			EventHandler<IEnumerable<string>> changesAccumulated = this.ChangesAccumulated;
			if (changesAccumulated != null)
			{
				changesAccumulated(this, changedFiles);
			}
		}

		private void ApplyFileChanges(object sender, ElapsedEventArgs e)
		{
			List<FileChangesAccumulator.Change> list;
			lock (this.changesListAccessGate)
			{
				list = this.accumulatedFileChanges;
				this.accumulatedFileChanges = new List<FileChangesAccumulator.Change>();
			}
			this.ProcessOverlapping(list);
			IEnumerable<string> changedFiles = (from _ in list
			where !_.IsOverlapped
			select _.FilePath).ToArray<string>();
			this.OnChangesAccumulated(changedFiles);
		}

		private void ProcessOverlapping(List<FileChangesAccumulator.Change> currentlyAccumulatedChanges)
		{
			for (int i = currentlyAccumulatedChanges.Count; i > 1; i--)
			{
				FileChangesAccumulator.Change change = currentlyAccumulatedChanges[i - 1];
				if (!change.IsOverlapped)
				{
					for (int j = i - 1; j > 0; j--)
					{
						FileChangesAccumulator.Change change2 = currentlyAccumulatedChanges[j - 1];
						if (StringComparer.InvariantCultureIgnoreCase.Compare(change2.FilePath, change.FilePath) == 0)
						{
							change2.IsOverlapped = true;
						}
					}
				}
			}
		}

		private void RestartTimer()
		{
			this.fileChangeAccumulationTimer.Stop();
			this.fileChangeAccumulationTimer.Start();
		}

		private void StopTimer()
		{
			this.fileChangeAccumulationTimer.Stop();
		}

		private void FileDeleted(object sender, FileSystemEventArgs e)
		{
		}

		private void LoadNewFile(object sender, FileSystemEventArgs e)
		{
			this.RestartTimer();
			FileChangesAccumulator.Change item = new FileChangesAccumulator.Change(e.FullPath);
			lock (this.changesListAccessGate)
			{
				this.accumulatedFileChanges.Add(item);
			}
		}

		private void FileRenamed(object sender, RenamedEventArgs e)
		{
			this.RestartTimer();
			string directoryName = Path.GetDirectoryName(e.FullPath);
			string fileName = Path.GetFileName(e.FullPath);
			FileChangesAccumulator.Change change = null;
			if (this.IsDirectoryMonitored(directoryName) && this.FitsMask(fileName, this.filter))
			{
				change = new FileChangesAccumulator.Change(e.FullPath);
			}
			lock (this.changesListAccessGate)
			{
				if (change != null)
				{
					this.accumulatedFileChanges.Add(change);
				}
			}
		}

		private void FileWatcherError(object sender, ErrorEventArgs e)
		{
			this.StopTimer();
			EventHandler<Exception> errorDetected = this.ErrorDetected;
			if (errorDetected != null)
			{
				errorDetected(this, e.GetException());
			}
		}

		private bool IsDirectoryMonitored(string directoryName)
		{
			bool flag = this.directoryPath == directoryName;
			if (flag)
			{
				return true;
			}
			bool flag2 = directoryName.IndexOf(this.directoryPath + "\\", StringComparison.InvariantCulture) != -1;
			return this.processSubdirectories && flag2;
		}

		private bool FitsMask(string fileName, string fileMask)
		{
			if (this.fileNameFilter == null)
			{
				string pattern = "^" + fileMask.Replace(".", "\\.").Replace("*", ".*").Replace("?", ".") + "$";
				this.fileNameFilter = new Regex(pattern, RegexOptions.IgnoreCase);
			}
			return this.fileNameFilter.IsMatch(fileName);
		}

		private readonly string directoryPath;

		private readonly string filter;

		private readonly bool processSubdirectories;

		private readonly Timer fileChangeAccumulationTimer;

		private readonly FileSystemWatcher fileWatcher;

		private readonly object changesListAccessGate;

		private List<FileChangesAccumulator.Change> accumulatedFileChanges;

		private Regex fileNameFilter;

		[DebuggerDisplay("File: {FilePath}; IsOverlapped: {IsOverlapped}")]
		private class Change
		{
			public Change(string fileFullPath)
			{
				this.FilePath = fileFullPath;
			}

			public string FilePath { get; private set; }

			public bool IsOverlapped { get; set; }
		}
	}
}
