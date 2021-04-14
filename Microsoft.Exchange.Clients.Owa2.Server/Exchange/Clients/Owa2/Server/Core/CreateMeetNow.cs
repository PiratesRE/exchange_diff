using System;
using System.Linq;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.OnlineMeetings;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CreateMeetNow : CreateMeetingBase
	{
		public CreateMeetNow(CallContext callContext, string sipUri, string subject, bool isPrivate) : base(callContext, sipUri, isPrivate)
		{
			WcfServiceCommandBase.ThrowIfNull("callContext", "sipUri", "CreateMeetNow");
			if (string.IsNullOrEmpty(sipUri))
			{
				throw new OwaInvalidRequestException("No sipUri specified");
			}
			this.subject = subject;
			OwsLogRegistry.Register(CreateMeetNow.CreateMeetNowActionName, typeof(CreateOnlineMeetingMetadata), new Type[0]);
		}

		protected override OnlineMeetingType ProcessOnlineMeetingResult(UserContext userContext, OnlineMeetingResult result)
		{
			return new OnlineMeetingType
			{
				WebUrl = result.OnlineMeeting.WebUrl
			};
		}

		protected override void SetDefaultValuesForOptics()
		{
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.IsTaskCompleted, bool.FalseString);
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.IsUcwaSupported, bool.TrueString);
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.ItemId, this.subject);
		}

		protected override bool ShoudlMeetingBeCreated()
		{
			return true;
		}

		protected override OnlineMeetingSettings ConstructOnlineMeetingSettings()
		{
			OnlineMeetingSettings onlineMeetingSettings = new OnlineMeetingSettings();
			if (this.subject != null)
			{
				onlineMeetingSettings.Subject = this.subject;
			}
			return onlineMeetingSettings;
		}

		protected override void UpdateOpticsLog(OnlineMeetingResult createMeeting)
		{
			OnlineMeeting onlineMeeting = createMeeting.OnlineMeeting;
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.UserGuid, ExtensibleLogger.FormatPIIValue(this.sipUri));
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.ConferenceId, onlineMeeting.PstnMeetingId);
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.LeaderCount, onlineMeeting.Leaders.Count<string>());
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.AttendeeCount, onlineMeeting.Attendees.Count<string>());
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.ExpiryTime, onlineMeeting.ExpiryTime);
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.AutomaticLeaderAssignment, onlineMeeting.AutomaticLeaderAssignment.ToString());
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.AccessLevel, onlineMeeting.Accesslevel.ToString());
		}

		private static readonly string CreateMeetNowActionName = typeof(CreateMeetNow).Name;

		private readonly string subject;
	}
}
