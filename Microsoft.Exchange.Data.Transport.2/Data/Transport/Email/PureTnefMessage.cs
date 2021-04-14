using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Encoders;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class PureTnefMessage : MessageImplementation, IBody, IMapiPropertyAccess, IRelayable
	{
		internal PureTnefMessage(MimeTnefMessage topMessage, MimePart tnefPart, DataStorage tnefStorage, long tnefStart, long tnefEnd)
		{
			tnefStorage.AddRef();
			this.tnefStorage = tnefStorage;
			this.tnefStart = tnefStart;
			this.tnefEnd = tnefEnd;
			this.topMessage = topMessage;
			this.properties = new TnefPropertyBag(this);
			this.accessToken = new PureTnefMessage.PureTnefMessageThreadAccessToken(this);
		}

		internal PureTnefMessage(TnefAttachmentData parentAttachmentData, DataStorage tnefStorage, long tnefStart, long tnefEnd)
		{
			tnefStorage.AddRef();
			this.tnefStorage = tnefStorage;
			this.tnefStart = tnefStart;
			this.tnefEnd = tnefEnd;
			this.parentAttachmentData = parentAttachmentData;
			this.properties = new TnefPropertyBag(this);
			this.accessToken = new PureTnefMessage.PureTnefMessageThreadAccessToken(this);
		}

		internal static IEnumerable<string> SystemMessageClassNames
		{
			get
			{
				return PureTnefMessage.systemMessageClassNames;
			}
		}

		internal static IEnumerable<string> SystemMessageClassPrefixes
		{
			get
			{
				return PureTnefMessage.systemMessageClassPrefixes;
			}
		}

		internal static IEnumerable<string> InterpersonalMessageClassNames
		{
			get
			{
				return PureTnefMessage.interpersonalMessageClassNames;
			}
		}

		internal static IEnumerable<string> InterpersonalMessageClassPrefixes
		{
			get
			{
				return PureTnefMessage.interpersonalMessageClassPrefixes;
			}
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
				this.ThrowIfDisposed();
				if (this.FromRecipient == null && this.GetProperty(TnefPropertyTag.SentRepresentingEmailAddressA) != null)
				{
					this.FromRecipient = new EmailRecipient(new TnefRecipient(this, int.MinValue, (this.GetProperty(TnefPropertyTag.SentRepresentingNameA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SentRepresentingEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SentRepresentingEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SentRepresentingAddrtypeA) as string) ?? string.Empty));
				}
				return this.FromRecipient;
			}
			set
			{
				this.ThrowIfDisposed();
				this.FromRecipient = value;
				if (value == null)
				{
					this.SetProperty(TnefPropertyTag.SentRepresentingNameA, null);
					this.SetProperty(TnefPropertyTag.SentRepresentingEmailAddressA, null);
					this.SetProperty(TnefPropertyTag.SentRepresentingAddrtypeA, null);
				}
				else
				{
					this.SetProperty(TnefPropertyTag.SentRepresentingNameA, value.DisplayName);
					this.SetProperty(TnefPropertyTag.SentRepresentingEmailAddressA, value.SmtpAddress);
					this.SetProperty(TnefPropertyTag.SentRepresentingAddrtypeA, value.NativeAddressType);
				}
				this.SetProperty(TnefPropertyTag.SentRepresentingEntryId, null);
			}
		}

		public override EmailRecipientCollection To
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.ToRecipients == null)
				{
					this.ToRecipients = new EmailRecipientCollection(this, RecipientType.To);
				}
				return this.ToRecipients;
			}
		}

		public override EmailRecipientCollection Cc
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.CcRecipients == null)
				{
					this.CcRecipients = new EmailRecipientCollection(this, RecipientType.Cc);
				}
				return this.CcRecipients;
			}
		}

		public override EmailRecipientCollection Bcc
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.BccRecipients == null)
				{
					this.BccRecipients = new EmailRecipientCollection(this, RecipientType.Bcc, true);
				}
				return this.BccRecipients;
			}
		}

		public override EmailRecipientCollection ReplyTo
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.ReplyToRecipients == null)
				{
					this.ReplyToRecipients = new EmailRecipientCollection(this, RecipientType.ReplyTo, true);
				}
				return this.ReplyToRecipients;
			}
		}

		public override EmailRecipient DispositionNotificationTo
		{
			get
			{
				this.ThrowIfDisposed();
				if (!(bool)(this.GetProperty(TnefPropertyTag.ReadReceiptRequested) ?? false))
				{
					return null;
				}
				if (this.DntRecipient == null)
				{
					if (this.properties[TnefPropertyTag.ReadReceiptEmailAddressA] != null)
					{
						this.DntRecipient = new EmailRecipient(new TnefRecipient(this, int.MinValue, (this.GetProperty(TnefPropertyTag.ReadReceiptDisplayNameA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.ReadReceiptEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.ReadReceiptEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.ReadReceiptAddrtypeA) as string) ?? string.Empty));
					}
					else if (this.properties[TnefPropertyTag.SentRepresentingEmailAddressA] != null)
					{
						this.DntRecipient = new EmailRecipient(new TnefRecipient(this, int.MinValue, (this.GetProperty(TnefPropertyTag.SentRepresentingNameA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SentRepresentingEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SentRepresentingEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SentRepresentingAddrtypeA) as string) ?? string.Empty));
					}
				}
				return this.DntRecipient;
			}
			set
			{
				this.ThrowIfDisposed();
				this.DntRecipient = value;
				this.SetProperty(TnefPropertyTag.ReadReceiptRequested, value != null);
				if (value == null)
				{
					this.SetProperty(TnefPropertyTag.ReadReceiptDisplayNameA, null);
					this.SetProperty(TnefPropertyTag.ReadReceiptEmailAddressA, null);
					this.SetProperty(TnefPropertyTag.ReadReceiptAddrtypeA, null);
				}
				else
				{
					this.SetProperty(TnefPropertyTag.ReadReceiptDisplayNameA, value.DisplayName);
					this.SetProperty(TnefPropertyTag.ReadReceiptEmailAddressA, value.NativeAddress);
					this.SetProperty(TnefPropertyTag.ReadReceiptAddrtypeA, value.NativeAddressType);
				}
				this.SetProperty(TnefPropertyTag.ReadReceiptEntryId, null);
			}
		}

		public override EmailRecipient Sender
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.SenderRecipient == null)
				{
					if (this.properties[TnefPropertyTag.SenderEmailAddressA] != null)
					{
						this.SenderRecipient = new EmailRecipient(new TnefRecipient(this, int.MinValue, (this.GetProperty(TnefPropertyTag.SenderNameA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SenderEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SenderEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SenderAddrtypeA) as string) ?? string.Empty));
					}
					else if (this.properties[TnefPropertyTag.SentRepresentingEmailAddressA] != null)
					{
						this.SenderRecipient = new EmailRecipient(new TnefRecipient(this, int.MinValue, (this.GetProperty(TnefPropertyTag.SentRepresentingNameA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SentRepresentingEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SentRepresentingEmailAddressA) as string) ?? string.Empty, (this.GetProperty(TnefPropertyTag.SentRepresentingAddrtypeA) as string) ?? string.Empty));
					}
				}
				return this.SenderRecipient;
			}
			set
			{
				this.ThrowIfDisposed();
				this.SenderRecipient = value;
				if (value == null)
				{
					this.SetProperty(TnefPropertyTag.SenderNameA, null);
					this.SetProperty(TnefPropertyTag.SenderEmailAddressA, null);
					this.SetProperty(TnefPropertyTag.SenderAddrtypeA, null);
				}
				else
				{
					this.SetProperty(TnefPropertyTag.SenderNameA, value.DisplayName);
					this.SetProperty(TnefPropertyTag.SenderEmailAddressA, value.SmtpAddress);
					this.SetProperty(TnefPropertyTag.SenderAddrtypeA, value.NativeAddressType);
				}
				this.SetProperty(TnefPropertyTag.SenderEntryId, null);
			}
		}

		public override DateTime Date
		{
			get
			{
				DateTime? property = this.GetProperty<DateTime>(TnefPropertyTag.ClientSubmitTime);
				if (property == null)
				{
					return DateTime.MinValue;
				}
				return property.Value;
			}
			set
			{
				this.SetProperty(TnefPropertyTag.ClientSubmitTime, (value != DateTime.MinValue) ? value : null);
			}
		}

		public override DateTime Expires
		{
			get
			{
				DateTime? property = this.GetProperty<DateTime>(TnefPropertyTag.ExpiryTime);
				if (property == null)
				{
					return DateTime.MinValue;
				}
				return property.Value;
			}
			set
			{
				this.SetProperty(TnefPropertyTag.ExpiryTime, (value != DateTime.MinValue) ? value : null);
			}
		}

		public override DateTime ReplyBy
		{
			get
			{
				DateTime? property = this.GetProperty<DateTime>(TnefPropertyTag.ReplyTime);
				if (property == null)
				{
					return DateTime.MinValue;
				}
				return property.Value;
			}
			set
			{
				this.SetProperty(TnefPropertyTag.ReplyTime, (value != DateTime.MinValue) ? value : null);
			}
		}

		public override string Subject
		{
			get
			{
				string text = this.GetProperty(TnefPropertyTag.SubjectA) as string;
				string text2 = this.GetProperty(TnefPropertyTag.SubjectPrefixA) as string;
				string text3 = this.GetProperty(TnefPropertyTag.NormalizedSubjectA) as string;
				if (string.IsNullOrEmpty(text))
				{
					if (!string.IsNullOrEmpty(text2))
					{
						text = text2;
					}
					if (!string.IsNullOrEmpty(text3))
					{
						text = (string.IsNullOrEmpty(text) ? text3 : (text + text3));
					}
					if (string.IsNullOrEmpty(text))
					{
						text = string.Empty;
					}
				}
				else
				{
					if (string.IsNullOrEmpty(text2))
					{
						text2 = string.Empty;
					}
					if (string.IsNullOrEmpty(text3))
					{
						text3 = string.Empty;
					}
					int num = text2.Length + text3.Length;
					if (num > text.Length || (text2.Length > 0 && text3.Length > 0 && num < text.Length) || !text.StartsWith(text2) || !text.EndsWith(text3))
					{
						this.SetProperty(TnefPropertyTag.SubjectPrefixA, null);
						this.SetProperty(TnefPropertyTag.NormalizedSubjectA, null);
					}
				}
				return text;
			}
			set
			{
				this.SetProperty(TnefPropertyTag.SubjectA, value);
				this.SetProperty(TnefPropertyTag.SubjectPrefixA, null);
				this.SetProperty(TnefPropertyTag.NormalizedSubjectA, null);
				this.SetProperty(TnefPropertyTag.ConversationTopicA, null);
			}
		}

		public override string MessageId
		{
			get
			{
				string text = this.GetProperty(TnefPropertyTag.InternetMessageIdA) as string;
				if (string.IsNullOrEmpty(text))
				{
					text = string.Empty;
				}
				return text;
			}
			set
			{
				this.SetProperty(TnefPropertyTag.InternetMessageIdA, value);
			}
		}

		public override Importance Importance
		{
			get
			{
				Importance result;
				this.TryGetImportance(out result);
				return result;
			}
			set
			{
				this.ThrowIfDisposed();
				int num = 1;
				if (Importance.Low == value)
				{
					num = 0;
				}
				else if (Importance.High == value)
				{
					num = 2;
				}
				else if (value != Importance.Normal)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SetProperty(TnefPropertyTag.Importance, (num != 1) ? num : null);
			}
		}

		public override Priority Priority
		{
			get
			{
				Priority result;
				this.TryGetPriority(out result);
				return result;
			}
			set
			{
				this.ThrowIfDisposed();
				int num = 0;
				if (Priority.NonUrgent == value)
				{
					num = -1;
				}
				else if (Priority.Urgent == value)
				{
					num = 1;
				}
				else if (value != Priority.Normal)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SetProperty(TnefPropertyTag.Priority, (num != 0) ? num : null);
			}
		}

		public override Sensitivity Sensitivity
		{
			get
			{
				Sensitivity result;
				this.TryGetSensitivity(out result);
				return result;
			}
			set
			{
				this.ThrowIfDisposed();
				if (Sensitivity.CompanyConfidential < value || value < Sensitivity.None)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SetProperty(TnefPropertyTag.Sensitivity, (value != Sensitivity.None) ? ((int)value) : null);
			}
		}

		public override string MapiMessageClass
		{
			get
			{
				this.ThrowIfDisposed();
				return (this.GetProperty(TnefPropertyTag.MessageClassA) as string) ?? "IPM.Note";
			}
		}

		public override MimeDocument MimeDocument
		{
			get
			{
				this.ThrowIfDisposed();
				return null;
			}
		}

		public override MimePart RootPart
		{
			get
			{
				this.ThrowIfDisposed();
				return null;
			}
		}

		public override MimePart TnefPart
		{
			get
			{
				this.ThrowIfDisposed();
				return null;
			}
		}

		public override bool IsInterpersonalMessage
		{
			get
			{
				return this.MatchMessageClass(PureTnefMessage.InterpersonalMessageClassPrefixes, PureTnefMessage.InterpersonalMessageClassNames) && !this.IsRightsProtectedMessage;
			}
		}

		public override bool IsSystemMessage
		{
			get
			{
				return this.MatchMessageClass(PureTnefMessage.SystemMessageClassPrefixes, PureTnefMessage.SystemMessageClassNames) || this.IsPublicFolderReplicationMessage || this.IsLinkMonitorMessage;
			}
		}

		public override bool IsPublicFolderReplicationMessage
		{
			get
			{
				string mapiMessageClass = this.MapiMessageClass;
				if (mapiMessageClass.Equals("IPM.Replication", StringComparison.OrdinalIgnoreCase))
				{
					string text = this.GetProperty(TnefPropertyTag.ContentIdentifierW) as string;
					if (!string.IsNullOrEmpty(text) && text.Equals("ExSysMessage", StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				else if (mapiMessageClass.Equals("IPM.Conflict.Folder", StringComparison.OrdinalIgnoreCase) || mapiMessageClass.Equals("IPM.Conflict.Message", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				return false;
			}
		}

		private bool IsRightsProtectedMessage
		{
			get
			{
				if (1 != this.tnefAttachments.Count)
				{
					return false;
				}
				string text = this.properties[TnefPropertyBag.TnefNameIdContentClass] as string;
				if (string.IsNullOrEmpty(text))
				{
					return false;
				}
				if (!text.Equals("rpmsg.message", StringComparison.OrdinalIgnoreCase) && !text.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase) && !text.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				TnefAttachmentData dataAtPublicIndex = this.tnefAttachments.GetDataAtPublicIndex(0);
				string text2;
				return Utility.TrySanitizeAttachmentFileName(Utility.GetRawFileName(dataAtPublicIndex.Properties, this.stnef), out text2) && text2.Equals("message.rpmsg", StringComparison.Ordinal);
			}
		}

		public override bool IsOpaqueMessage
		{
			get
			{
				MessageSecurityType messageSecurityType = this.MessageSecurityType;
				return messageSecurityType == MessageSecurityType.Encrypted || messageSecurityType == MessageSecurityType.OpaqueSigned;
			}
		}

		public override MessageSecurityType MessageSecurityType
		{
			get
			{
				string mapiMessageClass = this.MapiMessageClass;
				if (mapiMessageClass.Equals("IPM.Note.SMIME", StringComparison.OrdinalIgnoreCase))
				{
					string text = this.properties[TnefPropertyBag.TnefNameIdContentType] as string;
					if (string.IsNullOrEmpty(text))
					{
						return MessageSecurityType.Encrypted;
					}
					if (text.StartsWith("application/octet-stream", StringComparison.OrdinalIgnoreCase))
					{
						return MessageSecurityType.OpaqueSigned;
					}
					int num = text.IndexOf("smime-type", StringComparison.OrdinalIgnoreCase);
					if (-1 == num)
					{
						return MessageSecurityType.Encrypted;
					}
					if (text.Length < num + 1 || text[num] != '=')
					{
						return MessageSecurityType.Encrypted;
					}
					if (-1 != text.IndexOf("signed-data", num, StringComparison.OrdinalIgnoreCase))
					{
						return MessageSecurityType.OpaqueSigned;
					}
					return MessageSecurityType.Encrypted;
				}
				else
				{
					if (mapiMessageClass.Equals("IPM.Note.SMIME.MultipartSigned", StringComparison.OrdinalIgnoreCase))
					{
						return MessageSecurityType.ClearSigned;
					}
					if (mapiMessageClass.Equals("IPM.Note.Secure.Sign", StringComparison.OrdinalIgnoreCase))
					{
						return MessageSecurityType.ClearSigned;
					}
					if (mapiMessageClass.Equals("IPM.Note.Secure", StringComparison.OrdinalIgnoreCase))
					{
						return MessageSecurityType.Encrypted;
					}
					if (this.IsRightsProtectedMessage)
					{
						return MessageSecurityType.Encrypted;
					}
					return MessageSecurityType.None;
				}
			}
		}

		internal override void Dispose(bool disposing)
		{
			if (disposing && this.tnefStorage != null)
			{
				foreach (TnefAttachmentData tnefAttachmentData in this.tnefAttachments.InternalList)
				{
					if (tnefAttachmentData != null)
					{
						this.DisposeAttachment(tnefAttachmentData);
					}
				}
				this.bodyData.Dispose();
				this.tnefStorage.Release();
				this.tnefStorage = null;
				this.properties.Dispose();
			}
			base.Dispose(disposing);
		}

		public override void Normalize(bool allowUTF8 = false)
		{
			this.Normalize(NormalizeOptions.NormalizeTnef, allowUTF8);
		}

		internal override void Normalize(NormalizeOptions normalizeOptions, bool allowUTF8)
		{
			if (this.topMessage == null)
			{
				return;
			}
			if ((normalizeOptions & NormalizeOptions.DropTnefRecipientTable) != (NormalizeOptions)0 && !this.dropRecipientTable)
			{
				this.topMessage.InvalidateTnefContent();
				this.dropRecipientTable = true;
			}
			if ((normalizeOptions & NormalizeOptions.DropTnefSenderProperties) != (NormalizeOptions)0)
			{
				for (int i = 0; i < PureTnefMessage.SenderProperties.Length; i++)
				{
					this.SetProperty(PureTnefMessage.SenderProperties[i], null);
				}
			}
		}

		internal override void Synchronize()
		{
		}

		internal override int Version
		{
			get
			{
				return 1;
			}
		}

		internal override EmailRecipientCollection BccFromOrgHeader
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.BccFromOrgHeaderRecipients == null)
				{
					this.BccFromOrgHeaderRecipients = new EmailRecipientCollection(this, RecipientType.Bcc, true);
				}
				return this.BccFromOrgHeaderRecipients;
			}
		}

		internal override void SetReadOnly(bool makeReadOnly)
		{
		}

		internal bool Stnef
		{
			get
			{
				return this.stnef;
			}
			set
			{
				this.stnef = value;
			}
		}

		internal string Correlator
		{
			get
			{
				byte[] array = this.GetProperty(TnefPropertyTag.TnefCorrelationKey) as byte[];
				if (array == null)
				{
					return null;
				}
				int num = array.Length;
				if (num == 0)
				{
					return string.Empty;
				}
				return ByteString.BytesToString(array, 0, num - 1, false);
			}
		}

		internal Charset TextCharset
		{
			get
			{
				return this.textCharset;
			}
		}

		internal Charset BinaryCharset
		{
			get
			{
				return this.binaryCharset;
			}
		}

		internal EmailRecipientCollection GetRecipientCollection(RecipientType recipientType)
		{
			if (recipientType == RecipientType.To)
			{
				return this.ToRecipients;
			}
			if (RecipientType.Cc == recipientType)
			{
				return this.CcRecipients;
			}
			return null;
		}

		internal override void AddRecipient(RecipientType recipientType, ref object cachedHeader, EmailRecipient newRecipient)
		{
			this.ThrowIfDisposed();
			newRecipient.TnefRecipient.TnefMessage = this;
			this.SetDirty();
		}

		internal override void RemoveRecipient(RecipientType recipientType, ref object cachedHeader, EmailRecipient oldRecipient)
		{
			this.ThrowIfDisposed();
			oldRecipient.TnefRecipient.TnefMessage = null;
			oldRecipient.TnefRecipient.OriginalIndex = int.MinValue;
			this.SetDirty();
		}

		internal override void ClearRecipients(RecipientType recipientType, ref object cachedHeader)
		{
			this.ThrowIfDisposed();
			foreach (EmailRecipient emailRecipient in this.GetRecipientCollection(recipientType))
			{
				emailRecipient.TnefRecipient.TnefMessage = null;
				emailRecipient.TnefRecipient.OriginalIndex = int.MinValue;
			}
			this.SetDirty();
		}

		internal override IBody GetBody()
		{
			return this;
		}

		BodyFormat IBody.GetBodyFormat()
		{
			return this.bodyData.BodyFormat;
		}

		string IBody.GetCharsetName()
		{
			return this.bodyData.CharsetName;
		}

		MimePart IBody.GetMimePart()
		{
			return null;
		}

		Stream IBody.GetContentReadStream()
		{
			Stream readStream = this.bodyData.GetReadStream();
			return this.bodyData.ConvertReadStreamFormat(readStream);
		}

		bool IBody.TryGetContentReadStream(out Stream stream)
		{
			stream = this.bodyData.GetReadStream();
			stream = this.bodyData.ConvertReadStreamFormat(stream);
			return true;
		}

		Stream IBody.GetContentWriteStream(Charset charset)
		{
			this.bodyData.ReleaseStorage();
			if (this.topMessage != null && !this.stnef)
			{
				MimePart legacyPlainTextBody = this.topMessage.GetLegacyPlainTextBody();
				if (legacyPlainTextBody != null)
				{
					legacyPlainTextBody.SetStorage(null, 0L, 0L);
					if (charset != null && charset != this.bodyData.Charset)
					{
						Charset charset2 = Utility.TranslateWriteStreamCharset(charset);
						if (charset2 != this.binaryCharset)
						{
							this.binaryCharset = charset2;
							this.forceUnicode = true;
							this.SetProperty(TnefPropertyTag.InternetCPID, this.binaryCharset.CodePage);
						}
					}
					this.topMessage.PureMimeMessage.SetPartCharset(legacyPlainTextBody, this.binaryCharset.Name);
					ContentTransferEncoding contentTransferEncoding = legacyPlainTextBody.ContentTransferEncoding;
					if (contentTransferEncoding == ContentTransferEncoding.Unknown || ContentTransferEncoding.SevenBit == contentTransferEncoding || ContentTransferEncoding.EightBit == contentTransferEncoding)
					{
						legacyPlainTextBody.UpdateTransferEncoding(ContentTransferEncoding.QuotedPrintable);
					}
				}
			}
			if (this.bodyPropertyTag.Id == TnefPropertyId.RtfCompressed)
			{
				this.properties[TnefPropertyId.Body] = null;
				if (this.bodyData.BodyFormat == BodyFormat.Html)
				{
					this.properties[TnefPropertyId.RtfCompressed] = null;
					this.bodyPropertyTag = TnefPropertyTag.BodyHtmlB;
					this.properties[this.bodyPropertyTag] = new StoragePropertyValue(this.bodyPropertyTag, null, 0L, 0L);
					this.bodyData.SetFormat(BodyFormat.Html, InternalBodyFormat.Html, this.binaryCharset);
				}
				if (this.bodyData.BodyFormat == BodyFormat.Text && charset != null)
				{
					this.binaryCharset = charset;
					this.SetProperty(TnefPropertyTag.InternetCPID, this.binaryCharset.CodePage);
				}
			}
			else if (this.bodyPropertyTag.Id == TnefPropertyId.BodyHtml)
			{
				this.properties[TnefPropertyId.RtfCompressed] = null;
				this.properties[TnefPropertyId.Body] = null;
				StoragePropertyValue storagePropertyValue = this.properties[TnefPropertyId.BodyHtml] as StoragePropertyValue;
				if (storagePropertyValue == null)
				{
					storagePropertyValue = (this.properties[TnefPropertyTag.BodyHtmlB] as StoragePropertyValue);
				}
				storagePropertyValue.SetStorage(null, 0L, 0L);
				storagePropertyValue.SetBinaryPropertyTag();
				this.bodyPropertyTag = storagePropertyValue.PropertyTag;
				this.bodyData.SetFormat(BodyFormat.Html, InternalBodyFormat.Html, this.binaryCharset);
			}
			else
			{
				StoragePropertyValue storagePropertyValue = this.properties[TnefPropertyId.Body] as StoragePropertyValue;
				storagePropertyValue.SetStorage(null, 0L, 0L);
				if (charset != null && charset != this.bodyData.Charset && this.bodyData.Charset != Charset.Unicode)
				{
					storagePropertyValue.SetUnicodePropertyTag();
					this.bodyPropertyTag = storagePropertyValue.PropertyTag;
					this.bodyData.SetFormat(BodyFormat.Text, InternalBodyFormat.Text, Charset.Unicode);
					this.binaryCharset = Charset.UTF8;
					this.SetProperty(TnefPropertyTag.InternetCPID, this.binaryCharset.CodePage);
				}
			}
			Stream stream = new BodyContentWriteStream(this);
			return this.bodyData.ConvertWriteStreamFormat(stream, charset);
		}

		void IBody.SetNewContent(DataStorage storage, long start, long end)
		{
			this.bodyData.SetStorage(storage, start, end);
			this.BodyModified();
		}

		bool IBody.ConversionNeeded(int[] validCodepages)
		{
			bool result = false;
			object property = this.GetProperty(TnefPropertyTag.InternetCPID);
			if (property != null && property is int)
			{
				result = true;
				foreach (int num in validCodepages)
				{
					if (num == (int)property)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		private void PickBestBody()
		{
			StoragePropertyValue storagePropertyValue = this.properties[TnefPropertyId.BodyHtml] as StoragePropertyValue;
			if (storagePropertyValue != null)
			{
				Charset charset = this.PickCharsetBasedOnPropertyTag(storagePropertyValue.PropertyTag);
				this.SetBody(storagePropertyValue.PropertyTag, charset, storagePropertyValue.Storage, storagePropertyValue.Start, storagePropertyValue.End);
				return;
			}
			StoragePropertyValue storagePropertyValue2 = this.properties[TnefPropertyId.RtfCompressed] as StoragePropertyValue;
			if (storagePropertyValue2 != null)
			{
				this.SetBody(storagePropertyValue2.PropertyTag, this.binaryCharset, storagePropertyValue2.Storage, storagePropertyValue2.Start, storagePropertyValue2.End);
				return;
			}
			StoragePropertyValue storagePropertyValue3 = this.properties[TnefPropertyId.Body] as StoragePropertyValue;
			if (storagePropertyValue3 != null)
			{
				Charset charset2 = this.PickCharsetBasedOnPropertyTag(storagePropertyValue3.PropertyTag);
				this.SetBody(storagePropertyValue3.PropertyTag, charset2, storagePropertyValue3.Storage, storagePropertyValue3.Start, storagePropertyValue3.End);
				return;
			}
			this.bodyData.SetFormat(BodyFormat.None, InternalBodyFormat.None, null);
			this.bodyData.SetStorage(null, 0L, 0L);
		}

		private void SetBody(TnefPropertyTag tag, Charset charset, DataStorage storage, long start, long end)
		{
			this.bodyData.SetStorage(storage, start, end);
			this.bodyPropertyTag = tag;
			BodyFormat format;
			InternalBodyFormat internalFormat;
			this.GetFormat(out format, out internalFormat, ref charset);
			this.bodyData.SetFormat(format, internalFormat, charset);
		}

		private Charset PickCharsetBasedOnPropertyTag(TnefPropertyTag tag)
		{
			if (tag.TnefType == TnefPropertyType.Binary)
			{
				return this.binaryCharset;
			}
			if (tag.TnefType != TnefPropertyType.String8)
			{
				return Charset.Unicode;
			}
			return this.textCharset;
		}

		private void GetFormat(out BodyFormat bodyFormat, out InternalBodyFormat internalBodyFormat, ref Charset charset)
		{
			TnefPropertyId id = this.bodyPropertyTag.Id;
			if (id == TnefPropertyId.Body)
			{
				bodyFormat = BodyFormat.Text;
				internalBodyFormat = InternalBodyFormat.Text;
				return;
			}
			if (id != TnefPropertyId.RtfCompressed)
			{
				if (id != TnefPropertyId.BodyHtml)
				{
					bodyFormat = BodyFormat.None;
					internalBodyFormat = InternalBodyFormat.None;
					return;
				}
				bodyFormat = BodyFormat.Html;
				internalBodyFormat = InternalBodyFormat.Html;
				return;
			}
			else
			{
				bodyFormat = BodyFormat.Rtf;
				internalBodyFormat = InternalBodyFormat.RtfCompressed;
				Stream readStream = this.bodyData.GetReadStream();
				Stream inputRtfStream = new ConverterStream(readStream, new RtfCompressedToRtf(), ConverterStreamAccess.Read);
				this.bodyRtfPreviewStream = new RtfPreviewStream(inputRtfStream, 4096);
				switch (this.bodyRtfPreviewStream.Encapsulation)
				{
				case RtfEncapsulation.None:
					this.bodyRtfPreviewStream.Dispose();
					this.bodyRtfPreviewStream = null;
					return;
				case RtfEncapsulation.Text:
					bodyFormat = BodyFormat.Text;
					charset = Charset.Unicode;
					return;
				case RtfEncapsulation.Html:
					bodyFormat = BodyFormat.Html;
					return;
				default:
					return;
				}
			}
		}

		internal void BodyModified()
		{
			StoragePropertyValue storagePropertyValue;
			DataStorage dataStorage;
			long num;
			long num2;
			if (this.bodyPropertyTag.Id == TnefPropertyId.RtfCompressed)
			{
				storagePropertyValue = (this.properties[TnefPropertyId.RtfCompressed] as StoragePropertyValue);
				this.bodyData.GetStorage(InternalBodyFormat.RtfCompressed, this.binaryCharset, out dataStorage, out num, out num2);
			}
			else if (this.bodyPropertyTag.Id == TnefPropertyId.BodyHtml)
			{
				storagePropertyValue = (this.properties[TnefPropertyId.BodyHtml] as StoragePropertyValue);
				if (storagePropertyValue == null)
				{
					storagePropertyValue = (this.properties[TnefPropertyTag.BodyHtmlB] as StoragePropertyValue);
				}
				this.bodyData.GetStorage(InternalBodyFormat.Html, this.binaryCharset, out dataStorage, out num, out num2);
			}
			else
			{
				storagePropertyValue = (this.properties[TnefPropertyId.Body] as StoragePropertyValue);
				Charset charset = this.PickCharsetBasedOnPropertyTag(storagePropertyValue.PropertyTag);
				this.bodyData.GetStorage(InternalBodyFormat.Text, charset, out dataStorage, out num, out num2);
			}
			storagePropertyValue.SetStorage(dataStorage, num, num2);
			dataStorage.Release();
			this.properties.Touch(this.bodyPropertyTag.Id);
			this.SetDirty();
			if (this.topMessage != null && !this.stnef)
			{
				MimePart legacyPlainTextBody = this.topMessage.GetLegacyPlainTextBody();
				if (legacyPlainTextBody != null)
				{
					this.bodyData.GetStorage(InternalBodyFormat.Text, this.binaryCharset, out dataStorage, out num, out num2);
					legacyPlainTextBody.SetStorage(dataStorage, num, num2);
					dataStorage.Release();
				}
			}
		}

		internal override AttachmentCookie AttachmentCollection_AddAttachment(Attachment attachment)
		{
			this.ThrowIfDisposed();
			if (this.IsOpaqueMessage)
			{
				throw new InvalidOperationException(EmailMessageStrings.CannotAddAttachment);
			}
			this.SetDirty();
			TnefAttachmentData tnefAttachmentData = new TnefAttachmentData(int.MinValue, this);
			tnefAttachmentData.Attachment = attachment;
			int index = this.tnefAttachments.Add(tnefAttachmentData);
			AttachmentCookie result = new AttachmentCookie(index, this);
			return result;
		}

		internal override bool AttachmentCollection_RemoveAttachment(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			TnefAttachmentData dataAtPrivateIndex = this.tnefAttachments.GetDataAtPrivateIndex(cookie.Index);
			bool flag = this.tnefAttachments.RemoveAtPrivateIndex(cookie.Index);
			if (flag)
			{
				dataAtPrivateIndex.Invalidate();
				this.SetDirty();
			}
			return flag;
		}

		internal override void AttachmentCollection_ClearAttachments()
		{
			this.ThrowIfDisposed();
			foreach (TnefAttachmentData tnefAttachmentData in this.tnefAttachments.InternalList)
			{
				if (tnefAttachmentData != null)
				{
					tnefAttachmentData.Invalidate();
				}
			}
			this.tnefAttachments.Clear();
			this.SetDirty();
		}

		internal override int AttachmentCollection_Count()
		{
			return this.tnefAttachments.Count;
		}

		internal override object AttachmentCollection_Indexer(int publicIndex)
		{
			TnefAttachmentData dataAtPublicIndex = this.tnefAttachments.GetDataAtPublicIndex(publicIndex);
			return dataAtPublicIndex.Attachment;
		}

		internal override AttachmentCookie AttachmentCollection_CacheAttachment(int publicIndex, object attachment)
		{
			TnefAttachmentData dataAtPublicIndex = this.tnefAttachments.GetDataAtPublicIndex(publicIndex);
			dataAtPublicIndex.Attachment = attachment;
			int privateIndex = this.tnefAttachments.GetPrivateIndex(publicIndex);
			AttachmentCookie result = new AttachmentCookie(privateIndex, this);
			return result;
		}

		private TnefAttachmentData DataFromCookie(AttachmentCookie cookie)
		{
			TnefAttachmentData dataAtPrivateIndex = this.tnefAttachments.GetDataAtPrivateIndex(cookie.Index);
			if (dataAtPrivateIndex == null)
			{
				throw new InvalidOperationException(EmailMessageStrings.AttachmentRemovedFromMessage);
			}
			return dataAtPrivateIndex;
		}

		internal override MimePart Attachment_GetMimePart(AttachmentCookie cookie)
		{
			return null;
		}

		internal override string Attachment_GetContentType(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			return this.GetAttachmentContentType(tnefAttachmentData.Properties);
		}

		internal override void Attachment_SetContentType(AttachmentCookie cookie, string contentType)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			this.SetAttachmentContentType(tnefAttachmentData.Properties, contentType);
		}

		internal override AttachmentMethod Attachment_GetAttachmentMethod(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			AttachmentMethod attachmentMethod = Utility.GetAttachmentMethod(tnefAttachmentData.Properties);
			if (AttachmentMethod.EmbeddedMessage == attachmentMethod && tnefAttachmentData.EmbeddedMessage == null)
			{
				attachmentMethod = AttachmentMethod.AttachByValue;
			}
			return attachmentMethod;
		}

		internal override InternalAttachmentType Attachment_GetAttachmentType(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			return tnefAttachmentData.InternalAttachmentType;
		}

		internal override void Attachment_SetAttachmentType(AttachmentCookie cookie, InternalAttachmentType attachmentType)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			tnefAttachmentData.InternalAttachmentType = attachmentType;
		}

		internal override EmailMessage Attachment_GetEmbeddedMessage(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			return tnefAttachmentData.EmbeddedMessage;
		}

		internal override void Attachment_SetEmbeddedMessage(AttachmentCookie cookie, EmailMessage value)
		{
			throw new InvalidOperationException(EmailMessageStrings.CannotSetEmbeddedMessageForTnefAttachment);
		}

		internal override string Attachment_GetFileName(AttachmentCookie cookie, ref int attachmentNumber)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			if (tnefAttachmentData.FileName != null)
			{
				return tnefAttachmentData.FileName;
			}
			string rawFileName = Utility.GetRawFileName(tnefAttachmentData.Properties, this.stnef);
			tnefAttachmentData.FileName = Utility.SanitizeOrRegenerateFileName(rawFileName, ref attachmentNumber);
			return tnefAttachmentData.FileName;
		}

		internal override void Attachment_SetFileName(AttachmentCookie cookie, string name)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			if (Utility.GetAttachmentMethod(tnefAttachmentData.Properties) == AttachmentMethod.EmbeddedMessage)
			{
				tnefAttachmentData.Properties.SetProperty(TnefPropertyTag.DisplayNameA, this.stnef, name);
			}
			else
			{
				tnefAttachmentData.Properties.SetProperty(TnefPropertyTag.AttachLongFilenameA, this.stnef, name);
				tnefAttachmentData.Properties.SetProperty(TnefPropertyTag.AttachTransportNameA, this.stnef, name);
				string value = null;
				try
				{
					value = Path.GetExtension(name);
				}
				catch (ArgumentException)
				{
					value = null;
				}
				tnefAttachmentData.Properties.SetProperty(TnefPropertyTag.AttachExtensionA, this.stnef, value);
				tnefAttachmentData.Properties.SetProperty(TnefPropertyTag.AttachPathnameA, this.stnef, null);
				tnefAttachmentData.Properties.SetProperty(TnefPropertyTag.AttachFilenameA, this.stnef, null);
				tnefAttachmentData.Properties.SetProperty(TnefPropertyTag.DisplayNameA, this.stnef, null);
			}
			tnefAttachmentData.FileName = name;
		}

		internal override string Attachment_GetContentDisposition(AttachmentCookie cookie)
		{
			return "attachment";
		}

		internal override void Attachment_SetContentDisposition(AttachmentCookie cookie, string unused)
		{
			throw new NotSupportedException();
		}

		internal override bool Attachment_IsAppleDouble(AttachmentCookie cookie)
		{
			return false;
		}

		internal override Stream Attachment_GetContentReadStream(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			return this.GetContentReadStream(tnefAttachmentData.Properties);
		}

		private Stream GetContentReadStream(TnefPropertyBag properties)
		{
			StoragePropertyValue storagePropertyValue = properties[TnefPropertyId.AttachData] as StoragePropertyValue;
			if (storagePropertyValue == null)
			{
				return DataStorage.NewEmptyReadStream();
			}
			return storagePropertyValue.Storage.OpenReadStream(storagePropertyValue.Start, storagePropertyValue.End);
		}

		internal override bool Attachment_TryGetContentReadStream(AttachmentCookie cookie, out Stream result)
		{
			result = this.Attachment_GetContentReadStream(cookie);
			return true;
		}

		internal override Stream Attachment_GetContentWriteStream(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			tnefAttachmentData.EmbeddedMessage = null;
			this.SetDirty(tnefAttachmentData);
			TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage();
			tnefAttachmentData.Properties.Touch(TnefPropertyId.AttachData);
			StoragePropertyValue storagePropertyValue = tnefAttachmentData.Properties[TnefPropertyId.AttachData] as StoragePropertyValue;
			if (storagePropertyValue == null)
			{
				tnefAttachmentData.Properties[TnefPropertyId.AttachData] = new StoragePropertyValue(TnefPropertyTag.AttachDataBin, temporaryDataStorage, 0L, long.MaxValue);
			}
			else
			{
				storagePropertyValue.SetStorage(temporaryDataStorage, 0L, long.MaxValue);
			}
			return temporaryDataStorage.OpenWriteStream(true);
		}

		internal override int Attachment_GetRenderingPosition(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			int result = -1;
			object obj = tnefAttachmentData.Properties[TnefPropertyId.RenderingPosition];
			if (obj is int)
			{
				result = (int)obj;
			}
			return result;
		}

		internal override string Attachment_GetAttachContentID(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			return tnefAttachmentData.Properties[TnefPropertyId.AttachContentId] as string;
		}

		internal override string Attachment_GetAttachContentLocation(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			return tnefAttachmentData.Properties[TnefPropertyId.AttachContentLocation] as string;
		}

		internal override byte[] Attachment_GetAttachRendering(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			return tnefAttachmentData.Properties[TnefPropertyId.AttachRendering] as byte[];
		}

		internal override int Attachment_GetAttachmentFlags(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			int result = 0;
			object obj = tnefAttachmentData.Properties[TnefPropertyId.AttachmentFlags];
			if (obj is int)
			{
				result = (int)obj;
			}
			return result;
		}

		internal override bool Attachment_GetAttachHidden(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			bool result = false;
			object obj = tnefAttachmentData.Properties[TnefPropertyId.AttachHidden];
			if (obj is bool)
			{
				result = (bool)obj;
			}
			return result;
		}

		internal override int Attachment_GetHashCode(AttachmentCookie cookie)
		{
			TnefAttachmentData tnefAttachmentData = this.DataFromCookie(cookie);
			return tnefAttachmentData.Properties.GetHashCode();
		}

		internal override void Attachment_Dispose(AttachmentCookie cookie)
		{
			TnefAttachmentData data = this.DataFromCookie(cookie);
			this.DisposeAttachment(data);
		}

		private string GetAttachmentContentType(TnefPropertyBag properties)
		{
			string text = properties.GetProperty(TnefPropertyTag.AttachMimeTagA, this.stnef) as string;
			if (string.IsNullOrEmpty(text))
			{
				text = "application/octet-stream";
			}
			return text;
		}

		private void SetAttachmentContentType(TnefPropertyBag properties, string contentType)
		{
			properties.SetProperty(TnefPropertyTag.AttachMimeTagA, this.stnef, contentType);
		}

		private void DisposeAttachment(TnefAttachmentData data)
		{
			if (data.EmbeddedMessage != null)
			{
				data.EmbeddedMessage.Dispose();
				data.EmbeddedMessage = null;
			}
			data.Properties.Dispose();
			data.Invalidate();
		}

		private void SetDirty(TnefAttachmentData data)
		{
			this.SetDirty();
			if (data.EmbeddedMessage != null)
			{
				data.Properties.Touch(TnefPropertyId.AttachData);
			}
		}

		internal override void CopyTo(MessageImplementation destination)
		{
		}

		internal bool TryGetImportance(out Importance importance)
		{
			this.ThrowIfDisposed();
			importance = Importance.Normal;
			object property = this.GetProperty(TnefPropertyTag.Importance);
			if (property == null || !(property is int))
			{
				return false;
			}
			int num = (int)property;
			if (num == 0)
			{
				importance = Importance.Low;
			}
			else if (2 == num)
			{
				importance = Importance.High;
			}
			return true;
		}

		internal bool TryGetPriority(out Priority priority)
		{
			this.ThrowIfDisposed();
			priority = Priority.Normal;
			object property = this.GetProperty(TnefPropertyTag.Priority);
			if (property == null || !(property is int))
			{
				return false;
			}
			int num = (int)property;
			if (-1 == num)
			{
				priority = Priority.NonUrgent;
			}
			else if (1 == num)
			{
				priority = Priority.Urgent;
			}
			return true;
		}

		internal bool TryGetSensitivity(out Sensitivity sensitivity)
		{
			this.ThrowIfDisposed();
			sensitivity = Sensitivity.None;
			object property = this.GetProperty(TnefPropertyTag.Sensitivity);
			if (property == null || !(property is int))
			{
				return false;
			}
			int num = (int)property;
			if (0 < num && num < 4)
			{
				sensitivity = (Sensitivity)num;
			}
			return true;
		}

		internal override IMapiPropertyAccess MapiProperties
		{
			get
			{
				return this;
			}
		}

		public object GetProperty(TnefPropertyTag tag)
		{
			return this.properties.GetProperty(tag, this.stnef);
		}

		public void SetProperty(TnefPropertyTag tag, object value)
		{
			this.properties.SetProperty(tag, this.stnef, value);
		}

		public object GetProperty(TnefNameTag nameTag)
		{
			return this.properties[nameTag];
		}

		public void SetProperty(TnefNameTag nameTag, object value)
		{
			this.properties[nameTag] = value;
		}

		private void ThrowIfDisposed()
		{
			if (this.tnefStorage == null)
			{
				throw new ObjectDisposedException("EmailMessage");
			}
		}

		private TnefAttachmentData FindAttachment(int originalIndex)
		{
			foreach (TnefAttachmentData tnefAttachmentData in this.tnefAttachments.InternalList)
			{
				if (tnefAttachmentData != null && originalIndex == tnefAttachmentData.OriginalIndex)
				{
					return tnefAttachmentData;
				}
			}
			return null;
		}

		private EmailRecipient FindRecipient(int originalIndex)
		{
			foreach (EmailRecipient emailRecipient in this.To)
			{
				if (originalIndex == emailRecipient.TnefRecipient.OriginalIndex)
				{
					return emailRecipient;
				}
			}
			foreach (EmailRecipient emailRecipient2 in this.Cc)
			{
				if (originalIndex == emailRecipient2.TnefRecipient.OriginalIndex)
				{
					return emailRecipient2;
				}
			}
			return null;
		}

		internal void SetDirty()
		{
			if (this.parentAttachmentData != null)
			{
				if (this.parentAttachmentData.MessageImplementation != null)
				{
					PureTnefMessage pureTnefMessage = (PureTnefMessage)this.parentAttachmentData.MessageImplementation;
					pureTnefMessage.SetDirty();
				}
				if (this.parentAttachmentData.EmbeddedMessage != null)
				{
					this.parentAttachmentData.Properties.Touch(TnefPropertyId.AttachData);
					return;
				}
			}
			else if (this.topMessage != null)
			{
				this.topMessage.InvalidateTnefContent();
			}
		}

		internal void SetDirty(TnefRecipient tnefRecipient)
		{
			if (this.FromRecipient != null && tnefRecipient == this.FromRecipient.TnefRecipient)
			{
				this.From = this.FromRecipient;
				return;
			}
			if (this.SenderRecipient != null && tnefRecipient == this.SenderRecipient.TnefRecipient)
			{
				this.Sender = this.SenderRecipient;
				return;
			}
			if (this.DntRecipient != null && tnefRecipient == this.DntRecipient.TnefRecipient)
			{
				this.DispositionNotificationTo = this.DntRecipient;
				return;
			}
			this.SetDirty();
		}

		internal void Attachment_Invalidate(object cookie, bool isEmbeddedMessage)
		{
			TnefPropertyBag tnefPropertyBag = (TnefPropertyBag)cookie;
			this.SetDirty();
			if (isEmbeddedMessage)
			{
				tnefPropertyBag.Touch(TnefPropertyId.AttachData);
			}
		}

		internal T? GetProperty<T>(TnefPropertyTag tag) where T : struct
		{
			this.ThrowIfDisposed();
			object property = this.GetProperty(tag);
			if (property == null || !(property is T))
			{
				return null;
			}
			return new T?((T)((object)property));
		}

		internal void SetProperty<T>(TnefPropertyTag tag, T? propertyValue) where T : struct
		{
			this.ThrowIfDisposed();
			this.SetProperty(tag, (propertyValue != null) ? propertyValue.Value : null);
		}

		private bool MatchMessageClass(IEnumerable<string> prefixes, IEnumerable<string> names)
		{
			string mapiMessageClass = this.MapiMessageClass;
			foreach (string value in prefixes)
			{
				if (mapiMessageClass.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			foreach (string value2 in names)
			{
				if (mapiMessageClass.Equals(value2, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsLinkMonitorMessage
		{
			get
			{
				string text = this.GetProperty(TnefPropertyTag.ContentIdentifierW) as string;
				return !string.IsNullOrEmpty(text) && text.Equals("ExLinkMonitor", StringComparison.OrdinalIgnoreCase);
			}
		}

		private bool MimeCameFromLegacyExchange()
		{
			Header header = this.topMessage.RootPart.Headers.FindFirst("X-MimeOLE");
			if (header == null)
			{
				return false;
			}
			string headerValue = Utility.GetHeaderValue(header);
			return headerValue.StartsWith("Produced By Microsoft Exchange", StringComparison.Ordinal);
		}

		internal bool Load(Stream tnefReadStream)
		{
			this.GetTnefCharsetsFromMime();
			bool result;
			try
			{
				int defaultMessageCodepage = (this.textCharset != null) ? this.textCharset.CodePage : 0;
				using (TnefReader tnefReader = new TnefReader(tnefReadStream, defaultMessageCodepage, TnefComplianceMode.Loose))
				{
					bool flag = this.Load(tnefReader, 0, this.binaryCharset);
					result = flag;
				}
			}
			catch (ByteEncoderException)
			{
				result = false;
			}
			return result;
		}

		internal bool Load(TnefReader reader, int embeddingDepth, Charset binaryCharset)
		{
			if (this.topMessage == null)
			{
				this.textCharset = Charset.GetCharset(reader.MessageCodepage);
				this.binaryCharset = binaryCharset;
			}
			int num = 0;
			bool flag = this.properties.Load(reader, this.tnefStorage, this.tnefStart, this.tnefEnd, TnefAttributeLevel.Message, embeddingDepth, binaryCharset);
			while (flag && TnefAttributeTag.AttachRenderData == reader.AttributeTag)
			{
				TnefAttachmentData tnefAttachmentData = new TnefAttachmentData(num++, this);
				TnefPropertyBag tnefPropertyBag = tnefAttachmentData.Properties;
				flag = tnefPropertyBag.Load(reader, this.tnefStorage, this.tnefStart, this.tnefEnd, TnefAttributeLevel.Attachment, embeddingDepth, binaryCharset);
				this.tnefAttachments.Add(tnefAttachmentData);
				if (flag && TnefAttributeLevel.Message == reader.AttributeLevel)
				{
					flag = this.properties.Load(reader, this.tnefStorage, this.tnefStart, this.tnefEnd, TnefAttributeLevel.Message, embeddingDepth, binaryCharset);
				}
			}
			if (this.textCharset == null)
			{
				this.textCharset = Charset.GetCharset((reader.MessageCodepage == 0) ? 1252 : reader.MessageCodepage);
			}
			object property = this.GetProperty(TnefPropertyTag.InternetCPID);
			Charset charset;
			if (property != null && property is int && Charset.TryGetCharset((int)property, out charset))
			{
				this.binaryCharset = charset;
			}
			this.PickBestBody();
			return EmailMessage.TestabilityEnableBetterFuzzing || TnefComplianceStatus.Compliant == (reader.ComplianceStatus & ~(TnefComplianceStatus.InvalidAttributeChecksum | TnefComplianceStatus.InvalidMessageCodepage | TnefComplianceStatus.InvalidDate));
		}

		internal void LoadRecipients(TnefPropertyReader propertyReader)
		{
			int num = 0;
			while (propertyReader.ReadNextRow())
			{
				EmailRecipientCollection emailRecipientCollection = null;
				string displayName = null;
				string smtpAddress = null;
				string nativeAddress = null;
				string nativeAddressType = null;
				while (propertyReader.ReadNextProperty())
				{
					TnefPropertyTag propertyTag = propertyReader.PropertyTag;
					if (!propertyTag.IsMultiValued && !propertyReader.IsLargeValue)
					{
						TnefPropertyId id = propertyTag.Id;
						if (TnefPropertyTag.RecipientType == propertyTag)
						{
							if (TnefPropertyType.Long == propertyTag.TnefType || TnefPropertyType.I2 == propertyTag.TnefType)
							{
								int num2 = propertyReader.ReadValueAsInt32();
								emailRecipientCollection = ((1 == num2) ? this.To : ((2 == num2) ? this.Cc : null));
							}
						}
						else if (TnefPropertyId.SmtpAddress == id && (propertyTag.TnefType == TnefPropertyType.String8 || propertyTag.TnefType == TnefPropertyType.Unicode))
						{
							smtpAddress = this.ReadStringValue(propertyReader, propertyTag);
						}
						else if (TnefPropertyId.EmailAddress == id && (propertyTag.TnefType == TnefPropertyType.String8 || propertyTag.TnefType == TnefPropertyType.Unicode))
						{
							nativeAddress = this.ReadStringValue(propertyReader, propertyTag);
						}
						else if (TnefPropertyId.Addrtype == id && (propertyTag.TnefType == TnefPropertyType.String8 || propertyTag.TnefType == TnefPropertyType.Unicode))
						{
							nativeAddressType = this.ReadStringValue(propertyReader, propertyTag);
						}
						else if (TnefPropertyId.DisplayName == id && (propertyTag.TnefType == TnefPropertyType.String8 || propertyTag.TnefType == TnefPropertyType.Unicode))
						{
							displayName = this.ReadStringValue(propertyReader, propertyTag);
						}
					}
				}
				if (emailRecipientCollection != null)
				{
					emailRecipientCollection.InternalAdd(new EmailRecipient(new TnefRecipient(this, num, displayName, smtpAddress, nativeAddress, nativeAddressType)));
				}
				num++;
			}
		}

		private string ReadStringValue(TnefPropertyReader reader, TnefPropertyTag tag)
		{
			if (TnefPropertyType.String8 == tag.TnefType || TnefPropertyType.Unicode == tag.TnefType)
			{
				return reader.ReadValueAsString();
			}
			return string.Empty;
		}

		private void GetTnefCharsetsFromMime()
		{
			this.binaryCharset = this.topMessage.GetMessageCharsetFromMime();
			if (this.MimeCameFromLegacyExchange())
			{
				this.textCharset = this.binaryCharset.Culture.WindowsCharset;
				if (this.textCharset.CodePage == 1200)
				{
					this.textCharset = null;
					return;
				}
			}
			else
			{
				this.textCharset = null;
			}
		}

		public void WriteTo(Stream destination)
		{
			if (this.scratchBuffer == null)
			{
				this.scratchBuffer = new byte[8192];
			}
			int codePage = this.textCharset.CodePage;
			using (TnefReader tnefReader = new TnefReader(this.tnefStorage.OpenReadStream(this.tnefStart, this.tnefEnd), codePage, TnefComplianceMode.Loose))
			{
				using (TnefWriter tnefWriter = new TnefWriter(new SuppressCloseStream(destination), tnefReader.AttachmentKey, tnefReader.MessageCodepage, TnefWriterFlags.NoStandardAttributes))
				{
					this.WriteMessage(tnefReader, tnefWriter, this.scratchBuffer);
				}
			}
		}

		internal bool WriteMessage(TnefReader reader, TnefWriter writer, byte[] scratchBuffer)
		{
			bool flag = this.WriteMessageProperties(reader, writer, scratchBuffer);
			int num = 0;
			while (flag && TnefAttributeTag.AttachRenderData == reader.AttributeTag && TnefAttributeLevel.Attachment == reader.AttributeLevel)
			{
				TnefAttachmentData attachmentData = this.FindAttachment(num++);
				flag = this.WriteAttachmentProperties(reader, writer, attachmentData, scratchBuffer);
				if (flag && TnefAttributeLevel.Message == reader.AttributeLevel)
				{
					while (flag && TnefAttributeTag.AttachRenderData != reader.AttributeTag)
					{
						flag = this.properties.Write(reader, writer, TnefAttributeLevel.Message, this.dropRecipientTable, this.forceUnicode, scratchBuffer);
					}
				}
			}
			this.WriteNewAttachments(writer, scratchBuffer);
			return flag;
		}

		private bool WriteMessageProperties(TnefReader reader, TnefWriter writer, byte[] scratchBuffer)
		{
			bool flag = reader.ReadNextAttribute();
			if (flag && TnefAttributeTag.AttachRenderData != reader.AttributeTag && TnefAttributeLevel.Message == reader.AttributeLevel)
			{
				flag = this.properties.Write(reader, writer, TnefAttributeLevel.Message, this.dropRecipientTable, this.forceUnicode, scratchBuffer);
			}
			return flag;
		}

		internal void WriteRecipients(TnefPropertyReader propertyReader, TnefWriter writer, ref char[] buffer)
		{
			if (this.To.Count > 0 || this.Cc.Count > 0)
			{
				writer.StartAttribute(TnefAttributeTag.RecipientTable, TnefAttributeLevel.Message);
			}
			int num = 0;
			while (propertyReader.ReadNextRow())
			{
				EmailRecipient emailRecipient = this.FindRecipient(num++);
				if (emailRecipient != null)
				{
					writer.StartRow();
					bool flag = true;
					bool flag2 = true;
					bool flag3 = true;
					bool flag4 = true;
					while (propertyReader.ReadNextProperty())
					{
						TnefPropertyTag tag = propertyReader.PropertyTag;
						TnefPropertyId id = tag.Id;
						if (this.forceUnicode)
						{
							tag = tag.ToUnicode();
						}
						if (TnefPropertyId.SmtpAddress == id && (tag.TnefType == TnefPropertyType.String8 || tag.TnefType == TnefPropertyType.Unicode))
						{
							if (emailRecipient.SmtpAddress != null)
							{
								writer.WriteProperty(tag, emailRecipient.SmtpAddress);
								flag = false;
							}
						}
						else if (TnefPropertyId.EmailAddress == id && (tag.TnefType == TnefPropertyType.String8 || tag.TnefType == TnefPropertyType.Unicode))
						{
							if (emailRecipient.NativeAddress != null)
							{
								writer.WriteProperty(tag, emailRecipient.NativeAddress);
								flag2 = false;
							}
						}
						else if (TnefPropertyId.Addrtype == id && (tag.TnefType == TnefPropertyType.String8 || tag.TnefType == TnefPropertyType.Unicode))
						{
							if (emailRecipient.NativeAddressType != null)
							{
								writer.WriteProperty(tag, emailRecipient.NativeAddressType);
								flag3 = false;
							}
						}
						else if (TnefPropertyId.DisplayName == id && (tag.TnefType == TnefPropertyType.String8 || tag.TnefType == TnefPropertyType.Unicode))
						{
							if (emailRecipient.DisplayName != null)
							{
								writer.WriteProperty(tag, emailRecipient.DisplayName);
								flag4 = false;
							}
						}
						else if (tag.IsTnefTypeValid && tag.ValueTnefType != TnefPropertyType.Object)
						{
							if (this.forceUnicode && propertyReader.PropertyTag.ValueTnefType == TnefPropertyType.String8)
							{
								TnefPropertyBag.WriteUnicodeProperty(writer, propertyReader, tag, ref buffer);
							}
							else
							{
								writer.WriteProperty(propertyReader);
							}
						}
					}
					if (flag && emailRecipient.SmtpAddress != null)
					{
						writer.WriteProperty(TnefPropertyTag.SmtpAddressW, emailRecipient.SmtpAddress);
					}
					if (flag2 && emailRecipient.NativeAddress != null)
					{
						writer.WriteProperty(TnefPropertyTag.EmailAddressW, emailRecipient.NativeAddress);
					}
					if (flag3 && emailRecipient.NativeAddressType != null)
					{
						writer.WriteProperty(TnefPropertyTag.AddrtypeW, emailRecipient.NativeAddressType);
					}
					if (flag4 && emailRecipient.DisplayName != null)
					{
						writer.WriteProperty(TnefPropertyTag.DisplayNameW, emailRecipient.DisplayName);
					}
				}
			}
			this.WriteNewRecipients(writer);
		}

		private void WriteNewRecipients(TnefWriter writer)
		{
			for (int i = 0; i < 2; i++)
			{
				EmailRecipientCollection emailRecipientCollection = (i == 0) ? this.ToRecipients : this.CcRecipients;
				if (emailRecipientCollection != null)
				{
					int value = (i == 0) ? 1 : 2;
					foreach (EmailRecipient emailRecipient in emailRecipientCollection)
					{
						if (-2147483648 == emailRecipient.TnefRecipient.OriginalIndex)
						{
							writer.StartRow();
							writer.WriteProperty(TnefPropertyTag.DisplayNameW, emailRecipient.DisplayName ?? string.Empty);
							writer.WriteProperty(TnefPropertyTag.AddrtypeW, emailRecipient.NativeAddressType ?? string.Empty);
							writer.WriteProperty(TnefPropertyTag.EmailAddressW, emailRecipient.NativeAddress ?? string.Empty);
							writer.WriteProperty(TnefPropertyTag.RecipientType, value);
							writer.WriteProperty(TnefPropertyTag.SmtpAddressW, emailRecipient.SmtpAddress ?? string.Empty);
						}
					}
				}
			}
		}

		private bool WriteAttachmentProperties(TnefReader reader, TnefWriter writer, TnefAttachmentData attachmentData, byte[] scratchBuffer)
		{
			bool result;
			if (attachmentData != null)
			{
				result = attachmentData.Properties.Write(reader, writer, TnefAttributeLevel.Attachment, this.dropRecipientTable, this.forceUnicode, scratchBuffer);
			}
			else
			{
				while ((result = reader.ReadNextAttribute()) && TnefAttributeTag.AttachRenderData != reader.AttributeTag && TnefAttributeLevel.Message != reader.AttributeLevel)
				{
				}
			}
			return result;
		}

		private void WriteNewAttachments(TnefWriter writer, byte[] scratchBuffer)
		{
			DateTime utcNow = DateTime.UtcNow;
			foreach (TnefAttachmentData tnefAttachmentData in this.tnefAttachments.InternalList)
			{
				if (tnefAttachmentData != null && -2147483648 == tnefAttachmentData.OriginalIndex)
				{
					writer.StartAttribute(TnefAttributeTag.AttachRenderData, TnefAttributeLevel.Attachment);
					writer.WriteProperty(TnefPropertyTag.AttachMethod, tnefAttachmentData.AttachmentMethod);
					int value = -1;
					writer.WriteProperty(TnefPropertyTag.RenderingPosition, value);
					string value2 = tnefAttachmentData.FileName ?? string.Empty;
					if (!this.forceUnicode)
					{
						writer.StartAttribute(TnefAttributeTag.AttachTitle, TnefAttributeLevel.Attachment);
						writer.WriteProperty(TnefPropertyTag.AttachFilenameA, value2);
					}
					writer.StartAttribute(TnefAttributeTag.AttachCreateDate, TnefAttributeLevel.Attachment);
					writer.WriteProperty(TnefPropertyTag.CreationTime, utcNow);
					writer.StartAttribute(TnefAttributeTag.AttachModifyDate, TnefAttributeLevel.Attachment);
					writer.WriteProperty(TnefPropertyTag.LastModificationTime, utcNow);
					using (Stream contentReadStream = this.GetContentReadStream(tnefAttachmentData.Properties))
					{
						int num = contentReadStream.Read(scratchBuffer, 0, scratchBuffer.Length);
						if (num > 0)
						{
							writer.StartAttribute(TnefAttributeTag.AttachData, TnefAttributeLevel.Attachment);
							do
							{
								writer.WriteAttributeRawValue(scratchBuffer, 0, num);
								num = contentReadStream.Read(scratchBuffer, 0, scratchBuffer.Length);
							}
							while (num > 0);
						}
					}
					string attachmentContentType = this.GetAttachmentContentType(tnefAttachmentData.Properties);
					writer.StartAttribute(TnefAttributeTag.Attachment, TnefAttributeLevel.Attachment);
					writer.WriteProperty(TnefPropertyTag.CreationTime, utcNow);
					writer.WriteProperty(TnefPropertyTag.LastModificationTime, utcNow);
					writer.WriteProperty(TnefPropertyTag.AttachLongFilenameW, value2);
					writer.WriteProperty(TnefPropertyTag.RenderingPosition, value);
					writer.WriteProperty(TnefPropertyTag.AttachMimeTagW, attachmentContentType);
				}
			}
		}

		internal const TnefComplianceStatus BannedTnefComplianceViolations = ~(TnefComplianceStatus.InvalidAttributeChecksum | TnefComplianceStatus.InvalidMessageCodepage | TnefComplianceStatus.InvalidDate);

		internal EmailRecipientCollection ToRecipients;

		internal EmailRecipientCollection CcRecipients;

		internal EmailRecipientCollection BccRecipients;

		internal EmailRecipientCollection BccFromOrgHeaderRecipients;

		internal EmailRecipientCollection ReplyToRecipients;

		internal EmailRecipient FromRecipient;

		internal EmailRecipient SenderRecipient;

		internal EmailRecipient DntRecipient;

		private static readonly TnefPropertyTag[] SenderProperties = new TnefPropertyTag[]
		{
			TnefPropertyTag.SenderNameA,
			TnefPropertyTag.SenderEmailAddressA,
			TnefPropertyTag.SenderAddrtypeA,
			TnefPropertyTag.SenderEntryId,
			TnefPropertyTag.ReadReceiptRequested,
			TnefPropertyTag.SentRepresentingNameA,
			TnefPropertyTag.SentRepresentingEmailAddressA,
			TnefPropertyTag.SentRepresentingAddrtypeA,
			TnefPropertyTag.SentRepresentingEntryId
		};

		private static readonly string[] systemMessageClassPrefixes = new string[]
		{
			"Report.IPM.Note.",
			"IPM.Document.",
			"IPM.Note.StorageQuotaWarning.",
			"IPM.Mailbeat.Bounce."
		};

		private static readonly string[] systemMessageClassNames = new string[]
		{
			"SrvInfo.Expiry",
			"IPM.Note.StorageQuotaWarning",
			"IPM.Microsoft.Approval.Initiation"
		};

		private static readonly string[] interpersonalMessageClassPrefixes = new string[]
		{
			"IPM.Schedule.Meeting.",
			"IPM.Note.Rules.ExternalOofTemplate.",
			"IPM.Note.Rules.OofTemplate.",
			"IPM.Recall.Report.",
			"IPM.Form.",
			"IPM.Note.Rules.ReplyTemplate.",
			"IPM.Note.Microsoft.Approval."
		};

		private static readonly string[] interpersonalMessageClassNames = new string[]
		{
			"IPM.Outlook.Recall",
			"IPM.Note",
			"IPM.Form",
			"IPM.TaskRequest",
			"IPM.Note.Microsoft.Conversation.Voice",
			"IPM.Note.Microsoft.Missed.Voice",
			"IPM.Note.Microsoft.Voicemail.UM",
			"IPM.Note.Microsoft.Voicemail.UM.CA"
		};

		private PureTnefMessage.PureTnefMessageThreadAccessToken accessToken;

		private AttachmentDataCollection<TnefAttachmentData> tnefAttachments = new AttachmentDataCollection<TnefAttachmentData>();

		private TnefPropertyBag properties;

		private DataStorage tnefStorage;

		private long tnefStart;

		private long tnefEnd;

		private Charset textCharset;

		private Charset binaryCharset;

		private MimeTnefMessage topMessage;

		private TnefAttachmentData parentAttachmentData;

		private byte[] scratchBuffer;

		private bool dropRecipientTable;

		private bool forceUnicode;

		private bool stnef;

		private TnefPropertyTag bodyPropertyTag;

		private RtfPreviewStream bodyRtfPreviewStream;

		private BodyData bodyData = new BodyData();

		private class PureTnefMessageThreadAccessToken : ObjectThreadAccessToken
		{
			internal PureTnefMessageThreadAccessToken(PureTnefMessage parent)
			{
			}
		}
	}
}
