using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public sealed class RespondToEventParametersSchema : EventWorkflowParametersSchema
	{
		public RespondToEventParametersSchema()
		{
			base.RegisterPropertyDefinition(RespondToEventParametersSchema.StaticMeetingRequestIdToBeDeletedProperty);
			base.RegisterPropertyDefinition(RespondToEventParametersSchema.StaticProposedStartTimeProperty);
			base.RegisterPropertyDefinition(RespondToEventParametersSchema.StaticProposedEndTimeProperty);
			base.RegisterPropertyDefinition(RespondToEventParametersSchema.StaticResponseProperty);
			base.RegisterPropertyDefinition(RespondToEventParametersSchema.StaticSendResponseProperty);
		}

		public TypedPropertyDefinition<string> MeetingRequestIdToBeDeletedProperty
		{
			get
			{
				return RespondToEventParametersSchema.StaticMeetingRequestIdToBeDeletedProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime?> ProposedStartTimeProperty
		{
			get
			{
				return RespondToEventParametersSchema.StaticProposedStartTimeProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime?> ProposedEndTimeProperty
		{
			get
			{
				return RespondToEventParametersSchema.StaticProposedEndTimeProperty;
			}
		}

		public TypedPropertyDefinition<ResponseType> ResponseProperty
		{
			get
			{
				return RespondToEventParametersSchema.StaticResponseProperty;
			}
		}

		public TypedPropertyDefinition<bool> SendResponseProperty
		{
			get
			{
				return RespondToEventParametersSchema.StaticSendResponseProperty;
			}
		}

		private static readonly TypedPropertyDefinition<string> StaticMeetingRequestIdToBeDeletedProperty = new TypedPropertyDefinition<string>("RespondToEventParameters.MeetingRequestIdToBeDeleted", null, true);

		private static readonly TypedPropertyDefinition<ExDateTime?> StaticProposedStartTimeProperty = new TypedPropertyDefinition<ExDateTime?>("RespondToEventParameters.ProposedStartTime", null, true);

		private static readonly TypedPropertyDefinition<ExDateTime?> StaticProposedEndTimeProperty = new TypedPropertyDefinition<ExDateTime?>("RespondToEventParameters.ProposedEndTime", null, true);

		private static readonly TypedPropertyDefinition<ResponseType> StaticResponseProperty = new TypedPropertyDefinition<ResponseType>("RespondToEventParameters.Response", ResponseType.None, true);

		private static readonly TypedPropertyDefinition<bool> StaticSendResponseProperty = new TypedPropertyDefinition<bool>("RespondToEventParameters.SendResponse", false, true);
	}
}
