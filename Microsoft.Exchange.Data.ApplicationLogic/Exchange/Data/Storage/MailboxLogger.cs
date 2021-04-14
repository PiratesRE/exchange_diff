using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	internal sealed class MailboxLogger : DisposeTrackableBase, IEnumerable<TextReader>, IEnumerable
	{
		public MailboxLogger(MailboxSession mailboxSession, string protocolName) : this(mailboxSession, protocolName, null)
		{
		}

		public MailboxLogger(MailboxSession mailboxSession, string protocolName, string clientName)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (string.IsNullOrEmpty(protocolName))
			{
				throw new ArgumentNullException("protocolName");
			}
			this.Mailbox = mailboxSession;
			string displayName;
			if (string.IsNullOrEmpty(clientName))
			{
				displayName = protocolName;
			}
			else
			{
				displayName = protocolName + " " + clientName;
			}
			lock (this.Mailbox)
			{
				bool isConnected = this.Mailbox.IsConnected;
				try
				{
					if (!isConnected)
					{
						this.Mailbox.Connect();
					}
					using (Folder folder = Folder.Create(this.Mailbox, this.Mailbox.GetDefaultFolderId(DefaultFolderType.Configuration), StoreObjectType.Folder, displayName, CreateMode.OpenIfExists))
					{
						if (folder.Id == null)
						{
							folder.Save();
							folder.Load();
						}
						this.logFolderId = folder.Id.ObjectId;
						this.LogsExist = (folder.ItemCount > 0);
					}
				}
				catch (LocalizedException lastError)
				{
					this.LastError = lastError;
				}
				finally
				{
					if (!isConnected)
					{
						try
						{
							this.Mailbox.Disconnect();
						}
						catch (LocalizedException)
						{
						}
					}
				}
			}
		}

		public MailboxSession Mailbox { get; set; }

		public bool LogsExist { get; private set; }

		public LocalizedException LastError { get; private set; }

		private byte[] TempBuffer
		{
			get
			{
				if (this.tempBuffer == null)
				{
					if (MailboxLogger.bufferPool == null)
					{
						MailboxLogger.bufferPool = new BufferPool(30720);
					}
					this.tempBuffer = MailboxLogger.bufferPool.Acquire();
				}
				return this.tempBuffer;
			}
		}

		public void WriteLog(string logString)
		{
			if (logString == null)
			{
				throw new ArgumentNullException("logString");
			}
			if (this.LastError != null)
			{
				return;
			}
			lock (this.Mailbox)
			{
				byte[] array = null;
				if (Encoding.Unicode.GetByteCount(logString) <= this.TempBuffer.Length)
				{
					this.tempBufferDataSize = Encoding.Unicode.GetBytes(logString, 0, logString.Length, this.TempBuffer, 0);
				}
				else
				{
					array = Encoding.Unicode.GetBytes(logString);
				}
				this.streamType = MailboxLogger.StreamType.Unicode;
				this.InternalCreateNewLog(array, 0, (array == null) ? 0 : array.Length);
			}
		}

		public void WriteLog(byte[] logBuffer)
		{
			if (logBuffer == null)
			{
				throw new ArgumentNullException("logBuffer");
			}
			if (this.LastError != null)
			{
				return;
			}
			this.WriteLog(logBuffer, 0, logBuffer.Length);
		}

		public void WriteLog(byte[] logBuffer, int offset, int size)
		{
			if (logBuffer == null)
			{
				throw new ArgumentNullException("logBuffer");
			}
			if (offset < 0 || offset > logBuffer.Length)
			{
				throw new ArgumentException("offset");
			}
			if (size < 0 || offset + size > logBuffer.Length)
			{
				throw new ArgumentException("size");
			}
			if (this.LastError != null)
			{
				return;
			}
			lock (this.Mailbox)
			{
				this.streamType = MailboxLogger.StreamType.Ascii;
				this.InternalCreateNewLog(logBuffer, offset, size);
			}
		}

		public void AppendLog(string logString)
		{
			if (logString == null)
			{
				throw new ArgumentNullException("logString");
			}
			if (this.LastError != null)
			{
				return;
			}
			Encoding encoding;
			switch (this.streamType)
			{
			case MailboxLogger.StreamType.Ascii:
				encoding = Encoding.ASCII;
				break;
			case MailboxLogger.StreamType.Unicode:
				encoding = Encoding.Unicode;
				break;
			default:
				throw new InvalidOperationException("Unknown streamType");
			}
			lock (this.Mailbox)
			{
				if (this.tempBufferDataSize + encoding.GetByteCount(logString) < this.TempBuffer.Length)
				{
					this.tempBufferDataSize += encoding.GetBytes(logString, 0, logString.Length, this.TempBuffer, this.tempBufferDataSize);
				}
				else
				{
					byte[] bytes = encoding.GetBytes(logString);
					this.InternalWrite(bytes, 0, bytes.Length);
				}
			}
		}

		public void AppendLog(byte[] logBuffer)
		{
			if (logBuffer == null)
			{
				throw new ArgumentNullException("logBuffer");
			}
			this.AppendLog(logBuffer, 0, logBuffer.Length);
		}

		public void AppendLog(byte[] logBuffer, int offset, int size)
		{
			if (logBuffer == null)
			{
				throw new ArgumentNullException("logBuffer");
			}
			if (offset < 0 || offset > logBuffer.Length)
			{
				throw new ArgumentException("offset");
			}
			if (size < 0 || offset + size > logBuffer.Length)
			{
				throw new ArgumentException("size");
			}
			if (this.streamType != MailboxLogger.StreamType.Ascii)
			{
				throw new InvalidOperationException("The log was not opened as ASCII");
			}
			if (this.LastError != null)
			{
				return;
			}
			lock (this.Mailbox)
			{
				if (this.tempBufferDataSize + size < this.TempBuffer.Length)
				{
					Array.Copy(logBuffer, offset, this.TempBuffer, this.tempBufferDataSize, size);
					this.tempBufferDataSize += size;
				}
				else
				{
					this.InternalWrite(logBuffer, offset, size);
				}
			}
		}

		public void Flush()
		{
			if (this.LastError != null)
			{
				return;
			}
			lock (this.Mailbox)
			{
				if (this.tempBufferDataSize != 0)
				{
					this.InternalWrite(null, 0, 0);
				}
			}
		}

		public void ClearOldLogs(int maxNumberOfLogs, long maxTotalLogSize)
		{
			if (maxNumberOfLogs < 0)
			{
				throw new ArgumentException(string.Format("maxNumberOfLogs value {0} is invalid", maxNumberOfLogs));
			}
			if (maxTotalLogSize < 0L)
			{
				throw new ArgumentException(string.Format("maxTotalLogSize value {0} is invalid", maxTotalLogSize));
			}
			if (this.LastError != null)
			{
				return;
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3890621757U, ref maxNumberOfLogs);
			lock (this.Mailbox)
			{
				bool isConnected = this.Mailbox.IsConnected;
				try
				{
					if (!isConnected)
					{
						this.Mailbox.Connect();
					}
					if (maxNumberOfLogs == 0 || maxTotalLogSize == 0L)
					{
						this.Mailbox.DeleteAllObjects(DeleteItemFlags.SoftDelete, this.logFolderId);
						this.LogsExist = false;
					}
					else
					{
						int num = 0;
						long num2 = 0L;
						using (Folder folder = Folder.Bind(this.Mailbox, this.logFolderId))
						{
							List<StoreObjectId> list = new List<StoreObjectId>(folder.ItemCount);
							using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, MailboxLogger.SortBySubjectD, MailboxLogger.DeleteQueryProperties))
							{
								object[][] rows;
								do
								{
									rows = queryResult.GetRows(10000);
									for (int i = 0; i < rows.Length; i++)
									{
										num++;
										num2 += (long)((int)rows[i][1]);
										if (num > maxNumberOfLogs / 2 || num2 > maxTotalLogSize / 2L)
										{
											list.Add(((VersionedId)rows[i][0]).ObjectId);
										}
									}
								}
								while (rows.Length != 0);
							}
							if (num > maxNumberOfLogs || num2 > maxTotalLogSize)
							{
								for (int j = 0; j < list.Count; j += 256)
								{
									int num3 = Math.Min(256, list.Count - j);
									this.Mailbox.Delete(DeleteItemFlags.SoftDelete, list.GetRange(j, num3).ToArray());
									num -= num3;
								}
							}
							this.LogsExist = (num > 0);
						}
					}
				}
				catch (LocalizedException lastError)
				{
					this.LastError = lastError;
				}
				finally
				{
					if (!isConnected)
					{
						try
						{
							this.Mailbox.Disconnect();
						}
						catch (LocalizedException)
						{
						}
					}
				}
			}
		}

		public IEnumerator<TextReader> GetEnumerator()
		{
			MailboxLogger.<GetEnumerator>d__8 <GetEnumerator>d__ = new MailboxLogger.<GetEnumerator>d__8(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.tempBuffer != null)
			{
				MailboxLogger.bufferPool.Release(this.tempBuffer);
				this.tempBuffer = null;
			}
		}

		private void InternalWrite(Item logItem, bool createNew, byte[] newData, int offset, int size, byte[] preamble)
		{
			try
			{
				logItem.OpenAsReadWrite();
				PropertyOpenMode openMode = PropertyOpenMode.Modify;
				if (createNew)
				{
					openMode = PropertyOpenMode.Create;
				}
				using (Stream stream = logItem.OpenPropertyStream(ItemSchema.ProtocolLog, openMode))
				{
					if (!createNew)
					{
						stream.Seek(0L, SeekOrigin.End);
					}
					if (preamble != null)
					{
						stream.Write(preamble, 0, preamble.Length);
					}
					if (this.tempBufferDataSize > 0)
					{
						stream.Write(this.TempBuffer, 0, this.tempBufferDataSize);
						this.tempBufferDataSize = 0;
					}
					if (newData != null && size > 0)
					{
						stream.Write(newData, offset, size);
					}
				}
				logItem.Save(SaveMode.NoConflictResolution);
			}
			catch (ObjectNotFoundException)
			{
				this.InternalCreateNewLog(newData, offset, size);
			}
		}

		private void InternalWrite(byte[] newData, int offset, int size)
		{
			if (this.currentLogId != null)
			{
				bool isConnected = this.Mailbox.IsConnected;
				try
				{
					try
					{
						if (!isConnected)
						{
							this.Mailbox.Connect();
						}
						using (Item item = Item.Bind(this.Mailbox, this.currentLogId))
						{
							this.InternalWrite(item, false, newData, offset, size, null);
						}
					}
					catch (LocalizedException lastError)
					{
						this.LastError = lastError;
					}
					return;
				}
				finally
				{
					if (!isConnected)
					{
						try
						{
							this.Mailbox.Disconnect();
						}
						catch (LocalizedException)
						{
						}
					}
				}
			}
			this.InternalCreateNewLog(newData, offset, size);
		}

		private void InternalCreateNewLog(byte[] newData, int offset, int size)
		{
			bool isConnected = this.Mailbox.IsConnected;
			try
			{
				if (!isConnected)
				{
					this.Mailbox.Connect();
				}
				string subject = ExDateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo);
				using (MessageItem messageItem = MessageItem.Create(this.Mailbox, this.logFolderId))
				{
					messageItem.Subject = subject;
					PolicyTagHelper.SetRetentionProperties(messageItem, ExDateTime.UtcNow.AddDays(5.0), 5);
					byte[] preamble = null;
					switch (this.streamType)
					{
					case MailboxLogger.StreamType.Ascii:
						preamble = Encoding.ASCII.GetPreamble();
						break;
					case MailboxLogger.StreamType.Unicode:
						preamble = Encoding.Unicode.GetPreamble();
						break;
					}
					this.InternalWrite(messageItem, true, newData, offset, size, preamble);
					messageItem.Load();
					this.currentLogId = messageItem.Id.ObjectId;
					this.LogsExist = true;
				}
			}
			catch (LocalizedException lastError)
			{
				this.LastError = lastError;
			}
			finally
			{
				if (!isConnected)
				{
					try
					{
						this.Mailbox.Disconnect();
					}
					catch (LocalizedException)
					{
					}
				}
			}
		}

		private const int LogRetentionInDays = 5;

		private const int MaxBulkSize = 256;

		private const int Id = 0;

		private const int Size = 1;

		private static readonly SortBy[] SortBySubjectA = new SortBy[]
		{
			new SortBy(ItemSchema.Subject, SortOrder.Ascending)
		};

		private static readonly SortBy[] SortBySubjectD = new SortBy[]
		{
			new SortBy(ItemSchema.Subject, SortOrder.Descending)
		};

		private static readonly PropertyDefinition[] DeleteQueryProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.Size
		};

		private static readonly PropertyDefinition[] ListQueryProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ProtocolLog
		};

		private static readonly PropertyDefinition[] LoadProperties = new PropertyDefinition[]
		{
			ItemSchema.ProtocolLog
		};

		private static BufferPool bufferPool;

		private StoreObjectId logFolderId;

		private StoreObjectId currentLogId;

		private MailboxLogger.StreamType streamType;

		private byte[] tempBuffer;

		private int tempBufferDataSize;

		private enum StreamType
		{
			Ascii,
			Unicode
		}
	}
}
