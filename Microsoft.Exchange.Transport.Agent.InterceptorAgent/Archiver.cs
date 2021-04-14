using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal class Archiver : IDisposable
	{
		private Archiver(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			if (!Path.IsPathRooted(path))
			{
				throw new ArgumentException("Use absolute path");
			}
			this.archivePath = path;
			this.archiveCleaner = new ArchiveCleaner(this.archivePath, Archiver.MaximumArchivedItemAge, 0, 0L);
			this.archiveCleaner.StartMonitoring(Archiver.ArchiveCleanupInterval);
		}

		public static Archiver Instance
		{
			get
			{
				if (Archiver.instance == null)
				{
					throw new InvalidOperationException();
				}
				return Archiver.instance;
			}
		}

		public string ArchivedMessagesPath
		{
			get
			{
				string result;
				if ((result = this.archivedMessagesPath) == null)
				{
					result = (this.archivedMessagesPath = Path.Combine(this.archivePath, InterceptorAgentSettings.ArchivedMessagesDirectory));
				}
				return result;
			}
		}

		public string ArchivedMessageHeadersPath
		{
			get
			{
				string result;
				if ((result = this.archivedMessageHeadersPath) == null)
				{
					result = (this.archivedMessageHeadersPath = Path.Combine(this.archivePath, InterceptorAgentSettings.ArchivedMessageHeadersDirectory));
				}
				return result;
			}
		}

		public static void CreateArchiver(string directory)
		{
			if (Archiver.instance == null)
			{
				lock (Archiver.syncRoot)
				{
					if (Archiver.instance == null)
					{
						Archiver archiver = new Archiver(directory);
						Thread.MemoryBarrier();
						Archiver.instance = archiver;
					}
				}
			}
		}

		public void Dispose()
		{
			this.archiveCleaner.Dispose();
		}

		public bool TryArchiveHeaders(MailItem mail, string relativePath)
		{
			return this.TryArchive(mail, relativePath, true);
		}

		public bool TryArchiveMessage(MailItem mail, string relativePath)
		{
			return this.TryArchive(mail, relativePath, false);
		}

		private static bool WriteMessage(FileStream stream, TransportMailItem tmi)
		{
			byte[] buffer = new byte[65536];
			Stream stream2;
			if (!ExportStream.TryCreate(tmi, tmi.Recipients, true, out stream2) || stream2 == null)
			{
				throw new InvalidOperationException("Failed to create an export stream because there were no ready recipients");
			}
			using (stream2)
			{
				stream2.Position = 0L;
				for (;;)
				{
					int num = stream2.Read(buffer, 0, 65536);
					if (num == 0)
					{
						break;
					}
					stream.Write(buffer, 0, num);
				}
			}
			return true;
		}

		private static bool WriteMessageHeaders(FileStream stream, TransportMailItem tmi)
		{
			if (!ExportStream.TryWriteReplayXHeaders(stream, tmi, tmi.Recipients, false))
			{
				throw new InvalidOperationException("Recipient cannot be written");
			}
			return true;
		}

		private static string CreateFolder(string root, string relativePath)
		{
			string text = Path.Combine(root, relativePath);
			try
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
			catch (UnauthorizedAccessException arg)
			{
				ExTraceGlobals.InterceptorAgentTracer.TraceError<string, UnauthorizedAccessException>((long)typeof(Archiver).GetHashCode(), "Interceptor agent does not have permissions to write to this directory '{0}': {1}", text, arg);
				Util.EventLog.LogEvent(TransportEventLogConstants.Tuple_InterceptorAgentAccessDenied, null, new object[]
				{
					text
				});
			}
			catch (Exception arg2)
			{
				ExTraceGlobals.InterceptorAgentTracer.TraceInformation<string, Exception>(0, (long)typeof(Archiver).GetHashCode(), "Exception when trying to create directory '{0}': {1}", text, arg2);
			}
			return null;
		}

		private static TransportMailItem GetTransportMailItem(MailItem mailItem)
		{
			TransportMailItemWrapper transportMailItemWrapper = mailItem as TransportMailItemWrapper;
			if (transportMailItemWrapper == null)
			{
				return null;
			}
			return transportMailItemWrapper.TransportMailItem;
		}

		private bool TryArchive(MailItem mail, string relativePath, bool headersOnly)
		{
			TransportMailItem transportMailItem = Archiver.GetTransportMailItem(mail);
			if (transportMailItem == null)
			{
				return false;
			}
			bool result = false;
			Exception ex = null;
			string text = Archiver.CreateFolder(headersOnly ? this.ArchivedMessageHeadersPath : this.ArchivedMessagesPath, relativePath);
			if (text == null)
			{
				return false;
			}
			string path = string.Format("{0}-{1}-{2}.{3}.eml", new object[]
			{
				Environment.MachineName,
				transportMailItem.RecordId,
				DateTime.UtcNow.ToString("yyyyMMddHHmmssZ", DateTimeFormatInfo.InvariantInfo),
				((IQueueItem)transportMailItem).Priority
			});
			string path2 = Path.Combine(text, path);
			try
			{
				using (FileStream fileStream = new FileStream(path2, FileMode.CreateNew, FileAccess.Write, FileShare.None))
				{
					result = (headersOnly ? Archiver.WriteMessageHeaders(fileStream, transportMailItem) : Archiver.WriteMessage(fileStream, transportMailItem));
				}
			}
			catch (UnauthorizedAccessException ex2)
			{
				Util.EventLog.LogEvent(TransportEventLogConstants.Tuple_InterceptorAgentAccessDenied, null, new object[]
				{
					Path.GetDirectoryName(path2)
				});
				ex = ex2;
			}
			catch (Exception ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.InterceptorAgentTracer.TraceError<string>((long)this.GetHashCode(), "Interceptor agent encountered an error: {0}", ex.Message);
			}
			return result;
		}

		private const int BlockSize = 65536;

		private static readonly TimeSpan MaximumArchivedItemAge = TimeSpan.FromDays(14.0);

		private static readonly TimeSpan ArchiveCleanupInterval = TimeSpan.FromHours(12.0);

		private static readonly object syncRoot = new object();

		private static Archiver instance;

		private readonly string archivePath;

		private readonly ArchiveCleaner archiveCleaner;

		private string archivedMessagesPath;

		private string archivedMessageHeadersPath;
	}
}
