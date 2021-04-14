using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class AttachmentLog : IDisposable
	{
		public AttachmentLog(string logFileName, string logHeader)
		{
			this.logFileName = logFileName;
			this.logFullPathFileName = Path.Combine(Path.GetTempPath(), logFileName);
			this.logWriter = new StreamWriter(this.logFullPathFileName);
			this.logWriter.AutoFlush = true;
			if (!string.IsNullOrEmpty(logHeader))
			{
				this.logWriter.WriteLine(logHeader);
			}
		}

		public void WriteLogs(IEnumerable<string> logEntries)
		{
			foreach (string value in logEntries)
			{
				this.logWriter.WriteLine(value);
			}
		}

		public byte[] GetCompressedLogData()
		{
			return this.CreateZipFileAndGetTheContent();
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				if (this.logWriter != null)
				{
					this.logWriter.Flush();
					this.logWriter.Dispose();
					this.logWriter = null;
				}
				if (File.Exists(this.logFullPathFileName))
				{
					File.Delete(this.logFullPathFileName);
				}
				this.disposed = true;
			}
		}

		private byte[] CreateZipFileAndGetTheContent()
		{
			byte[] array = null;
			this.logWriter.Flush();
			this.logWriter.Dispose();
			this.logWriter = null;
			string path = Path.GetTempFileName() + ".zip";
			try
			{
				using (Package package = Package.Open(path, FileMode.CreateNew, FileAccess.ReadWrite))
				{
					PackagePart packagePart = package.CreatePart(new Uri(Uri.EscapeUriString("/" + this.logFileName), UriKind.Relative), "application/zip", CompressionOption.Maximum);
					using (StreamReader streamReader = new StreamReader(this.logFullPathFileName))
					{
						using (StreamWriter streamWriter = new StreamWriter(packagePart.GetStream(FileMode.Create, FileAccess.Write)))
						{
							char[] buffer = new char[1000];
							for (;;)
							{
								int num = streamReader.Read(buffer, 0, 1000);
								if (num <= 0)
								{
									break;
								}
								streamWriter.Write(buffer, 0, num);
							}
						}
					}
				}
				using (Stream stream = new FileStream(path, FileMode.Open))
				{
					array = new byte[stream.Length];
					stream.Seek(0L, SeekOrigin.Begin);
					stream.Read(array, 0, (int)stream.Length);
				}
			}
			finally
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
			this.logWriter = new StreamWriter(this.logFullPathFileName, true);
			return array;
		}

		internal const string CsvFileExtensionName = ".csv";

		internal const string ZipFileExtensionName = ".zip";

		internal const int BufferSize = 1000;

		private readonly string logFileName;

		private readonly string logFullPathFileName;

		private bool disposed;

		private StreamWriter logWriter;
	}
}
