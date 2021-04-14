using System;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class LrsSession
	{
		internal LrsSession(LrsLog lrsLog, LogRowFormatter row)
		{
			this.lrsLog = lrsLog;
			this.row = row;
		}

		public void SetEndpoints(IPEndPoint remoteEndPoint, IPEndPoint localEndPoint)
		{
			this.row[1] = remoteEndPoint.Address;
			this.row[2] = localEndPoint.Address;
		}

		public void SetEndpoints(string remoteEndPoint, string localEndPoint)
		{
			this.row[1] = remoteEndPoint;
			this.row[2] = localEndPoint;
		}

		public void LogMessage(IImapItemConverter converter, long totalBytes)
		{
			MimePartHeaders headers = converter.GetHeaders();
			Header header = headers["Message-Id"];
			Header header2 = headers["Subject"];
			Header header3 = headers["Sender"];
			if (header3 == null)
			{
				header3 = headers["From"];
			}
			AddressHeader addressHeader = header3 as AddressHeader;
			string value;
			if (addressHeader != null)
			{
				value = LrsSession.ListRecipients(addressHeader);
			}
			else
			{
				value = ((header3 != null) ? header3.Value : null);
			}
			uint num = LrsSession.CountRecipients(headers["To"] as AddressHeader) + LrsSession.CountRecipients(headers["Cc"] as AddressHeader) + LrsSession.CountRecipients(headers["Bcc"] as AddressHeader);
			lock (this.loggingLock)
			{
				this.row[0] = ExDateTime.UtcNow;
				this.row[3] = ((header != null) ? header.Value : null);
				this.row[9] = ((header2 != null) ? header2.Value : null);
				this.row[5] = value;
				this.row[7] = totalBytes;
				this.row[8] = num;
				this.lrsLog.Append(this.row);
			}
		}

		private static uint CountRecipients(AddressHeader recipients)
		{
			uint num = 0U;
			if (recipients != null)
			{
				foreach (AddressItem addressItem in recipients)
				{
					num += 1U;
				}
			}
			return num;
		}

		private static string ListRecipients(AddressHeader recipients)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (recipients != null)
			{
				foreach (AddressItem addressItem in recipients)
				{
					MimeRecipient mimeRecipient = addressItem as MimeRecipient;
					if (mimeRecipient != null && !string.IsNullOrEmpty(mimeRecipient.Email))
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append("; ");
						}
						stringBuilder.Append(mimeRecipient.Email);
					}
				}
			}
			return stringBuilder.ToString();
		}

		private LrsLog lrsLog;

		private LogRowFormatter row;

		private object loggingLock = new object();
	}
}
