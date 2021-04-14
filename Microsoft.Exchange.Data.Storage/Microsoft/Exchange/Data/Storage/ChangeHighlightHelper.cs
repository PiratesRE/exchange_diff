using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ChangeHighlightHelper
	{
		private static Dictionary<StorePropertyDefinition, ChangeHighlightProperties> BuildPropertyGroupings(Dictionary<StorePropertyDefinition, ChangeHighlightProperties> propertyGroupings)
		{
			return new Dictionary<StorePropertyDefinition, ChangeHighlightProperties>(30)
			{
				{
					InternalSchema.MapiStartTime,
					ChangeHighlightProperties.MapiStartTime
				},
				{
					InternalSchema.MapiEndTime,
					ChangeHighlightProperties.MapiEndTime
				},
				{
					InternalSchema.Duration,
					ChangeHighlightProperties.Duration
				},
				{
					InternalSchema.AppointmentRecurring,
					ChangeHighlightProperties.RecurrenceProps
				},
				{
					InternalSchema.AppointmentRecurrenceBlob,
					ChangeHighlightProperties.RecurrenceProps
				},
				{
					InternalSchema.MapiRecurrenceType,
					ChangeHighlightProperties.RecurrenceProps
				},
				{
					InternalSchema.RecurrencePattern,
					ChangeHighlightProperties.RecurrenceProps
				},
				{
					InternalSchema.Location,
					ChangeHighlightProperties.Location
				},
				{
					InternalSchema.SubjectPrefixInternal,
					ChangeHighlightProperties.Subject
				},
				{
					InternalSchema.NormalizedSubjectInternal,
					ChangeHighlightProperties.Subject
				},
				{
					InternalSchema.TextBody,
					ChangeHighlightProperties.BodyProps
				},
				{
					InternalSchema.HtmlBody,
					ChangeHighlightProperties.BodyProps
				},
				{
					InternalSchema.RtfBody,
					ChangeHighlightProperties.BodyProps
				},
				{
					InternalSchema.RtfInSync,
					ChangeHighlightProperties.BodyProps
				},
				{
					InternalSchema.MeetingWorkspaceUrl,
					ChangeHighlightProperties.BodyProps
				},
				{
					InternalSchema.BillingInformation,
					ChangeHighlightProperties.BillMilesCompany
				},
				{
					InternalSchema.Mileage,
					ChangeHighlightProperties.BillMilesCompany
				},
				{
					InternalSchema.Companies,
					ChangeHighlightProperties.BillMilesCompany
				},
				{
					InternalSchema.MapiInternetCpid,
					ChangeHighlightProperties.BillMilesCompany
				},
				{
					InternalSchema.IsSilent,
					ChangeHighlightProperties.IsSilent
				},
				{
					InternalSchema.DisallowNewTimeProposal,
					ChangeHighlightProperties.DisallowNewTimeProposal
				},
				{
					InternalSchema.IsOnlineMeeting,
					ChangeHighlightProperties.NetMeetingProps
				},
				{
					InternalSchema.OnlineMeetingChanged,
					ChangeHighlightProperties.NetMeetingProps
				},
				{
					InternalSchema.ConferenceType,
					ChangeHighlightProperties.NetMeetingProps
				},
				{
					InternalSchema.NetShowURL,
					ChangeHighlightProperties.NetShowProps
				},
				{
					InternalSchema.NetMeetingServer,
					ChangeHighlightProperties.NetShowProps
				}
			};
		}

		public ChangeHighlightHelper(int highlight, bool isUpdate)
		{
			this.changeHighlight = highlight;
			this.IsUpdate = isUpdate;
		}

		public ChangeHighlightHelper(int highlight) : this(highlight, false)
		{
		}

		public MeetingMessageType SuggestedMeetingType
		{
			get
			{
				if (this.changeHighlight == 0 && !this.IsUpdate)
				{
					return MeetingMessageType.NewMeetingRequest;
				}
				if ((this.changeHighlight & 7) != 0)
				{
					return MeetingMessageType.FullUpdate;
				}
				return MeetingMessageType.InformationalUpdate;
			}
		}

		private bool IsUpdate { get; set; }

		public int HighlightFlags
		{
			get
			{
				return this.changeHighlight;
			}
		}

		public bool this[StorePropertyDefinition def]
		{
			get
			{
				return ChangeHighlightHelper.propertyGroupings.ContainsKey(def) && 0 != (this.changeHighlight & (int)ChangeHighlightHelper.propertyGroupings[def]);
			}
			set
			{
				if (!ChangeHighlightHelper.propertyGroupings.ContainsKey(def))
				{
					this.changeHighlight |= 134217728;
					return;
				}
				if (value)
				{
					this.changeHighlight |= (int)ChangeHighlightHelper.propertyGroupings[def];
					return;
				}
				if ((this.changeHighlight & 3) != 0 && ChangeHighlightHelper.propertyGroupings[def] == ChangeHighlightProperties.Duration)
				{
					return;
				}
				this.changeHighlight &= (int)(~(int)ChangeHighlightHelper.propertyGroupings[def]);
			}
		}

		private static Dictionary<StorePropertyDefinition, ChangeHighlightProperties> propertyGroupings = ChangeHighlightHelper.BuildPropertyGroupings(ChangeHighlightHelper.propertyGroupings);

		private int changeHighlight;

		public static readonly StorePropertyDefinition[] HighlightProperties = new StorePropertyDefinition[]
		{
			InternalSchema.MapiStartTime,
			InternalSchema.MapiEndTime,
			InternalSchema.Duration,
			InternalSchema.AppointmentRecurring,
			InternalSchema.AppointmentRecurrenceBlob,
			InternalSchema.MapiRecurrenceType,
			InternalSchema.RecurrencePattern,
			InternalSchema.Location,
			InternalSchema.SubjectPrefixInternal,
			InternalSchema.NormalizedSubjectInternal,
			InternalSchema.RtfInSync,
			InternalSchema.MeetingWorkspaceUrl,
			InternalSchema.BillingInformation,
			InternalSchema.Mileage,
			InternalSchema.Companies,
			InternalSchema.MapiInternetCpid,
			InternalSchema.IsSilent,
			InternalSchema.DisallowNewTimeProposal,
			InternalSchema.IsOnlineMeeting,
			InternalSchema.OnlineMeetingChanged,
			InternalSchema.ConferenceType,
			InternalSchema.NetShowURL,
			InternalSchema.NetMeetingServer
		};
	}
}
