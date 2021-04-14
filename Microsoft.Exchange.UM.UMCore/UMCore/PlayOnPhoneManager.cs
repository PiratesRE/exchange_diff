using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PlayOnPhoneManager : ActivityManager
	{
		internal PlayOnPhoneManager(ActivityManager manager, PlayOnPhoneManager.ConfigClass config) : base(manager, config)
		{
		}

		internal bool ProtectedMessage
		{
			get
			{
				return this.protectedMessage;
			}
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
			if (!this.isAutoAttendantCall)
			{
				base.CheckAuthorization(u);
			}
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			vo.CurrentCallContext.WebServiceRequest.EndResult = UMOperationResult.InProgress;
			this.isAutoAttendantCall = (vo.CurrentCallContext.WebServiceRequest is PlayOnPhoneAAGreetingRequest);
			base.Start(vo, refInfo);
		}

		internal override void DropCall(BaseUMCallSession vo, DropCallReason reason)
		{
			if (reason != DropCallReason.GracefulHangup)
			{
				this.SetOperationResultFailed(vo);
			}
			base.DropCall(vo, reason);
		}

		internal string SetOperationResultFailed(BaseUMCallSession vo)
		{
			vo.CurrentCallContext.WebServiceRequest.EndResult = UMOperationResult.Failure;
			return null;
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneManager asked to do action {0}.", new object[]
			{
				action
			});
			string input = null;
			CallContext currentCallContext = vo.CurrentCallContext;
			if (string.Equals(action, "getPlayOnPhoneType", StringComparison.OrdinalIgnoreCase))
			{
				if (currentCallContext.WebServiceRequest is PlayOnPhoneAAGreetingRequest)
				{
					input = "playOnPhoneAAGreeting";
				}
				else
				{
					if (currentCallContext.WebServiceRequest is PlayOnPhoneMessageRequest)
					{
						PlayOnPhoneMessageRequest request = (PlayOnPhoneMessageRequest)currentCallContext.WebServiceRequest;
						try
						{
							this.PrepareMessagePlayer(request);
							this.CheckIfProtectedMessage(vo);
							if (currentCallContext.OCFeature.FeatureType == OCFeatureType.SingleVoicemail)
							{
								this.MarkMessageAsRead(vo);
							}
							input = "playOnPhoneVoicemail";
							goto IL_353;
						}
						catch (ArgumentException ex)
						{
							CallIdTracer.TraceWarning(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneManager::ExecuteAction(getPlayOnPhoneType)", new object[]
							{
								ex.ToString()
							});
							input = "xsoError";
							goto IL_353;
						}
						catch (CorruptDataException ex2)
						{
							CallIdTracer.TraceWarning(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneManager::ExecuteAction(getPlayOnPhoneType)", new object[]
							{
								ex2.ToString()
							});
							input = "xsoError";
							goto IL_353;
						}
					}
					if (currentCallContext.WebServiceRequest is PlayOnPhoneGreetingRequest)
					{
						PlayOnPhoneGreetingRequest playOnPhoneGreetingRequest = (PlayOnPhoneGreetingRequest)currentCallContext.WebServiceRequest;
						base.WriteVariable("greetingType", playOnPhoneGreetingRequest.GreetingType.ToString());
						base.WriteVariable("OofCustom", UMGreetingType.OofCustom.ToString());
						base.WriteVariable("NormalCustom", UMGreetingType.NormalCustom.ToString());
						using (GreetingBase greetingBase = (playOnPhoneGreetingRequest.GreetingType == UMGreetingType.NormalCustom) ? currentCallContext.CallerInfo.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Voicemail) : currentCallContext.CallerInfo.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Away))
						{
							this.FetchGreeting(greetingBase);
						}
						input = "playOnPhoneGreeting";
					}
				}
			}
			else
			{
				if (string.Equals(action, "getExternal", StringComparison.OrdinalIgnoreCase))
				{
					using (GreetingBase greetingBase2 = currentCallContext.CallerInfo.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Voicemail))
					{
						this.FetchGreeting(greetingBase2);
						goto IL_353;
					}
				}
				if (string.Equals(action, "saveExternal", StringComparison.OrdinalIgnoreCase))
				{
					using (GreetingBase greetingBase3 = currentCallContext.CallerInfo.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Voicemail))
					{
						this.SaveGreeting(greetingBase3);
						goto IL_353;
					}
				}
				if (string.Equals(action, "deleteExternal", StringComparison.OrdinalIgnoreCase))
				{
					using (GreetingBase greetingBase4 = currentCallContext.CallerInfo.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Voicemail))
					{
						greetingBase4.Delete();
						goto IL_353;
					}
				}
				if (string.Equals(action, "getOof", StringComparison.OrdinalIgnoreCase))
				{
					using (GreetingBase greetingBase5 = currentCallContext.CallerInfo.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Away))
					{
						this.FetchGreeting(greetingBase5);
						goto IL_353;
					}
				}
				if (string.Equals(action, "saveOof", StringComparison.OrdinalIgnoreCase))
				{
					using (GreetingBase greetingBase6 = currentCallContext.CallerInfo.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Away))
					{
						this.SaveGreeting(greetingBase6);
						goto IL_353;
					}
				}
				if (string.Equals(action, "deleteOof", StringComparison.OrdinalIgnoreCase))
				{
					using (GreetingBase greetingBase7 = currentCallContext.CallerInfo.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Away))
					{
						greetingBase7.Delete();
						goto IL_353;
					}
				}
				if (!string.Equals(action, "resetCallType", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				vo.CurrentCallContext.CallType = 1;
				vo.CurrentCallContext.OCFeature.FeatureType = OCFeatureType.None;
				this.GlobalManager.WriteVariable("ocFeature", null);
			}
			IL_353:
			return base.CurrentActivity.GetTransition(input);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PlayOnPhoneManager>(this);
		}

		private void PrepareMessagePlayer(PlayOnPhoneMessageRequest request)
		{
			byte[] entryId = Convert.FromBase64String(request.ObjectId);
			StoreObjectId id = StoreObjectId.FromProviderSpecificId(entryId);
			base.MessagePlayerContext.Reset(id);
		}

		private void CheckIfProtectedMessage(BaseUMCallSession vo)
		{
			try
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneManager attempting to CheckIfProtectedMessage {0}", new object[]
				{
					base.MessagePlayerContext.Id
				});
				UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = callerInfo.CreateSessionLock())
				{
					using (MessageItem messageItem = MessageItem.Bind(mailboxSessionLock.Session, base.MessagePlayerContext.Id))
					{
						this.protectedMessage = messageItem.IsRestricted;
					}
				}
			}
			catch (ObjectNotFoundException ex)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneManager.CheckIfProtectedMessage: {0}", new object[]
				{
					ex
				});
			}
		}

		private void MarkMessageAsRead(BaseUMCallSession vo)
		{
			try
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneManager attempting to mark msg {0} as read.", new object[]
				{
					base.MessagePlayerContext.Id
				});
				UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = callerInfo.CreateSessionLock())
				{
					using (MessageItem messageItem = MessageItem.Bind(mailboxSessionLock.Session, base.MessagePlayerContext.Id))
					{
						messageItem.OpenAsReadWrite();
						messageItem.Load(new PropertyDefinition[]
						{
							MessageItemSchema.IsRead
						});
						if (!messageItem.IsRead)
						{
							messageItem.IsRead = true;
							messageItem.Save(SaveMode.ResolveConflicts);
						}
					}
				}
			}
			catch (ObjectNotFoundException ex)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneManager.MarkMessageAsRead: {0}", new object[]
				{
					ex
				});
			}
		}

		private bool FetchGreeting(GreetingBase g)
		{
			base.WriteVariable("greeting", null);
			base.RecordContext.Reset();
			ITempWavFile tempWavFile = g.Get();
			if (tempWavFile == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "No greeting found for {0}.  Returning false.", new object[]
				{
					g.GetType()
				});
				return false;
			}
			base.WriteVariable("greeting", tempWavFile);
			return true;
		}

		private void SaveGreeting(GreetingBase g)
		{
			ITempWavFile recording = base.RecordContext.Recording;
			base.RecordContext.Reset();
			if (recording != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Saving greeting at file={0} to store.", new object[]
				{
					recording.FilePath
				});
				g.Put(recording.FilePath);
			}
		}

		private bool protectedMessage;

		private bool isAutoAttendantCall;

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing PlayOnPhone activity manager.", new object[0]);
				return new PlayOnPhoneManager(manager, this);
			}
		}
	}
}
