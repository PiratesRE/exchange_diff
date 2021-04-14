using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MeetingRequestTypeProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private MeetingRequestTypeProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static MeetingRequestTypeProperty CreateCommand(CommandContext commandContext)
		{
			return new MeetingRequestTypeProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("MeetingRequestTypeProperty.ToXml should not be called");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			MeetingRequest meetingRequest = (MeetingRequest)commandSettings.StoreObject;
			string value = MeetingRequestTypeProperty.MeetingMessageTypeToString(meetingRequest.MeetingRequestType);
			if (!string.IsNullOrEmpty(value))
			{
				serviceObject[this.commandContext.PropertyInformation] = value;
			}
		}

		private static string MeetingMessageTypeToString(MeetingMessageType meetingMessageType)
		{
			if (meetingMessageType <= MeetingMessageType.FullUpdate)
			{
				if (meetingMessageType == MeetingMessageType.NewMeetingRequest)
				{
					return "NewMeetingRequest";
				}
				if (meetingMessageType == MeetingMessageType.FullUpdate)
				{
					return "FullUpdate";
				}
			}
			else
			{
				if (meetingMessageType == MeetingMessageType.InformationalUpdate)
				{
					return "InformationalUpdate";
				}
				if (meetingMessageType == MeetingMessageType.SilentUpdate)
				{
					return "SilentUpdate";
				}
				if (meetingMessageType == MeetingMessageType.Outdated)
				{
					return "Outdated";
				}
			}
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
			{
				if (meetingMessageType == MeetingMessageType.None)
				{
					return "None";
				}
				if (meetingMessageType == MeetingMessageType.PrincipalWantsCopy)
				{
					return "PrincipalWantsCopy";
				}
			}
			ExTraceGlobals.CalendarDataTracer.TraceDebug<MeetingMessageType, ExchangeVersion>(0L, "Could not emit meeting request type value for value {0}, request schema version {1}", meetingMessageType, ExchangeVersion.Current);
			return null;
		}

		private const string FullUpdateValue = "FullUpdate";

		private const string InformationalUpdateValue = "InformationalUpdate";

		private const string NewMeetingRequestValue = "NewMeetingRequest";

		private const string NoneValue = "None";

		private const string OutdatedValue = "Outdated";

		private const string PrincipalWantsCopy = "PrincipalWantsCopy";

		private const string SilentUpdateValue = "SilentUpdate";
	}
}
