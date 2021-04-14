using System;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class MeetingMessageTypePropertyProvider : SimpleEwsPropertyProvider
	{
		public MeetingMessageTypePropertyProvider(PropertyInformation propertyInformation) : base(propertyInformation)
		{
		}

		protected override void GetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			MeetingMessageType meetingMessageType = MeetingMessageType.None;
			string text = ewsObject[ItemSchema.ItemClass] as string;
			string a;
			if ((a = text) != null)
			{
				if (!(a == "IPM.Schedule.Meeting.Request"))
				{
					if (!(a == "IPM.Schedule.Meeting.Canceled"))
					{
						if (!(a == "IPM.Schedule.Meeting.Resp.Pos"))
						{
							if (!(a == "IPM.Schedule.Meeting.Resp.Neg"))
							{
								if (a == "IPM.Schedule.Meeting.Resp.Tent")
								{
									meetingMessageType = MeetingMessageType.MeetingTenativelyAccepted;
								}
							}
							else
							{
								meetingMessageType = MeetingMessageType.MeetingDeclined;
							}
						}
						else
						{
							meetingMessageType = MeetingMessageType.MeetingAccepted;
						}
					}
					else
					{
						meetingMessageType = MeetingMessageType.MeetingCancelled;
					}
				}
				else
				{
					meetingMessageType = MeetingMessageType.MeetingRequest;
				}
			}
			entity[property] = meetingMessageType;
		}

		public override string GetQueryConstant(object value)
		{
			if (value is MeetingMessageType)
			{
				switch ((MeetingMessageType)Enum.Parse(typeof(MeetingMessageType), value.ToString()))
				{
				case MeetingMessageType.None:
					return "IPM.Note";
				case MeetingMessageType.MeetingRequest:
					return "IPM.Schedule.Meeting.Request";
				case MeetingMessageType.MeetingCancelled:
					return "IPM.Schedule.Meeting.Canceled";
				case MeetingMessageType.MeetingAccepted:
					return "IPM.Schedule.Meeting.Resp.Pos";
				case MeetingMessageType.MeetingTenativelyAccepted:
					return "IPM.Schedule.Meeting.Resp.Tent";
				case MeetingMessageType.MeetingDeclined:
					return "IPM.Schedule.Meeting.Resp.Neg";
				}
			}
			return null;
		}
	}
}
