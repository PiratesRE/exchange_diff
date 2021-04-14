using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class PureMimeMessage : MessageImplementation, IBody
	{
		public PureMimeMessage(BodyFormat bodyFormat, bool createAlternative, string charsetName)
		{
			this.mimeDocument = new MimeDocument();
			MimePart mimePart = new MimePart();
			this.mimeDocument.RootPart = mimePart;
			this.rootPart = mimePart;
			mimePart.Headers.AppendChild(new AsciiTextHeader("MIME-Version", "1.0"));
			this.bodyList = new List<MimePart>();
			if (createAlternative)
			{
				ContentTypeHeader newChild = new ContentTypeHeader("multipart/alternative");
				mimePart.Headers.AppendChild(newChild);
				MimePart mimePart2 = Utility.CreateBodyPart("text/plain", charsetName);
				mimePart.AppendChild(mimePart2);
				this.bodyList.Add(mimePart2);
				MimePart mimePart3 = Utility.CreateBodyPart("text/html", charsetName);
				mimePart.AppendChild(mimePart3);
				this.bodyList.Add(mimePart3);
				this.bodyStructure = BodyStructure.AlternativeBodies;
			}
			else
			{
				this.bodyList.Add(this.rootPart);
				string value = (bodyFormat == BodyFormat.Text) ? "text/plain" : "text/html";
				ContentTypeHeader contentTypeHeader = new ContentTypeHeader(value);
				MimeParameter newChild2 = new MimeParameter("charset", charsetName);
				contentTypeHeader.AppendChild(newChild2);
				mimePart.Headers.AppendChild(contentTypeHeader);
				this.bodyStructure = BodyStructure.SingleBody;
			}
			this.messageType = MessageType.Normal;
			this.PickBestBody();
			this.UpdateMimeVersion();
			this.defaultBodyCharset = Charset.GetCharset(charsetName);
			this.accessToken = new PureMimeMessage.PureMimeMessageThreadAccessToken(this);
		}

		public PureMimeMessage(MimeDocument mimeDocument)
		{
			this.mimeDocument = mimeDocument;
			this.rootPart = mimeDocument.RootPart;
			this.GetCharsetFromMimeDocument(mimeDocument);
			if (mimeDocument.IsReadOnly)
			{
				this.Synchronize();
			}
			this.accessToken = new PureMimeMessage.PureMimeMessageThreadAccessToken(this);
		}

		public PureMimeMessage(MimePart rootPart)
		{
			this.rootPart = rootPart;
			MimeDocument mimeDocument;
			MimeNode mimeNode;
			rootPart.GetMimeDocumentOrTreeRoot(out mimeDocument, out mimeNode);
			if (mimeDocument != null)
			{
				this.GetCharsetFromMimeDocument(mimeDocument);
				if (mimeDocument.IsReadOnly)
				{
					this.Synchronize();
				}
			}
		}

		public PureMimeMessage(Stream source)
		{
			this.mimeDocument = new MimeDocument();
			this.mimeDocument.Load(source, CachingMode.Copy);
			this.rootPart = this.mimeDocument.RootPart;
			this.GetCharsetFromMimeDocument(this.mimeDocument);
			this.accessToken = new PureMimeMessage.PureMimeMessageThreadAccessToken(this);
		}

		internal override ObjectThreadAccessToken AccessToken
		{
			get
			{
				return this.accessToken;
			}
		}

		public override EmailRecipient From
		{
			get
			{
				EmailRecipient fromRecipient;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!this.IsSynchronized || this.FromRecipient == null)
					{
						this.FromRecipient = this.GetRecipient(HeaderId.From);
					}
					fromRecipient = this.FromRecipient;
				}
				return fromRecipient;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (value != null && (value.MimeRecipient.Parent != null || value.TnefRecipient.TnefMessage != null))
					{
						throw new ArgumentException(EmailMessageStrings.RecipientAlreadyHasParent, "value");
					}
					this.SetRecipient(HeaderId.From, value);
					this.FromRecipient = value;
				}
			}
		}

		public override EmailRecipientCollection To
		{
			get
			{
				EmailRecipientCollection toRecipients;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!this.IsSynchronized || this.ToRecipients == null)
					{
						this.ToRecipients = this.GetRecipientCollection(RecipientType.To);
					}
					toRecipients = this.ToRecipients;
				}
				return toRecipients;
			}
		}

		public override EmailRecipientCollection Cc
		{
			get
			{
				EmailRecipientCollection ccRecipients;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!this.IsSynchronized || this.CcRecipients == null)
					{
						this.CcRecipients = this.GetRecipientCollection(RecipientType.Cc);
					}
					ccRecipients = this.CcRecipients;
				}
				return ccRecipients;
			}
		}

		public override EmailRecipientCollection Bcc
		{
			get
			{
				EmailRecipientCollection bccRecipients;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!this.IsSynchronized || this.BccRecipients == null)
					{
						this.BccRecipients = this.GetRecipientCollection(RecipientType.Bcc);
					}
					bccRecipients = this.BccRecipients;
				}
				return bccRecipients;
			}
		}

		public override EmailRecipientCollection ReplyTo
		{
			get
			{
				EmailRecipientCollection replyToRecipients;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!this.IsSynchronized || this.ReplyToRecipients == null)
					{
						this.ReplyToRecipients = this.GetRecipientCollection(RecipientType.ReplyTo);
					}
					replyToRecipients = this.ReplyToRecipients;
				}
				return replyToRecipients;
			}
		}

		public override EmailRecipient DispositionNotificationTo
		{
			get
			{
				EmailRecipient dntRecipient;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!this.IsSynchronized || this.DntRecipient == null)
					{
						this.DntRecipient = this.GetRecipient(HeaderId.DispositionNotificationTo);
					}
					dntRecipient = this.DntRecipient;
				}
				return dntRecipient;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (value != null && (value.MimeRecipient.Parent != null || value.TnefRecipient.TnefMessage != null))
					{
						throw new ArgumentException(EmailMessageStrings.RecipientAlreadyHasParent, "value");
					}
					this.SetRecipient(HeaderId.DispositionNotificationTo, value);
					this.DntRecipient = value;
				}
			}
		}

		public override EmailRecipient Sender
		{
			get
			{
				EmailRecipient senderRecipient;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!this.IsSynchronized || this.SenderRecipient == null)
					{
						this.SenderRecipient = this.GetRecipient(HeaderId.Sender);
						if (this.SenderRecipient == null)
						{
							this.SenderRecipient = this.GetRecipient(HeaderId.From);
						}
					}
					senderRecipient = this.SenderRecipient;
				}
				return senderRecipient;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (value != null && (value.MimeRecipient.Parent != null || value.TnefRecipient.TnefMessage != null))
					{
						throw new ArgumentException(EmailMessageStrings.RecipientAlreadyHasParent, "value");
					}
					this.SetRecipient(HeaderId.Sender, value);
					this.SenderRecipient = value;
				}
			}
		}

		public override DateTime Date
		{
			get
			{
				DateTime headerDate;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					headerDate = this.GetHeaderDate(HeaderId.Date);
				}
				return headerDate;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.SetHeaderDate(HeaderId.Date, value);
				}
			}
		}

		public override DateTime Expires
		{
			get
			{
				DateTime result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					DateTime dateTime = DateTime.MinValue;
					foreach (Header header in this.rootPart.Headers)
					{
						DateHeader dateHeader = header as DateHeader;
						if (dateHeader != null && (HeaderId.ExpiryDate == dateHeader.HeaderId || HeaderId.Expires == dateHeader.HeaderId))
						{
							dateTime = dateHeader.UtcDateTime;
						}
					}
					result = dateTime;
				}
				return result;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					bool isSynchronized = this.IsSynchronized;
					bool flag = value == DateTime.MinValue;
					foreach (Header header in this.rootPart.Headers)
					{
						if (HeaderId.ExpiryDate == header.HeaderId)
						{
							header.RemoveFromParent();
						}
						else if (HeaderId.Expires == header.HeaderId)
						{
							DateHeader dateHeader = header as DateHeader;
							if (flag || dateHeader == null)
							{
								header.RemoveFromParent();
							}
							else
							{
								dateHeader.DateTime = value;
								flag = true;
							}
						}
					}
					if (!flag)
					{
						DateHeader dateHeader2 = Header.Create(HeaderId.Expires) as DateHeader;
						dateHeader2.DateTime = value;
						this.rootPart.Headers.AppendChild(dateHeader2);
					}
					if (isSynchronized)
					{
						this.UpdateMimeVersion();
					}
				}
			}
		}

		public override DateTime ReplyBy
		{
			get
			{
				DateTime headerDate;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					headerDate = this.GetHeaderDate(HeaderId.ReplyBy);
				}
				return headerDate;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.SetHeaderDate(HeaderId.ReplyBy, value);
				}
			}
		}

		public override string Subject
		{
			get
			{
				string result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					string text = this.GetHeaderString(HeaderId.Subject);
					if (string.IsNullOrEmpty(text) && this.HasCalendar)
					{
						text = this.calendarMessage.Subject;
					}
					result = (text ?? string.Empty);
				}
				return result;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.SetHeaderString(HeaderId.Subject, value);
					bool isSynchronized = this.IsSynchronized;
					this.RootPart.Headers.RemoveAll("Thread-Topic");
					if (isSynchronized)
					{
						this.UpdateMimeVersion();
					}
					if (this.HasCalendar)
					{
						this.calendarMessage.Subject = value;
					}
				}
			}
		}

		public override string MessageId
		{
			get
			{
				string result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = (this.GetHeaderString(HeaderId.MessageId) ?? string.Empty);
				}
				return result;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.SetHeaderString(HeaderId.MessageId, value);
				}
			}
		}

		public override Importance Importance
		{
			get
			{
				Importance result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					int num = 0;
					if (this.TryGetHeaderEnum(HeaderId.Importance, EnumUtility.ImportanceMap, ref num))
					{
						result = (Importance)num;
					}
					else if (this.TryGetHeaderEnum(HeaderId.XPriority, EnumUtility.XPriorityMap, ref num))
					{
						result = (Importance)num;
					}
					else if (this.TryGetHeaderEnum(HeaderId.XMSMailPriority, EnumUtility.ImportanceMap, ref num))
					{
						result = (Importance)num;
					}
					else
					{
						result = (Importance)num;
					}
				}
				return result;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.SetHeaderEnum(HeaderId.Importance, EnumUtility.ImportanceMap, (int)value);
					this.SetHeaderEnum(HeaderId.XPriority, EnumUtility.XPriorityMap, (int)value);
					this.RemoveHeaders(HeaderId.XMSMailPriority);
				}
			}
		}

		public override Priority Priority
		{
			get
			{
				Priority result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					int num = 0;
					this.TryGetHeaderEnum(HeaderId.Priority, EnumUtility.PriorityMap, ref num);
					result = (Priority)num;
				}
				return result;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.SetHeaderEnum(HeaderId.Priority, EnumUtility.PriorityMap, (int)value);
				}
			}
		}

		public override Sensitivity Sensitivity
		{
			get
			{
				Sensitivity result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					int num = 0;
					this.TryGetHeaderEnum(HeaderId.Sensitivity, EnumUtility.SensitivityMap, ref num);
					result = (Sensitivity)num;
				}
				return result;
			}
			set
			{
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.SetHeaderEnum(HeaderId.Sensitivity, EnumUtility.SensitivityMap, (int)value);
				}
			}
		}

		public override string MapiMessageClass
		{
			get
			{
				string mapiMessageClass;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.Synchronize();
					if (this.HasCalendar)
					{
						mapiMessageClass = this.calendarMessage.MapiMessageClass;
					}
					else
					{
						mapiMessageClass = this.messageClass;
					}
				}
				return mapiMessageClass;
			}
		}

		public override MimeDocument MimeDocument
		{
			get
			{
				MimeDocument result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.mimeDocument;
				}
				return result;
			}
		}

		public override MimePart RootPart
		{
			get
			{
				MimePart result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.rootPart;
				}
				return result;
			}
		}

		public override MimePart CalendarPart
		{
			get
			{
				MimePart result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.Synchronize();
					result = this.calendarPart;
				}
				return result;
			}
		}

		public override MimePart TnefPart
		{
			get
			{
				MimePart result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.tnefPart;
				}
				return result;
			}
		}

		public override bool IsInterpersonalMessage
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.Synchronize();
					MessageFlags messageFlags = MessageTypeTable.GetMessageFlags(this.messageType);
					result = ((messageFlags & MessageFlags.Normal) != MessageFlags.None);
				}
				return result;
			}
		}

		public override bool IsSystemMessage
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.Synchronize();
					MessageFlags messageFlags = MessageTypeTable.GetMessageFlags(this.messageType);
					result = ((messageFlags & MessageFlags.System) > MessageFlags.None);
				}
				return result;
			}
		}

		public override bool IsOpaqueMessage
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.Synchronize();
					MessageSecurityType messageSecurityType = MessageTypeTable.GetMessageSecurityType(this.messageType);
					result = (messageSecurityType == MessageSecurityType.Encrypted || messageSecurityType == MessageSecurityType.OpaqueSigned);
				}
				return result;
			}
		}

		public override MessageSecurityType MessageSecurityType
		{
			get
			{
				MessageSecurityType result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.Synchronize();
					MessageSecurityType messageSecurityType = MessageTypeTable.GetMessageSecurityType(this.messageType);
					result = messageSecurityType;
				}
				return result;
			}
		}

		public override bool IsPublicFolderReplicationMessage
		{
			get
			{
				return false;
			}
		}

		internal bool IsSynchronized
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = (this.Version == this.version);
				}
				return result;
			}
		}

		internal override int Version
		{
			get
			{
				if (this.mimeDocument == null)
				{
					return this.rootPart.Version;
				}
				return this.mimeDocument.Version;
			}
		}

		internal override EmailRecipientCollection BccFromOrgHeader
		{
			get
			{
				EmailRecipientCollection bccFromOrgHeaderRecipients;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!this.IsSynchronized || this.BccFromOrgHeaderRecipients == null)
					{
						this.BccFromOrgHeaderRecipients = this.GetRecipientCollection(RecipientType.Bcc, HeaderId.XExchangeBcc);
					}
					bccFromOrgHeaderRecipients = this.BccFromOrgHeaderRecipients;
				}
				return bccFromOrgHeaderRecipients;
			}
		}

		internal bool HasCalendar
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.CalendarPart == null)
					{
						result = false;
					}
					else if (!this.LoadCalendar())
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
				return result;
			}
		}

		private bool UseCalendarBody
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPrivate(this.accessToken))
				{
					if (this.bodyMimePart == null)
					{
						result = false;
					}
					else if (this.bodyMimePart != this.calendarPart)
					{
						result = false;
					}
					else if (!this.HasCalendar)
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
				return result;
			}
		}

		internal string TnefCorrelator
		{
			get
			{
				string tnefCorrelator;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					tnefCorrelator = Utility.GetTnefCorrelator(this.rootPart);
				}
				return tnefCorrelator;
			}
		}

		internal BodyStructure BodyStructure
		{
			get
			{
				BodyStructure result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.bodyStructure;
				}
				return result;
			}
		}

		private static bool CheckKeyPart(MimePart currentPart, bool match, ref bool expected, ref MimePart referencePart)
		{
			if (match && expected)
			{
				referencePart = currentPart;
				expected = false;
				return true;
			}
			return false;
		}

		private static bool IsVoiceContentType(string contentType)
		{
			return contentType == "audio/wav" || contentType == "audio/wma" || contentType == "audio/mp3" || contentType == "audio/gsm";
		}

		private static bool FileNameIndicatesSmime(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				foreach (string value in PureMimeMessage.smimeExtensions)
				{
					if (name.EndsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static HeaderId GetHeaderId(RecipientType recipientType)
		{
			switch (recipientType)
			{
			case RecipientType.To:
				return HeaderId.To;
			case RecipientType.From:
				return HeaderId.From;
			case RecipientType.Cc:
				return HeaderId.Cc;
			case RecipientType.Bcc:
				return HeaderId.Bcc;
			case RecipientType.ReplyTo:
				return HeaderId.ReplyTo;
			default:
				return HeaderId.Unknown;
			}
		}

		internal static void GetMessageClassSuffix(IEnumerable<KeyValuePair<string, string>> map, string key, ref string suffix)
		{
			bool flag = suffix == null;
			foreach (KeyValuePair<string, string> keyValuePair in map)
			{
				if (!flag && suffix == keyValuePair.Value)
				{
					flag = true;
				}
				if (key.Contains(keyValuePair.Key) && flag)
				{
					suffix = keyValuePair.Value;
				}
			}
		}

		internal override void SetReadOnly(bool makeReadOnly)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.calendarRelayStorage != null)
				{
					this.calendarRelayStorage.SetReadOnly(makeReadOnly);
				}
			}
		}

		private static bool IsAppleSingleAttachment(MimeAttachmentData data)
		{
			return data.AttachmentPart.ContentType == "application/applefile";
		}

		private static Dictionary<string, string> ReadReportHeaders(Stream stream)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			using (Stream stream2 = new SuppressCloseStream(stream))
			{
				using (MimeReader mimeReader = new MimeReader(stream2))
				{
					long position = stream.Position;
					if (!mimeReader.ReadNextPart())
					{
						return null;
					}
					MimeHeaderReader headerReader = mimeReader.HeaderReader;
					while (headerReader.ReadNextHeader())
					{
						DecodingResults decodingResults;
						string text;
						headerReader.TryGetValue(Utility.DecodeOrFallBack, out decodingResults, out text);
						if (text != null)
						{
							dictionary[headerReader.Name] = Utility.RemoveMimeHeaderComments(text);
						}
					}
					stream.Position = position + mimeReader.StreamOffset;
				}
			}
			return dictionary;
		}

		private static Charset DetermineTextPartCharset(MimePart part, Charset defaultBodyCharset, out bool defaulted)
		{
			string parameterValue = Utility.GetParameterValue(part, HeaderId.ContentType, "charset");
			Charset result;
			if (Charset.TryGetCharset(parameterValue, out result))
			{
				defaulted = false;
				return result;
			}
			defaulted = true;
			return defaultBodyCharset;
		}

		internal void SetTnefPart(MimePart tnefPart)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.tnefPart = tnefPart;
				this.Synchronize();
			}
		}

		internal override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.bodyData != null)
				{
					this.bodyData.Dispose();
					this.bodyData = null;
				}
				if (this.mimeDocument != null)
				{
					this.mimeDocument.Dispose();
					this.mimeDocument = null;
					this.rootPart = null;
				}
				else if (this.rootPart != null)
				{
					this.rootPart.Dispose();
					this.rootPart = null;
				}
				if (this.calendarRelayStorage != null)
				{
					this.calendarRelayStorage.Release();
					this.calendarRelayStorage = null;
				}
			}
			base.Dispose(disposing);
		}

		public override void Normalize(bool allowUTF8 = false)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.Normalize(NormalizeOptions.NormalizeMime, allowUTF8);
			}
		}

		internal override void Normalize(NormalizeOptions normalizeOptions, bool allowUTF8)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if ((normalizeOptions & NormalizeOptions.NormalizeMimeStructure) != (NormalizeOptions)0)
				{
					this.NormalizeStructure(false);
				}
				if ((normalizeOptions & NormalizeOptions.NormalizeMessageId) != (NormalizeOptions)0)
				{
					this.NormalizeMessageId(allowUTF8);
				}
				if ((normalizeOptions & NormalizeOptions.NormalizeCte) != (NormalizeOptions)0)
				{
					this.NormalizeCte();
				}
				if ((normalizeOptions & NormalizeOptions.MergeAddressHeaders) != (NormalizeOptions)0)
				{
					bool isSynchronized = this.IsSynchronized;
					Utility.NormalizeHeaders(this.rootPart, Utility.HeaderNormalization.MergeAddressHeaders);
					if (isSynchronized)
					{
						this.UpdateMimeVersion();
					}
				}
				if ((normalizeOptions & NormalizeOptions.RemoveDuplicateHeaders) != (NormalizeOptions)0)
				{
					bool isSynchronized2 = this.IsSynchronized;
					Utility.NormalizeHeaders(this.rootPart, Utility.HeaderNormalization.PruneRestrictedHeaders);
					if (isSynchronized2)
					{
						this.UpdateMimeVersion();
					}
				}
			}
		}

		private void NormalizeMessageId(bool allowUTF8)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.rootPart != null)
				{
					Header lastHeaderRemoveDuplicates = this.GetLastHeaderRemoveDuplicates(HeaderId.MessageId);
					if (lastHeaderRemoveDuplicates == null)
					{
						Header header = Header.Create(HeaderId.MessageId);
						header.Value = this.CreateMessageId(allowUTF8);
						this.rootPart.Headers.AppendChild(header);
					}
					else
					{
						string headerValue = Utility.GetHeaderValue(lastHeaderRemoveDuplicates);
						if (string.IsNullOrEmpty(headerValue) || headerValue.Trim().Length == 0)
						{
							lastHeaderRemoveDuplicates.Value = this.CreateMessageId(allowUTF8);
						}
						else if (headerValue.Length > PureMimeMessage.maxMessageIdLength)
						{
							lastHeaderRemoveDuplicates.Value = this.CreateMessageId(allowUTF8);
						}
						else
						{
							foreach (char c in headerValue)
							{
								if ((!allowUTF8 || c < '\u0080') && (c < ' ' || c > '~'))
								{
									lastHeaderRemoveDuplicates.Value = this.CreateMessageId(allowUTF8);
									break;
								}
							}
						}
					}
				}
			}
		}

		private void NormalizeCte()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				using (MimePart.SubtreeEnumerator enumerator = this.rootPart.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.RevisitParent, false))
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.LastVisit && !enumerator.FirstVisit)
						{
							this.NormalizeParentCte(enumerator.Current);
						}
					}
				}
			}
		}

		private void NormalizeParentCte(MimePart parent)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				ContentTransferEncoding contentTransferEncoding = ContentTransferEncoding.SevenBit;
				foreach (MimePart mimePart in parent)
				{
					if (mimePart.ContentTransferEncoding == ContentTransferEncoding.Binary)
					{
						contentTransferEncoding = ContentTransferEncoding.Binary;
						break;
					}
					if (mimePart.ContentTransferEncoding == ContentTransferEncoding.EightBit)
					{
						contentTransferEncoding = ContentTransferEncoding.EightBit;
					}
				}
				if (ContentTransferEncoding.Binary == contentTransferEncoding || ContentTransferEncoding.EightBit == contentTransferEncoding)
				{
					string value = (ContentTransferEncoding.Binary == contentTransferEncoding) ? "binary" : "8bit";
					Header header = parent.Headers.FindFirst(HeaderId.ContentTransferEncoding);
					if (header != null)
					{
						if (contentTransferEncoding != MimePart.GetEncodingType(header.FirstRawToken))
						{
							header.Value = value;
						}
					}
					else
					{
						header = Header.Create(HeaderId.ContentTransferEncoding);
						header.Value = value;
						parent.Headers.AppendChild(header);
					}
				}
				else
				{
					Header header2 = parent.Headers.FindFirst(HeaderId.ContentTransferEncoding);
					if (header2 != null)
					{
						contentTransferEncoding = MimePart.GetEncodingType(header2.FirstRawToken);
						if (ContentTransferEncoding.Binary == contentTransferEncoding || ContentTransferEncoding.EightBit == contentTransferEncoding)
						{
							header2.RemoveFromParent();
						}
					}
				}
			}
		}

		internal void NormalizeStructure(bool forceRebuild)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (forceRebuild || this.rebuildStructureAtNextOpportunity)
				{
					this.RebuildMessage();
				}
				else
				{
					this.Synchronize();
					MessageFlags messageFlags = MessageTypeTable.GetMessageFlags(this.messageType);
					if ((messageFlags & MessageFlags.Normal) == MessageFlags.None || this.rebuildStructureAtNextOpportunity)
					{
						this.RebuildMessage();
					}
				}
			}
		}

		internal void InvalidateCalendarContent()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.calendarRelayStorage == null)
				{
					this.calendarRelayStorage = new RelayStorage(this.calendarMessage);
				}
				else
				{
					this.calendarRelayStorage.Invalidate();
				}
				bool contentDirty = this.calendarPart.ContentDirty;
				bool isSynchronized = this.IsSynchronized;
				ContentTransferEncoding contentTransferEncoding = this.calendarPart.ContentTransferEncoding;
				if (contentTransferEncoding == ContentTransferEncoding.Unknown || ContentTransferEncoding.SevenBit == contentTransferEncoding || ContentTransferEncoding.EightBit == contentTransferEncoding)
				{
					Header header = this.calendarPart.Headers.FindFirst(HeaderId.ContentTransferEncoding);
					if (header == null)
					{
						header = Header.Create(HeaderId.ContentTransferEncoding);
						this.calendarPart.Headers.AppendChild(header);
					}
					header.Value = "quoted-printable";
				}
				this.calendarPart.SetStorage(this.calendarRelayStorage, 0L, long.MaxValue);
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
				this.calendarPart.ContentDirty = contentDirty;
			}
		}

		internal void SetPartCharset(MimePart part, string charsetName)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				Utility.SetParameterValue(part, HeaderId.ContentType, "charset", charsetName);
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
			}
		}

		internal EmailRecipientCollection GetRecipientCollection(RecipientType recipientType)
		{
			HeaderId headerId = PureMimeMessage.GetHeaderId(recipientType);
			return this.GetRecipientCollection(recipientType, headerId);
		}

		internal EmailRecipientCollection GetRecipientCollection(RecipientType recipientType, HeaderId headerId)
		{
			EmailRecipientCollection result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				EmailRecipientCollection emailRecipientCollection = new EmailRecipientCollection(this, recipientType, this.MimeDocument != null && this.MimeDocument.IsReadOnly);
				int num = 0;
				for (Header header = this.rootPart.Headers.FindFirst(headerId); header != null; header = this.rootPart.Headers.FindNext(header))
				{
					num++;
					if (1 == num)
					{
						emailRecipientCollection.Cache = header;
					}
					else
					{
						emailRecipientCollection.Cache = null;
					}
					foreach (MimeNode mimeNode in header)
					{
						MimeRecipient mimeRecipient = mimeNode as MimeRecipient;
						if (mimeRecipient != null)
						{
							emailRecipientCollection.InternalAdd(new EmailRecipient(mimeRecipient));
						}
						else
						{
							MimeGroup mimeGroup = mimeNode as MimeGroup;
							foreach (MimeRecipient recipient in mimeGroup)
							{
								emailRecipientCollection.InternalAdd(new EmailRecipient(recipient));
							}
						}
					}
				}
				result = emailRecipientCollection;
			}
			return result;
		}

		internal override void AddRecipient(RecipientType recipientType, ref object cachedHeader, EmailRecipient newRecipient)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				AddressHeader recipientHeader = this.GetRecipientHeader(recipientType, ref cachedHeader);
				recipientHeader.AppendChild(newRecipient.MimeRecipient);
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
			}
		}

		internal override void RemoveRecipient(RecipientType recipientType, ref object cachedHeader, EmailRecipient oldRecipient)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				MimeNode parent = oldRecipient.MimeRecipient.Parent;
				oldRecipient.MimeRecipient.RemoveFromParent();
				if (parent is MimeGroup && !parent.HasChildren)
				{
					parent.RemoveFromParent();
				}
				AddressHeader recipientHeader = this.GetRecipientHeader(recipientType, ref cachedHeader);
				if (!recipientHeader.HasChildren)
				{
					recipientHeader.RemoveFromParent();
					cachedHeader = null;
				}
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
			}
		}

		internal override void ClearRecipients(RecipientType recipientType, ref object cachedHeader)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				if (cachedHeader != null)
				{
					AddressHeader addressHeader = cachedHeader as AddressHeader;
					addressHeader.RemoveAll();
					addressHeader.RemoveFromParent();
					cachedHeader = null;
				}
				else
				{
					HeaderId headerId = PureMimeMessage.GetHeaderId(recipientType);
					Header header2;
					for (Header header = this.rootPart.Headers.FindFirst(headerId); header != null; header = header2)
					{
						header2 = this.rootPart.Headers.FindNext(header);
						header.RemoveAll();
						header.RemoveFromParent();
					}
				}
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
			}
		}

		internal override AttachmentCookie AttachmentCollection_AddAttachment(Attachment attachment)
		{
			AttachmentCookie result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.IsOpaqueMessage)
				{
					throw new InvalidOperationException(EmailMessageStrings.CannotAddAttachment);
				}
				this.NormalizeStructure(false);
				if (this.multipartMixed == null)
				{
					this.multipartMixed = new MimePart("multipart/mixed");
					MimePart mimePart;
					if (this.multipartSigned == null)
					{
						mimePart = this.rootPart;
						this.SetRootPart(this.multipartMixed);
					}
					else
					{
						mimePart = (this.multipartSigned.FirstChild as MimePart);
						this.multipartSigned.ReplaceChild(this.multipartMixed, mimePart);
					}
					this.multipartMixed.AppendChild(mimePart);
				}
				MimePart mimePart2 = new MimePart();
				this.multipartMixed.AppendChild(mimePart2);
				int index;
				MimeAttachmentData attachmentData = this.GetAttachmentData(mimePart2, InternalAttachmentType.Regular, this.RootPart.Version, out index);
				attachmentData.Attachment = attachment;
				AttachmentCookie attachmentCookie = new AttachmentCookie(index, this);
				result = attachmentCookie;
			}
			return result;
		}

		internal override bool AttachmentCollection_RemoveAttachment(AttachmentCookie cookie)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData dataAtPrivateIndex = this.mimeAttachments.GetDataAtPrivateIndex(cookie.Index);
				this.mimeAttachments.RemoveAtPrivateIndex(cookie.Index);
				bool flag = this.RemoveAttachment(dataAtPrivateIndex.AttachmentPart);
				dataAtPrivateIndex.Invalidate();
				result = flag;
			}
			return result;
		}

		internal override void AttachmentCollection_ClearAttachments()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				while (0 < this.mimeAttachments.Count)
				{
					MimeAttachmentData dataAtPublicIndex = this.mimeAttachments.GetDataAtPublicIndex(0);
					this.RemoveAttachment(dataAtPublicIndex.AttachmentPart);
					int privateIndex = this.mimeAttachments.GetPrivateIndex(0);
					this.mimeAttachments.RemoveAtPrivateIndex(privateIndex);
					dataAtPublicIndex.Invalidate();
				}
				this.mimeAttachments.Clear();
			}
		}

		internal override int AttachmentCollection_Count()
		{
			int count;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				count = this.mimeAttachments.Count;
			}
			return count;
		}

		internal override object AttachmentCollection_Indexer(int publicIndex)
		{
			object attachment;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData dataAtPublicIndex = this.mimeAttachments.GetDataAtPublicIndex(publicIndex);
				attachment = dataAtPublicIndex.Attachment;
			}
			return attachment;
		}

		internal override AttachmentCookie AttachmentCollection_CacheAttachment(int publicIndex, object attachment)
		{
			AttachmentCookie result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData dataAtPublicIndex = this.mimeAttachments.GetDataAtPublicIndex(publicIndex);
				dataAtPublicIndex.Attachment = attachment;
				int privateIndex = this.mimeAttachments.GetPrivateIndex(publicIndex);
				AttachmentCookie attachmentCookie = new AttachmentCookie(privateIndex, this);
				result = attachmentCookie;
			}
			return result;
		}

		internal bool RemoveAttachment(MimePart part)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				bool flag = false;
				if (this.rootPart == part)
				{
					this.CreateEmptyMessage();
				}
				else
				{
					MimePart mimePart = part.Parent as MimePart;
					if (mimePart != null)
					{
						part.RemoveFromParent();
						flag = true;
						this.RemoveDegenerateMultiparts(mimePart);
					}
				}
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
				result = flag;
			}
			return result;
		}

		private void AddRelatedAttachment(MimePart part)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.multipartRelated == null)
				{
					MimePart mimePart = null;
					foreach (MimePart mimePart2 in this.bodyList)
					{
						if ("text/html" == mimePart2.ContentType)
						{
							mimePart = mimePart2;
							break;
						}
					}
					if (mimePart == null)
					{
						throw new InvalidOperationException(EmailMessageStrings.CanOnlyAddInlineAttachmentsToHtmlBody);
					}
					this.multipartRelated = new MimePart("multipart/related");
					this.InsertMultipart(this.multipartRelated, mimePart);
				}
				this.multipartRelated.AppendChild(part);
			}
		}

		private void AddRegularAttachment(MimePart part)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.multipartMixed == null)
				{
					this.multipartMixed = new MimePart("multipart/mixed");
					MimePart oldPart;
					if (this.RootPart == this.multipartSigned)
					{
						oldPart = (this.multipartSigned.FirstChild as MimePart);
					}
					else
					{
						oldPart = this.RootPart;
					}
					this.InsertMultipart(this.multipartMixed, oldPart);
				}
				this.multipartMixed.AppendChild(part);
			}
		}

		private void InsertMultipart(MimePart newContainer, MimePart oldPart)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (oldPart.Parent == null)
				{
					this.SetRootPart(newContainer);
				}
				else
				{
					oldPart.Parent.ReplaceChild(newContainer, oldPart);
				}
				newContainer.AppendChild(oldPart);
			}
		}

		internal override IBody GetBody()
		{
			IBody result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.Synchronize();
				if (this.UseCalendarBody)
				{
					result = this.calendarMessage;
				}
				else
				{
					result = this;
				}
			}
			return result;
		}

		BodyFormat IBody.GetBodyFormat()
		{
			BodyFormat bodyFormat;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				bodyFormat = this.bodyData.BodyFormat;
			}
			return bodyFormat;
		}

		string IBody.GetCharsetName()
		{
			string charsetName;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				charsetName = this.bodyData.CharsetName;
			}
			return charsetName;
		}

		MimePart IBody.GetMimePart()
		{
			MimePart result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.bodyMimePart;
			}
			return result;
		}

		Stream IBody.GetContentReadStream()
		{
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.bodyMimePart == null)
				{
					result = DataStorage.NewEmptyReadStream();
				}
				else
				{
					result = this.bodyData.ConvertReadStreamFormat(Utility.GetContentReadStream(this.bodyMimePart));
				}
			}
			return result;
		}

		bool IBody.TryGetContentReadStream(out Stream stream)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.bodyMimePart == null)
				{
					stream = null;
					result = false;
				}
				else
				{
					bool flag = this.bodyMimePart.TryGetContentReadStream(out stream);
					if (flag)
					{
						stream = this.bodyData.ConvertReadStreamFormat(stream);
					}
					result = flag;
				}
			}
			return result;
		}

		Stream IBody.GetContentWriteStream(Charset charset)
		{
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.bodyData.ReleaseStorage();
				bool isSynchronized = this.IsSynchronized;
				if (charset != null && charset != this.bodyData.Charset)
				{
					Charset charset2 = Utility.TranslateWriteStreamCharset(charset);
					this.SetPartCharset(this.bodyMimePart, charset2.Name);
					this.bodyData.SetNewCharset(charset2);
				}
				ContentTransferEncoding contentTransferEncoding = this.bodyMimePart.ContentTransferEncoding;
				if (contentTransferEncoding == ContentTransferEncoding.Unknown || ContentTransferEncoding.SevenBit == contentTransferEncoding || ContentTransferEncoding.EightBit == contentTransferEncoding)
				{
					this.bodyMimePart.UpdateTransferEncoding(ContentTransferEncoding.QuotedPrintable);
				}
				this.bodyMimePart.SetStorage(null, 0L, 0L);
				this.bodyWriteStreamMimePart = this.bodyMimePart;
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
				Stream stream = new BodyContentWriteStream(this);
				stream = this.bodyData.ConvertWriteStreamFormat(stream, charset);
				result = stream;
			}
			return result;
		}

		void IBody.SetNewContent(DataStorage storage, long start, long end)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.Synchronize();
				this.bodyWriteStreamMimePart.SetStorage(storage, start, end);
				if (this.bodyWriteStreamMimePart == this.bodyMimePart)
				{
					this.bodyData.SetStorage(storage, start, end);
					this.UpdateMimeVersion();
					this.BodyModified();
				}
				this.bodyWriteStreamMimePart = null;
				this.UpdateMimeVersion();
			}
		}

		bool IBody.ConversionNeeded(int[] validCodepages)
		{
			return false;
		}

		private void PickBestBody()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.Synchronize();
				MimePart mimePart = null;
				MimePart mimePart2 = null;
				MimePart mimePart3 = null;
				foreach (MimePart mimePart4 in this.bodyList)
				{
					string contentType = mimePart4.ContentType;
					if (contentType == "text/html")
					{
						this.SetBody(mimePart4, BodyFormat.Html, InternalBodyFormat.Html);
						return;
					}
					if (mimePart2 == null && contentType == "text/enriched")
					{
						mimePart2 = mimePart4;
					}
					if (mimePart == null && contentType == "text/plain")
					{
						mimePart = mimePart4;
					}
					if (mimePart3 == null && contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
					{
						mimePart3 = mimePart4;
					}
				}
				if (mimePart2 != null)
				{
					this.SetBody(mimePart2, BodyFormat.Html, InternalBodyFormat.Enriched);
				}
				else if (mimePart != null)
				{
					this.SetBody(mimePart, BodyFormat.Text, InternalBodyFormat.Text);
				}
				else if (mimePart3 != null)
				{
					this.SetBody(mimePart3, BodyFormat.Text, InternalBodyFormat.Text);
				}
				else
				{
					this.bodyData.SetFormat(BodyFormat.None, InternalBodyFormat.None, null);
					this.bodyData.SetStorage(null, 0L, 0L);
				}
			}
		}

		private void SetBody(MimePart part, BodyFormat format, InternalBodyFormat internalFormat)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.bodyMimePart = part;
				bool flag;
				Charset charset = PureMimeMessage.DetermineTextPartCharset(part, this.defaultBodyCharset, out flag);
				this.bodyData.SetFormat(format, internalFormat, charset);
				this.bodyData.SetStorage(part.Storage, part.DataStart, part.DataEnd);
				if (flag || FeInboundCharsetDetector.IsSupportedFarEastCharset(charset) || charset.CodePage == 20127)
				{
					Stream stream = null;
					try
					{
						if (this.bodyMimePart.TryGetContentReadStream(out stream))
						{
							this.bodyData.ValidateCharset(flag, stream);
						}
					}
					finally
					{
						if (stream != null)
						{
							stream.Dispose();
							stream = null;
						}
					}
				}
			}
		}

		internal void BodyModified()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				Charset charset = this.bodyData.Charset;
				bool flag = BodyFormat.Text != this.bodyData.BodyFormat;
				bool flag2 = BodyFormat.Html == this.bodyData.BodyFormat && this.bodyData.InternalBodyFormat != InternalBodyFormat.Enriched;
				foreach (MimePart mimePart in this.bodyList)
				{
					if (mimePart != this.bodyMimePart)
					{
						if (mimePart == this.calendarPart && this.HasCalendar)
						{
							BodyData bodyData = this.calendarMessage.BodyData;
							if (bodyData != null && bodyData.HasContent)
							{
								if (bodyData.Charset != Charset.UTF8 && bodyData.Charset != charset)
								{
									this.SetPartCharset(mimePart, Charset.UTF8.Name);
									bodyData.SetNewCharset(Charset.UTF8);
									this.calendarMessage.TargetCharset = Charset.UTF8;
								}
								DataStorage dataStorage;
								long num;
								long num2;
								this.bodyData.GetStorage(InternalBodyFormat.Text, bodyData.Charset, out dataStorage, out num, out num2);
								bodyData.SetStorage(dataStorage, num, num2);
								dataStorage.Release();
								this.calendarMessage.TouchBody();
							}
						}
						else
						{
							string contentType = mimePart.ContentType;
							DataStorage dataStorage;
							long num;
							long num2;
							if (flag && contentType == "text/plain")
							{
								flag = false;
								this.SetPartCharset(mimePart, this.bodyData.CharsetName);
								this.bodyData.GetStorage(InternalBodyFormat.Text, charset, out dataStorage, out num, out num2);
							}
							else if (flag2 && contentType == "text/enriched")
							{
								flag2 = false;
								this.SetPartCharset(mimePart, this.bodyData.CharsetName);
								this.bodyData.GetStorage(InternalBodyFormat.Enriched, charset, out dataStorage, out num, out num2);
							}
							else
							{
								bool isSynchronized = this.IsSynchronized;
								mimePart.RemoveFromParent();
								if (isSynchronized)
								{
									this.UpdateMimeVersion();
									continue;
								}
								continue;
							}
							bool contentDirty = mimePart.ContentDirty;
							bool isSynchronized2 = this.IsSynchronized;
							Utility.SynchronizeEncoding(this.bodyData, mimePart);
							mimePart.SetStorage(dataStorage, num, num2);
							dataStorage.Release();
							ContentTransferEncoding contentTransferEncoding = mimePart.ContentTransferEncoding;
							if (contentTransferEncoding == ContentTransferEncoding.Unknown || contentTransferEncoding == ContentTransferEncoding.SevenBit || contentTransferEncoding == ContentTransferEncoding.EightBit)
							{
								mimePart.UpdateTransferEncoding(ContentTransferEncoding.QuotedPrintable);
							}
							if (isSynchronized2)
							{
								this.UpdateMimeVersion();
							}
							mimePart.ContentDirty = contentDirty;
						}
					}
				}
			}
		}

		private MimeAttachmentData DataFromCookie(AttachmentCookie cookie)
		{
			MimeAttachmentData result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				MimeAttachmentData dataAtPrivateIndex = this.mimeAttachments.GetDataAtPrivateIndex(cookie.Index);
				if (dataAtPrivateIndex == null)
				{
					throw new InvalidOperationException(EmailMessageStrings.AttachmentRemovedFromMessage);
				}
				result = dataAtPrivateIndex;
			}
			return result;
		}

		internal override MimePart Attachment_GetMimePart(AttachmentCookie cookie)
		{
			MimePart result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				MimePart mimePart = mimeAttachmentData.DataPart ?? mimeAttachmentData.AttachmentPart;
				result = mimePart;
			}
			return result;
		}

		internal override string Attachment_GetContentType(AttachmentCookie cookie)
		{
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				if (PureMimeMessage.IsAppleSingleAttachment(mimeAttachmentData))
				{
					result = "application/octet-stream";
				}
				else
				{
					MimePart mimePart = mimeAttachmentData.DataPart ?? mimeAttachmentData.AttachmentPart;
					result = mimePart.ContentType;
				}
			}
			return result;
		}

		internal override void Attachment_SetContentType(AttachmentCookie cookie, string contentType)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				this.PromoteIfAppleDoubleAttachment(mimeAttachmentData);
				ContentTypeHeader contentTypeHeader = mimeAttachmentData.AttachmentPart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
				if (contentTypeHeader == null)
				{
					contentTypeHeader = new ContentTypeHeader();
					mimeAttachmentData.AttachmentPart.Headers.AppendChild(contentTypeHeader);
				}
				contentTypeHeader.Value = contentType;
			}
		}

		internal override AttachmentMethod Attachment_GetAttachmentMethod(AttachmentCookie cookie)
		{
			AttachmentMethod result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				MimePart mimePart = mimeAttachmentData.DataPart ?? mimeAttachmentData.AttachmentPart;
				if (mimePart.IsEmbeddedMessage)
				{
					result = AttachmentMethod.EmbeddedMessage;
				}
				else
				{
					result = AttachmentMethod.AttachByValue;
				}
			}
			return result;
		}

		internal override InternalAttachmentType Attachment_GetAttachmentType(AttachmentCookie cookie)
		{
			InternalAttachmentType internalAttachmentType;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				internalAttachmentType = mimeAttachmentData.InternalAttachmentType;
			}
			return internalAttachmentType;
		}

		internal override void Attachment_SetAttachmentType(AttachmentCookie cookie, InternalAttachmentType attachmentType)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				this.PromoteIfAppleDoubleAttachment(mimeAttachmentData);
				if (mimeAttachmentData.InternalAttachmentType != attachmentType)
				{
					mimeAttachmentData.InternalAttachmentType = attachmentType;
					if (InternalAttachmentType.Related == mimeAttachmentData.InternalAttachmentType)
					{
						this.RemoveAttachment(mimeAttachmentData.AttachmentPart);
						this.AddRelatedAttachment(mimeAttachmentData.AttachmentPart);
					}
					else
					{
						this.RemoveAttachment(mimeAttachmentData.AttachmentPart);
						this.AddRegularAttachment(mimeAttachmentData.AttachmentPart);
					}
				}
				this.Attachment_SetContentDisposition(cookie, (attachmentType == InternalAttachmentType.Regular) ? "attachment" : "inline");
			}
		}

		internal override EmailMessage Attachment_GetEmbeddedMessage(AttachmentCookie cookie)
		{
			EmailMessage result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				MimePart mimePart = mimeAttachmentData.DataPart ?? mimeAttachmentData.AttachmentPart;
				if (!mimePart.IsEmbeddedMessage)
				{
					result = null;
				}
				else
				{
					if (mimePart.FirstChild == null)
					{
						bool isSynchronized = this.IsSynchronized;
						MimePart mimePart2 = new MimePart("text/plain");
						mimePart2.Headers.AppendChild(new AsciiTextHeader("MIME-Version", "1.0"));
						mimePart.AppendChild(mimePart2);
						if (isSynchronized)
						{
							this.UpdateMimeVersion();
						}
					}
					if (mimeAttachmentData.EmbeddedMessage == null || mimeAttachmentData.EmbeddedMessage.RootPart != mimePart.FirstChild)
					{
						MimeTnefMessage message = new MimeTnefMessage((MimePart)mimePart.FirstChild);
						mimeAttachmentData.EmbeddedMessage = new EmailMessage(message);
					}
					result = mimeAttachmentData.EmbeddedMessage;
				}
			}
			return result;
		}

		internal override void Attachment_SetEmbeddedMessage(AttachmentCookie cookie, EmailMessage value)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (value.RootPart == null)
				{
					throw new InvalidOperationException(EmailMessageStrings.CannotAttachEmbeddedMapiMessageToMime);
				}
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				this.PromoteIfAppleDoubleAttachment(mimeAttachmentData);
				if (value != mimeAttachmentData.EmbeddedMessage)
				{
					if (mimeAttachmentData.EmbeddedMessage == null)
					{
						MimeTnefMessage message = new MimeTnefMessage(new MimePart());
						mimeAttachmentData.EmbeddedMessage = new EmailMessage(message);
					}
					value.CopyTo(mimeAttachmentData.EmbeddedMessage);
					mimeAttachmentData.AttachmentPart.RemoveAll();
					mimeAttachmentData.AttachmentPart.AppendChild(mimeAttachmentData.EmbeddedMessage.RootPart);
					Utility.UpdateTransferEncoding(mimeAttachmentData.AttachmentPart, mimeAttachmentData.EmbeddedMessage.RootPart.ContentTransferEncoding);
				}
			}
		}

		internal override string Attachment_GetFileName(AttachmentCookie cookie, ref int sequenceNumber)
		{
			string fileName;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				if (mimeAttachmentData.FileName != null)
				{
					fileName = mimeAttachmentData.FileName;
				}
				else
				{
					string rawFileName = Utility.GetRawFileName(mimeAttachmentData.AttachmentPart);
					mimeAttachmentData.FileName = Utility.SanitizeOrRegenerateFileName(rawFileName, ref sequenceNumber);
					fileName = mimeAttachmentData.FileName;
				}
			}
			return fileName;
		}

		internal override void Attachment_SetFileName(AttachmentCookie cookie, string value)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				this.PromoteIfAppleDoubleAttachment(mimeAttachmentData);
				InternalAttachmentType internalAttachmentType = this.Attachment_GetAttachmentType(cookie);
				AttachmentType attachmentType = (internalAttachmentType == InternalAttachmentType.Regular) ? AttachmentType.Regular : AttachmentType.Inline;
				Utility.SetFileName(mimeAttachmentData.AttachmentPart, attachmentType, value);
				mimeAttachmentData.FileName = value;
			}
		}

		internal override string Attachment_GetContentDisposition(AttachmentCookie cookie)
		{
			string headerValue;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				ContentDispositionHeader header = mimeAttachmentData.AttachmentPart.Headers.FindFirst(HeaderId.ContentDisposition) as ContentDispositionHeader;
				headerValue = Utility.GetHeaderValue(header);
			}
			return headerValue;
		}

		internal override void Attachment_SetContentDisposition(AttachmentCookie cookie, string value)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				this.PromoteIfAppleDoubleAttachment(mimeAttachmentData);
				Header header = mimeAttachmentData.AttachmentPart.Headers.FindFirst(HeaderId.ContentDisposition);
				if (header == null)
				{
					header = Header.Create(HeaderId.ContentDisposition);
					mimeAttachmentData.AttachmentPart.Headers.AppendChild(header);
				}
				header.Value = value;
			}
		}

		internal override bool Attachment_IsAppleDouble(AttachmentCookie cookie)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				bool flag = mimeAttachmentData.AttachmentPart.ContentType == "multipart/appledouble";
				result = flag;
			}
			return result;
		}

		internal override Stream Attachment_GetContentReadStream(AttachmentCookie cookie)
		{
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				Stream contentReadStream2;
				if (PureMimeMessage.IsAppleSingleAttachment(mimeAttachmentData))
				{
					Stream contentReadStream = Utility.GetContentReadStream(mimeAttachmentData.AttachmentPart);
					MimeAppleTranscoder.GetDataForkFromAppleSingle(contentReadStream, out contentReadStream2);
				}
				else
				{
					MimePart part = mimeAttachmentData.DataPart ?? mimeAttachmentData.AttachmentPart;
					contentReadStream2 = Utility.GetContentReadStream(part);
				}
				result = contentReadStream2;
			}
			return result;
		}

		internal override bool Attachment_TryGetContentReadStream(AttachmentCookie cookie, out Stream result)
		{
			bool result2;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				MimePart mimePart = mimeAttachmentData.DataPart ?? mimeAttachmentData.AttachmentPart;
				result2 = mimePart.TryGetContentReadStream(out result);
			}
			return result2;
		}

		internal override Stream Attachment_GetContentWriteStream(AttachmentCookie cookie)
		{
			Stream contentWriteStream;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				this.PromoteIfAppleDoubleAttachment(mimeAttachmentData);
				ContentTransferEncoding contentTransferEncoding;
				if (mimeAttachmentData.AttachmentPart.IsEmbeddedMessage)
				{
					contentTransferEncoding = ContentTransferEncoding.SevenBit;
					Utility.UpdateTransferEncoding(mimeAttachmentData.AttachmentPart, contentTransferEncoding);
				}
				else
				{
					contentTransferEncoding = mimeAttachmentData.AttachmentPart.ContentTransferEncoding;
					if (contentTransferEncoding == ContentTransferEncoding.Unknown || ContentTransferEncoding.SevenBit == contentTransferEncoding || ContentTransferEncoding.EightBit == contentTransferEncoding)
					{
						contentTransferEncoding = ContentTransferEncoding.Base64;
					}
				}
				if (PureMimeMessage.IsAppleSingleAttachment(mimeAttachmentData))
				{
					this.Attachment_SetContentType(cookie, "application/octet-stream");
				}
				contentWriteStream = mimeAttachmentData.AttachmentPart.GetContentWriteStream(contentTransferEncoding);
			}
			return contentWriteStream;
		}

		internal override int Attachment_GetRenderingPosition(AttachmentCookie cookie)
		{
			return -1;
		}

		internal override string Attachment_GetAttachContentID(AttachmentCookie cookie)
		{
			string headerValue;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				Header header = mimeAttachmentData.AttachmentPart.Headers.FindFirst(HeaderId.ContentId);
				headerValue = Utility.GetHeaderValue(header);
			}
			return headerValue;
		}

		internal override string Attachment_GetAttachContentLocation(AttachmentCookie cookie)
		{
			string headerValue;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				Header header = mimeAttachmentData.AttachmentPart.Headers.FindFirst(HeaderId.ContentLocation);
				headerValue = Utility.GetHeaderValue(header);
			}
			return headerValue;
		}

		internal override byte[] Attachment_GetAttachRendering(AttachmentCookie cookie)
		{
			return null;
		}

		internal override int Attachment_GetAttachmentFlags(AttachmentCookie cookie)
		{
			return 0;
		}

		internal override bool Attachment_GetAttachHidden(AttachmentCookie cookie)
		{
			return false;
		}

		internal override int Attachment_GetHashCode(AttachmentCookie cookie)
		{
			int hashCode;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				hashCode = mimeAttachmentData.AttachmentPart.GetHashCode();
			}
			return hashCode;
		}

		internal override void Attachment_Dispose(AttachmentCookie cookie)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeAttachmentData mimeAttachmentData = this.DataFromCookie(cookie);
				if (mimeAttachmentData.EmbeddedMessage != null)
				{
					mimeAttachmentData.EmbeddedMessage.Dispose();
					mimeAttachmentData.EmbeddedMessage = null;
				}
			}
		}

		private void PromoteIfAppleDoubleAttachment(MimeAttachmentData data)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				MimePart mimePart;
				MimePart mimePart2;
				if (Utility.TryGetAppleDoubleParts(data.AttachmentPart, out mimePart, out mimePart2))
				{
					bool isSynchronized = this.IsSynchronized;
					MimePart mimePart3 = data.AttachmentPart.Parent as MimePart;
					if (mimePart3 == null)
					{
						this.SetRootPart(mimePart);
					}
					else
					{
						mimePart.RemoveFromParent();
						mimePart3.ReplaceChild(mimePart, data.AttachmentPart);
					}
					data.AttachmentPart = mimePart;
					if (isSynchronized)
					{
						this.UpdateMimeVersion();
					}
				}
			}
		}

		internal override void CopyTo(MessageImplementation destination)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				PureMimeMessage pureMimeMessage = (PureMimeMessage)destination;
				using (ThreadAccessGuard.EnterPublic(pureMimeMessage.accessToken))
				{
					base.CopyTo(destination);
					pureMimeMessage.ToRecipients = null;
					pureMimeMessage.CcRecipients = null;
					pureMimeMessage.BccRecipients = null;
					pureMimeMessage.ReplyToRecipients = null;
					pureMimeMessage.FromRecipient = null;
					pureMimeMessage.SenderRecipient = null;
					pureMimeMessage.DntRecipient = null;
					pureMimeMessage.mimeAttachments = null;
					pureMimeMessage.messageType = MessageType.Undefined;
					pureMimeMessage.bodyStructure = BodyStructure.Undefined;
					pureMimeMessage.bodyTypes = BodyTypes.None;
					pureMimeMessage.version = -1;
					pureMimeMessage.placeholderBody = null;
					pureMimeMessage.multipartRelated = null;
					pureMimeMessage.multipartMixed = null;
					pureMimeMessage.multipartAlternative = null;
					pureMimeMessage.multipartSigned = null;
					pureMimeMessage.signaturePart = null;
					pureMimeMessage.messageClass = "IPM.Note";
					pureMimeMessage.tnefPart = null;
					pureMimeMessage.calendarPart = null;
					pureMimeMessage.bodyList = null;
					this.rootPart.CopyTo(pureMimeMessage.rootPart);
				}
			}
		}

		private void RemoveDegenerateMultiparts(MimePart parentPart)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				for (;;)
				{
					MimePart mimePart = parentPart.Parent as MimePart;
					if (parentPart.FirstChild == null)
					{
						if (mimePart == null || mimePart.IsEmbeddedMessage)
						{
							break;
						}
						parentPart.RemoveFromParent();
					}
					else
					{
						if (!Utility.HasExactlyOneChild(parentPart))
						{
							goto IL_C2;
						}
						if (mimePart == null)
						{
							this.SetRootPart(parentPart.FirstChild as MimePart);
						}
						else
						{
							MimeNode firstChild = parentPart.FirstChild;
							firstChild.RemoveFromParent();
							mimePart.ReplaceChild(firstChild, parentPart);
						}
					}
					if (this.multipartMixed == parentPart)
					{
						this.multipartMixed = null;
					}
					else if (this.multipartRelated == parentPart)
					{
						this.multipartRelated = null;
					}
					else if (this.multipartAlternative == parentPart)
					{
						this.multipartAlternative = null;
					}
					else if (this.multipartSigned == parentPart)
					{
						this.multipartSigned = null;
					}
					parentPart = mimePart;
					if (parentPart == null)
					{
						goto IL_C2;
					}
				}
				this.CreateEmptyMessage();
				IL_C2:;
			}
		}

		internal void UpdateMimeVersion()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.version = this.Version;
			}
		}

		internal void RebuildMessage()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.Synchronize();
				this.multipartMixed = null;
				this.multipartRelated = null;
				this.multipartAlternative = null;
				MimePart mimePart = this.NormalizeBodies();
				mimePart = this.NormalizeAttachments(mimePart);
				if (mimePart == null)
				{
					this.CreateEmptyMessage();
					mimePart = this.RootPart;
				}
				if (this.multipartSigned != null)
				{
					mimePart.RemoveFromParent();
					this.multipartSigned.InsertAfter(mimePart, null);
					if (this.multipartSigned.FirstChild.NextSibling != this.signaturePart)
					{
						this.multipartSigned.RemoveChild(this.multipartSigned.FirstChild.NextSibling);
					}
					mimePart = this.multipartSigned;
				}
				this.SetRootPart(mimePart);
				this.rebuildStructureAtNextOpportunity = false;
				this.Synchronize();
				this.UpdateMimeVersion();
			}
		}

		internal override void Synchronize()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (!this.IsSynchronized)
				{
					this.ResetSynchronization();
					this.messageClass = "IPM.Note";
					this.version = -1;
					this.messageType = MessageType.Undefined;
					this.bodyStructure = BodyStructure.Undefined;
					this.bodyTypes = BodyTypes.None;
					this.DetectMessageType();
					this.UpdateMimeVersion();
					this.RetireOldMimeAttachmentData();
					this.PickBestBody();
				}
			}
		}

		private void RetireOldMimeAttachmentData()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				for (int i = 0; i < this.mimeAttachments.InternalList.Count; i++)
				{
					MimeAttachmentData mimeAttachmentData = this.mimeAttachments.InternalList[i];
					if (mimeAttachmentData != null && !mimeAttachmentData.Referenced)
					{
						this.mimeAttachments.RemoveAtPrivateIndex(i);
					}
				}
			}
		}

		[Conditional("DEBUG")]
		private void ValidateStructure()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				MessageFlags messageFlags = MessageTypeTable.GetMessageFlags(this.messageType);
				if ((messageFlags & MessageFlags.Normal) > MessageFlags.None)
				{
					int count = this.bodyList.Count;
					foreach (MimeAttachmentData mimeAttachmentData in this.mimeAttachments.InternalList)
					{
						if (mimeAttachmentData != null && InternalAttachmentType.Related != mimeAttachmentData.InternalAttachmentType && this.multipartMixed == null)
						{
							MimeNode parent = mimeAttachmentData.AttachmentPart.Parent;
						}
					}
				}
				MimePart mimePart = this.multipartAlternative;
				MimePart mimePart2 = this.multipartRelated;
				MimePart mimePart3 = this.multipartMixed;
			}
		}

		private void FindBodiesAndAttachmentsHeuristically(MimePart root)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.ResetDetection();
				this.messageType = MessageType.Unknown;
				this.multipartMixed = null;
				this.multipartAlternative = null;
				this.multipartRelated = null;
				bool flag = true;
				MimePart mimePart = null;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = true;
				bool flag5 = true;
				bool flag6 = true;
				MimePart.SubtreeEnumerator subtreeEnumerator = new MimePart.SubtreeEnumerator(root, MimePart.SubtreeEnumerationOptions.RevisitParent, false);
				while (subtreeEnumerator.MoveNext())
				{
					MimePart mimePart2 = subtreeEnumerator.Current;
					string contentType = mimePart2.ContentType;
					if (!subtreeEnumerator.FirstVisit)
					{
						if (subtreeEnumerator.LastVisit)
						{
							if (mimePart2 == this.multipartAlternative)
							{
								flag2 = false;
							}
							if (mimePart2 == this.multipartRelated)
							{
								flag3 = false;
							}
						}
					}
					else
					{
						flag = (mimePart2 == this.RootPart || (flag && mimePart2 == mimePart2.Parent.FirstChild) || mimePart2 == mimePart);
						contentType == "multipart/signed";
						bool match = contentType == "multipart/mixed";
						bool match2 = contentType == "multipart/alternative";
						bool flag7 = contentType == "multipart/related";
						bool flag8 = contentType == "multipart/appledouble";
						contentType == "multipart/report";
						MimePart dataPart = null;
						MimePart mimePart3;
						if (flag8 && !Utility.TryGetAppleDoubleParts(mimePart2, out dataPart, out mimePart3))
						{
							flag8 = false;
							flag2 = false;
						}
						if (mimePart2 != this.placeholderBody && (!mimePart2.IsMultipart || flag8))
						{
							ContentDispositionHeader contentDispositionHeader = mimePart2.Headers.FindFirst(HeaderId.ContentDisposition) as ContentDispositionHeader;
							bool flag9 = contentDispositionHeader == null || "inline".Equals(Utility.GetHeaderValue(contentDispositionHeader), StringComparison.OrdinalIgnoreCase);
							bool flag10 = flag && Utility.IsBodyContentType(contentType);
							bool flag11 = this.multipartAlternative != null && this.multipartAlternative == mimePart2.Parent;
							bool flag12 = flag2 && mimePart2.Parent != null && this.multipartAlternative == mimePart2.Parent.Parent && (mimePart2 == mimePart || (mimePart == null && mimePart2 == mimePart2.Parent.FirstChild));
							if (flag9 && (flag10 || flag11 || flag12))
							{
								this.bodyList.Add(mimePart2);
							}
							else if (this.multipartSigned == null || mimePart2.Parent != this.multipartSigned)
							{
								InternalAttachmentType internalAttachmentType = flag3 ? InternalAttachmentType.Related : InternalAttachmentType.Regular;
								if (flag8)
								{
									this.GetAttachmentData(mimePart2, dataPart, this.Version);
									subtreeEnumerator.SkipChildren();
								}
								else
								{
									if (internalAttachmentType == InternalAttachmentType.Regular)
									{
										internalAttachmentType = Utility.CheckContentDisposition(mimePart2);
									}
									this.GetAttachmentData(mimePart2, internalAttachmentType, this.Version);
								}
							}
						}
						else if (flag7)
						{
							if ((flag || mimePart2.Parent == this.multipartAlternative) && PureMimeMessage.CheckKeyPart(mimePart2, flag7, ref flag6, ref this.multipartRelated) && !subtreeEnumerator.LastVisit)
							{
								flag3 = true;
							}
							mimePart = Utility.GetStartChild(mimePart2);
							if (mimePart != null)
							{
								flag = false;
							}
						}
						PureMimeMessage.CheckKeyPart(mimePart2, match, ref flag4, ref this.multipartMixed);
						if (flag && PureMimeMessage.CheckKeyPart(mimePart2, match2, ref flag5, ref this.multipartAlternative) && !subtreeEnumerator.LastVisit)
						{
							flag2 = true;
						}
					}
				}
			}
		}

		private bool LoadCalendar()
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.calendarMessage != null)
				{
					if (!this.calendarPart.ContentDirty)
					{
						return true;
					}
					this.calendarMessage = null;
				}
				this.calendarPart.ContentDirty = false;
				bool flag;
				PureCalendarMessage pureCalendarMessage = new PureCalendarMessage(this, this.calendarPart, PureMimeMessage.DetermineTextPartCharset(this.calendarPart, Charset.UTF8, out flag));
				if (!pureCalendarMessage.Load())
				{
					result = false;
				}
				else
				{
					this.calendarMessage = pureCalendarMessage;
					result = true;
				}
			}
			return result;
		}

		internal override IMapiPropertyAccess MapiProperties
		{
			get
			{
				return null;
			}
		}

		private AddressHeader GetRecipientHeader(RecipientType recipientType, ref object cached)
		{
			AddressHeader result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				AddressHeader addressHeader = cached as AddressHeader;
				if (addressHeader == null)
				{
					HeaderId headerId = PureMimeMessage.GetHeaderId(recipientType);
					addressHeader = this.GetRecipientHeader(headerId);
					cached = addressHeader;
				}
				result = addressHeader;
			}
			return result;
		}

		private AddressHeader GetRecipientHeader(HeaderId headerId)
		{
			AddressHeader result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				AddressHeader addressHeader = null;
				Header header2;
				for (Header header = this.rootPart.Headers.FindFirst(headerId); header != null; header = header2)
				{
					header2 = this.rootPart.Headers.FindNext(header);
					if (addressHeader == null)
					{
						addressHeader = (header as AddressHeader);
					}
					else
					{
						header.RemoveFromParent();
						foreach (MimeNode child in header)
						{
							Utility.MoveChildToNewParent(addressHeader, child);
						}
					}
				}
				if (addressHeader == null)
				{
					addressHeader = (Header.Create(headerId) as AddressHeader);
					this.rootPart.Headers.AppendChild(addressHeader);
				}
				result = addressHeader;
			}
			return result;
		}

		private EmailRecipient GetRecipient(HeaderId headerId)
		{
			EmailRecipient result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				AddressHeader addressHeader = Utility.FindLastHeader(this.rootPart, headerId) as AddressHeader;
				if (addressHeader == null)
				{
					result = null;
				}
				else
				{
					foreach (MimeNode mimeNode in addressHeader)
					{
						MimeRecipient mimeRecipient = mimeNode as MimeRecipient;
						if (mimeRecipient == null)
						{
							MimeGroup mimeGroup = mimeNode as MimeGroup;
							mimeRecipient = (mimeGroup.FirstChild as MimeRecipient);
							if (mimeRecipient == null)
							{
								continue;
							}
						}
						return new EmailRecipient(mimeRecipient);
					}
					result = null;
				}
			}
			return result;
		}

		private void SetRecipient(HeaderId headerId, EmailRecipient value)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				AddressHeader addressHeader = this.GetLastHeaderRemoveDuplicates(headerId) as AddressHeader;
				if (value != null)
				{
					if (addressHeader == null)
					{
						addressHeader = (Header.Create(headerId) as AddressHeader);
						this.rootPart.Headers.AppendChild(addressHeader);
					}
					addressHeader.RemoveAll();
					addressHeader.AppendChild(value.MimeRecipient);
				}
				else if (addressHeader != null)
				{
					addressHeader.RemoveFromParent();
				}
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
			}
		}

		private string GetHeaderString(HeaderId headerId)
		{
			string headerValue;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = Utility.FindLastHeader(this.rootPart, headerId);
				headerValue = Utility.GetHeaderValue(header);
			}
			return headerValue;
		}

		private DateTime GetHeaderDate(HeaderId headerId)
		{
			DateTime result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				DateHeader dateHeader = Utility.FindLastHeader(this.rootPart, headerId) as DateHeader;
				if (dateHeader == null)
				{
					result = DateTime.MinValue;
				}
				else
				{
					result = dateHeader.UtcDateTime;
				}
			}
			return result;
		}

		private bool TryGetHeaderEnum(HeaderId headerId, EnumUtility.StringIntPair[] map, ref int result)
		{
			bool result2;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				string headerString = this.GetHeaderString(headerId);
				result2 = EnumUtility.TryGetInt(map, headerString, ref result);
			}
			return result2;
		}

		private Header GetLastHeaderRemoveDuplicates(HeaderId headerId)
		{
			Header result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = null;
				for (Header header2 = this.rootPart.Headers.FindFirst(headerId); header2 != null; header2 = this.rootPart.Headers.FindNext(header2))
				{
					if (header != null)
					{
						header.RemoveFromParent();
					}
					header = header2;
				}
				result = header;
			}
			return result;
		}

		private Header GetLastHeader(HeaderId headerId)
		{
			Header result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = null;
				for (Header header2 = this.rootPart.Headers.FindFirst(headerId); header2 != null; header2 = this.rootPart.Headers.FindNext(header2))
				{
					header = header2;
				}
				result = header;
			}
			return result;
		}

		private void SetHeaderString(HeaderId headerId, string value)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				Header header = this.GetLastHeaderRemoveDuplicates(headerId);
				if (value != null)
				{
					if (header == null)
					{
						header = Header.Create(headerId);
						this.rootPart.Headers.AppendChild(header);
					}
					header.Value = value;
				}
				else if (header != null)
				{
					header.RemoveFromParent();
				}
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
			}
		}

		private void SetHeaderDate(HeaderId headerId, DateTime value)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				DateHeader dateHeader = this.GetLastHeaderRemoveDuplicates(headerId) as DateHeader;
				if (value != DateTime.MinValue)
				{
					if (dateHeader == null)
					{
						dateHeader = (Header.Create(headerId) as DateHeader);
						this.rootPart.Headers.AppendChild(dateHeader);
					}
					dateHeader.DateTime = value;
				}
				else if (dateHeader != null)
				{
					dateHeader.RemoveFromParent();
				}
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
			}
		}

		private void SetHeaderEnum(HeaderId headerId, EnumUtility.StringIntPair[] map, int value)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				if (value == 0)
				{
					this.RemoveHeaders(headerId);
				}
				else
				{
					string @string = EnumUtility.GetString(map, value);
					this.SetHeaderString(headerId, @string);
				}
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
			}
		}

		private void RemoveHeaders(HeaderId headerId)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header2;
				for (Header header = this.rootPart.Headers.FindFirst(headerId); header != null; header = header2)
				{
					header2 = this.rootPart.Headers.FindNext(header);
					header.RemoveFromParent();
				}
			}
		}

		private string CreateMessageId(bool allowUTF8)
		{
			string result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				string text = Guid.NewGuid().ToString();
				int num = PureMimeMessage.maxMessageIdLength - text.Length - 3;
				string text2 = null;
				try
				{
					string dnsPhysicalFullyQualifiedDomainName = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;
					if (MimeAddressParser.IsValidDomain(dnsPhysicalFullyQualifiedDomainName, 0, false, allowUTF8) && dnsPhysicalFullyQualifiedDomainName.Length <= num)
					{
						text2 = dnsPhysicalFullyQualifiedDomainName;
					}
				}
				catch (Exception)
				{
					text2 = null;
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "localhost";
				}
				StringBuilder stringBuilder = new StringBuilder(text.Length + text2.Length + 3);
				stringBuilder.Append('<');
				stringBuilder.Append(text);
				stringBuilder.Append('@');
				stringBuilder.Append(text2);
				stringBuilder.Append('>');
				result = stringBuilder.ToString();
			}
			return result;
		}

		private MimePart NormalizeBodies()
		{
			MimePart result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.bodyList.RemoveAll((MimePart part) => !Utility.IsBodyContentType(part.ContentType));
				this.bodyTypes = BodyTypes.None;
				List<MimePart> duplicates = new List<MimePart>();
				foreach (MimePart mimePart in this.bodyList)
				{
					BodyTypes bodyType = Utility.GetBodyType(mimePart.ContentType);
					if ((this.bodyTypes & bodyType) != BodyTypes.None || bodyType < this.bodyTypes)
					{
						duplicates.Add(mimePart);
					}
					else
					{
						this.bodyTypes |= bodyType;
					}
				}
				this.bodyList.RemoveAll((MimePart part) => duplicates.Contains(part));
				MimePart mimePart2 = null;
				if (this.bodyList.Count == 0)
				{
					if (this.NeedPlaceholderBody())
					{
						if (this.placeholderBody == null)
						{
							this.placeholderBody = new MimePart();
						}
						mimePart2 = this.placeholderBody;
					}
				}
				else if (1 == this.bodyList.Count)
				{
					MimePart mimePart3 = this.bodyList[0];
					mimePart3.RemoveFromParent();
					mimePart2 = mimePart3;
				}
				else
				{
					mimePart2 = new MimePart("multipart/alternative");
					this.multipartAlternative = mimePart2;
					foreach (MimePart child in this.bodyList)
					{
						Utility.MoveChildToNewParent(mimePart2, child);
					}
				}
				result = mimePart2;
			}
			return result;
		}

		private MimePart NormalizeAttachments(MimePart root)
		{
			MimePart result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.tnefPart != null)
				{
					root = this.NormalizeRegularAttachment(root, this.tnefPart);
				}
				List<MimeAttachmentData> internalList = this.mimeAttachments.InternalList;
				for (int i = 0; i < internalList.Count; i++)
				{
					MimeAttachmentData mimeAttachmentData = internalList[i];
					if (mimeAttachmentData != null)
					{
						if (mimeAttachmentData.InternalAttachmentType == InternalAttachmentType.Regular || mimeAttachmentData.InternalAttachmentType == InternalAttachmentType.Inline)
						{
							root = this.NormalizeRegularAttachment(root, mimeAttachmentData.AttachmentPart);
						}
						else
						{
							root = this.NormalizeRelatedAttachment(root, mimeAttachmentData.AttachmentPart, i);
						}
					}
				}
				result = root;
			}
			return result;
		}

		private MimePart NormalizeRegularAttachment(MimePart root, MimePart attachmentPart)
		{
			MimePart result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (root == null)
				{
					result = attachmentPart;
				}
				else
				{
					if (this.multipartMixed == null)
					{
						this.multipartMixed = new MimePart("multipart/mixed");
						Utility.MoveChildToNewParent(this.multipartMixed, root);
						root = this.multipartMixed;
					}
					Utility.MoveChildToNewParent(this.multipartMixed, attachmentPart);
					result = root;
				}
			}
			return result;
		}

		private MimePart NormalizeRelatedAttachment(MimePart root, MimePart attachmentPart, int index)
		{
			MimePart result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.bodyList.Count == 0)
				{
					this.mimeAttachments.RemoveAtPrivateIndex(index);
					result = root;
				}
				else
				{
					if (this.multipartRelated == null)
					{
						this.multipartRelated = new MimePart("multipart/related");
						if (this.multipartMixed == null)
						{
							if (root == this.multipartAlternative || Utility.IsBodyContentType(root.ContentType))
							{
								Utility.MoveChildToNewParent(this.multipartRelated, root);
								root = this.multipartRelated;
							}
						}
						else
						{
							Utility.MoveChildToNewParent(this.multipartRelated, this.multipartMixed.FirstChild);
							this.multipartMixed.InsertAfter(this.multipartRelated, null);
						}
					}
					Utility.MoveChildToNewParent(this.multipartRelated, attachmentPart);
					result = root;
				}
			}
			return result;
		}

		private void CreateEmptyMessage()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.ResetSynchronization();
				this.bodyData.SetFormat(BodyFormat.Text, InternalBodyFormat.Text, this.defaultBodyCharset);
				this.bodyData.ReleaseStorage();
				this.bodyMimePart = new MimePart("text/plain");
				this.bodyStructure = BodyStructure.SingleBody;
				this.bodyTypes = BodyTypes.Text;
				this.messageType = MessageType.Normal;
				this.SetRootPart(this.bodyMimePart);
			}
		}

		private void ResetSynchronization()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.bodyList == null)
				{
					this.bodyList = new List<MimePart>();
				}
				if (this.mimeAttachments == null)
				{
					this.mimeAttachments = new AttachmentDataCollection<MimeAttachmentData>();
				}
				this.ToRecipients = null;
				this.CcRecipients = null;
				this.BccRecipients = null;
				this.ReplyToRecipients = null;
				this.FromRecipient = null;
				this.SenderRecipient = null;
				this.DntRecipient = null;
				this.bodyData.SetFormat(BodyFormat.None, InternalBodyFormat.None, this.defaultBodyCharset);
				this.bodyData.ReleaseStorage();
				this.ResetDetection();
			}
		}

		private void ResetDetection()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.mimeAttachments.Reset();
				this.bodyList.Clear();
				this.bodyMimePart = null;
				this.bodyTypes = BodyTypes.None;
				this.bodyStructure = BodyStructure.Undefined;
				this.messageType = MessageType.Undefined;
				this.messageClass = "IPM.Note";
				this.multipartMixed = null;
				this.multipartRelated = null;
				this.multipartAlternative = null;
				this.multipartSigned = null;
				this.signaturePart = null;
				if (this.calendarMessage != null)
				{
					this.calendarMessage = null;
					if (this.calendarRelayStorage != null)
					{
						this.calendarRelayStorage.Release();
						this.calendarRelayStorage = null;
					}
				}
				this.calendarPart = null;
				this.rebuildStructureAtNextOpportunity = false;
			}
		}

		private MimePart SetRootPart(MimePart newRoot)
		{
			MimePart result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (newRoot == this.rootPart)
				{
					result = newRoot;
				}
				else
				{
					newRoot.RemoveFromParent();
					foreach (Header header in newRoot.Headers)
					{
						switch (header.HeaderId)
						{
						case HeaderId.ContentDescription:
						case HeaderId.ContentDisposition:
						case HeaderId.ContentMD5:
						case HeaderId.ContentTransferEncoding:
						case HeaderId.ContentType:
							break;
						default:
							header.RemoveFromParent();
							break;
						}
					}
					foreach (Header header2 in this.rootPart.Headers)
					{
						switch (header2.HeaderId)
						{
						case HeaderId.ContentDescription:
						case HeaderId.ContentDisposition:
						case HeaderId.ContentMD5:
						case HeaderId.ContentTransferEncoding:
						case HeaderId.ContentType:
							break;
						default:
							Utility.MoveChildToNewParent(newRoot.Headers, header2);
							break;
						}
					}
					if (this.mimeDocument != null)
					{
						this.mimeDocument.RootPart = newRoot;
						this.rootPart = newRoot;
					}
					else
					{
						MimeNode parent = this.rootPart.Parent;
						if (parent != null)
						{
							parent.ReplaceChild(newRoot, this.rootPart);
						}
						this.rootPart = newRoot;
					}
					result = newRoot;
				}
			}
			return result;
		}

		private void DetectMessageType()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (!this.IsSynchronized)
				{
					MimePart mimePart = this.RootPart;
					ContentTypeHeader contentTypeHeader = mimePart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
					if (contentTypeHeader == null)
					{
						this.messageType = MessageType.Normal;
						this.bodyTypes = BodyTypes.Text;
						this.bodyStructure = BodyStructure.SingleBody;
					}
					else
					{
						string headerValue = Utility.GetHeaderValue(contentTypeHeader);
						if (!this.DetectTnefStructure(headerValue) && !this.DetectVoiceMessage(headerValue) && !this.DetectJournalMessage(headerValue) && !this.DetectDsnOrMdn(headerValue, contentTypeHeader) && !this.DetectRightsProtectedMessage(headerValue) && !this.DetectFaxMessage(headerValue) && !this.DetectUMPartnerMessage(headerValue) && !this.DetectQuotaMessage(headerValue) && !this.DetectApprovalMessage(headerValue))
						{
							if (this.DetectSmime(headerValue, contentTypeHeader))
							{
								this.DetectInfoPathFormMessage();
							}
							else if (!this.DetectOpenPgp(headerValue, contentTypeHeader) && !this.DetectAdReplicationMessage(headerValue))
							{
								if (!this.DetectNormalMessage(mimePart, headerValue))
								{
									this.FindBodiesAndAttachmentsHeuristically(mimePart);
								}
								this.DetectSharingMessage();
								this.DetectInfoPathFormMessage();
								this.DetectCustomClassMessage();
							}
						}
					}
				}
			}
		}

		private bool DetectTnefStructure(string rootContentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (!this.DetectLegacyTnef(rootContentType) && !this.DetectSummaryTnef(rootContentType) && !this.DetectSuperLegacyTnefWithAttachments(rootContentType))
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private bool DetectLegacyTnef(string rootContentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (rootContentType != "multipart/mixed" || !Utility.HasExactlyTwoChildren(this.RootPart))
				{
					result = false;
				}
				else
				{
					MimePart mimePart = this.RootPart.FirstChild as MimePart;
					if (mimePart.ContentType != "text/plain")
					{
						result = false;
					}
					else
					{
						MimePart mimePart2 = mimePart.NextSibling as MimePart;
						if (!(mimePart2.ContentType == "application/ms-tnef"))
						{
							if (!(mimePart2.ContentType == "application/octet-stream"))
							{
								return false;
							}
							string parameterValue = Utility.GetParameterValue(mimePart2, HeaderId.ContentType, "name");
							string parameterValue2 = Utility.GetParameterValue(mimePart2, HeaderId.ContentDisposition, "filename");
							if (parameterValue != "winmail.dat" && parameterValue2 != "winmail.dat")
							{
								return false;
							}
						}
						this.messageType = MessageType.LegacyTnef;
						this.bodyStructure = BodyStructure.SingleBody;
						this.AddBody(mimePart);
						this.AddAttachment(mimePart2, InternalAttachmentType.Regular);
						this.multipartMixed = this.RootPart;
						result = true;
					}
				}
			}
			return result;
		}

		private bool DetectSummaryTnef(string rootContentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (rootContentType != "application/ms-tnef")
				{
					result = false;
				}
				else
				{
					string parameterValue = Utility.GetParameterValue(this.RootPart, HeaderId.ContentType, "name");
					if (parameterValue != "winmail.dat")
					{
						result = false;
					}
					else
					{
						this.messageType = MessageType.SummaryTnef;
						this.AddAttachment(this.RootPart, InternalAttachmentType.Regular);
						result = true;
					}
				}
			}
			return result;
		}

		private bool DetectSuperLegacyTnefWithAttachments(string rootContentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (rootContentType != "multipart/mixed")
				{
					result = false;
				}
				else
				{
					MimePart mimePart = this.RootPart.FirstChild as MimePart;
					if (mimePart == null || mimePart.ContentType != "text/plain")
					{
						result = false;
					}
					else
					{
						MimePart mimePart2 = this.RootPart.LastChild as MimePart;
						if (mimePart2 == null || mimePart2 == mimePart)
						{
							result = false;
						}
						else if (this.RootPart.Headers.FindFirst("X-ConvertedToMime") == null)
						{
							result = false;
						}
						else if (mimePart2.ContentType == "application/octet-stream")
						{
							string parameterValue = Utility.GetParameterValue(mimePart2, HeaderId.ContentType, "name");
							if (parameterValue != "winmail.dat")
							{
								result = false;
							}
							else
							{
								this.messageType = MessageType.LegacyTnef;
								this.bodyStructure = BodyStructure.SingleBody;
								this.AddBody(mimePart);
								if (!this.AddAttachments(this.RootPart, mimePart, InternalAttachmentType.Regular))
								{
									result = false;
								}
								else
								{
									this.multipartMixed = this.RootPart;
									result = true;
								}
							}
						}
						else
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		private bool DetectAdReplicationMessage(string rootContentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (rootContentType != "image/gif")
				{
					result = false;
				}
				else if (ContentTransferEncoding.Base64 != this.RootPart.ContentTransferEncoding)
				{
					result = false;
				}
				else
				{
					EmailRecipient from = this.From;
					if (from == null)
					{
						result = false;
					}
					else
					{
						EmailRecipientCollection to = this.To;
						if (to.Count != 1)
						{
							result = false;
						}
						else
						{
							EmailRecipient emailRecipient = to[0];
							string smtpAddress = from.SmtpAddress;
							string smtpAddress2 = emailRecipient.SmtpAddress;
							string text = "_IsmService";
							if (!smtpAddress.StartsWith(text, StringComparison.Ordinal))
							{
								result = false;
							}
							else if (!smtpAddress2.StartsWith(text, StringComparison.Ordinal))
							{
								result = false;
							}
							else
							{
								string text2 = "_msdcs";
								int num = 37;
								int num2 = text.Length + num;
								int num3 = num2 + text2.Length + 1;
								if (num3 > smtpAddress.Length || num3 > smtpAddress2.Length)
								{
									result = false;
								}
								else if (smtpAddress[num2] != '.' || smtpAddress2[num2] != '.')
								{
									result = false;
								}
								else
								{
									string text3 = smtpAddress.Substring(num2 + 1, text2.Length);
									if (!text3.Equals(text2))
									{
										result = false;
									}
									else
									{
										text3 = smtpAddress2.Substring(num2 + 1, text2.Length);
										if (!text3.Equals(text2))
										{
											result = false;
										}
										else
										{
											this.messageType = MessageType.AdReplicationMessage;
											this.AddAttachment(this.RootPart, InternalAttachmentType.Regular);
											result = true;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private bool DetectRightsProtectedMessage(string contentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (contentType != "multipart/mixed" || this.RootPart.FirstChild == null)
				{
					result = false;
				}
				else if (!Utility.HasExactlyTwoChildren(this.RootPart))
				{
					result = false;
				}
				else
				{
					MimePart mimePart = this.RootPart.FirstChild as MimePart;
					MimePart mimePart2 = mimePart.NextSibling as MimePart;
					if (!mimePart2.ContentType.Equals("application/x-microsoft-rpmsg-message", StringComparison.OrdinalIgnoreCase))
					{
						result = false;
					}
					else
					{
						AsciiTextHeader asciiTextHeader = this.RootPart.Headers.FindFirst(HeaderId.ContentClass) as AsciiTextHeader;
						if (asciiTextHeader == null)
						{
							result = false;
						}
						else
						{
							string headerValue = Utility.GetHeaderValue(asciiTextHeader);
							if (string.IsNullOrEmpty(headerValue))
							{
								result = false;
							}
							else
							{
								if (headerValue.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase))
								{
									this.messageClass = "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA";
								}
								else if (headerValue.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase))
								{
									this.messageClass = "IPM.Note.rpmsg.Microsoft.Voicemail.UM";
								}
								else if (!headerValue.Equals("rpmsg.message", StringComparison.OrdinalIgnoreCase))
								{
									return false;
								}
								if (!this.AddBody(mimePart))
								{
									if (!mimePart.ContentType.Equals("multipart/alternative", StringComparison.OrdinalIgnoreCase) || !this.FindBodiesAndRelatedAttachments(mimePart))
									{
										this.ResetDetection();
										return false;
									}
								}
								else
								{
									this.bodyStructure = BodyStructure.SingleBody;
								}
								this.messageType = MessageType.MsRightsProtected;
								this.AddAttachment(mimePart2, InternalAttachmentType.Regular);
								this.multipartMixed = this.RootPart;
								if (this.mimeAttachments.Count != 1)
								{
									this.ResetDetection();
									result = false;
								}
								else
								{
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private bool DetectOpenPgp(string contentType, ContentTypeHeader contentTypeHeader)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (contentType != "multipart/encrypted")
				{
					result = false;
				}
				else
				{
					MimePart mimePart = this.RootPart.FirstChild as MimePart;
					if (mimePart == null || mimePart.ContentType != "application/pgp-encrypted")
					{
						result = false;
					}
					else
					{
						MimePart mimePart2 = mimePart.NextSibling as MimePart;
						if (mimePart2 == null || mimePart2.ContentType != "application/octet-stream")
						{
							result = false;
						}
						else
						{
							MimePart mimePart3 = mimePart2.NextSibling as MimePart;
							if (mimePart3 != null)
							{
								result = false;
							}
							else
							{
								string parameterValue = Utility.GetParameterValue(contentTypeHeader, "protocol");
								if (string.IsNullOrEmpty(parameterValue))
								{
									result = false;
								}
								else if (!parameterValue.Equals("application/pgp-encrypted", StringComparison.OrdinalIgnoreCase))
								{
									result = false;
								}
								else
								{
									this.messageType = MessageType.PgpEncrypted;
									this.AddAttachment(mimePart, InternalAttachmentType.Regular);
									this.AddAttachment(mimePart2, InternalAttachmentType.Regular);
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private bool DetectSmime(string contentType, ContentTypeHeader contentTypeHeader)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (contentType == "application/pkcs7-mime" || contentType == "application/x-pkcs7-mime")
				{
					string parameterValue = Utility.GetParameterValue(contentTypeHeader, "smime-type");
					if (parameterValue == null)
					{
						this.messageType = MessageType.SmimeEncrypted;
					}
					else if (parameterValue.Equals("signed-data", StringComparison.OrdinalIgnoreCase))
					{
						this.messageType = MessageType.SmimeOpaqueSigned;
					}
					else if (parameterValue.Equals("enveloped-data", StringComparison.OrdinalIgnoreCase))
					{
						this.messageType = MessageType.SmimeEncrypted;
					}
					else
					{
						if (!parameterValue.Equals("certs-only", StringComparison.OrdinalIgnoreCase))
						{
							return false;
						}
						this.messageType = MessageType.SmimeOpaqueSigned;
					}
					this.messageClass = "IPM.Note.SMIME";
					result = true;
				}
				else
				{
					if (contentType == "application/octet-stream")
					{
						string parameterValue2 = Utility.GetParameterValue(contentTypeHeader, "name");
						string parameterValue3 = Utility.GetParameterValue(this.RootPart, HeaderId.ContentDisposition, "filename");
						if (PureMimeMessage.FileNameIndicatesSmime(parameterValue2) || PureMimeMessage.FileNameIndicatesSmime(parameterValue3))
						{
							this.messageClass = "IPM.Note.SMIME";
							this.messageType = MessageType.SmimeOpaqueSigned;
							return true;
						}
					}
					else if (contentType == "multipart/signed")
					{
						if (!Utility.HasExactlyTwoChildren(this.RootPart))
						{
							return false;
						}
						MimePart mimePart = this.RootPart.FirstChild as MimePart;
						if (mimePart == null)
						{
							return false;
						}
						MimePart mimePart2 = mimePart.NextSibling as MimePart;
						if (mimePart2 == null)
						{
							return false;
						}
						string contentType2 = mimePart2.ContentType;
						if (contentType2 == "application/pkcs7-signature" || contentType2 == "application/x-pkcs7-signature" || contentType2 == "application/pgp-signature")
						{
							string contentType3 = mimePart.ContentType;
							if (contentType3 == "application/pkcs7-mime" || contentType3 == "application/x-pkcs7-mime")
							{
								this.messageType = MessageType.SmimeSignedEncrypted;
								this.messageClass = "IPM.Note.SMIME";
							}
							else
							{
								if (this.DetectNormalMessage(mimePart, null))
								{
									this.messageType = MessageType.SmimeSignedNormal;
								}
								else
								{
									this.FindBodiesAndAttachmentsHeuristically(mimePart);
									this.messageType = MessageType.SmimeSignedUnknown;
								}
								this.messageClass = "IPM.Note.SMIME.MultipartSigned";
							}
							this.multipartSigned = this.RootPart;
							this.signaturePart = mimePart2;
							return true;
						}
						return false;
					}
					result = false;
				}
			}
			return result;
		}

		private bool DetectDsnOrMdn(string contentType, ContentTypeHeader contentTypeHeader)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (contentType != "multipart/report")
				{
					result = false;
				}
				else
				{
					string text = Utility.GetParameterValue(contentTypeHeader, "report-type");
					if (string.IsNullOrEmpty(text))
					{
						result = false;
					}
					else
					{
						text = text.ToLower();
						MimePart mimePart = this.RootPart.FirstChild as MimePart;
						MimePart mimePart2 = (mimePart == null) ? null : (mimePart.NextSibling as MimePart);
						Stream stream = null;
						try
						{
							if (mimePart2 != null && (mimePart2.ContentType == "message/delivery-status" || mimePart2.ContentType == "message/disposition-notification") && mimePart2.TryGetContentReadStream(out stream))
							{
								KeyValuePair<string, string>[] map = null;
								string text2 = null;
								if (string.Equals(text, "delivery-status", StringComparison.OrdinalIgnoreCase))
								{
									text2 = "action";
									map = PureMimeMessage.actionToClassSuffix;
									this.messageType = MessageType.Dsn;
									PureMimeMessage.ReadReportHeaders(stream);
								}
								else if (string.Equals(text, "disposition-notification", StringComparison.OrdinalIgnoreCase))
								{
									text2 = "disposition";
									map = PureMimeMessage.dispositionToClassSuffix;
									this.messageType = MessageType.Mdn;
								}
								if (!string.IsNullOrEmpty(text2))
								{
									Dictionary<string, string> dictionary = PureMimeMessage.ReadReportHeaders(stream);
									string text3 = null;
									while (dictionary != null && dictionary.Count > 0)
									{
										string text4;
										if (dictionary.TryGetValue(text2, out text4))
										{
											PureMimeMessage.GetMessageClassSuffix(map, text4.ToLowerInvariant(), ref text3);
										}
										dictionary = PureMimeMessage.ReadReportHeaders(stream);
									}
									if (!string.IsNullOrEmpty(text3) && this.FindBodiesAndRelatedAttachments(mimePart) && this.AddAttachments(this.RootPart, mimePart, InternalAttachmentType.Regular))
									{
										this.messageClass = "Report.IPM.Note." + text3;
										return true;
									}
								}
							}
						}
						finally
						{
							if (stream != null)
							{
								stream.Dispose();
								stream = null;
							}
						}
						this.ResetDetection();
						result = false;
					}
				}
			}
			return result;
		}

		private bool DetectJournalMessage(string contentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (contentType != "multipart/mixed")
				{
					result = false;
				}
				else if (this.RootPart.Headers.FindFirst("X-MS-Journal-Report") == null)
				{
					result = false;
				}
				else
				{
					MimePart mimePart = this.RootPart.FirstChild as MimePart;
					if (mimePart == null || mimePart.ContentType != "text/plain")
					{
						result = false;
					}
					else
					{
						MimePart mimePart2 = mimePart.NextSibling as MimePart;
						if (mimePart2 == null)
						{
							result = false;
						}
						else
						{
							bool flag = false;
							if (mimePart2.ContentType != "message/rfc822")
							{
								if (!mimePart2.ContentType.StartsWith("application/", StringComparison.OrdinalIgnoreCase))
								{
									return false;
								}
								flag = true;
							}
							MimePart mimePart3 = mimePart2.NextSibling as MimePart;
							if (mimePart3 != null)
							{
								if (flag || mimePart3.ContentType != "message/rfc822")
								{
									return false;
								}
								MimePart mimePart4 = mimePart3.NextSibling as MimePart;
								if (mimePart4 != null)
								{
									return false;
								}
							}
							if (!this.AddBody(mimePart))
							{
								this.ResetDetection();
								result = false;
							}
							else if (!this.AddAttachment(mimePart2, InternalAttachmentType.Regular))
							{
								this.ResetDetection();
								result = false;
							}
							else if (mimePart3 != null && !this.AddAttachment(mimePart3, InternalAttachmentType.Regular))
							{
								this.ResetDetection();
								result = false;
							}
							else
							{
								this.messageType = MessageType.Journal;
								this.multipartMixed = this.RootPart;
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private bool DetectQuotaMessage(string contentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = this.rootPart.Headers.FindFirst("X-MS-Exchange-Organization-StorageQuota");
				if (header == null)
				{
					result = false;
				}
				else if (!this.DetectNormalBodyStructure(this.RootPart))
				{
					this.ResetDetection();
					result = false;
				}
				else
				{
					string headerValue = Utility.GetHeaderValue(header);
					if (string.IsNullOrEmpty(headerValue))
					{
						this.ResetDetection();
						result = false;
					}
					else if (headerValue.Equals("warning", StringComparison.OrdinalIgnoreCase))
					{
						this.messageClass = "IPM.Note.StorageQuotaWarning.Warning";
						this.messageType = MessageType.Quota;
						result = true;
					}
					else if (headerValue.Equals("send", StringComparison.OrdinalIgnoreCase))
					{
						this.messageClass = "IPM.Note.StorageQuotaWarning.Send";
						this.messageType = MessageType.Quota;
						result = true;
					}
					else if (headerValue.Equals("sendreceive", StringComparison.OrdinalIgnoreCase))
					{
						this.messageClass = "IPM.Note.StorageQuotaWarning.SendReceive";
						this.messageType = MessageType.Quota;
						result = true;
					}
					else
					{
						this.ResetDetection();
						result = false;
					}
				}
			}
			return result;
		}

		private bool DetectVoiceMessage(string contentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = this.RootPart.Headers.FindFirst(HeaderId.ContentClass);
				if (header == null)
				{
					result = false;
				}
				else if (!this.DetectNormalMessage(this.RootPart, contentType))
				{
					this.ResetDetection();
					result = false;
				}
				else
				{
					string headerValue = Utility.GetHeaderValue(header);
					if (string.IsNullOrEmpty(headerValue))
					{
						this.ResetDetection();
						result = false;
					}
					else
					{
						foreach (MimeAttachmentData mimeAttachmentData in this.mimeAttachments.InternalList)
						{
							if (mimeAttachmentData != null)
							{
								string contentType2 = mimeAttachmentData.AttachmentPart.ContentType;
								if (!PureMimeMessage.IsVoiceContentType(contentType2))
								{
									this.ResetDetection();
									return false;
								}
							}
						}
						if (headerValue.Equals("voice", StringComparison.OrdinalIgnoreCase))
						{
							this.messageClass = "IPM.Note.Microsoft.Voicemail.UM";
							this.messageType = MessageType.Voice;
							result = true;
						}
						else if (headerValue.Equals("voice-ca", StringComparison.OrdinalIgnoreCase))
						{
							this.messageClass = "IPM.Note.Microsoft.Voicemail.UM.CA";
							this.messageType = MessageType.Voice;
							result = true;
						}
						else if (headerValue.Equals("voice-uc", StringComparison.OrdinalIgnoreCase))
						{
							this.messageClass = "IPM.Note.Microsoft.Conversation.Voice";
							this.messageType = MessageType.Voice;
							result = true;
						}
						else if (headerValue.Equals("missedcall", StringComparison.OrdinalIgnoreCase))
						{
							this.messageClass = "IPM.Note.Microsoft.Missed.Voice";
							this.messageType = MessageType.Voice;
							result = true;
						}
						else
						{
							this.ResetDetection();
							result = false;
						}
					}
				}
			}
			return result;
		}

		private bool DetectUMPartnerMessage(string contentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = this.RootPart.Headers.FindFirst(HeaderId.ContentClass);
				if (header == null)
				{
					result = false;
				}
				else if (!this.DetectNormalMessage(this.RootPart, contentType))
				{
					this.ResetDetection();
					result = false;
				}
				else
				{
					string headerValue = Utility.GetHeaderValue(header);
					if (string.IsNullOrEmpty(headerValue))
					{
						this.ResetDetection();
						result = false;
					}
					else if (headerValue.Equals("MS-Exchange-UM-Partner", StringComparison.OrdinalIgnoreCase))
					{
						this.messageClass = "IPM.Note.Microsoft.Partner.UM";
						this.messageType = MessageType.UMPartner;
						result = true;
					}
					else
					{
						this.ResetDetection();
						result = false;
					}
				}
			}
			return result;
		}

		private bool DetectSharingMessage()
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = this.RootPart.Headers.FindFirst(HeaderId.ContentClass);
				if (header == null)
				{
					result = false;
				}
				else
				{
					string headerValue = Utility.GetHeaderValue(header);
					if (string.IsNullOrEmpty(headerValue))
					{
						result = false;
					}
					else if (headerValue.Equals("Sharing", StringComparison.OrdinalIgnoreCase))
					{
						this.messageClass = "IPM.Sharing";
						this.messageType = MessageType.Normal;
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		private bool DetectCustomClassMessage()
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = this.rootPart.Headers.FindFirst(HeaderId.ContentClass);
				if (header == null)
				{
					result = false;
				}
				else
				{
					string headerValue = Utility.GetHeaderValue(header);
					if (string.IsNullOrEmpty(headerValue))
					{
						result = false;
					}
					else if (headerValue.Equals("RSS", StringComparison.OrdinalIgnoreCase))
					{
						this.messageClass = "IPM.Post.RSS";
						result = true;
					}
					else if (headerValue.Equals("MS-OMS-SMS", StringComparison.OrdinalIgnoreCase))
					{
						this.messageClass = "IPM.Note.Mobile.SMS";
						result = true;
					}
					else if (headerValue.Equals("MS-OMS-MMS", StringComparison.OrdinalIgnoreCase))
					{
						this.messageClass = "IPM.Note.Mobile.MMS";
						result = true;
					}
					else if (headerValue.StartsWith("urn:content-class:custom.", StringComparison.OrdinalIgnoreCase))
					{
						string str = headerValue.Substring("urn:content-class:custom.".Length);
						this.messageClass = "IPM.Note.Custom." + str;
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		private bool DetectInfoPathFormMessage()
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = this.rootPart.Headers.FindFirst(HeaderId.ContentClass);
				if (header == null)
				{
					result = false;
				}
				else
				{
					string headerValue = Utility.GetHeaderValue(header);
					if (string.IsNullOrEmpty(headerValue))
					{
						result = false;
					}
					else
					{
						if (headerValue.StartsWith("InfoPathForm.", StringComparison.OrdinalIgnoreCase))
						{
							string text = headerValue.Substring("InfoPathForm.".Length);
							int num = text.IndexOf('.');
							if (num > 0)
							{
								try
								{
									string text2 = text.Substring(0, num);
									new Guid(text2);
									text.Substring(num + 1);
									string str = string.Empty;
									if (this.messageType == MessageType.SmimeEncrypted || this.messageType == MessageType.SmimeOpaqueSigned)
									{
										str = ".SMIME";
									}
									else if (this.messageType == MessageType.SmimeSignedEncrypted || this.messageType == MessageType.SmimeSignedNormal || this.messageType == MessageType.SmimeSignedUnknown)
									{
										str = ".SMIME.MultipartSigned";
									}
									this.messageClass = "IPM.InfoPathForm." + text2 + str;
									return true;
								}
								catch (FormatException)
								{
								}
								catch (OverflowException)
								{
								}
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		private bool DetectFaxMessage(string contentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				Header header = this.rootPart.Headers.FindFirst(HeaderId.ContentClass);
				if (header == null)
				{
					result = false;
				}
				else if ("multipart/mixed" != contentType)
				{
					result = false;
				}
				else
				{
					MimePart mimePart = this.RootPart.FirstChild as MimePart;
					if (mimePart == null)
					{
						result = false;
					}
					else
					{
						MimePart mimePart2 = mimePart.NextSibling as MimePart;
						if (mimePart2 == null)
						{
							result = false;
						}
						else if ("text/html" != mimePart.ContentType)
						{
							result = false;
						}
						else if ("image/tiff" != mimePart2.ContentType)
						{
							result = false;
						}
						else if (!this.AddBody(mimePart))
						{
							this.ResetDetection();
							result = false;
						}
						else if (!this.AddAttachment(mimePart2, InternalAttachmentType.Regular))
						{
							this.ResetDetection();
							result = false;
						}
						else
						{
							string headerValue = Utility.GetHeaderValue(header);
							if (string.IsNullOrEmpty(headerValue))
							{
								this.ResetDetection();
								result = false;
							}
							else if (headerValue.Equals("fax", StringComparison.OrdinalIgnoreCase))
							{
								this.messageClass = "IPM.Note.Microsoft.Fax";
								this.messageType = MessageType.Fax;
								this.multipartMixed = this.RootPart;
								result = true;
							}
							else if (headerValue.Equals("fax-ca", StringComparison.OrdinalIgnoreCase))
							{
								this.messageClass = "IPM.Note.Microsoft.Fax.CA";
								this.messageType = MessageType.Fax;
								this.multipartMixed = this.RootPart;
								result = true;
							}
							else
							{
								this.ResetDetection();
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		private bool DetectApprovalMessage(string contentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Approval-Initiator") == null)
				{
					this.ResetDetection();
					result = false;
				}
				else if (!this.DetectNormalMessage(this.RootPart, contentType))
				{
					this.ResetDetection();
					result = false;
				}
				else
				{
					this.messageClass = "IPM.Microsoft.Approval.Initiation";
					this.messageType = MessageType.ApprovalInitiation;
					result = true;
				}
			}
			return result;
		}

		private bool DetectNormalMessage(MimePart part, string contentType)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (string.IsNullOrEmpty(contentType))
				{
					contentType = part.ContentType;
				}
				if (this.AddBody(part))
				{
					this.messageType = MessageType.Normal;
					this.bodyStructure = BodyStructure.SingleBody;
					result = true;
				}
				else if (this.IsAttachmentCandidate(part))
				{
					this.AddAttachment(part, InternalAttachmentType.Regular);
					this.messageType = MessageType.SingleAttachment;
					this.bodyTypes = BodyTypes.None;
					this.bodyStructure = BodyStructure.None;
					result = true;
				}
				else if (contentType == "multipart/mixed")
				{
					this.multipartMixed = part;
					if (!this.DetectNormalMultipartMixed(part))
					{
						this.multipartMixed = null;
						result = false;
					}
					else
					{
						result = true;
					}
				}
				else if (contentType == "multipart/related" || contentType == "multipart/alternative")
				{
					this.messageType = MessageType.Normal;
					if (!this.DetectNormalBodyStructure(part))
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private bool DetectNormalMultipartMixed(MimePart root)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				MimePart mimePart = root.FirstChild as MimePart;
				if (mimePart == null)
				{
					result = false;
				}
				else
				{
					if (mimePart.NextSibling == null)
					{
						this.rebuildStructureAtNextOpportunity = true;
					}
					this.messageType = MessageType.MultipleAttachments;
					this.bodyStructure = BodyStructure.None;
					bool flag = this.bodyList.Count == 0;
					while (mimePart != null)
					{
						string contentType = mimePart.ContentType;
						if (flag && this.AddBody(mimePart))
						{
							this.messageType = MessageType.NormalWithRegularAttachments;
							this.bodyStructure = BodyStructure.SingleBody;
							flag = false;
						}
						else if (flag && (contentType == "multipart/related" || contentType == "multipart/alternative"))
						{
							this.messageType = MessageType.NormalWithRegularAttachments;
							if (!this.DetectNormalBodyStructure(mimePart))
							{
								return false;
							}
							flag = false;
						}
						else
						{
							if (!this.IsAttachmentCandidate(mimePart))
							{
								return false;
							}
							this.AddAttachment(mimePart, InternalAttachmentType.Regular);
						}
						mimePart = (mimePart.NextSibling as MimePart);
					}
					result = true;
				}
			}
			return result;
		}

		private bool IsAttachmentCandidate(MimePart part)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				MimePart mimePart;
				MimePart mimePart2;
				if (!part.IsMultipart)
				{
					result = true;
				}
				else if (Utility.TryGetAppleDoubleParts(part, out mimePart, out mimePart2))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private bool DetectNormalBodyStructure(MimePart root)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.bodyStructure = BodyStructure.Undefined;
				this.bodyTypes = BodyTypes.None;
				if (!this.FindBodiesAndRelatedAttachments(root))
				{
					this.bodyStructure = BodyStructure.None;
					this.bodyTypes = BodyTypes.None;
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		internal bool FindBodiesAndRelatedAttachments(MimePart root)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				string contentType = root.ContentType;
				if (this.AddBody(root))
				{
					this.bodyStructure = BodyStructure.SingleBody;
					result = true;
				}
				else if (contentType == "multipart/alternative")
				{
					if (root.FirstChild == null)
					{
						result = false;
					}
					else
					{
						if (root.FirstChild.NextSibling == null)
						{
							this.rebuildStructureAtNextOpportunity = true;
						}
						bool flag = false;
						foreach (MimePart bodyPart in root)
						{
							if (!this.AddAlternativeBody(bodyPart, ref flag))
							{
								return false;
							}
						}
						if (flag)
						{
							this.bodyStructure = BodyStructure.AlternativeBodiesWithMhtml;
						}
						else
						{
							this.bodyStructure = BodyStructure.AlternativeBodies;
						}
						this.multipartAlternative = root;
						result = true;
					}
				}
				else if (contentType == "multipart/related")
				{
					if (root.FirstChild == null)
					{
						result = false;
					}
					else
					{
						if (root.FirstChild.NextSibling == null)
						{
							this.rebuildStructureAtNextOpportunity = true;
						}
						MimePart startChild = Utility.GetStartChild(root);
						if (startChild == null)
						{
							result = false;
						}
						else
						{
							string contentType2 = startChild.ContentType;
							if (contentType2 == "text/html")
							{
								if (!this.AddBody(startChild))
								{
									result = false;
								}
								else if (!this.AddAttachments(root, startChild, InternalAttachmentType.Related))
								{
									result = false;
								}
								else
								{
									this.multipartRelated = root;
									this.bodyStructure = BodyStructure.SingleBodyWithRelatedAttachments;
									result = true;
								}
							}
							else if (contentType2 == "multipart/alternative")
							{
								if (startChild.FirstChild == null)
								{
									result = false;
								}
								else
								{
									if (startChild.FirstChild.NextSibling == null)
									{
										this.rebuildStructureAtNextOpportunity = true;
									}
									foreach (MimePart part in startChild)
									{
										if (!this.AddBody(part))
										{
											return false;
										}
									}
									if ((this.bodyTypes & BodyTypes.Html) == BodyTypes.None)
									{
										result = false;
									}
									else if (!this.AddAttachments(root, startChild, InternalAttachmentType.Related))
									{
										result = false;
									}
									else
									{
										this.bodyStructure = BodyStructure.AlternativeBodiesWithSharedAttachments;
										this.multipartRelated = root;
										this.multipartAlternative = startChild;
										result = true;
									}
								}
							}
							else
							{
								result = false;
							}
						}
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private bool AddAlternativeBody(MimePart bodyPart, ref bool mhtml)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				BodyTypes bodyType = Utility.GetBodyType(bodyPart.ContentType);
				if (bodyType != BodyTypes.None)
				{
					bool flag = this.AddBody(bodyPart);
					result = flag;
				}
				else if (bodyPart.ContentType != "multipart/related")
				{
					result = false;
				}
				else
				{
					this.multipartRelated = bodyPart;
					MimePart startChild = Utility.GetStartChild(bodyPart);
					if (startChild == null)
					{
						result = false;
					}
					else
					{
						string contentType = startChild.ContentType;
						if (contentType != "text/html")
						{
							result = false;
						}
						else if (!this.AddBody(startChild))
						{
							result = false;
						}
						else if (!this.AddAttachments(bodyPart, startChild, InternalAttachmentType.Related))
						{
							result = false;
						}
						else
						{
							mhtml = true;
							result = true;
						}
					}
				}
			}
			return result;
		}

		private void DetectCalendarAttachment(MimePart part)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (part.Parent == this.multipartMixed && part.ContentType == "text/calendar")
				{
					ContentDispositionHeader header = part.Headers.FindFirst(HeaderId.ContentDisposition) as ContentDispositionHeader;
					string headerValue = Utility.GetHeaderValue(header);
					if (headerValue != "attachment" && this.calendarPart == null)
					{
						this.calendarPart = part;
					}
				}
			}
		}

		private bool AddAttachments(MimePart multipart, MimePart exclude, InternalAttachmentType type)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				foreach (MimePart mimePart in multipart)
				{
					if (mimePart != exclude && !this.AddAttachment(mimePart, type))
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		private bool AddAttachment(MimePart part, InternalAttachmentType type)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (part == this.tnefPart)
				{
					result = true;
				}
				else if (part.IsMultipart)
				{
					if (type != InternalAttachmentType.Regular)
					{
						result = false;
					}
					else
					{
						result = this.AddAppleDoubleAttachment(part);
					}
				}
				else
				{
					this.DetectCalendarAttachment(part);
					if (type == InternalAttachmentType.Regular)
					{
						type = Utility.CheckContentDisposition(part);
					}
					this.GetAttachmentData(part, type, this.Version);
					result = true;
				}
			}
			return result;
		}

		private bool AddAppleDoubleAttachment(MimePart part)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				MimePart dataPart;
				MimePart mimePart;
				if (Utility.TryGetAppleDoubleParts(part, out dataPart, out mimePart))
				{
					this.GetAttachmentData(part, dataPart, this.Version);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private bool AddBody(MimePart part)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				BodyTypes bodyType = Utility.GetBodyType(part.ContentType);
				if (bodyType == BodyTypes.None)
				{
					result = false;
				}
				else if (this.bodyTypes >= bodyType)
				{
					result = false;
				}
				else
				{
					string headerValue = Utility.GetHeaderValue(part, HeaderId.ContentDisposition);
					if (string.Equals(headerValue, "attachment", StringComparison.OrdinalIgnoreCase))
					{
						result = false;
					}
					else
					{
						this.bodyTypes |= bodyType;
						if (bodyType == BodyTypes.Calendar)
						{
							this.calendarPart = part;
						}
						this.bodyList.Add(part);
						result = true;
					}
				}
			}
			return result;
		}

		private MimeAttachmentData GetAttachmentData(MimePart part, InternalAttachmentType attachmentType, int snapshotVersion)
		{
			MimeAttachmentData result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				int num;
				MimeAttachmentData attachmentData = this.GetAttachmentData(part, snapshotVersion, this, out num);
				attachmentData.InternalAttachmentType = attachmentType;
				result = attachmentData;
			}
			return result;
		}

		private MimeAttachmentData GetAttachmentData(MimePart part, InternalAttachmentType attachmentType, int snapshotVersion, out int index)
		{
			MimeAttachmentData result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				MimeAttachmentData attachmentData = this.GetAttachmentData(part, snapshotVersion, this, out index);
				attachmentData.InternalAttachmentType = attachmentType;
				result = attachmentData;
			}
			return result;
		}

		private MimeAttachmentData GetAttachmentData(MimePart part, MimePart dataPart, int snapshotVersion)
		{
			MimeAttachmentData result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				int num;
				MimeAttachmentData attachmentData = this.GetAttachmentData(part, snapshotVersion, this, out num);
				attachmentData.InternalAttachmentType = InternalAttachmentType.Regular;
				attachmentData.DataPart = dataPart;
				result = attachmentData;
			}
			return result;
		}

		private MimeAttachmentData GetAttachmentData(MimePart part, int snapshotVersion, MessageImplementation message, out int index)
		{
			MimeAttachmentData result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				for (index = 0; index < this.mimeAttachments.InternalList.Count; index++)
				{
					MimeAttachmentData mimeAttachmentData = this.mimeAttachments.InternalList[index];
					if (mimeAttachmentData != null && mimeAttachmentData.AttachmentPart == part)
					{
						mimeAttachmentData.Referenced = true;
						mimeAttachmentData.FlushCache();
						return mimeAttachmentData;
					}
				}
				MimeAttachmentData mimeAttachmentData2 = new MimeAttachmentData(part, message);
				index = this.mimeAttachments.Add(mimeAttachmentData2);
				result = mimeAttachmentData2;
			}
			return result;
		}

		private bool NeedPlaceholderBody()
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.mimeAttachments.Count == 0)
				{
					result = true;
				}
				else
				{
					MimeAttachmentData dataAtPublicIndex = this.mimeAttachments.GetDataAtPublicIndex(0);
					MimePart mimePart = dataAtPublicIndex.DataPart ?? dataAtPublicIndex.AttachmentPart;
					string contentType = mimePart.ContentType;
					bool flag = Utility.IsBodyContentType(contentType);
					result = flag;
				}
			}
			return result;
		}

		internal void SetBodyPartCharset(MimePart part, Charset charset)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				bool isSynchronized = this.IsSynchronized;
				this.SetPartCharset(part, charset.Name);
				if (isSynchronized)
				{
					this.UpdateMimeVersion();
				}
			}
		}

		private void GetCharsetFromMimeDocument(MimeDocument mimeDocument)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (mimeDocument.EffectiveHeaderDecodingOptions.Charset != null)
				{
					this.defaultBodyCharset = mimeDocument.EffectiveHeaderDecodingOptions.Charset;
				}
				if (this.defaultBodyCharset == null)
				{
					this.defaultBodyCharset = Charset.DefaultMimeCharset;
				}
			}
		}

		[Conditional("DEBUG")]
		private void AssertNothingDetectedYet()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				int num = 0;
				foreach (MimeAttachmentData mimeAttachmentData in this.mimeAttachments)
				{
					if (mimeAttachmentData != null && mimeAttachmentData.Referenced)
					{
						num++;
					}
				}
			}
		}

		internal EmailRecipientCollection ToRecipients;

		internal EmailRecipientCollection CcRecipients;

		internal EmailRecipientCollection BccRecipients;

		internal EmailRecipientCollection BccFromOrgHeaderRecipients;

		internal EmailRecipientCollection ReplyToRecipients;

		internal EmailRecipient FromRecipient;

		internal EmailRecipient SenderRecipient;

		internal EmailRecipient DntRecipient;

		private static string[] smimeExtensions = new string[]
		{
			"p7m",
			"p7s",
			"p7c"
		};

		private static int maxMessageIdLength = 1953;

		private static KeyValuePair<string, string>[] actionToClassSuffix = new KeyValuePair<string, string>[]
		{
			new KeyValuePair<string, string>("delivered", "DR"),
			new KeyValuePair<string, string>("expanded", "Expanded.DR"),
			new KeyValuePair<string, string>("relayed", "Relayed.DR"),
			new KeyValuePair<string, string>("delayed", "Delayed.DR"),
			new KeyValuePair<string, string>("failed", "NDR")
		};

		private static KeyValuePair<string, string>[] dispositionToClassSuffix = new KeyValuePair<string, string>[]
		{
			new KeyValuePair<string, string>("displayed", "IPNRN"),
			new KeyValuePair<string, string>("dispatched", "IPNRN"),
			new KeyValuePair<string, string>("processed", "IPNRN"),
			new KeyValuePair<string, string>("deleted", "IPNNRN"),
			new KeyValuePair<string, string>("denied", "IPNNRN"),
			new KeyValuePair<string, string>("failed", "IPNNRN")
		};

		private PureMimeMessage.PureMimeMessageThreadAccessToken accessToken;

		private MimeDocument mimeDocument;

		private MimePart rootPart;

		private MessageType messageType;

		private BodyStructure bodyStructure;

		private BodyTypes bodyTypes;

		private bool rebuildStructureAtNextOpportunity;

		private int version = -1;

		private MimePart placeholderBody;

		private MimePart multipartRelated;

		private MimePart multipartMixed;

		private MimePart multipartAlternative;

		private MimePart multipartSigned;

		private MimePart signaturePart;

		private string messageClass = "IPM.Note";

		private MimePart tnefPart;

		private PureCalendarMessage calendarMessage;

		private MimePart calendarPart;

		private RelayStorage calendarRelayStorage;

		private List<MimePart> bodyList;

		private MimePart bodyMimePart;

		private MimePart bodyWriteStreamMimePart;

		private BodyData bodyData = new BodyData();

		private AttachmentDataCollection<MimeAttachmentData> mimeAttachments = new AttachmentDataCollection<MimeAttachmentData>();

		private Charset defaultBodyCharset;

		private class PureMimeMessageThreadAccessToken : ObjectThreadAccessToken
		{
			internal PureMimeMessageThreadAccessToken(PureMimeMessage parent)
			{
			}
		}
	}
}
