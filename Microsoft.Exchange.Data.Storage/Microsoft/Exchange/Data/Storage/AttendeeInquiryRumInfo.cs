using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttendeeInquiryRumInfo : AttendeeRumInfo
	{
		private AttendeeInquiryRumInfo(bool wouldRepair, MeetingInquiryAction predictedRepairAction) : this(null, wouldRepair, predictedRepairAction)
		{
		}

		private AttendeeInquiryRumInfo(ExDateTime? originalStartTime, bool wouldRepair, MeetingInquiryAction predictedRepairAction) : base(RumType.Inquiry, originalStartTime)
		{
			this.WouldRepair = wouldRepair;
			this.PredictedRepairAction = predictedRepairAction;
		}

		public static AttendeeInquiryRumInfo CreateMasterInstance(bool wouldRepair, MeetingInquiryAction predictedRepairAction)
		{
			EnumValidator<MeetingInquiryAction>.ThrowIfInvalid(predictedRepairAction, "predictedRepairAction");
			return new AttendeeInquiryRumInfo(wouldRepair, predictedRepairAction);
		}

		public static AttendeeInquiryRumInfo CreateOccurrenceInstance(ExDateTime originalStartTime, bool wouldRepair, MeetingInquiryAction predictedRepairAction)
		{
			EnumValidator<MeetingInquiryAction>.ThrowIfInvalid(predictedRepairAction, "predictedRepairAction");
			return new AttendeeInquiryRumInfo(new ExDateTime?(originalStartTime), wouldRepair, predictedRepairAction);
		}

		public bool WouldRepair { get; private set; }

		public MeetingInquiryAction PredictedRepairAction { get; private set; }
	}
}
