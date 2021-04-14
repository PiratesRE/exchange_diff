using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Transport.Pickup
{
	internal sealed class DirectoryScanner
	{
		public DirectoryScanner(string directory, int maxFilesPerMinute, string filter, DirectoryScanner.FileFoundCallBack fileFoundCallBack, DirectoryScanner.CheckDirectoryCallBack checkDirectorCallBack)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(directory);
			this.fileFoundCallBack = fileFoundCallBack;
			this.numberOfFilesPerPoll = maxFilesPerMinute / 12;
			this.numberOfFilesOnLastPoll = this.numberOfFilesPerPoll + maxFilesPerMinute % 12;
			this.checkDirectoryCallBack = checkDirectorCallBack;
			this.fullDirectoryPath = directoryInfo.FullName;
			this.fileList = new FileList(this.fullDirectoryPath, filter);
		}

		public void Start()
		{
			this.stopping = false;
			this.filePollTimer = new GuardedTimer(new TimerCallback(this.FilePoll), null, TimeSpan.Zero, TimeSpan.FromSeconds(5.0));
		}

		public void Stop()
		{
			this.stopping = true;
			if (this.filePollTimer != null)
			{
				this.filePollTimer.Dispose(true);
				this.filePollTimer = null;
			}
			this.fileList.Dispose();
		}

		private bool CheckDirectory()
		{
			return this.checkDirectoryCallBack(this.fullDirectoryPath);
		}

		private void FilePoll(object empty)
		{
			int num = this.numberOfFilesPerPoll;
			if (++this.currentPollCount == 12)
			{
				num = this.numberOfFilesOnLastPoll;
				this.currentPollCount = 0;
			}
			if (!this.CheckDirectory())
			{
				return;
			}
			int num2 = 0;
			string fullFilePath;
			ulong num3;
			while (this.fileList.GetNextFile(out fullFilePath, out num3))
			{
				if (this.stopping || ++num2 > num || !this.fileFoundCallBack(fullFilePath))
				{
					this.fileList.StopSearch();
					return;
				}
			}
		}

		private const int PollingIntervalSeconds = 5;

		private const int NumberOfPollsPerMinute = 12;

		private readonly int numberOfFilesOnLastPoll;

		private readonly int numberOfFilesPerPoll;

		private bool stopping;

		private GuardedTimer filePollTimer;

		private int currentPollCount;

		private FileList fileList;

		private string fullDirectoryPath;

		private DirectoryScanner.CheckDirectoryCallBack checkDirectoryCallBack;

		private DirectoryScanner.FileFoundCallBack fileFoundCallBack;

		public delegate bool CheckDirectoryCallBack(string fullDirectoryPath);

		public delegate bool FileFoundCallBack(string fullFilePath);
	}
}
