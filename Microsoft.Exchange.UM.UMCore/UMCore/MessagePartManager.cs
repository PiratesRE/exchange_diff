using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.LAD;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MessagePartManager : ActivityManager
	{
		internal MessagePartManager(ActivityManager manager, MessagePartManager.ConfigClass config) : base(manager, config)
		{
			this.context = (base.MessagePlayerContext = manager.MessagePlayerContext);
		}

		internal OrganizationId OrgId
		{
			get
			{
				return this.user.ADUser.OrganizationId;
			}
		}

		private PlaybackContent ContentType
		{
			get
			{
				if (PlaybackContent.Unknown == this.context.ContentType)
				{
					this.SetPlaybackContentType();
				}
				return this.context.ContentType;
			}
		}

		private LinkedListNode<MessagePartManager.MessagePart> CurrentPart
		{
			get
			{
				LinkedListNode<MessagePartManager.MessagePart> result = null;
				switch (this.context.Mode)
				{
				case PlaybackMode.Audio:
					result = this.context.CurrentWavePart;
					break;
				case PlaybackMode.Text:
					result = this.context.CurrentTextPart;
					break;
				}
				return result;
			}
			set
			{
				switch (this.context.Mode)
				{
				case PlaybackMode.Audio:
					this.context.CurrentWavePart = value;
					return;
				case PlaybackMode.Text:
					this.context.CurrentTextPart = value;
					return;
				default:
					return;
				}
			}
		}

		internal override void PreActionExecute(BaseUMCallSession vo)
		{
			if (this.user != null)
			{
				this.guard = this.user.CreateConnectionGuard();
			}
			base.PreActionExecute(vo);
		}

		internal override void PostActionExecute(BaseUMCallSession vo)
		{
			if (this.item != null)
			{
				this.item.Dispose();
				this.item = null;
			}
			if (this.guard != null)
			{
				this.guard.Dispose();
				this.guard = null;
			}
			base.PostActionExecute(vo);
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			this.user = vo.CurrentCallContext.CallerInfo;
			this.preferredCulture = vo.CurrentCallContext.Culture;
			base.Start(vo, refInfo);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
			if (!u.IsAuthenticated)
			{
				base.CheckAuthorization(u);
			}
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "MessagePartManager::ExecuteAction action={0}.", new object[]
			{
				action
			});
			string input;
			if (string.Equals(action, "nextMessagePart", StringComparison.OrdinalIgnoreCase))
			{
				input = this.NextMessagePart();
			}
			else if (string.Equals(action, "firstMessagePart", StringComparison.OrdinalIgnoreCase))
			{
				input = this.FirstMessagePart();
			}
			else if (string.Equals(action, "nextMessageSection", StringComparison.OrdinalIgnoreCase))
			{
				input = this.NextMessageSection();
			}
			else
			{
				if (string.Equals(action, "selectLanguagePause", StringComparison.OrdinalIgnoreCase))
				{
					base.ExecuteAction("pause", vo);
					return base.ExecuteAction("selectLanguage", vo);
				}
				if (string.Equals(action, "nextLanguagePause", StringComparison.OrdinalIgnoreCase))
				{
					base.ExecuteAction("pause", vo);
					return base.ExecuteAction("nextLanguage", vo);
				}
				return base.ExecuteAction(action, vo);
			}
			return base.CurrentActivity.GetTransition(input);
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "MessagePartManager::Dispose.", new object[0]);
					if (this.item != null)
					{
						this.item.Dispose();
						this.item = null;
					}
					if (this.guard != null)
					{
						this.guard.Dispose();
						this.guard = null;
					}
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MessagePartManager>(this);
		}

		private Item GetMessage(MailboxSession session)
		{
			if (this.item == null)
			{
				StoreObjectId id = this.context.Id;
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Getting item={0} from the store.", new object[]
				{
					id.ToBase64String()
				});
				this.item = Item.Bind(session, id);
				if (XsoUtil.IsProtectedVoicemail(this.item.ClassName))
				{
					Item item = null;
					try
					{
						this.item.Load(StoreObjectSchema.ContentConversionProperties);
						item = DRMUtils.OpenRestrictedContent((MessageItem)this.item, this.OrgId);
						UMEventNotificationHelper.PublishUMSuccessEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.ProtectedVoiceMessageEncryptDecrypt.ToString());
					}
					catch (OpenRestrictedContentException obj)
					{
						UmGlobals.ExEvent.LogEvent(this.user.OrganizationId, UMEventLogConstants.Tuple_RMSReadFailure, this.user.OrganizationId.ToString(), this.user.ToString(), CommonUtil.ToEventLogString(obj));
						Util.IncrementCounter(SubscriberAccessCounters.VoiceMessageDecryptionFailures);
						UMEventNotificationHelper.PublishUMFailureEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.ProtectedVoiceMessageEncryptDecrypt.ToString());
						throw;
					}
					finally
					{
						this.item.Dispose();
						this.item = item;
					}
				}
			}
			return this.item;
		}

		private void SetPlaybackContentType()
		{
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				string value = (this.GetMessage(mailboxSessionLock.Session).Body.PreviewText ?? string.Empty).Trim();
				if (XsoUtil.IsPureVoice(this.GetMessage(mailboxSessionLock.Session).ClassName) || XsoUtil.IsProtectedVoicemail(this.GetMessage(mailboxSessionLock.Session).ClassName))
				{
					this.context.ContentType = PlaybackContent.Audio;
				}
				else if (XsoUtil.IsMixedVoice(this.GetMessage(mailboxSessionLock.Session).ClassName))
				{
					this.context.ContentType = (string.IsNullOrEmpty(value) ? PlaybackContent.Audio : PlaybackContent.AudioText);
				}
				else
				{
					this.context.ContentType = PlaybackContent.Text;
					ICollection<string> collection = MessagePartManager.WavePartBuilder.BuildAttachmentPlayOrder(this.GetMessage(mailboxSessionLock.Session));
					if (0 < collection.Count)
					{
						this.context.ContentType = (string.IsNullOrEmpty(value) ? PlaybackContent.Audio : PlaybackContent.TextAudio);
					}
				}
			}
		}

		private string FirstMessagePart()
		{
			if (this.builder == null)
			{
				this.InitializePlayback();
			}
			string result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				LinkedListNode<MessagePartManager.MessagePart> next = (this.CurrentPart != null) ? this.CurrentPart.List.First : this.builder.BuildParts(this.GetMessage(mailboxSessionLock.Session));
				result = this.MoveToNextNonEmptyPart(next);
			}
			return result;
		}

		private string NextMessagePart()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "MessagePartManager::NextMessagePart.", new object[0]);
			string result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				LinkedListNode<MessagePartManager.MessagePart> next;
				if (this.CurrentPart.Equals(this.CurrentPart.List.Last))
				{
					next = this.builder.BuildParts(this.GetMessage(mailboxSessionLock.Session));
				}
				else
				{
					next = this.CurrentPart.Next;
				}
				result = this.MoveToNextNonEmptyPart(next);
			}
			return result;
		}

		private string MoveToNextNonEmptyPart(LinkedListNode<MessagePartManager.MessagePart> next)
		{
			if (next == null || next.Value == null)
			{
				return "endOfSection";
			}
			this.MoveToPart(next);
			if (!next.Value.IsEmpty)
			{
				return null;
			}
			return this.NextMessagePart();
		}

		private void MoveToPart(LinkedListNode<MessagePartManager.MessagePart> part)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "MessagePartManager::MoveToPart.", new object[0]);
			if (this.CurrentPart != null)
			{
				this.CurrentPart.Value.Deactivate(this);
			}
			if (part != null)
			{
				this.CurrentPart = part;
				try
				{
					this.CurrentPart.Value.Activate(this);
				}
				catch (OpenRestrictedContentException obj)
				{
					this.user.ToString();
					UmGlobals.ExEvent.LogEvent(this.user.OrganizationId, UMEventLogConstants.Tuple_RMSReadFailure, this.user.OrganizationId.ToString(), this.user.ToString(), CommonUtil.ToEventLogString(obj));
					Util.IncrementCounter(SubscriberAccessCounters.VoiceMessageDecryptionFailures);
					throw;
				}
				if (this.context.Language == null && UmCultures.GetSupportedPromptCultures().Count > 1)
				{
					this.CurrentPart.Value.DetectLanguage(this.preferredCulture);
					this.context.Language = this.CurrentPart.Value.Language;
					base.Manager.WriteVariable("messageLanguage", this.context.Language);
					if (!object.Equals(this.context.Language, this.preferredCulture))
					{
						base.Manager.WriteVariable("languageDetected", this.context.Language);
					}
				}
				this.WriteIntroConditions();
			}
			string varName = (this.context.Mode == PlaybackMode.Audio) ? "isEmptyWave" : "isEmptyText";
			object obj2 = this.ReadVariable(varName);
			bool flag = this.CurrentPart == null || this.CurrentPart.Value.IsEmpty;
			if ((obj2 == null || (bool)obj2) && (PlaybackContent.TextAudio == this.ContentType || PlaybackContent.AudioText == this.ContentType))
			{
				base.WriteVariable(varName, flag);
			}
		}

		private string NextMessageSection()
		{
			string result = "endOfSection";
			base.WriteVariable("isEmptyWave", null);
			base.WriteVariable("isEmptyText", null);
			if (this.ContentType == PlaybackContent.AudioText && this.context.Mode == PlaybackMode.Audio)
			{
				this.builder = new MessagePartManager.TextPartBuilder(this.context);
				this.context.Mode = PlaybackMode.Text;
				result = this.FirstMessagePart();
			}
			else if (this.ContentType == PlaybackContent.TextAudio && PlaybackMode.Text == this.context.Mode)
			{
				this.builder = new MessagePartManager.WavePartBuilder(this.context);
				this.context.Mode = PlaybackMode.Audio;
				result = this.FirstMessagePart();
			}
			return result;
		}

		private void InitializePlayback()
		{
			switch (this.ContentType)
			{
			case PlaybackContent.Audio:
			case PlaybackContent.AudioText:
				this.builder = new MessagePartManager.WavePartBuilder(this.context);
				this.context.Mode = PlaybackMode.Audio;
				return;
			case PlaybackContent.TextAudio:
			case PlaybackContent.Text:
				this.builder = new MessagePartManager.TextPartBuilder(this.context);
				this.context.Mode = PlaybackMode.Text;
				return;
			default:
				return;
			}
		}

		private void WriteIntroConditions()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = this.CurrentPart != null && this.CurrentPart == this.CurrentPart.List.First;
			if (this.ContentType == PlaybackContent.AudioText)
			{
				flag2 = (flag4 && PlaybackMode.Audio == this.context.Mode);
				flag3 = (flag4 && PlaybackMode.Text == this.context.Mode);
				flag = flag2;
			}
			else if (this.ContentType == PlaybackContent.TextAudio)
			{
				flag2 = (flag4 && PlaybackMode.Audio == this.context.Mode);
				flag3 = (flag4 && PlaybackMode.Text == this.context.Mode);
				flag = flag3;
			}
			base.WriteVariable("playMixedContentIntro", flag);
			base.WriteVariable("playAudioContentIntro", flag2);
			base.WriteVariable("playTextContentIntro", flag3);
		}

		private Item item;

		private MessagePartManager.MessagePartBuilder builder;

		private UMSubscriber user;

		private UMMailboxRecipient.MailboxConnectionGuard guard;

		private CultureInfo preferredCulture;

		private MessagePlayerContext context;

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing MessagePartConfig activity manager.", new object[0]);
				return new MessagePartManager(manager, this);
			}
		}

		internal abstract class MessagePartBuilder
		{
			protected MessagePartBuilder(MessagePlayerContext context)
			{
				this.context = context;
			}

			protected MessagePlayerContext Context
			{
				get
				{
					return this.context;
				}
			}

			internal abstract LinkedListNode<MessagePartManager.MessagePart> BuildParts(Item item);

			private MessagePlayerContext context;
		}

		internal class TextPartBuilder : MessagePartManager.MessagePartBuilder
		{
			internal TextPartBuilder(MessagePlayerContext context) : base(context)
			{
			}

			internal override LinkedListNode<MessagePartManager.MessagePart> BuildParts(Item item)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "TextPartBuilder::BuildParts.", new object[0]);
				StreamReader streamReader = this.GetReader(item);
				char[] array = new char[16384];
				base.Context.Remnant.CopyTo(array, 0);
				int num = base.Context.Remnant.Length;
				base.Context.Remnant = new char[0];
				num += streamReader.ReadBlock(array, num, array.Length - num);
				base.Context.SeekPosition = streamReader.BaseStream.Position;
				if (num < 1)
				{
					return null;
				}
				LinkedList<MessagePartManager.MessagePart> linkedList = (base.Context.CurrentTextPart == null) ? new LinkedList<MessagePartManager.MessagePart>() : base.Context.CurrentTextPart.List;
				LinkedListNode<MessagePartManager.MessagePart> last = linkedList.Last;
				int num2 = 0;
				MessagePartManager.TextMessagePart textMessagePart = this.BuildTextPart(array, ref num2, num);
				if (textMessagePart != null)
				{
					linkedList.AddLast(textMessagePart);
				}
				int num3 = Math.Max(0, num - num2);
				base.Context.Remnant = new char[num3];
				if (num3 > 0)
				{
					Array.Copy(array, num2, base.Context.Remnant, 0, num3);
				}
				if (last != null)
				{
					return last.Next;
				}
				return linkedList.First;
			}

			private static bool IsCalendarHeaderPattern(char[] buffer, int bufIdx, int maxBufSize)
			{
				if (Constants.CDOCalendarSeparator.Length > maxBufSize - bufIdx)
				{
					return false;
				}
				int i = 0;
				while (i < Constants.CDOCalendarSeparator.Length)
				{
					if (Constants.CDOCalendarSeparator[i] != buffer[bufIdx])
					{
						return false;
					}
					i++;
					bufIdx++;
				}
				return true;
			}

			private static bool IsEmailHeaderPattern(char[] buffer, int bufIdx, int maxBufSize)
			{
				bool result = true;
				int num = 0;
				while (bufIdx < maxBufSize && num < 5)
				{
					if ('-' != buffer[bufIdx] && '_' != buffer[bufIdx])
					{
						result = false;
						break;
					}
					num++;
					bufIdx++;
				}
				return result;
			}

			private Stream GetItemStream(Item item)
			{
				if (this.stream == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "TextPartBuilder::ItemStream.", new object[0]);
					long size = item.Body.Size;
					if (size > 262144L)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Using file stream to back item store content with size={0}.", new object[]
						{
							size
						});
						this.tempFile = TempFileFactory.CreateTempFile();
						this.stream = new FileStream(this.tempFile.FilePath, FileMode.Create);
					}
					else
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Using memory stream to back item store content with size={0}.", new object[]
						{
							size
						});
						this.stream = new MemoryStream();
					}
					BodyReadConfiguration configuration = new BodyReadConfiguration(BodyFormat.TextPlain, "unicode");
					using (Stream stream = item.Body.OpenReadStream(configuration))
					{
						byte[] array = new byte[8192];
						for (;;)
						{
							int num = stream.Read(array, 0, array.Length);
							if (num == 0)
							{
								break;
							}
							this.stream.Write(array, 0, num);
						}
					}
					this.stream.Seek(base.Context.SeekPosition, SeekOrigin.Begin);
				}
				return this.stream;
			}

			private StreamReader GetReader(Item item)
			{
				if (this.reader == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "TextPartBuilder::Reader.", new object[0]);
					this.reader = new StreamReader(this.GetItemStream(item), Encoding.Unicode);
				}
				return this.reader;
			}

			private MessagePartManager.TextMessagePart BuildTextPart(char[] buffer, ref int idx, int bufSize)
			{
				int num = idx;
				int num2 = idx;
				int num3 = idx;
				int num4 = 0;
				bufSize = Math.Min(checked(idx + 16384), bufSize);
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "idx={0}, bufSize={1}.", new object[]
				{
					idx,
					bufSize
				});
				int num5 = idx;
				while (idx < bufSize)
				{
					char c = buffer[idx];
					if (c <= '.')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							if (idx > 0 && buffer[idx - 1] != '\r')
							{
								num4++;
							}
							num = idx;
							goto IL_193;
						case '\v':
						case '\f':
							goto IL_193;
						case '\r':
							num4++;
							num = idx;
							goto IL_193;
						default:
							switch (c)
							{
							case ' ':
								break;
							case '!':
								goto IL_FF;
							default:
								switch (c)
								{
								case '*':
								{
									if (!MessagePartManager.TextPartBuilder.IsCalendarHeaderPattern(buffer, idx, bufSize))
									{
										goto IL_193;
									}
									if (4 > num4 && num5 == 0)
									{
										num5 = idx + Constants.CDOCalendarSeparator.Length;
										goto IL_193;
									}
									int i = 0;
									while (i < Constants.CDOCalendarSeparator.Length)
									{
										buffer[idx] = ' ';
										i++;
										idx++;
									}
									goto IL_193;
								}
								case '+':
								case ',':
									goto IL_193;
								case '-':
									goto IL_165;
								case '.':
									goto IL_FF;
								default:
									goto IL_193;
								}
								break;
							}
							break;
						}
						num3 = idx;
					}
					else
					{
						switch (c)
						{
						case ':':
						case ';':
							goto IL_FF;
						default:
							if (c == '?')
							{
								goto IL_FF;
							}
							if (c == '_')
							{
								goto IL_165;
							}
							break;
						}
					}
					IL_193:
					idx++;
					continue;
					IL_FF:
					if (idx + 1 < bufSize && char.IsWhiteSpace(buffer[idx + 1]))
					{
						num2 = idx;
						goto IL_193;
					}
					goto IL_193;
					IL_165:
					if (MessagePartManager.TextPartBuilder.IsEmailHeaderPattern(buffer, idx, bufSize))
					{
						while (idx < bufSize && ('-' == buffer[idx] || '_' == buffer[idx]))
						{
							buffer[idx] = ' ';
							idx++;
						}
						goto IL_193;
					}
					goto IL_193;
				}
				if (bufSize == buffer.Length)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "maxParagraphEnd ={0}, maxPhraseEnd={1}, maxWhitespace={2}.", new object[]
					{
						num,
						num2,
						num3
					});
					int num6 = (bufSize - num5) / 2 + 1;
					if (num > num6)
					{
						idx = num;
					}
					else if (num2 > num6)
					{
						idx = num2;
					}
					else if (num3 > num5)
					{
						idx = num3;
					}
				}
				if (idx == num5)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Could not build a text part: idx={0}, startIdx={1}. Returning null.", new object[]
					{
						idx,
						num5
					});
					return null;
				}
				return new MessagePartManager.TextMessagePart(new string(buffer, num5, idx - num5));
			}

			internal const long MaximumMemoryBuffer = 262144L;

			internal const int MaximumTtsBlock = 16384;

			private Stream stream;

			private ITempFile tempFile;

			private StreamReader reader;
		}

		internal class WavePartBuilder : MessagePartManager.MessagePartBuilder
		{
			internal WavePartBuilder(MessagePlayerContext context) : base(context)
			{
			}

			internal static ICollection<string> BuildAttachmentPlayOrder(Item item)
			{
				List<string> list = new List<string>();
				string attachmentOrderString = XsoUtil.GetAttachmentOrderString(item);
				string[] array = attachmentOrderString.Split(new char[]
				{
					';'
				});
				for (int i = array.Length - 1; i >= 0; i--)
				{
					string text = array[i].Trim();
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(text);
					}
				}
				return list;
			}

			internal override LinkedListNode<MessagePartManager.MessagePart> BuildParts(Item item)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "WavePartBuilder::BuildParts.", new object[0]);
				if (base.Context.CurrentWavePart != null && base.Context.CurrentWavePart.Value is MessagePartManager.WaveMessagePart)
				{
					return null;
				}
				LinkedList<MessagePartManager.MessagePart> linkedList = (base.Context.CurrentWavePart == null) ? new LinkedList<MessagePartManager.MessagePart>() : base.Context.CurrentWavePart.List;
				LinkedListNode<MessagePartManager.MessagePart> last = linkedList.Last;
				ICollection<string> collection = MessagePartManager.WavePartBuilder.BuildAttachmentPlayOrder(item);
				foreach (string attachName in collection)
				{
					MessagePartManager.WaveMessagePart value = new MessagePartManager.WaveMessagePart(attachName);
					linkedList.AddLast(new LinkedListNode<MessagePartManager.MessagePart>(value));
				}
				if (last != null)
				{
					return last.Next;
				}
				return linkedList.First;
			}
		}

		internal abstract class MessagePart
		{
			internal abstract bool IsEmpty { get; }

			internal virtual CultureInfo Language
			{
				get
				{
					return null;
				}
			}

			internal abstract void Activate(MessagePartManager context);

			internal abstract void Deactivate(MessagePartManager context);

			internal virtual void DetectLanguage(CultureInfo preferred)
			{
			}
		}

		internal class TextMessagePart : MessagePartManager.MessagePart
		{
			internal TextMessagePart(string text)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, null, "TextMessagePart::TextMessagePart", new object[0]);
				this.text = new EmailNormalizedText(text);
				this.originalText = (text ?? string.Empty);
				this.originalText = this.originalText.Trim();
			}

			internal override bool IsEmpty
			{
				get
				{
					return this.text == null || this.text.ToString().Length == 0 || !Regex.IsMatch(this.text.ToString(), "[^\\s<>]");
				}
			}

			internal override CultureInfo Language
			{
				get
				{
					return this.detectedLanguage;
				}
			}

			internal override void Activate(MessagePartManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "TextMessagePart::Activate.", new object[0]);
				manager.WriteVariable("textMessagePart", this.text);
				manager.WriteVariable("textPart", true);
				manager.WriteVariable("wavePart", false);
			}

			internal override void Deactivate(MessagePartManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "TextMessagePart::Deactivate.", new object[0]);
				manager.WriteVariable("textMessagePart", null);
				manager.WriteVariable("textPart", null);
				manager.WriteVariable("wavePart", null);
			}

			internal override void DetectLanguage(CultureInfo preferred)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "TextMessagePart::DetectLanguage.", new object[0]);
				if (this.IsEmpty)
				{
					return;
				}
				uint length = (uint)this.originalText.Length;
				if (GlobCfg.LanguageAutoDetectionMinLength < 0)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "DetectLanguage: Language autodetection is disabled by configuration file.", new object[0]);
					return;
				}
				if ((ulong)length < (ulong)((long)GlobCfg.LanguageAutoDetectionMinLength))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "DetectLanguage: Text length({0})<LanguageAutoDetectionMinLength({1}), skipping.", new object[]
					{
						length,
						GlobCfg.LanguageAutoDetectionMinLength
					});
					return;
				}
				string text;
				if ((ulong)length > (ulong)((long)GlobCfg.LanguageAutoDetectionMaxLength))
				{
					text = this.originalText.Substring(0, GlobCfg.LanguageAutoDetectionMaxLength);
					length = (uint)text.Length;
				}
				else
				{
					text = this.originalText;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "TextMessagePart::DetectLanguage Starting language detection. Lenght = {0}", new object[]
				{
					length
				});
				CultureInfo cultureInfo = LanguageDetector.Instance.AnalyzeText(text, length);
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "TextMessagePart::DetectLanguage Language detection complete. Detected = {0}, Preferred = {1}", new object[]
				{
					cultureInfo,
					preferred
				});
				if (cultureInfo != null)
				{
					cultureInfo = UmCultures.GetBestSupportedPromptCulture(cultureInfo);
				}
				if (cultureInfo != null && preferred != null && string.Equals(cultureInfo.TwoLetterISOLanguageName, preferred.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
				{
					this.detectedLanguage = preferred;
				}
				else
				{
					this.detectedLanguage = cultureInfo;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "TextMessagePart::DetectLanguage: DetectedLanguage = {0}", new object[]
				{
					this.detectedLanguage
				});
			}

			private string originalText;

			private EmailNormalizedText text;

			private CultureInfo detectedLanguage;
		}

		internal class WaveMessagePart : MessagePartManager.MessagePart
		{
			internal WaveMessagePart(string attachName)
			{
				this.attachName = attachName;
			}

			internal override bool IsEmpty
			{
				get
				{
					return this.tmpWavFile == null || !File.Exists(this.tmpWavFile.FilePath) || 0L == new FileInfo(this.tmpWavFile.FilePath).Length;
				}
			}

			internal override void Activate(MessagePartManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "WaveMessagePart::Activate.", new object[0]);
				Stream stream = null;
				Stream stream2 = null;
				try
				{
					if (this.tmpWavFile == null)
					{
						using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = manager.user.CreateSessionLock())
						{
							using (ITempFile tempFile = TempFileFactory.CreateTempSoundFileFromAttachmentName(this.attachName))
							{
								using (Attachment attachmentStream = this.GetAttachmentStream(manager, mailboxSessionLock, out stream))
								{
									if (attachmentStream != null)
									{
										stream2 = new FileStream(tempFile.FilePath, FileMode.Create);
										CommonUtil.CopyStream(stream, stream2);
										stream.Close();
										stream = null;
										stream2.Close();
										stream2 = null;
									}
									this.tmpWavFile = MediaMethods.ToPcm(tempFile);
								}
							}
						}
					}
				}
				catch (AudioConversionException)
				{
					this.tmpWavFile = null;
				}
				finally
				{
					manager.WriteVariable("waveMessagePart", this.tmpWavFile);
					manager.WriteVariable("textPart", false);
					manager.WriteVariable("wavePart", !this.IsEmpty);
					if (stream != null)
					{
						stream.Close();
					}
					if (stream2 != null)
					{
						stream2.Close();
					}
				}
			}

			internal override void Deactivate(MessagePartManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "WaveMessagePart::Deactivate.", new object[0]);
				manager.WriteVariable("waveMessagePart", null);
				manager.WriteVariable("textPart", null);
				manager.WriteVariable("wavePart", null);
			}

			private Attachment GetAttachmentStream(MessagePartManager manager, UMMailboxRecipient.MailboxSessionLock mbx, out Stream result)
			{
				result = null;
				Item message = manager.GetMessage(mbx.Session);
				foreach (AttachmentHandle handle in message.AttachmentCollection)
				{
					Attachment attachment = null;
					try
					{
						attachment = message.AttachmentCollection.Open(handle);
						CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "found attachment name={0}.", new object[]
						{
							attachment.FileName
						});
						if (string.Equals(attachment.FileName, this.attachName, StringComparison.OrdinalIgnoreCase))
						{
							if (XsoUtil.IsProtectedVoicemail(message.ClassName))
							{
								result = DRMUtils.OpenProtectedAttachment(attachment, manager.OrgId);
							}
							else
							{
								result = (attachment as StreamAttachment).GetContentStream();
							}
							return attachment;
						}
					}
					finally
					{
						if (result == null && attachment != null)
						{
							attachment.Dispose();
						}
					}
				}
				return null;
			}

			private string attachName;

			private ITempWavFile tmpWavFile;
		}
	}
}
