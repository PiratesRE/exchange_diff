using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SearchFolderVerificationStage : SynchronousPipelineStageBase, IUMNetworkResource
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

		protected override void InternalDoSynchronousWork()
		{
			try
			{
				IUMCAMessage iumcamessage = base.WorkItem.Message as IUMCAMessage;
				ExAssert.RetailAssert(iumcamessage != null, "SearchFolderVerificationStage must operate only on PipelineContext which implements IUMCAMessage");
				UMSubscriber umsubscriber = iumcamessage.CAMessageRecipient as UMSubscriber;
				if (umsubscriber != null)
				{
					using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = umsubscriber.CreateSessionLock())
					{
						using (UMSearchFolder umsearchFolder = UMSearchFolder.Get(mailboxSessionLock.Session, UMSearchFolder.Type.VoiceMail))
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Invoking Get SearchFolder Criteria= {0}", new object[]
							{
								umsearchFolder.SearchFolder.GetSearchCriteria().ToString()
							});
						}
						using (UMSearchFolder umsearchFolder2 = UMSearchFolder.Get(mailboxSessionLock.Session, UMSearchFolder.Type.Fax))
						{
							if (umsearchFolder2.SearchFolderId != null)
							{
								CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Invoking Get SearchFolder Criteria= {0}", new object[]
								{
									umsearchFolder2.SearchFolder.GetSearchCriteria().ToString()
								});
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "SearchFolderVerificationStage exception = {0}", new object[]
				{
					ex
				});
				throw;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SearchFolderVerificationStage>(this);
		}
	}
}
