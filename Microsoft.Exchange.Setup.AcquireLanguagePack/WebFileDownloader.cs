using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal class WebFileDownloader
	{
		public WebFileDownloader()
		{
			this.numOfThreads = 0;
		}

		public WebFileDownloader(int numOfThreadsToDownload)
		{
			this.userSpecifiedNoThreads = true;
			this.numOfThreads = numOfThreadsToDownload;
		}

		public event DownloaderErrorHandler DownloaderErrorEvent;

		public event DownloadProgressChangeHandler DownloadProgressEvent;

		public event DownloadCompletedHandler DownloadCompletedEvent;

		public event DownloadCanceledHandler DownloadCancelEvent;

		private long TotalFileSize { get; set; }

		private long TotalDownloaded { get; set; }

		public int PercentageDownloaded
		{
			get
			{
				if (this.TotalFileSize > 0L)
				{
					return (int)(this.TotalDownloaded * 100L / this.TotalFileSize);
				}
				return 0;
			}
		}

		public void StartDownloading(List<DownloadFileInfo> downloads, string saveTo)
		{
			Logger.LoggerMessage("Getting ready to download...");
			try
			{
				ValidationHelper.ThrowIfNullOrEmpty<DownloadFileInfo>(downloads, "downloads");
				ValidationHelper.ThrowIfDirectoryNotExist(saveTo, "saveTo");
				this.errorFound = false;
				this.cancelDownload = false;
				this.TotalDownloaded = 0L;
				this.QueryDownloadFileInfo(downloads, saveTo);
				List<DownloadFileInfo> list = this.CheckForPreReqsBeforeDownload(downloads, saveTo);
				foreach (DownloadFileInfo downloadFileInfo in list)
				{
					this.currentDownloadFileInfo = downloadFileInfo;
					if (!this.userSpecifiedNoThreads)
					{
						this.numOfThreads = Math.Min(5, Environment.ProcessorCount + 1);
					}
					this.numOfThreads = Math.Min(this.numOfThreads, 5);
					if (this.currentDownloadFileInfo.FileSize < 40960L)
					{
						this.numOfThreads = 1;
					}
					this.allThreads = new List<Thread>();
					this.seg = new Segmentator(this.numOfThreads);
					DownloadParameter[] array = this.seg.SegmentTheFile(this.currentDownloadFileInfo.FileSize);
					Logger.LoggerMessage("Number of Threads: " + this.numOfThreads);
					this.numOfSegments = array.Length;
					for (int i = 0; i < this.numOfSegments; i++)
					{
						Logger.LoggerMessage(string.Concat(new object[]
						{
							"Thread ",
							i,
							" Start Position: ",
							array[i].StartPosition,
							" End Position: ",
							array[i].EndPosition
						}));
						Thread thread = new Thread(new ParameterizedThreadStart(this.DownloadFile));
						thread.Name = i.ToString();
						this.allThreads.Add(thread);
						thread.Start(array[i]);
					}
					this.cleanupThread = new Thread(new ParameterizedThreadStart(this.CleanUp));
					this.cleanupThread.Start(array);
					this.cleanupThread.Join();
				}
				this.DownloadCompletedEvent(list.Count);
			}
			catch (Exception error)
			{
				this.FindErrorTypeAndNotify(error);
			}
		}

		public void StopDownloading()
		{
			Logger.LoggerMessage("User cancelling the download...");
			this.cancelDownload = true;
			Thread.MemoryBarrier();
		}

		private void QueryDownloadFileInfo(List<DownloadFileInfo> downloads, string saveTo)
		{
			for (int i = 0; i < downloads.Count; i++)
			{
				if (downloads[i].UriLink.ToString().IndexOf("http://") != 0)
				{
					throw new WebException(Strings.URLCantBeReached(downloads[i].UriLink.ToString()));
				}
				DownloadFileInfo value = downloads[i];
				HttpProtocol.QueryFileNameSize(ref value);
				downloads[i] = value;
				downloads[i].FilePath = Path.Combine(saveTo, downloads[i].FilePath);
			}
		}

		private List<DownloadFileInfo> CheckForPreReqsBeforeDownload(List<DownloadFileInfo> downloads, string saveTo)
		{
			Logger.LoggerMessage("Checking the prechecks...");
			List<DownloadFileInfo> list = new List<DownloadFileInfo>();
			long num = 0L;
			foreach (DownloadFileInfo downloadFileInfo in downloads)
			{
				if (downloadFileInfo.FileSize <= 1024L)
				{
					throw new WebException(Strings.URLCantBeReached(downloadFileInfo.UriLink.ToString()));
				}
				if (!downloadFileInfo.IsFileNameValid())
				{
					if (!downloadFileInfo.IgnoreInvalidFileName)
					{
						throw new WebException(Strings.URLCantBeReached(downloadFileInfo.UriLink.ToString()));
					}
				}
				else
				{
					num += downloadFileInfo.FileSize;
					list.Add(downloadFileInfo);
				}
			}
			using (DiskSpaceValidator diskSpaceValidator = new DiskSpaceValidator((long)((double)num * 1.2), saveTo))
			{
				if (!diskSpaceValidator.Validate())
				{
					throw new IOException(Strings.InsufficientDiskSpace((double)num * 1.2));
				}
			}
			this.TotalFileSize = num;
			return list;
		}

		private void DownloadFile(object downloadParameters)
		{
			int num = -1;
			DownloadParameter downloadParameter = (DownloadParameter)downloadParameters;
			int num2 = 0;
			try
			{
				using (HttpProtocol httpProtocol = new HttpProtocol())
				{
					using (Stream stream = httpProtocol.GetStream(downloadParameter.StartPosition, downloadParameter.EndPosition, this.currentDownloadFileInfo.UriLink, this.numOfThreads))
					{
						if (stream == null)
						{
							throw new WebException(Strings.UnableToDownload);
						}
						using (FileStream fileStream = new FileStream(downloadParameter.PathToFile, FileMode.Create, FileAccess.Write, FileShare.None))
						{
							byte[] array = new byte[8096];
							while (num != 0)
							{
								num = stream.Read(array, 0, array.Length);
								num2 += num;
								fileStream.Write(array, 0, num);
								fileStream.Flush();
								lock (this)
								{
									this.TotalDownloaded += (long)num;
								}
								this.DownloadProgressEvent();
								if (this.cancelDownload)
								{
									httpProtocol.CancelDownload();
									break;
								}
							}
						}
					}
				}
			}
			catch (Exception error)
			{
				if (!this.errorFound)
				{
					Logger.LoggerMessage(string.Concat(new object[]
					{
						"This threaded has downloaded: ",
						num2,
						" -- Total Downloaded: ",
						this.TotalDownloaded
					}));
					this.errorFound = true;
					this.FindErrorTypeAndNotify(error);
				}
			}
		}

		private void DeleteTempFiles(DownloadParameter[] downloadParams)
		{
			try
			{
				Logger.LoggerMessage("Deleting the temp files...");
				for (int i = 0; i < this.numOfSegments; i++)
				{
					if (File.Exists(downloadParams[i].PathToFile))
					{
						File.Delete(downloadParams[i].PathToFile);
					}
				}
			}
			catch (IOException ex)
			{
				string message = ex.Message;
			}
		}

		private void CleanUp(object downloadParameters)
		{
			DownloadParameter[] array = (DownloadParameter[])downloadParameters;
			for (int i = 0; i < this.numOfSegments; i++)
			{
				this.allThreads[i].Join();
				Logger.LoggerMessage("Thread: " + i + " is done...");
			}
			Logger.LoggerMessage("All Threads are done...");
			try
			{
				if (!this.cancelDownload && !this.errorFound)
				{
					Logger.LoggerMessage("Attemptiing to append the temp files... ");
					this.AppendFile(array);
				}
				else if (this.cancelDownload)
				{
					this.DownloadCancelEvent();
				}
			}
			catch (Exception error)
			{
				this.FindErrorTypeAndNotify(error);
			}
			finally
			{
				this.DeleteTempFiles(array);
			}
		}

		private void AppendFile(DownloadParameter[] downloadParameters)
		{
			Logger.LoggerMessage("Appending the files...");
			if (File.Exists(this.currentDownloadFileInfo.FilePath))
			{
				File.Delete(this.currentDownloadFileInfo.FilePath);
			}
			using (FileStream fileStream = new FileStream(this.currentDownloadFileInfo.FilePath, FileMode.Append, FileAccess.Write, FileShare.None))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					for (int i = 0; i < this.numOfSegments; i++)
					{
						using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(downloadParameters[i].PathToFile)))
						{
							byte[] array = new byte[32768];
							int count;
							while ((count = binaryReader.Read(array, 0, array.Length)) > 0)
							{
								binaryWriter.Write(array, 0, count);
							}
						}
					}
				}
			}
			FileInfo fileInfo = new FileInfo(this.currentDownloadFileInfo.FilePath);
			if (fileInfo.Length != this.currentDownloadFileInfo.FileSize)
			{
				throw new ApplicationException(Strings.UnableToDownload);
			}
		}

		private void FindErrorTypeAndNotify(object error)
		{
			if (this.DownloaderErrorEvent != null)
			{
				if (error is AsyncCompletedEventArgs)
				{
					AsyncCompletedEventArgs asyncCompletedEventArgs = (AsyncCompletedEventArgs)error;
					WebDownloaderEventArgs.ErrorException = asyncCompletedEventArgs.Error;
				}
				else
				{
					WebDownloaderEventArgs.ErrorException = (Exception)error;
				}
				this.DownloaderErrorEvent(WebDownloaderEventArgs.ErrorException);
			}
		}

		public const int MaxNumOfThreadsAndSegments = 5;

		public const int MinFileSize = 40960;

		private const double InflateDiskSpace = 1.2;

		private const int MinValidFileSize = 1024;

		private List<Thread> allThreads;

		private Thread cleanupThread;

		private int numOfSegments;

		private int numOfThreads;

		private Segmentator seg;

		private DownloadFileInfo currentDownloadFileInfo;

		private volatile bool errorFound;

		private volatile bool cancelDownload;

		private bool userSpecifiedNoThreads;
	}
}
