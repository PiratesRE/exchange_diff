using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingResponseSchema : MeetingMessageInstanceSchema
	{
		public new static MeetingResponseSchema Instance
		{
			get
			{
				if (MeetingResponseSchema.instance == null)
				{
					MeetingResponseSchema.instance = new MeetingResponseSchema();
				}
				return MeetingResponseSchema.instance;
			}
		}

		protected override void AddConstraints(List<StoreObjectConstraint> constraints)
		{
			base.AddConstraints(constraints);
			constraints.Add(MeetingResponseSchema.StartTimeMustBeLessThanOrEqualToEndTimeConstraint);
		}

		internal override void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			base.CoreObjectUpdate(coreItem, operation);
			MeetingResponse.CoreObjectUpdateIsSilent(coreItem);
		}

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentCounterProposal = InternalSchema.AppointmentCounterProposal;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentCounterStartWhole = InternalSchema.AppointmentCounterStartWhole;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentCounterEndWhole = InternalSchema.AppointmentCounterEndWhole;

		public static readonly StorePropertyDefinition AppointmentProposedDuration = InternalSchema.AppointmentProposedDuration;

		[Autoload]
		public static readonly StorePropertyDefinition ResponseType = InternalSchema.MeetingMessageResponseType;

		private static MeetingResponseSchema instance = null;

		private static readonly PropertyComparisonConstraint StartTimeMustBeLessThanOrEqualToEndTimeConstraint = new PropertyComparisonConstraint(InternalSchema.AppointmentCounterStartWhole, InternalSchema.AppointmentCounterEndWhole, ComparisonOperator.LessThanOrEqual);
	}
}
