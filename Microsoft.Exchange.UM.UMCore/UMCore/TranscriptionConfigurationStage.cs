using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TranscriptionConfigurationStage : SynchronousPipelineStageBase, IUMNetworkResource
	{
		internal override PipelineDispatcher.PipelineResourceType ResourceType
		{
			get
			{
				return PipelineDispatcher.PipelineResourceType.NetworkBound;
			}
		}

		public string NetworkResourceId
		{
			get
			{
				return base.WorkItem.Message.GetMailboxServerId();
			}
		}

		internal override TimeSpan ExpectedRunTime
		{
			get
			{
				return TimeSpan.FromMinutes(1.0);
			}
		}

		protected override StageRetryDetails InternalGetRetrySchedule()
		{
			return new StageRetryDetails(StageRetryDetails.FinalAction.SkipStage);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TranscriptionConfigurationStage>(this);
		}

		protected override void InternalDoSynchronousWork()
		{
			this.BuildTranscriptionConfiguration();
		}

		private bool ShouldRunTranscriptionStage(UMSubscriber subscriber, TranscriptionEnabledSetting transcriptionEnabled, out RecoResultType recoResultType, out RecoErrorType recoErrorType, out CultureInfo culture)
		{
			bool result = false;
			culture = null;
			recoResultType = RecoResultType.Skipped;
			recoErrorType = RecoErrorType.Other;
			try
			{
				culture = Util.GetDefaultCulture(subscriber.DialPlan);
				if (transcriptionEnabled == TranscriptionEnabledSetting.Unknown)
				{
					PIIMessage data = PIIMessage.Create(PIIType._User, subscriber.ADRecipient.DistinguishedName);
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), data, "Could not retrieve transcription enabled setting in mailbox of user '_User'", new object[0]);
					recoResultType = RecoResultType.Skipped;
					recoErrorType = RecoErrorType.ErrorReadingSettings;
				}
				else if (!this.IsTranscriptionLanguageSupported(culture))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Language {0} is NOT supported for transcription", new object[]
					{
						culture
					});
					recoResultType = RecoResultType.Skipped;
					recoErrorType = RecoErrorType.LanguageNotSupported;
				}
				else if (this.IsMessageTooLongForTranscription())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Message is too long. Skipping", new object[0]);
					recoResultType = RecoResultType.Skipped;
					recoErrorType = RecoErrorType.MessageTooLong;
				}
				else if (this.ShouldThrottleTranscription())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Transcription throttle check failed. Skipping", new object[0]);
					recoResultType = RecoResultType.Skipped;
					recoErrorType = RecoErrorType.Throttled;
				}
				else
				{
					result = true;
				}
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Error determining if transcription should be run. e='{0}'", new object[]
				{
					ex
				});
				recoResultType = RecoResultType.Skipped;
				recoErrorType = RecoErrorType.Other;
			}
			return result;
		}

		private void BuildTranscriptionConfiguration()
		{
			IUMTranscribeAudio iumtranscribeAudio = base.WorkItem.Message as IUMTranscribeAudio;
			ExAssert.RetailAssert(iumtranscribeAudio != null, "TranscriptionStage must operate only on PipelineContext which implements IUMTranscribeAudio");
			if (iumtranscribeAudio.Duration.TotalSeconds <= 0.0 || iumtranscribeAudio.AttachmentPath == null)
			{
				base.WorkItem.TranscriptionContext.ShouldRunTranscriptionStage = false;
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "There is no recording to perform transcription upon", new object[0]);
				return;
			}
			if (iumtranscribeAudio.TranscriptionData != null)
			{
				base.WorkItem.TranscriptionContext.ShouldRunTranscriptionStage = false;
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "TranscriptionData already available, skipping EVM stage.", new object[0]);
				return;
			}
			RecoResultType recognitionResult = RecoResultType.Skipped;
			RecoErrorType recognitionError = RecoErrorType.Other;
			CultureInfo cultureInfo = null;
			TranscriptionEnabledSetting transcriptionEnabledSetting = this.IsTranscriptionEnabled();
			if (transcriptionEnabledSetting == TranscriptionEnabledSetting.Disabled)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Transcription for subscriber '{0}' and this message is not enabled. ", new object[]
				{
					iumtranscribeAudio.TranscriptionUser
				});
				return;
			}
			bool flag = this.ShouldRunTranscriptionStage(iumtranscribeAudio.TranscriptionUser, transcriptionEnabledSetting, out recognitionResult, out recognitionError, out cultureInfo);
			if (flag)
			{
				base.WorkItem.TranscriptionContext.Culture = cultureInfo;
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Message will be transcribed.  Updating transcription backlog with '{0}'.", new object[]
				{
					iumtranscribeAudio.Duration
				});
				base.WorkItem.TranscriptionContext.BacklogContribution = iumtranscribeAudio.Duration;
				TranscriptionStage.UpdateBacklog(iumtranscribeAudio.Duration);
				OffensiveWordsFilter offensiveWordsFilter;
				if (!OffensiveWordsFilter.TryGet(cultureInfo, out offensiveWordsFilter))
				{
					ExAssert.RetailAssert(false, "Couldn't find offensive words for transcription language '{0}'", new object[]
					{
						cultureInfo
					});
				}
				base.WorkItem.TranscriptionContext.ShouldRunTranscriptionStage = flag;
				base.WorkItem.TranscriptionContext.TopN = (iumtranscribeAudio.EnableTopNGrammar ? TopNData.Create(iumtranscribeAudio.TranscriptionUser, offensiveWordsFilter) : null);
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Message will NOT be transcribed.  Setting default transcription data.", new object[0]);
			TranscriptionData transcriptionData = new TranscriptionData(recognitionResult, recognitionError, cultureInfo, new List<IUMTranscriptionResult>());
			transcriptionData.UpdatePerfCounters();
			iumtranscribeAudio.TranscriptionData = transcriptionData;
			base.WorkItem.TranscriptionContext.ShouldRunTranscriptionStage = false;
		}

		private TranscriptionEnabledSetting IsTranscriptionEnabled()
		{
			IUMTranscribeAudio iumtranscribeAudio = base.WorkItem.Message as IUMTranscribeAudio;
			ExAssert.RetailAssert(iumtranscribeAudio != null, "TranscriptionStage must operate only on PipelineContext which implements IUMTranscribeAudio");
			if (iumtranscribeAudio.TranscriptionUser == null)
			{
				return TranscriptionEnabledSetting.Disabled;
			}
			VoiceMailTypeEnum voiceMailType = (base.WorkItem.Message is IUMCAMessage) ? VoiceMailTypeEnum.ReceivedVoiceMails : VoiceMailTypeEnum.SentVoiceMails;
			TranscriptionEnabledSetting transcriptionEnabledInMailboxConfig = iumtranscribeAudio.TranscriptionUser.IsTranscriptionEnabledInMailboxConfig(voiceMailType);
			return UMSubscriber.IsTranscriptionEnabled(iumtranscribeAudio.TranscriptionUser.UMMailboxPolicy, transcriptionEnabledInMailboxConfig);
		}

		private bool IsTranscriptionLanguageSupported(CultureInfo c)
		{
			if (Platform.Utilities.IsTranscriptionLanguageSupported(c))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "The culture '{0}' is supported for transcription", new object[]
				{
					c
				});
				return true;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "The culture '{0}' is NOT enabled for transcription", new object[]
			{
				c
			});
			return false;
		}

		private bool IsMessageTooLongForTranscription()
		{
			IUMTranscribeAudio iumtranscribeAudio = base.WorkItem.Message as IUMTranscribeAudio;
			return iumtranscribeAudio.Duration >= GlobCfg.TranscriptionMaximumMessageLength;
		}

		private bool ShouldThrottleTranscription()
		{
			IUMTranscribeAudio iumtranscribeAudio = base.WorkItem.Message as IUMTranscribeAudio;
			TimeSpan backlog = TranscriptionStage.GetBacklog();
			TimeSpan timeSpan = backlog + iumtranscribeAudio.Duration;
			int totalResourceCount = PipelineDispatcher.Instance.GetTotalResourceCount(PipelineDispatcher.PipelineResourceType.CpuBound);
			double num = GlobCfg.TranscriptionMaximumBacklogPerCore.TotalSeconds * (double)totalResourceCount;
			bool result;
			if (timeSpan.TotalSeconds > num)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "The duration of the message '{0}' exceeds the maximum transcription backlog value '{1}'.", new object[]
				{
					iumtranscribeAudio.Duration,
					num
				});
				result = true;
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "The message passes the throttle check for transcription.", new object[0]);
				result = false;
			}
			return result;
		}
	}
}
