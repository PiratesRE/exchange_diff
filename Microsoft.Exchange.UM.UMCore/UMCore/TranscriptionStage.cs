using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.Prompts.Config;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TranscriptionStage : AsynchronousPipelineStageBase
	{
		internal override PipelineDispatcher.PipelineResourceType ResourceType
		{
			get
			{
				return PipelineDispatcher.PipelineResourceType.LowPriorityCpuBound;
			}
		}

		internal override TimeSpan ExpectedRunTime
		{
			get
			{
				return TimeSpan.FromMinutes(3.0);
			}
		}

		internal static TimeSpan GetBacklog()
		{
			TimeSpan result;
			lock (TranscriptionStage.staticLock)
			{
				result = TranscriptionStage.transcriptionBacklog;
			}
			return result;
		}

		internal static void UpdateBacklog(TimeSpan amount)
		{
			lock (TranscriptionStage.staticLock)
			{
				TranscriptionStage.transcriptionBacklog = TranscriptionStage.transcriptionBacklog.Add(amount);
			}
		}

		internal override bool ShouldRunStage(PipelineWorkItem workItem)
		{
			return base.WorkItem.TranscriptionContext.ShouldRunTranscriptionStage;
		}

		protected override StageRetryDetails InternalGetRetrySchedule()
		{
			return new StageRetryDetails(StageRetryDetails.FinalAction.SkipStage);
		}

		protected override void InternalStartAsynchronousWork()
		{
			IUMTranscribeAudio iumtranscribeAudio = base.WorkItem.Message as IUMTranscribeAudio;
			ExAssert.RetailAssert(iumtranscribeAudio != null, "TranscriptionStage must operate only on PipelineContext which implements IUMTranscribeAudio");
			TranscriptionStage.UpdateBacklog(-base.WorkItem.TranscriptionContext.BacklogContribution);
			base.WorkItem.TranscriptionContext.BacklogContribution = TimeSpan.Zero;
			iumtranscribeAudio.TranscriptionData = new TranscriptionData(RecoResultType.Skipped, RecoErrorType.Other, base.WorkItem.TranscriptionContext.Culture, new List<IUMTranscriptionResult>());
			if (!Platform.Builder.TryCreateOfflineTranscriber(base.WorkItem.TranscriptionContext.Culture, out this.transcriber))
			{
				throw new InvalidOperationException("Transcriber not supported, but transcription configuration stage indicated it was supported");
			}
			this.transcriber.TranscribeCompleted += this.OnTranscribeCompleted;
			IUMResolveCaller iumresolveCaller = base.WorkItem.Message as IUMResolveCaller;
			if (iumresolveCaller != null)
			{
				this.transcriber.CallerInfo = iumresolveCaller.ContactInfo;
			}
			this.transcriber.TranscriptionUser = iumtranscribeAudio.TranscriptionUser;
			this.transcriber.CallingLineId = base.WorkItem.Message.CallerId.Number;
			ExAssert.RetailAssert(iumtranscribeAudio.EnableTopNGrammar || null == this.transcriber.TopN, "TopN data should only exist if the topN grammar is enabled.");
			this.transcriber.TopN = base.WorkItem.TranscriptionContext.TopN;
			this.audioFile = this.PrepareFileForTranscription(iumtranscribeAudio.AttachmentPath);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Start transcribing file: {0}. Time allowed  for transcription: {1}", new object[]
			{
				this.audioFile.FilePath,
				GlobCfg.MessageTranscriptionTimeout
			});
			this.transcriber.TranscribeFile(this.audioFile.FilePath);
			this.startTime = ExDateTime.UtcNow;
			TranscriptionCountersInstance instance = TranscriptionCounters.GetInstance(base.WorkItem.TranscriptionContext.Culture.Name);
			Util.IncrementCounter(instance.CurrentSessions);
			Util.IncrementCounter(AvailabilityCounters.PercentageTranscriptionFailures_Base);
			PipelineDispatcher.Instance.OnShutdown += this.OnShutdown;
			this.transcriptionTimer = new Timer(new TimerCallback(this.OnTimeout), null, GlobCfg.MessageTranscriptionTimeout, TimeSpan.Zero);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TranscriptionStage>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering TranscriptionStage.Dispose", new object[0]);
				PipelineDispatcher.Instance.OnShutdown -= this.OnShutdown;
				if (this.transcriber != null)
				{
					this.transcriber.TranscribeCompleted -= this.OnTranscribeCompleted;
					this.transcriber.Dispose();
					this.transcriber = null;
				}
				if (this.transcriptionTimer != null)
				{
					this.transcriptionTimer.Dispose();
					this.transcriptionTimer = null;
				}
				if (this.audioFile != null)
				{
					this.audioFile.Dispose();
					this.audioFile = null;
				}
			}
			base.InternalDispose(disposing);
		}

		private void OnShutdown(object sender, EventArgs e)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering TranscriptionStage.OnShutdown", new object[0]);
			this.CancelTranscription();
		}

		private void OnTimeout(object state)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering TranscriptionStage.OnTimeout", new object[0]);
			this.CancelTranscription();
		}

		private void CancelTranscription()
		{
			try
			{
				lock (this)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering TranscriptionStage.CancelTranscription", new object[0]);
					if (!this.completed && !this.cancelled)
					{
						this.transcriber.CancelTranscription();
						this.cancelled = true;
					}
				}
			}
			catch (InvalidOperationException)
			{
				BaseUMOfflineTranscriber.TranscribeCompletedEventArgs tcea = new BaseUMOfflineTranscriber.TranscribeCompletedEventArgs(new List<IUMTranscriptionResult>(), null, true, null);
				this.OnTranscribeCompleted(this, tcea);
			}
		}

		private void OnTranscribeCompleted(object sender, BaseUMOfflineTranscriber.TranscribeCompletedEventArgs tcea)
		{
			Exception ex = null;
			try
			{
				IUMTranscribeAudio iumtranscribeAudio = base.WorkItem.Message as IUMTranscribeAudio;
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering TranscriptionStage.OnTranscribeCompleted", new object[0]);
				lock (this)
				{
					if (this.completed)
					{
						return;
					}
					this.completed = true;
				}
				TranscriptionCountersInstance instance = TranscriptionCounters.GetInstance(base.WorkItem.TranscriptionContext.Culture.Name);
				Util.DecrementCounter(instance.CurrentSessions);
				if (tcea.Error != null)
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "TranscribeCompleted completed with an error: {0}. Deliver the voicemail with no transcription.", new object[]
					{
						tcea.Error
					});
					this.recoResultType = RecoResultType.Skipped;
					this.recoErrorType = RecoErrorType.Other;
				}
				else
				{
					this.transcriptionResults.AddRange(tcea.TranscriptionResults);
					if (tcea.Cancelled)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Transcription was cancelled", new object[0]);
						this.recoResultType = RecoResultType.Partial;
						this.recoErrorType = RecoErrorType.Success;
					}
					else
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Transcription completed successfully", new object[0]);
						this.recoResultType = RecoResultType.Attempted;
						this.recoErrorType = RecoErrorType.Success;
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "StageComplete(error='{0}')", new object[]
				{
					ex
				});
				TranscriptionData transcriptionData = new TranscriptionData(this.recoResultType, this.recoErrorType, base.WorkItem.TranscriptionContext.Culture, this.transcriptionResults);
				if (base.WorkItem.TranscriptionContext.TopN != null)
				{
					base.WorkItem.TranscriptionContext.TopN.TryCache();
				}
				transcriptionData.UpdatePerfCounters();
				transcriptionData.UpdateStatistics(base.WorkItem.PipelineStatisticsLogRow);
				base.WorkItem.PipelineStatisticsLogRow.TranscriptionElapsedTime = ExDateTime.UtcNow - this.startTime;
				iumtranscribeAudio.TranscriptionData = transcriptionData;
			}
			catch (Exception ex2)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Error happened in OnTranscribeCompleted e='{0}'", new object[]
				{
					ex2
				});
				ex = ex2;
			}
			finally
			{
				base.StageCompletionCallback(this, base.WorkItem, ex);
			}
		}

		private ITempWavFile PrepareFileForTranscription(string attachmentPath)
		{
			CultureInfo culture = base.WorkItem.TranscriptionContext.Culture;
			double noiseFloorLevelDB = SafeConvert.ToDouble(Strings.TranscriptionNoiseFloorLevelDB.ToString(culture), -100.0, 0.0, -78.0);
			double normalizationLevelDB = SafeConvert.ToDouble(Strings.TranscriptionNormalizationLevelDB.ToString(culture), -25.0, 0.0, -10.0);
			return MediaMethods.NormalizeAudio(attachmentPath, noiseFloorLevelDB, normalizationLevelDB);
		}

		private static TimeSpan lazyTranscriptionTimeout = TimeSpan.MinValue;

		private static TimeSpan transcriptionBacklog = new TimeSpan(0, 0, 0);

		private static object staticLock = new object();

		private BaseUMOfflineTranscriber transcriber;

		private List<IUMTranscriptionResult> transcriptionResults = new List<IUMTranscriptionResult>();

		private RecoResultType recoResultType = RecoResultType.Skipped;

		private RecoErrorType recoErrorType = RecoErrorType.Other;

		private Timer transcriptionTimer;

		private bool cancelled;

		private bool completed;

		private ExDateTime startTime;

		private ITempWavFile audioFile;
	}
}
