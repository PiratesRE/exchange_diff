using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class StreamLogItem : IDisposable
	{
		internal StreamLogItem(Referenced<MailboxSession> mailboxSession, StoreId messageId, StoreId folderId, string subject, string attachmentName)
		{
			if (mailboxSession == null || mailboxSession.Value == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (messageId == null)
			{
				this.messageItem = MessageItem.Create(mailboxSession, folderId);
				this.messageItem.IsDraft = false;
			}
			else
			{
				this.messageItem = MessageItem.Bind(mailboxSession, messageId, null);
			}
			this.messageItem.Subject = subject;
			this.messageItem.ClassName = "IPM.Note.Microsoft.Exchange.Search.Log";
			this.attachmentName = attachmentName;
			this.mailboxSession = mailboxSession.Reacquire();
		}

		internal MessageItem MessageItem
		{
			get
			{
				return this.messageItem;
			}
		}

		private string AttachmentName
		{
			get
			{
				return this.attachmentName;
			}
		}

		private StreamAttachment Attachment
		{
			get
			{
				return this.attachment;
			}
		}

		private StreamWriter AttachmentStream
		{
			get
			{
				return this.attachmentStream;
			}
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				if (this.Attachment != null)
				{
					this.CloseOpenedStream();
					this.Save(false);
				}
				if (this.MessageItem != null)
				{
					this.MessageItem.Dispose();
					this.messageItem = null;
				}
				this.mailboxSession.Dispose();
				this.mailboxSession = null;
				this.disposed = true;
			}
		}

		internal void WriteLogs(List<StreamLogItem.LogItem> logList)
		{
			lock (this.mailboxSession.Value)
			{
				foreach (StreamLogItem.LogItem logItem in logList)
				{
					StreamLogItem.LogItemAttachment subAttachment = null;
					this.subAttachments.TryGetValue(logItem.WorkerId, out subAttachment);
					if (subAttachment == null)
					{
						subAttachment = this.InitializeSubAttachment(logItem.WorkerId);
						this.subAttachments.Add(logItem.WorkerId, subAttachment);
					}
					logItem.Logs.ForEach(delegate(LocalizedString x)
					{
						subAttachment.AttachmentStream.WriteLine(x.ToString());
					});
				}
			}
		}

		internal void Save(bool reload)
		{
			lock (this.mailboxSession.Value)
			{
				try
				{
					ConflictResolutionResult conflictResolutionResult = this.MessageItem.Save(SaveMode.NoConflictResolutionForceSave);
					if (conflictResolutionResult.SaveStatus != SaveResult.Success)
					{
						StreamLogItem.Tracer.TraceError<SaveResult>((long)this.GetHashCode(), "Log item is saved with status {0}", conflictResolutionResult.SaveStatus);
					}
				}
				catch (MessageSubmissionExceededException)
				{
					StreamLogItem.Tracer.TraceError((long)this.GetHashCode(), "Log item is too large");
				}
				if (reload)
				{
					this.MessageItem.Load();
				}
			}
		}

		internal void CloseOpenedStream()
		{
			if (this.Attachment != null)
			{
				lock (this.mailboxSession.Value)
				{
					this.RemoveAllSubAttachments(false);
					if (this.AttachmentStream != null)
					{
						this.AttachmentStream.Dispose();
						this.attachmentStream = null;
					}
					if (this.logPackage != null)
					{
						this.logPackage.Close();
						this.logPackage = null;
					}
					this.Attachment.Save();
					this.Attachment.Dispose();
					this.attachment = null;
				}
			}
		}

		internal void ConsolidateLog(int workerId, bool merge)
		{
			lock (this.mailboxSession.Value)
			{
				if (this.subAttachments.Count != 0)
				{
					if (this.Attachment == null)
					{
						this.InitializeMainAttachment();
					}
					StreamLogItem.LogItemAttachment logItemAttachment = null;
					this.subAttachments.TryGetValue(workerId, out logItemAttachment);
					if (logItemAttachment != null)
					{
						this.RemoveSubAttachment(logItemAttachment, merge);
					}
				}
			}
		}

		private string GetAttachmentFileName(MessageItem messageItem)
		{
			string str;
			if (!this.AttachmentName.IsNullOrEmpty())
			{
				str = this.AttachmentName;
			}
			else if (!messageItem.Subject.IsNullOrEmpty())
			{
				str = messageItem.Subject;
			}
			else
			{
				str = "Search Results";
			}
			return str + ".csv";
		}

		private void InitializeMainAttachment()
		{
			this.attachment = (StreamAttachment)this.MessageItem.AttachmentCollection.Create(AttachmentType.Stream);
			string attachmentFileName = this.GetAttachmentFileName(this.MessageItem);
			this.Attachment.FileName = attachmentFileName + ".zip";
			this.Attachment[AttachmentSchema.DisplayName] = this.Attachment.FileName;
			Stream contentStream = this.Attachment.GetContentStream();
			this.logPackage = Package.Open(contentStream, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			string uriString = "/" + Uri.EscapeDataString(attachmentFileName);
			PackagePart packagePart = this.logPackage.CreatePart(new Uri(uriString, UriKind.Relative), "application/zip", CompressionOption.Maximum);
			this.attachmentStream = new StreamWriter(packagePart.GetStream(FileMode.Create, FileAccess.Write));
			this.AttachmentStream.WriteLine(Strings.SearchLogHeader);
		}

		private StreamLogItem.LogItemAttachment InitializeSubAttachment(int workerId)
		{
			string text = workerId.ToString();
			StreamLogItem.LogItemAttachment logItemAttachment = new StreamLogItem.LogItemAttachment();
			logItemAttachment.Id = workerId;
			logItemAttachment.Name = text;
			logItemAttachment.Attachment = (StreamAttachment)this.MessageItem.AttachmentCollection.Create(AttachmentType.Stream);
			logItemAttachment.Attachment.FileName = text + ".csv";
			logItemAttachment.Attachment[AttachmentSchema.DisplayName] = text;
			logItemAttachment.AttachmentStream = new StreamWriter(new GZipStream(logItemAttachment.Attachment.GetContentStream(), CompressionMode.Compress));
			return logItemAttachment;
		}

		private void RemoveAllSubAttachments(bool merge)
		{
			List<StreamLogItem.LogItemAttachment> list = new List<StreamLogItem.LogItemAttachment>();
			list.AddRange(this.subAttachments.Values);
			foreach (StreamLogItem.LogItemAttachment subAttachment in list)
			{
				this.RemoveSubAttachment(subAttachment, merge);
			}
			this.subAttachments.Clear();
		}

		private void RemoveSubAttachment(StreamLogItem.LogItemAttachment subAttachment, bool merge)
		{
			Stream baseStream = subAttachment.AttachmentStream.BaseStream;
			subAttachment.AttachmentStream.Flush();
			subAttachment.AttachmentStream.Dispose();
			baseStream.Dispose();
			subAttachment.Attachment.Save();
			subAttachment.Attachment.Load();
			if (merge)
			{
				using (StreamAttachment streamAttachment = (StreamAttachment)this.MessageItem.AttachmentCollection.Open(subAttachment.Attachment.Id))
				{
					using (GZipStream gzipStream = new GZipStream(streamAttachment.GetContentStream(), CompressionMode.Decompress))
					{
						using (StreamReader streamReader = new StreamReader(gzipStream))
						{
							string value;
							while ((value = streamReader.ReadLine()) != null)
							{
								this.AttachmentStream.WriteLine(value);
							}
						}
					}
				}
			}
			this.MessageItem.AttachmentCollection.Remove(subAttachment.Attachment.Id);
			this.MessageItem.Save(SaveMode.NoConflictResolutionForceSave);
			this.MessageItem.Load();
			int id = subAttachment.Id;
			subAttachment.AttachmentStream.Dispose();
			subAttachment.AttachmentStream = null;
			subAttachment.Attachment.Dispose();
			subAttachment.Attachment = null;
			subAttachment = null;
			this.subAttachments.Remove(id);
		}

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		private bool disposed;

		private string attachmentName;

		private Referenced<MailboxSession> mailboxSession;

		private MessageItem messageItem;

		private StreamAttachment attachment;

		private StreamWriter attachmentStream;

		private Dictionary<int, StreamLogItem.LogItemAttachment> subAttachments = new Dictionary<int, StreamLogItem.LogItemAttachment>();

		private Package logPackage;

		internal class LogItem
		{
			internal LogItem(int workerId, IEnumerable<LocalizedString> logs)
			{
				this.WorkerId = workerId;
				this.Logs = logs;
			}

			internal int WorkerId { get; private set; }

			internal IEnumerable<LocalizedString> Logs { get; private set; }
		}

		private class LogItemAttachment
		{
			internal int Id { get; set; }

			internal string Name { get; set; }

			internal StreamAttachment Attachment { get; set; }

			internal StreamWriter AttachmentStream { get; set; }
		}
	}
}
