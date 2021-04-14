using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SendMessageManager : ActivityManager
	{
		internal SendMessageManager(ActivityManager manager, ActivityManagerConfig config) : base(manager, config)
		{
			this.Recipients = new Stack<Participant>();
			base.WriteVariable("numRecipients", this.Recipients.Count);
			base.IsSentImportant = false;
			base.MessageMarkedPrivate = false;
		}

		protected Stack<Participant> Recipients
		{
			get
			{
				return this.recipients;
			}
			set
			{
				this.recipients = value;
			}
		}

		protected IRecordedMessage SendMsg
		{
			get
			{
				return this.sendMsg;
			}
			set
			{
				this.sendMsg = value;
			}
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SendMessageManager asked to do action: {0}.", new object[]
			{
				action
			});
			string input = null;
			UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
			if (string.Equals(action, "addRecipientBySearch", StringComparison.OrdinalIgnoreCase))
			{
				input = this.AddRecipientBySearch(vo);
			}
			else if (string.Equals(action, "removeRecipient", StringComparison.OrdinalIgnoreCase))
			{
				input = this.RemoveRecipient();
			}
			else if (string.Equals(action, "cancelMessage", StringComparison.OrdinalIgnoreCase))
			{
				this.CleanupMessage();
			}
			else if (string.Equals(action, "sendMessage", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SendMessage(vo);
			}
			else
			{
				if (!string.Equals(action, "sendMessageUrgent", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				base.IsSentImportant = true;
				input = this.SendMessage(vo);
			}
			return base.CurrentActivity.GetTransition(input);
		}

		internal string TogglePrivacy(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "TogglePrivacy", new object[0]);
			base.MessageMarkedPrivate = !base.MessageMarkedPrivate;
			return null;
		}

		internal string ToggleImportance(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "ToggleImportance", new object[0]);
			base.IsSentImportant = !base.IsSentImportant;
			return null;
		}

		internal string ClearSelection(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "ClearSelection", new object[0]);
			base.IsSentImportant = false;
			base.MessageMarkedPrivate = false;
			return null;
		}

		internal string SendMessagePrivate(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "SendMessagePrivate", new object[0]);
			base.MessageMarkedPrivate = true;
			return this.SendMessage(vo);
		}

		internal string SendMessagePrivateAndUrgent(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "SendMessagePrivateAndUrgent", new object[0]);
			base.MessageMarkedPrivate = true;
			base.IsSentImportant = true;
			return this.SendMessage(vo);
		}

		protected virtual string SendMessage(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SendMessageMananger::SendMessage. markPrivate={0}, imp={1}", new object[]
			{
				base.MessageMarkedPrivate,
				base.IsSentImportant
			});
			this.SendMsg.DoSubmit(base.IsSentImportant ? Importance.High : Importance.Normal, base.MessageMarkedPrivate, this.Recipients);
			base.MessageHasBeenSentWithHighImportance = base.IsSentImportant;
			this.CleanupMessage();
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SendMessageManager>(this);
		}

		protected void CleanupMessage()
		{
			base.RecordContext.Reset();
			this.Recipients.Clear();
			base.WriteVariable("numRecipients", this.Recipients.Count);
			this.SendMsg = null;
			this.ClearSelection(null);
		}

		private string AddRecipientBySearch(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SendMessageManager::AddRecipientBySearch.", new object[0]);
			string result = null;
			ContactSearchItem contactSearchItem = (ContactSearchItem)this.ReadVariable("directorySearchResult");
			Participant emailParticipant = contactSearchItem.EmailParticipant;
			if (null == emailParticipant)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SendMessageManager::AddRecipientBySearch did not find a valid recipient.", new object[0]);
				result = "unknownRecipient";
			}
			else
			{
				this.Recipients.Push(emailParticipant);
				if (contactSearchItem.Recipient != null)
				{
					base.SetRecordedName("userName", contactSearchItem.Recipient);
				}
				else
				{
					base.SetTextPartVariable("userName", contactSearchItem.FullName ?? contactSearchItem.PrimarySmtpAddress);
				}
			}
			base.WriteVariable("numRecipients", this.Recipients.Count);
			return result;
		}

		private string RemoveRecipient()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SendMessageMananger::RemoveRecipient.", new object[0]);
			string result = null;
			if (this.Recipients.Count != 0)
			{
				Participant participant = this.Recipients.Pop();
				PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, participant.EmailAddress);
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "Removed recipient p=_EmailAddress.", new object[0]);
			}
			if (this.Recipients.Count == 0)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Removed the final recipient.  Returning noRecipients event.", new object[0]);
				result = "noRecipients";
			}
			base.WriteVariable("numRecipients", this.Recipients.Count);
			return result;
		}

		private Stack<Participant> recipients;

		private IRecordedMessage sendMsg;
	}
}
