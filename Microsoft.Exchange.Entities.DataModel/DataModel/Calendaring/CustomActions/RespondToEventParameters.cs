using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public sealed class RespondToEventParameters : EventWorkflowParameters<RespondToEventParametersSchema>
	{
		public string MeetingRequestIdToBeDeleted
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.MeetingRequestIdToBeDeletedProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.MeetingRequestIdToBeDeletedProperty, value);
			}
		}

		public ExDateTime? ProposedStartTime
		{
			get
			{
				return base.GetPropertyValueOrDefault<ExDateTime?>(base.Schema.ProposedStartTimeProperty);
			}
			set
			{
				base.SetPropertyValue<ExDateTime?>(base.Schema.ProposedStartTimeProperty, value);
			}
		}

		public ExDateTime? ProposedEndTime
		{
			get
			{
				return base.GetPropertyValueOrDefault<ExDateTime?>(base.Schema.ProposedEndTimeProperty);
			}
			set
			{
				base.SetPropertyValue<ExDateTime?>(base.Schema.ProposedEndTimeProperty, value);
			}
		}

		public ResponseType Response
		{
			get
			{
				return base.GetPropertyValueOrDefault<ResponseType>(base.Schema.ResponseProperty);
			}
			set
			{
				base.SetPropertyValue<ResponseType>(base.Schema.ResponseProperty, value);
			}
		}

		public bool SendResponse
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.SendResponseProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.SendResponseProperty, value);
			}
		}
	}
}
