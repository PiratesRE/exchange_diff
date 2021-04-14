using System;
using System.Diagnostics;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.UM.PersonalAutoAttendant;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class NonBlockingCallAnsweringData : DisposableBase
	{
		internal NonBlockingCallAnsweringData(UMRecipient recipient, string callId, PhoneNumber callerId, string diversion, bool evaluatePAA)
		{
			this.callId = callId;
			this.callerId = callerId;
			this.diversion = diversion;
			this.shouldEvaluatePAA = evaluatePAA;
			this.paaEvaluationTimer = new Stopwatch();
			this.elapsedTime = new Stopwatch();
			this.reader = new NonBlockingReader(new NonBlockingReader.Operation(this.PopulateUserData), this, GlobCfg.CallAnswerMailboxDataDownloadTimeout, new NonBlockingReader.TimeoutCallback(this.TimedOutPopulatingUserData));
			this.subscriber = (recipient as UMSubscriber);
			PIIMessage data = PIIMessage.Create(PIIType._User, recipient.DisplayName);
			if (this.subscriber != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, data, "NonblockingUmSubscriberData(#{0})::ctor() user = _user WaitTimeout = {1} Threshold = {2}", new object[]
				{
					this.GetHashCode(),
					GlobCfg.CallAnswerMailboxDataDownloadTimeout,
					GlobCfg.CallAnswerMailboxDataDownloadTimeoutThreshold
				});
				this.subscriber.AddReference();
				this.reader.StartAsyncOperation();
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, data, "NonblockingUmSubscriberData(#{0})::ctor() user _User is not UMEnabled on a compatible mailbox", new object[]
			{
				this.GetHashCode()
			});
			this.greetingFile = null;
			this.outOfOffice = false;
			this.quotaExceeded = false;
			this.transcriptionEnabledInMailboxConfig = TranscriptionEnabledSetting.Disabled;
			this.reader.ForceCompletion();
		}

		public TranscriptionEnabledSetting TranscriptionEnabledInMailboxConfig
		{
			get
			{
				if (this.reader.WaitForCompletion())
				{
					return this.transcriptionEnabledInMailboxConfig;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "CallAnsweringData::Timeout checking whether transcription is enabled in mailbox config.", new object[0]);
				return TranscriptionEnabledSetting.Unknown;
			}
		}

		internal bool TimedOut
		{
			get
			{
				return this.reader.TimeOutExpired;
			}
		}

		internal bool IsQuotaExceeded
		{
			get
			{
				if (this.reader.WaitForCompletion())
				{
					return this.quotaExceeded;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "CallAnsweringData::Timeout checking the quota.", new object[0]);
				return false;
			}
		}

		internal ITempWavFile GreetingFile
		{
			get
			{
				if (this.reader.WaitForCompletion())
				{
					return this.greetingFile;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "CallAnsweringData::Timeout getting the greeting file.", new object[0]);
				return null;
			}
		}

		internal bool IsOOF
		{
			get
			{
				if (this.reader.WaitForCompletion())
				{
					return this.outOfOffice;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "CallAnsweringData::Timeout checking the OOF.", new object[0]);
				return false;
			}
		}

		internal PersonalAutoAttendant PersonalAutoAttendant
		{
			get
			{
				if (this.reader.WaitForCompletion())
				{
					return this.personalAutoAttendant;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "CallAnsweringData::PersonalAutoAttendant timed out", new object[0]);
				return null;
			}
		}

		internal TimeSpan PAAEvaluationTime
		{
			get
			{
				return this.paaEvaluationTimer.Elapsed;
			}
		}

		internal bool SubscriberHasPAAConfigured
		{
			get
			{
				return this.subscriberHasAtleastOnePAAConfigured;
			}
		}

		internal TimeSpan ElapsedTime
		{
			get
			{
				return this.elapsedTime.Elapsed;
			}
		}

		internal uint AdCount { get; set; }

		internal TimeSpan AdLatency { get; set; }

		internal uint RpcCount { get; set; }

		internal TimeSpan RpcLatency { get; set; }

		internal bool WaitForCompletion()
		{
			return this.reader.WaitForCompletion();
		}

		internal void TimedOutPopulatingUserData(object state)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_TimedOutRetrievingMailboxData, null, new object[]
			{
				this.subscriber.MailAddress,
				this.callId,
				this.subscriber.ExchangePrincipal.MailboxInfo.Location.ServerFqdn,
				this.subscriber.ExchangePrincipal.MailboxInfo.MailboxDatabase.ToString(),
				CommonUtil.ToEventLogString(new StackTrace(true))
			});
		}

		internal void PopulateUserData(object state)
		{
			LatencyDetectionContext latencyDetectionContext = null;
			this.elapsedTime.Start();
			try
			{
				using (this.subscriber.CreateConnectionGuard())
				{
					using (new CallId(this.callId))
					{
						latencyDetectionContext = PAAUtils.GetCallAnsweringDataFactory.CreateContext(CommonConstants.ApplicationVersion, CallId.Id ?? string.Empty, new IPerformanceDataProvider[]
						{
							RpcDataProvider.Instance,
							PerformanceContext.Current
						});
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "NonblockingUmSubscriberData({0})::PopulateUserData() IsPAAEnabled = {1} IsVirtualNumberEnabled = {2} shouldEvaluatePAA = {3}", new object[]
						{
							this.GetHashCode(),
							this.subscriber.IsPAAEnabled,
							this.subscriber.IsVirtualNumberEnabled,
							this.shouldEvaluatePAA
						});
						if ((this.subscriber.IsPAAEnabled || this.subscriber.IsVirtualNumberEnabled) && this.shouldEvaluatePAA)
						{
							this.EvaluatePAA(this.subscriber);
						}
						if (this.elapsedTime.Elapsed > GlobCfg.CallAnswerMailboxDataDownloadTimeoutThreshold)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "NonblockingUmSubscriberData({0})::PopulateUserData() Elapsed = {1} Threshold = {2}. CA data will not be fetched from mailbox", new object[]
							{
								this.GetHashCode(),
								this.elapsedTime.Elapsed,
								GlobCfg.CallAnswerMailboxDataDownloadTimeoutThreshold
							});
						}
						else
						{
							UMSubscriberCallAnsweringData umsubscriberCallAnsweringData = null;
							using (IUMUserMailboxStorage umuserMailboxAccessor = InterServerMailboxAccessor.GetUMUserMailboxAccessor(this.subscriber.ADUser, true))
							{
								umsubscriberCallAnsweringData = umuserMailboxAccessor.GetUMSubscriberCallAnsweringData(this.subscriber, GlobCfg.CallAnswerMailboxDataDownloadTimeoutThreshold.Subtract(this.elapsedTime.Elapsed));
							}
							this.outOfOffice = umsubscriberCallAnsweringData.IsOOF;
							this.greetingFile = umsubscriberCallAnsweringData.Greeting;
							this.transcriptionEnabledInMailboxConfig = umsubscriberCallAnsweringData.IsTranscriptionEnabledInMailboxConfig;
							this.quotaExceeded = umsubscriberCallAnsweringData.IsMailboxQuotaExceeded;
							if (umsubscriberCallAnsweringData.TaskTimedOut)
							{
								CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "NonblockingUmSubscriberData({0})::PopulateUserData() Elapsed = {1} Threshold = {2}. Quota check is not done", new object[]
								{
									this.GetHashCode(),
									this.elapsedTime.Elapsed,
									GlobCfg.CallAnswerMailboxDataDownloadTimeoutThreshold
								});
							}
						}
					}
				}
			}
			finally
			{
				TaskPerformanceData[] array = latencyDetectionContext.StopAndFinalizeCollection();
				TaskPerformanceData taskPerformanceData = array[0];
				PerformanceData end = taskPerformanceData.End;
				if (end != PerformanceData.Zero)
				{
					PerformanceData difference = taskPerformanceData.Difference;
					this.RpcCount = difference.Count;
					this.RpcLatency = difference.Latency;
				}
				TaskPerformanceData taskPerformanceData2 = array[1];
				PerformanceData end2 = taskPerformanceData2.End;
				if (end2 != PerformanceData.Zero)
				{
					PerformanceData difference2 = taskPerformanceData2.Difference;
					this.AdCount = difference2.Count;
					this.AdLatency = difference2.Latency;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "NonblockingUmSubscriberData({0})::PopulateUserData()) RPCRequests = {1} RPCLatency = {2} ADRequests = {3}, ADLatency = {4}", new object[]
				{
					this.GetHashCode(),
					this.RpcCount,
					this.RpcLatency.TotalMilliseconds,
					this.AdCount,
					this.AdLatency.TotalMilliseconds
				});
				this.elapsedTime.Stop();
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "NonblockingUmSubscriberData({0})::PopulateUserData()) Evaluation Elapsed Time = {1}", new object[]
				{
					this.GetHashCode(),
					this.elapsedTime.Elapsed
				});
			}
		}

		private void EvaluatePAA(UMSubscriber subscriber)
		{
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, subscriber.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, data, "NonblockingUmSubscriberData({0})::PopulateUserData() user = _UserDisplayName", new object[]
			{
				this.GetHashCode()
			});
			bool flag = false;
			this.paaEvaluationTimer.Start();
			try
			{
				Util.IncrementCounter(AvailabilityCounters.PercentageCARDownloadFailures_Base, 1L);
				using (IPAAEvaluator ipaaevaluator = EvaluateUserPAA.CreateSynchronous(subscriber, this.callerId, this.diversion))
				{
					flag = ipaaevaluator.GetEffectivePAA(out this.personalAutoAttendant);
					this.subscriberHasAtleastOnePAAConfigured = ipaaevaluator.SubscriberHasPAAConfigured;
				}
			}
			catch (CorruptedPAAStoreException)
			{
				Util.IncrementCounter(AvailabilityCounters.PercentageCARDownloadFailures, 1L);
			}
			catch (LocalizedException e)
			{
				XsoUtil.LogMailboxConnectionFailureException(e, subscriber.ExchangePrincipal);
				if (!XsoUtil.IsMailboxConnectionFailureException(e))
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FailedToRetrieveMailboxData, null, new object[]
					{
						CallId.Id,
						subscriber.MailAddress,
						CommonUtil.ToEventLogString(Utils.ConcatenateMessagesOnException(e))
					});
				}
				Util.IncrementCounter(AvailabilityCounters.PercentageCARDownloadFailures, 1L);
			}
			catch (XmlException)
			{
				Util.IncrementCounter(AvailabilityCounters.PercentageCARDownloadFailures, 1L);
			}
			this.paaEvaluationTimer.Stop();
			if (flag)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "NonblockingUmSubscriberData({0})::PopulateUserData() Got PAA ID={1} Enabled={2} Version={3} Valid={4}", new object[]
				{
					this.GetHashCode(),
					this.personalAutoAttendant.Identity.ToString(),
					this.personalAutoAttendant.Enabled,
					this.personalAutoAttendant.Version.ToString(),
					this.personalAutoAttendant.Valid
				});
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "NonblockingUmSubscriberData({0})::PopulateUserData() PAA Evaluation Time = {1}", new object[]
				{
					this.GetHashCode(),
					this.paaEvaluationTimer.Elapsed
				});
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "NonblockingUmSubscriberData({0})::PopulateUserData() Did not get a valid PAA", new object[]
			{
				this.GetHashCode()
			});
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.reader != null)
				{
					this.reader.Dispose();
					this.reader = null;
				}
				if (this.subscriber != null)
				{
					this.subscriber.ReleaseReference();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NonBlockingCallAnsweringData>(this);
		}

		private UMSubscriber subscriber;

		private ITempWavFile greetingFile;

		private bool quotaExceeded;

		private bool outOfOffice;

		private TranscriptionEnabledSetting transcriptionEnabledInMailboxConfig;

		private NonBlockingReader reader;

		private string callId;

		private PersonalAutoAttendant personalAutoAttendant;

		private PhoneNumber callerId;

		private string diversion;

		private bool subscriberHasAtleastOnePAAConfigured;

		private bool shouldEvaluatePAA;

		private Stopwatch paaEvaluationTimer;

		private Stopwatch elapsedTime;
	}
}
