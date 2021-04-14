using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.ApplicationLogic.UM;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class XSOVoiceMessagePipelineContext : VoiceMessagePipelineContextBase
	{
		internal XSOVoiceMessagePipelineContext(SubmissionHelper helper, string waveFilePath, int duration, AudioCodecEnum codec, string attachmentName, MessageItem message, UMSubscriber caller) : base(codec, helper, waveFilePath, duration)
		{
			if (caller == null)
			{
				throw new ArgumentNullException("caller");
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			this.caller = caller;
			this.caller.AddReference();
			base.MessageToSubmit = message;
			this.attachmentName = attachmentName;
			this.messageMarkedPrivate = (message.Sensitivity == Sensitivity.Private);
			this.pureVoiceMessage = XsoUtil.IsPureVoice(message.ClassName);
			this.replyToAProtectedVoicemail = (base.MessageToSubmit is RightsManagedMessageItem);
			base.MessageType = "XSOVoiceMail";
		}

		internal XSOVoiceMessagePipelineContext(SubmissionHelper helper) : base(helper)
		{
			bool flag = false;
			try
			{
				base.MessageType = "XSOVoiceMail";
				this.messageFilePath = (string)helper.CustomHeaders["MessageFilePath"];
				if (string.IsNullOrEmpty(this.messageFilePath) || !File.Exists(this.messageFilePath))
				{
					CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "MessageItem file {0} does not exist", new object[]
					{
						this.messageFilePath
					});
					throw new HeaderFileArgumentInvalidException(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
					{
						"MessageFilePath",
						this.messageFilePath
					}));
				}
				if (!helper.CustomHeaders.Contains("SenderObjectGuid"))
				{
					throw new HeaderFileArgumentInvalidException("SenderObjectGuid");
				}
				if (helper.CustomHeaders.Contains("Private"))
				{
					this.messageMarkedPrivate = bool.Parse((string)helper.CustomHeaders["Private"]);
				}
				if (helper.CustomHeaders.Contains("PureInterpersonalMessage"))
				{
					this.pureVoiceMessage = bool.Parse((string)helper.CustomHeaders["PureInterpersonalMessage"]);
				}
				if (helper.CustomHeaders.Contains("ProtectedReply"))
				{
					this.replyToAProtectedVoicemail = bool.Parse((string)helper.CustomHeaders["ProtectedReply"]);
				}
				Guid tenantGuid = helper.TenantGuid;
				this.caller = (base.CreateRecipientFromObjectGuid(new Guid((string)helper.CustomHeaders["SenderObjectGuid"]), tenantGuid) as UMSubscriber);
				base.TranscriptionUser = this.caller;
				if (this.pureVoiceMessage && this.caller.ShouldMessageBeProtected(false, this.messageMarkedPrivate))
				{
					this.messageNeedsToBeProtected = true;
				}
				if (base.Duration.TotalSeconds > 0.0)
				{
					this.attachmentName = (string)helper.CustomHeaders["AttachmentName"];
				}
				else
				{
					this.attachmentName = null;
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		internal override Pipeline Pipeline
		{
			get
			{
				if (this.messageNeedsToBeProtected || this.replyToAProtectedVoicemail)
				{
					return ProtectedXsoAudioPipeline.Instance;
				}
				return XSOAudioPipeline.Instance;
			}
		}

		internal UMSubscriber Caller
		{
			get
			{
				return this.caller;
			}
		}

		public override bool EnableTopNGrammar
		{
			get
			{
				return false;
			}
		}

		public override void PrepareUnProtectedMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:PrepareUnProtectedMessage.", new object[0]);
			try
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.Caller.CreateSessionLock())
				{
					bool flag = false;
					try
					{
						this.DeserializeMessageFromDisk(mailboxSessionLock);
						this.SetMessageProperties();
						flag = true;
					}
					finally
					{
						if (!flag && base.MessageToSubmit != null)
						{
							base.MessageToSubmit.Dispose();
						}
					}
				}
			}
			catch (StorageTransientException ex)
			{
				this.AddAdditionalInfoAndReThrowException(ex);
			}
		}

		public override string GetMailboxServerId()
		{
			return this.Caller.ADUser.ServerLegacyDN;
		}

		public override string GetRecipientIdForThrottling()
		{
			return this.Caller.ADUser.DistinguishedName;
		}

		public override void PrepareProtectedMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:PrepareProtectedMessage.", new object[0]);
			try
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.Caller.CreateSessionLock())
				{
					bool flag = false;
					try
					{
						UMMailboxPolicy ummailboxPolicy = this.Caller.UMMailboxPolicy;
						LocalizedString? customDRMText = base.GetCustomDRMText(ummailboxPolicy);
						if (this.replyToAProtectedVoicemail)
						{
							this.DeserializeMessageFromDisk(mailboxSessionLock);
							this.RebindToProtectedMessage(customDRMText);
						}
						else
						{
							this.DeserializeMessageFromDisk(mailboxSessionLock);
							this.SetMessageProperties();
							base.CreateRightsManagedItem(customDRMText, this.Caller, "IPM.Note.rpmsg.Microsoft.Voicemail.UM");
						}
						base.MessageToSubmit.Load();
						flag = true;
					}
					finally
					{
						if (!flag && base.MessageToSubmit != null)
						{
							base.MessageToSubmit.Dispose();
						}
					}
				}
			}
			catch (StorageTransientException ex)
			{
				this.AddAdditionalInfoAndReThrowException(ex);
			}
		}

		public override void PrepareNDRForFailureToGenerateProtectedMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:PrepareNDRForFailureToGenerateProtectedMessage.", new object[0]);
			this.sentNDRForFailureToGenerateRMSMessage = true;
			UmGlobals.ExEvent.LogEvent(this.Caller.OrganizationId, UMEventLogConstants.Tuple_RMSIntepersonalSendFailure, this.Caller.OrganizationId.ToString(), this.Caller.ToString());
			try
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.Caller.CreateSessionLock())
					{
						using (Folder folder = UMStagingFolder.OpenOrCreateUMStagingFolder(mailboxSessionLock.Session))
						{
							base.MessageToSubmit = XsoUtil.CreateTemporaryMessage(mailboxSessionLock.Session, folder, this.temporaryMessageRetentionDays);
							disposeGuard.Add<MessageItem>(base.MessageToSubmit);
							ExAssert.RetailAssert(this.recips != null && this.recips.Count != 0, "There has to be recipients in order to generate an NDR");
							MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(base.CultureInfo);
							base.MessageToSubmit.Subject = messageContentBuilder.GetNDRSubjectForInterpersonalDRM();
							messageContentBuilder.AddNDRBodyForInterpersonalDRM(this.Caller, this.recips, base.SentTime);
							base.SetNDRProperties(messageContentBuilder);
							base.MessageToSubmit.Recipients.Add(new Participant(this.Caller.ADRecipient));
							base.MessageToSubmit.Sender = new Participant(this.Caller.ADRecipient);
							disposeGuard.Success();
						}
					}
				}
			}
			catch (StorageTransientException ex)
			{
				this.AddAdditionalInfoAndReThrowException(ex);
			}
		}

		public override void PostCompletion()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext.PostCompletion - Removing the msg file '{0}'", new object[]
			{
				this.messageFilePath
			});
			Util.TryDeleteFile(this.messageFilePath);
			if (this.messageNeedsToBeProtected)
			{
				if (this.sentNDRForFailureToGenerateRMSMessage)
				{
					Util.IncrementCounter(SubscriberAccessCounters.VoiceMessageProtectionFailures);
					UMEventNotificationHelper.PublishUMFailureEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.ProtectedVoiceMessageEncryptDecrypt.ToString());
				}
				else
				{
					Util.IncrementCounter(SubscriberAccessCounters.ProtectedVoiceMessagesSent);
					UMEventNotificationHelper.PublishUMSuccessEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.ProtectedVoiceMessageEncryptDecrypt.ToString());
				}
			}
			base.PostCompletion();
		}

		internal static void SaveAndDeleteMessageItem(MessageItem message, string targetPath, UMRecipient recipient)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "MessageItem will be saved in file {0}", new object[]
			{
				targetPath
			});
			message.Save(SaveMode.NoConflictResolution);
			try
			{
				message.Load(StoreObjectSchema.ContentConversionProperties);
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Starting conversion of the MessageItem to the tnef format.", new object[0]);
				using (FileStream fileStream = File.Open(targetPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
				{
					ItemConversion.ConvertItemToMsgStorage(message, fileStream, XsoUtil.GetOutboundConversionOptions(recipient));
				}
			}
			finally
			{
				message.Session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					message.Id
				});
			}
		}

		internal override void WriteCustomHeaderFields(StreamWriter headerStream)
		{
			base.WriteCustomHeaderFields(headerStream);
			headerStream.WriteLine("SenderObjectGuid : " + this.caller.ADRecipient.Guid);
			headerStream.WriteLine("MessageFilePath : " + this.messageFilePath);
			string str = this.attachmentName ?? "null";
			headerStream.WriteLine("AttachmentName : " + str);
			headerStream.WriteLine("Private : " + this.messageMarkedPrivate);
			headerStream.WriteLine("ProtectedReply : " + this.replyToAProtectedVoicemail);
			headerStream.WriteLine("PureInterpersonalMessage : " + this.pureVoiceMessage);
		}

		internal override void SaveMessage()
		{
			this.messageFilePath = Path.Combine(Path.GetDirectoryName(base.HeaderFileName), Path.GetFileNameWithoutExtension(base.HeaderFileName) + ".msg");
			XSOVoiceMessagePipelineContext.SaveAndDeleteMessageItem(base.MessageToSubmit, this.messageFilePath, this.caller);
			base.SaveMessage();
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Successfully saved the XSOVoiceMessagePipelineContext message.", new object[0]);
		}

		protected override void SetMessageProperties()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:SetMessageProperties.", new object[0]);
			if (base.TranscriptionData != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:SetMessageProperties. Adding the TranscriptionData", new object[0]);
				this.InjectAudioPreviewInBody(base.MessageToSubmit.Body);
				if (!this.messageNeedsToBeProtected)
				{
					using (Stream stream = base.MessageToSubmit.OpenPropertyStream(MessageItemSchema.AsrData, PropertyOpenMode.Create))
					{
						using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
						{
							streamWriter.Write(base.TranscriptionData.TranscriptionXml.OuterXml);
						}
					}
				}
			}
			if (this.attachmentName != null)
			{
				if (this.messageNeedsToBeProtected)
				{
					XsoUtil.AddHiddenAttachment(base.MessageToSubmit.AttachmentCollection, base.CompressedAudioFile, this.attachmentName, Microsoft.Exchange.UM.UMCommon.AudioCodec.GetMimeType(base.AudioCodec));
				}
				else
				{
					XsoUtil.AddAttachment(base.MessageToSubmit.AttachmentCollection, base.CompressedAudioFile, this.attachmentName, Microsoft.Exchange.UM.UMCommon.AudioCodec.GetMimeType(base.AudioCodec));
				}
				XsoUtil.UpdateAttachementOrder(base.MessageToSubmit, this.attachmentName);
			}
			XsoUtil.AppendVoiceClass(base.MessageToSubmit);
			base.MessageToSubmit.Sender = new Participant(this.Caller.ADRecipient);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XSOVoiceMessagePipelineContext>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext.Dispose() called", new object[0]);
					if (this.caller != null)
					{
						this.caller.ReleaseReference();
					}
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		private void AddAdditionalInfoAndReThrowException(StorageTransientException ex)
		{
			throw new MailboxUnavailableException(base.MessageType, this.Caller.ADUser.Database.DistinguishedName, ex.Message, ex);
		}

		private void DeserializeMessageFromDisk(UMMailboxRecipient.MailboxSessionLock mbx)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:DeserializeMessageFromDisk.", new object[0]);
			using (Folder folder = UMStagingFolder.OpenOrCreateUMStagingFolder(mbx.Session))
			{
				base.MessageToSubmit = XsoUtil.CreateTemporaryMessage(mbx.Session, folder, this.temporaryMessageRetentionDays);
				AcceptedDomain defaultAcceptedDomain = Utils.GetDefaultAcceptedDomain(this.caller.ADRecipient);
				InboundConversionOptions inboundConversionOptions = new InboundConversionOptions(defaultAcceptedDomain.DomainName.ToString());
				inboundConversionOptions.UserADSession = ADRecipientLookupFactory.CreateFromUmUser(this.Caller).ScopedRecipientSession;
				using (FileStream fileStream = new FileStream(this.messageFilePath, FileMode.Open, FileAccess.Read))
				{
					ItemConversion.ConvertMsgStorageToItem(fileStream, base.MessageToSubmit, inboundConversionOptions);
				}
				this.recips = new RecipientDetails(base.MessageToSubmit.Recipients);
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext: Successfully read and converted the MessageItem from file {0}.", new object[]
				{
					this.messageFilePath
				});
			}
		}

		private void RebindToProtectedMessage(LocalizedString? outerMessageBody)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:RebindToProtectedMessage.", new object[0]);
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				FaultInjectionUtils.FaultInjectException(3034983741U);
				RightsManagedMessageItem rightsManagedMessageItem = RightsManagedMessageItem.ReBind(base.MessageToSubmit, XsoUtil.GetOutboundConversionOptions(this.Caller), true);
				disposeGuard.Add<RightsManagedMessageItem>(rightsManagedMessageItem);
				this.AddAttachmentsToProtectedReply(rightsManagedMessageItem);
				rightsManagedMessageItem.SetDefaultEnvelopeBody(outerMessageBody);
				rightsManagedMessageItem.ClassName = "IPM.Note.rpmsg.Microsoft.Voicemail.UM";
				rightsManagedMessageItem[StoreObjectSchema.ContentClass] = "IPM.Note.rpmsg.Microsoft.Voicemail.UM";
				rightsManagedMessageItem.Save(SaveMode.FailOnAnyConflict);
				disposeGuard.Success();
				base.MessageToSubmit = rightsManagedMessageItem;
			}
		}

		private void InjectAudioPreviewInBody(Body body)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:InjectAudioPreviewInBody.", new object[0]);
			MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(base.CultureInfo);
			messageContentBuilder.AddAudioPreview(base.TranscriptionData);
			string text = messageContentBuilder.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				this.InjectPrefixInMessageBody(text, body);
			}
		}

		private void InjectPrefixInMessageBody(string prefix, Body body)
		{
			BodyReadConfiguration configuration = new BodyReadConfiguration(BodyFormat.TextHtml, "unicode");
			BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(BodyFormat.TextHtml, "unicode");
			bodyWriteConfiguration.AddInjectedText(prefix, null, BodyInjectionFormat.Html);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (Stream stream = body.OpenReadStream(configuration))
				{
					Util.StreamHandler.CopyStreamData(stream, memoryStream);
				}
				using (Stream stream2 = body.OpenWriteStream(bodyWriteConfiguration))
				{
					memoryStream.WriteTo(stream2);
				}
			}
		}

		private void AddAttachmentsToProtectedReply(RightsManagedMessageItem message)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:AddAttachmentsToProtectedReply.", new object[0]);
			XsoUtil.AddHiddenAttachment(message.ProtectedAttachmentCollection, base.CompressedAudioFile, this.attachmentName, Microsoft.Exchange.UM.UMCommon.AudioCodec.GetMimeType(base.AudioCodec));
			XsoUtil.UpdateAttachementOrder(message, this.attachmentName);
			if (base.TranscriptionData != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "XSOVoiceMessagePipelineContext:SetMessageProperties. Adding the TranscriptionData", new object[0]);
				this.InjectAudioPreviewInBody(message.ProtectedBody);
				base.AddASRDataAttachment(message);
			}
		}

		private UMSubscriber caller;

		private string messageFilePath;

		private bool messageMarkedPrivate;

		private bool messageNeedsToBeProtected;

		private bool sentNDRForFailureToGenerateRMSMessage;

		private bool pureVoiceMessage;

		private bool replyToAProtectedVoicemail;

		private string attachmentName;

		private RecipientDetails recips;
	}
}
