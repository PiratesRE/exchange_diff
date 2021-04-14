using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReportMessage : MessageItem
	{
		internal ReportMessage(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public new static ReportMessage Bind(StoreSession session, StoreId reportMessageId)
		{
			return ReportMessage.Bind(session, reportMessageId, null);
		}

		public new static ReportMessage Bind(StoreSession session, StoreId reportMessageId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<ReportMessage>(session, reportMessageId, ReportMessageSchema.Instance, propsToReturn);
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return ReportMessageSchema.Instance;
			}
		}

		public override bool IsReplyAllowed
		{
			get
			{
				this.CheckDisposed("IsReplyAllowed::get");
				return false;
			}
		}

		public bool IsSendAgainAllowed
		{
			get
			{
				this.CheckDisposed("IsSendAgainAllowed::get");
				if (ObjectClass.IsReport(this.ClassName, "NDR") && base.AttachmentCollection.Count == 1)
				{
					using (Attachment attachment = base.AttachmentCollection.TryOpenFirstAttachment(AttachmentType.EmbeddedMessage))
					{
						return attachment != null;
					}
					return false;
				}
				return false;
			}
		}

		public Participant ReceivedRepresenting
		{
			get
			{
				this.CheckDisposed("ReceivedRepresenting::get");
				return base.GetValueOrDefault<Participant>(InternalSchema.ReceivedRepresenting);
			}
		}

		public MessageItem CreateSendAgain(StoreId parentFolderId)
		{
			this.CheckDisposed("CreateSendAgain");
			if (parentFolderId == null)
			{
				throw new ArgumentNullException("parentFolderId");
			}
			if (!this.IsSendAgainAllowed)
			{
				throw new NotSupportedException("CreateSendAgain() is not supported on this ReportMessage");
			}
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				using (ItemAttachment itemAttachment = (ItemAttachment)base.AttachmentCollection.TryOpenFirstAttachment(AttachmentType.EmbeddedMessage))
				{
					if (itemAttachment == null)
					{
						throw new CorruptDataException(ServerStrings.ExReportMessageCorruptedDueToWrongItemAttachmentType);
					}
					using (Item item = itemAttachment.GetItem())
					{
						if (!(item is MessageItem))
						{
							throw new CorruptDataException(ServerStrings.ExReportMessageCorruptedDueToWrongItemAttachmentType);
						}
						messageItem = (MessageItem)Microsoft.Exchange.Data.Storage.Item.CloneItem(base.Session, parentFolderId, item, false, false, null);
						this.SetResendItem(messageItem, (MessageItem)item);
					}
				}
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		public void GenerateReportBody(Stream outputStream, out Charset charset)
		{
			this.CheckDisposed("GenerateReportBody");
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			CultureInfo cultureInfo;
			ReportMessage.GenerateReportBody(this, outputStream, out cultureInfo, out charset);
		}

		internal static int EstimateGeneratedBodySize(MessageItem message)
		{
			return 3072 + 700 * message.Recipients.Count;
		}

		internal static void GenerateReportBody(MessageItem message, Stream outputStream, out CultureInfo cultureInfo, out Charset charset)
		{
			if (ObjectClass.IsDsn(message.ClassName))
			{
				ReportMessage.GenerateDsnBody(message, outputStream, DsnHumanReadableWriter.DefaultDsnHumanReadableWriter, null, out cultureInfo, out charset);
				return;
			}
			ReportMessage.GenerateOldFashionedReportBody(message, outputStream, out cultureInfo, out charset);
		}

		internal static Stream GenerateReportBody(MessageItem message, DsnHumanReadableWriter dsnWriter, HeaderList headerList, out CultureInfo cultureInfo, out Charset charset)
		{
			MemoryStream memoryStream = new MemoryStream(ReportMessage.EstimateGeneratedBodySize(message));
			if (ObjectClass.IsDsn(message.ClassName))
			{
				ReportMessage.GenerateDsnBody(message, memoryStream, dsnWriter, headerList, out cultureInfo, out charset);
			}
			else
			{
				ReportMessage.GenerateReportBody(message, memoryStream, out cultureInfo, out charset);
			}
			return memoryStream;
		}

		internal static void CoreObjectUpdateSubjectPrefix(CoreItem coreItem)
		{
			if (coreItem.Session != null)
			{
				string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
				LocalizedString localizedString;
				if (DsnMdnUtil.TryGetSubjectPrefix(valueOrDefault, out localizedString))
				{
					CultureInfo formatProvider = coreItem.Session.InternalPreferedCulture;
					if (ObjectClass.IsMdn(valueOrDefault))
					{
						formatProvider = ReportMessage.GetMdnCulture(coreItem);
					}
					coreItem.PropertyBag[InternalSchema.SubjectPrefix] = localizedString.ToString(formatProvider);
				}
			}
		}

		internal static IEnumerable<CultureInfo> GetDsnCultures(MailboxSession mailboxSession, HeaderList headerList, DsnHumanReadableWriter dsnWriter)
		{
			if (mailboxSession != null)
			{
				return mailboxSession.InternalGetMailboxCultures();
			}
			CultureInfo cultureInfo = null;
			if (headerList != null)
			{
				Header acceptLanguageHeader = null;
				Header contentLanguageHeader = headerList.FindFirst(HeaderId.ContentLanguage);
				cultureInfo = dsnWriter.GetDsnCulture(acceptLanguageHeader, contentLanguageHeader, true, null);
			}
			if (cultureInfo != null)
			{
				return new CultureInfo[]
				{
					cultureInfo
				};
			}
			return Array<CultureInfo>.Empty;
		}

		private static void GenerateDsnBody(MessageItem message, Stream outputStream, DsnHumanReadableWriter dsnWriter, HeaderList headerList, out CultureInfo cultureInfo, out Charset charset)
		{
			message.Load(ReportMessageSchema.Instance.AutoloadProperties);
			MailboxSession mailboxSession = message.Session as MailboxSession;
			List<DsnRecipientInfo> list = new List<DsnRecipientInfo>(message.Recipients.Count);
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Recipient recipient in message.Recipients)
			{
				list.Add(new DsnRecipientInfo(recipient.Participant.DisplayName, recipient.Participant.EmailAddress, recipient.Participant.RoutingType, DsnMdnUtil.GetMimeDsnRecipientStatusCode(recipient).Value, recipient.GetValueOrDefault<string>(InternalSchema.SupplementaryInfo)));
				string valueOrDefault = recipient.GetValueOrDefault<string>(InternalSchema.RemoteMta);
				if (!string.IsNullOrEmpty(valueOrDefault))
				{
					hashSet.TryAdd(valueOrDefault);
				}
			}
			StringBuilder stringBuilder = new StringBuilder(hashSet.Count * 20);
			foreach (string value in hashSet)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(", ");
			}
			dsnWriter.CreateDsnHumanReadableBody(outputStream, ReportMessage.GetDsnCultures(mailboxSession, headerList, dsnWriter), message.GetValueOrDefault<string>(InternalSchema.OriginalSubject), list, ReportMessage.GetDsnFlags(message.ClassName), message.GetValueOrDefault<string>(InternalSchema.ReportingMta, string.Empty), (stringBuilder.Length > 0) ? stringBuilder.ToString(0, stringBuilder.Length - ", ".Length) : null, null, headerList, out cultureInfo, out charset);
		}

		private static void GenerateOldFashionedReportBody(MessageItem message, Stream outputStream, out CultureInfo cultureInfo, out Charset charset)
		{
			Encoding unicode = Encoding.Unicode;
			string text = ReportMessage.GenerateOldFashionedReportBody(message, out cultureInfo).ToString(cultureInfo);
			OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
			outboundCodePageDetector.AddText(text);
			charset = Charset.GetCharset(outboundCodePageDetector.GetCodePage(Culture.GetCulture(cultureInfo.Name), false));
			using (MemoryStream memoryStream = new MemoryStream(unicode.GetBytes(text)))
			{
				new TextToHtml
				{
					InputEncoding = unicode,
					OutputEncoding = charset.GetEncoding()
				}.Convert(memoryStream, outputStream);
			}
		}

		private static LocalizedString GenerateOldFashionedReportBody(MessageItem message, out CultureInfo culture)
		{
			message.Load(ReportMessageSchema.Instance.AutoloadProperties);
			ExDateTime? valueAsNullable = message.GetValueAsNullable<ExDateTime>(InternalSchema.ReportTime);
			ExTimeZone exTimeZone = ExTimeZone.CurrentTimeZone;
			StoreSession session = message.Session;
			if (session != null)
			{
				exTimeZone = session.ExTimeZone;
			}
			byte[] valueOrDefault = message.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionStart);
			ExTimeZone exTimeZone2;
			if (valueOrDefault != null && O12TimeZoneFormatter.TryParseTruncatedTimeZoneBlob(valueOrDefault, out exTimeZone2))
			{
				exTimeZone = (TimeZoneHelper.PromoteCustomizedTimeZone(exTimeZone2) ?? exTimeZone2);
			}
			culture = ReportMessage.GetMdnCulture(message.CoreItem);
			LocalizedString originalMessageInfo = ReportMessage.GetOriginalMessageInfo(message, exTimeZone, culture);
			LocalizedString result;
			if (ObjectClass.IsReport(message.ClassName, "IPNRN"))
			{
				if (valueAsNullable != null)
				{
					result = ClientStrings.MdnRead(originalMessageInfo, exTimeZone.ConvertDateTime(valueAsNullable.Value), new LocalizedString(exTimeZone.LocalizableDisplayName.ToString(culture)));
				}
				else
				{
					result = ClientStrings.MdnReadNoTime(originalMessageInfo);
				}
			}
			else
			{
				if (!ObjectClass.IsReport(message.ClassName, "IPNNRN"))
				{
					ExTraceGlobals.StorageTracer.TraceDebug((long)message.GetHashCode(), ServerStrings.UnsupportedReportType(message.ClassName));
					return new LocalizedString(ServerStrings.UnsupportedReportType(message.ClassName));
				}
				if (valueAsNullable != null)
				{
					result = ClientStrings.MdnNotRead(originalMessageInfo, exTimeZone.ConvertDateTime(valueAsNullable.Value), new LocalizedString(exTimeZone.LocalizableDisplayName.ToString(culture)));
				}
				else
				{
					result = ClientStrings.MdnNotReadNoTime(originalMessageInfo);
				}
			}
			return result;
		}

		private static CultureInfo GetMdnCulture(ICoreItem item)
		{
			int num = 0;
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;
			if (item.Session != null)
			{
				cultureInfo = item.Session.InternalPreferedCulture;
			}
			CultureInfo result = cultureInfo;
			object obj = item.PropertyBag.TryGetProperty(ReportMessageSchema.MessageLocaleId);
			if (obj is int)
			{
				num = (int)obj;
			}
			try
			{
				if (num != 0)
				{
					CultureInfo cultureInfo2 = LocaleMap.GetCultureFromLcid(num);
					if (cultureInfo2.IsNeutralCulture)
					{
						cultureInfo2 = new CultureInfo(cultureInfo2.Name);
					}
					result = cultureInfo2;
				}
			}
			catch (ArgumentException)
			{
				result = cultureInfo;
			}
			return result;
		}

		private static LocalizedString GetOriginalMessageInfo(MessageItem message, ExTimeZone timezone, CultureInfo culture)
		{
			string valueOrDefault = message.GetValueOrDefault<string>(ReportMessageSchema.OriginalSubject);
			if (valueOrDefault == null)
			{
				valueOrDefault = message.GetValueOrDefault<string>(ItemSchema.NormalizedSubject, string.Empty);
			}
			string valueOrDefault2 = message.GetValueOrDefault<string>(ItemSchema.SentRepresentingDisplayName, string.Empty);
			ExDateTime valueOrDefault3 = message.GetValueOrDefault<ExDateTime>(ReportMessageSchema.OriginalSentTime, ExDateTime.UtcNow);
			StringBuilder stringBuilder = new StringBuilder(200 + valueOrDefault.Length + valueOrDefault2.Length);
			string value = "\r\n   ";
			string value2 = " ";
			stringBuilder.Append("\r\n");
			stringBuilder.Append(value);
			string value3 = ClientStrings.ToColon.ToString(culture);
			stringBuilder.Append(value3);
			stringBuilder.Append(value2);
			stringBuilder.Append(valueOrDefault2);
			stringBuilder.Append(value);
			value3 = ClientStrings.SubjectColon.ToString(culture);
			stringBuilder.Append(value3);
			stringBuilder.Append(value2);
			stringBuilder.Append(valueOrDefault);
			stringBuilder.Append(value);
			value3 = ClientStrings.SentColon.ToString(culture);
			stringBuilder.Append(value3);
			stringBuilder.Append(value2);
			stringBuilder.Append(timezone.ConvertDateTime(valueOrDefault3).ToString("F", culture));
			stringBuilder.Append(" ");
			stringBuilder.Append(timezone.LocalizableDisplayName.ToString(culture));
			stringBuilder.Append("\r\n\r\n");
			return new LocalizedString(stringBuilder.ToString());
		}

		private static DsnFlags GetDsnFlags(string itemClass)
		{
			foreach (KeyValuePair<string, DsnFlags> keyValuePair in ReportMessage.DsnClassToDsnFlag)
			{
				if (ObjectClass.IsReport(itemClass, keyValuePair.Key))
				{
					return keyValuePair.Value;
				}
			}
			return DsnFlags.None;
		}

		private void SetResendItem(MessageItem resendMessage, MessageItem originalMessage)
		{
			resendMessage[InternalSchema.IsResend] = true;
			resendMessage[InternalSchema.IsDraft] = true;
			foreach (Recipient recipient in resendMessage.Recipients)
			{
				recipient.Submitted = true;
			}
			foreach (Recipient recipient2 in base.Recipients)
			{
				Recipient recipient3 = resendMessage.Recipients.Add(recipient2.Participant);
				recipient3.Submitted = false;
			}
			if (!(resendMessage is MeetingMessage))
			{
				resendMessage.From = null;
			}
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession != null)
			{
				resendMessage.Sender = new Participant(mailboxSession.MailboxOwner);
			}
			string text = Guid.NewGuid().ToString("N");
			if (resendMessage.InternetMessageId.Length > 0 && resendMessage.InternetMessageId[0] == '<')
			{
				resendMessage.InternetMessageId = resendMessage.InternetMessageId.Insert(1, text);
			}
			else
			{
				resendMessage.InternetMessageId += text;
			}
			resendMessage[InternalSchema.ReceivedTime] = ExDateTime.UtcNow;
		}

		private const string RemoteMtaStringDelimiter = ", ";

		internal static KeyValuePair<string, DsnFlags>[] DsnClassToDsnFlag = new KeyValuePair<string, DsnFlags>[]
		{
			Util.Pair<string, DsnFlags>("NDR", DsnFlags.Failure),
			Util.Pair<string, DsnFlags>("Expanded.DR", DsnFlags.Expansion),
			Util.Pair<string, DsnFlags>("Relayed.DR", DsnFlags.Relay),
			Util.Pair<string, DsnFlags>("Delayed.DR", DsnFlags.Delay),
			Util.Pair<string, DsnFlags>("DR", DsnFlags.Delivery)
		};
	}
}
