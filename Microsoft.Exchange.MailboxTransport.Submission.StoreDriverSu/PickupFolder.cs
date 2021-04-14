using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class PickupFolder
	{
		public bool WriteMessage(TransportMailItem mailItem, IEnumerable<MailRecipient> recipients, out SmtpResponse smtpResponse, out string exceptionMessage)
		{
			Stream stream = null;
			bool result = false;
			exceptionMessage = null;
			smtpResponse = SmtpResponse.NoopOk;
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}-{1}-{2}.{3}.eml", new object[]
				{
					Environment.MachineName,
					mailItem.RecordId,
					DateTime.UtcNow.ToString("yyyyMMddHHmmssZ", DateTimeFormatInfo.InvariantInfo),
					((IQueueItem)mailItem).Priority
				});
				string path = Path.Combine(this.dropDirectory, stringBuilder.ToString());
				if (!ExportStream.TryCreate(mailItem, recipients, true, out stream) || stream == null)
				{
					throw new InvalidOperationException("Failed to create an export stream because there were no ready recipients");
				}
				using (stream)
				{
					using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
					{
						stream.Position = 0L;
						for (;;)
						{
							int num = stream.Read(this.buffer, 0, 65536);
							if (num == 0)
							{
								break;
							}
							fileStream.Write(this.buffer, 0, num);
						}
					}
				}
				result = true;
			}
			catch (PathTooLongException)
			{
				smtpResponse = AckReason.GWPathTooLongException;
			}
			catch (IOException ex)
			{
				exceptionMessage = ex.Message;
				smtpResponse = AckReason.GWIOException;
			}
			catch (UnauthorizedAccessException)
			{
				smtpResponse = AckReason.GWUnauthorizedAccess;
			}
			return result;
		}

		private bool DropDirectoryExists()
		{
			string rootDropDirectoryPath = Components.Configuration.LocalServer.TransportServer.RootDropDirectoryPath;
			this.dropDirectory = rootDropDirectoryPath;
			this.directoryInfo = new DirectoryInfo(this.dropDirectory);
			return this.directoryInfo.Exists;
		}

		private const int BlockSize = 65536;

		private DirectoryInfo directoryInfo;

		private string dropDirectory = "C:\\root\\";

		private byte[] buffer = new byte[65536];
	}
}
