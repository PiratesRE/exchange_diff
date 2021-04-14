using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MailboxLoggerHandler : DisposeTrackableBase
	{
		public MailboxLoggerHandler(MailboxSession mailboxSession, string protocol, string clientName, bool clearOldLogs)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNullOrEmpty("protocol", protocol);
			ArgumentValidator.ThrowIfNullOrEmpty("clientName", clientName);
			this.mailboxLoggerHelper = new MailboxLogger(mailboxSession, protocol, clientName);
			if (this.Enabled)
			{
				this.currentDataTable = new Dictionary<MailboxLogDataName, string>();
				if (clearOldLogs)
				{
					this.mailboxLoggerHelper.ClearOldLogs(5000, 10485760L);
				}
			}
		}

		public LocalizedException LastError
		{
			get
			{
				return this.mailboxLoggerHelper.LastError;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.LastError == null;
			}
		}

		public bool LogsExist
		{
			get
			{
				return this.Enabled && this.mailboxLoggerHelper.LogsExist;
			}
		}

		public MailboxSession MailboxSession
		{
			get
			{
				if (!this.Enabled)
				{
					return null;
				}
				return this.mailboxLoggerHelper.Mailbox;
			}
			set
			{
				if (!this.Enabled)
				{
					return;
				}
				this.mailboxLoggerHelper.Mailbox = value;
			}
		}

		public bool DataExists(MailboxLogDataName name)
		{
			return this.Enabled && this.currentDataTable.ContainsKey(name);
		}

		public string GenerateReport()
		{
			if (!this.Enabled || this.MailboxSession == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(1024);
			char[] array = new char[1024];
			int num = 0;
			foreach (TextReader textReader in this.mailboxLoggerHelper)
			{
				stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "\r\n-----------------\r\n Log Entry: {0}\r\n-----------------\r\n\r\n", new object[]
				{
					num.ToString(CultureInfo.InvariantCulture)
				}));
				int charCount;
				while ((charCount = textReader.Read(array, 0, array.Length)) > 0)
				{
					stringBuilder.Append(array, 0, charCount);
				}
				num++;
			}
			return stringBuilder.ToString();
		}

		public void GenerateReport(Stream outputStream)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (!this.Enabled || this.MailboxSession == null)
			{
				return;
			}
			char[] array = new char[1024];
			byte[] array2 = new byte[Encoding.UTF8.GetMaxByteCount(array.Length)];
			int num = 0;
			foreach (TextReader textReader in this.mailboxLoggerHelper)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "\r\n-----------------\r\n Log Entry: {0}\r\n-----------------\r\n\r\n", new object[]
				{
					num.ToString(CultureInfo.InvariantCulture)
				});
				int bytes = Encoding.UTF8.GetBytes(text, 0, text.Length, array2, 0);
				outputStream.Write(array2, 0, bytes);
				int charCount;
				while ((charCount = textReader.Read(array, 0, array.Length)) > 0)
				{
					bytes = Encoding.UTF8.GetBytes(array, 0, charCount, array2, 0);
					outputStream.Write(array2, 0, bytes);
				}
				num++;
			}
		}

		public void SaveLogToMailbox()
		{
			if (!this.Enabled)
			{
				return;
			}
			this.mailboxLoggerHelper.WriteLog(this.GenerateLog());
		}

		public void AppendToLogInMailbox()
		{
			if (!this.Enabled)
			{
				return;
			}
			this.mailboxLoggerHelper.AppendLog(this.GenerateLog());
			this.mailboxLoggerHelper.Flush();
		}

		public void SetData(MailboxLogDataName name, object data)
		{
			if (!this.Enabled)
			{
				return;
			}
			this.currentDataTable[name] = data.ToString();
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.mailboxLoggerHelper != null)
			{
				this.mailboxLoggerHelper.Dispose();
				this.mailboxLoggerHelper = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxLoggerHandler>(this);
		}

		private string GenerateLog()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			foreach (KeyValuePair<MailboxLogDataName, string> keyValuePair in this.currentDataTable)
			{
				stringBuilder.Append(keyValuePair.Key).Append(" : ").AppendLine();
				stringBuilder.Append(keyValuePair.Value).AppendLine().AppendLine();
			}
			this.currentDataTable = new Dictionary<MailboxLogDataName, string>();
			return stringBuilder.ToString();
		}

		private const int MaxNumberOfMailboxLogs = 5000;

		private const long MaxTotalLogSize = 10485760L;

		private const string LogHeader = "\r\n-----------------\r\n Log Entry: {0}\r\n-----------------\r\n\r\n";

		private Dictionary<MailboxLogDataName, string> currentDataTable;

		private MailboxLogger mailboxLoggerHelper;
	}
}
