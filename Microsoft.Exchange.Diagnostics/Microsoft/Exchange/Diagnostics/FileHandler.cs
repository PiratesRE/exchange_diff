using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class FileHandler : IDisposable
	{
		private event Action ChangedEvent;

		public event Action Changed
		{
			add
			{
				lock (this.locker)
				{
					this.ChangedEvent += value;
				}
			}
			remove
			{
				lock (this.locker)
				{
					this.ChangedEvent -= value;
				}
			}
		}

		public void Dispose()
		{
			this.fileWatcher.Dispose();
		}

		internal FileHandler(string filePath)
		{
			this.Initialize(filePath);
		}

		internal void ChangeFile(string filePath)
		{
			this.fileWatcher.Dispose();
			this.Initialize(filePath);
			this.FileChangeHandler();
		}

		private void Initialize(string filePath)
		{
			this.fileWatcher = new FileSystemWatcherTimer(filePath, new Action(this.FileChangeHandler));
		}

		private void FileChangeHandler()
		{
			Action action = null;
			lock (this.locker)
			{
				action = this.ChangedEvent;
			}
			if (action != null)
			{
				action();
			}
		}

		private FileSystemWatcherTimer fileWatcher;

		private object locker = new object();
	}
}
