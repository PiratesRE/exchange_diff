using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Management.Tracking;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RecipientTrackingEventsFilter : ResultSizeFilter
	{
		public RecipientTrackingEventsFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MessageTrackingReport";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}

		[DataMember]
		public string Identity
		{
			get
			{
				return (string)base["Identity"];
			}
			set
			{
				base["Identity"] = value;
			}
		}

		[DataMember]
		public RecipientDeliveryStatus RecipientStatus
		{
			get
			{
				if (base["Status"] != null)
				{
					return (RecipientDeliveryStatus)base["Status"];
				}
				return RecipientDeliveryStatus.All;
			}
			set
			{
				if (value != RecipientDeliveryStatus.All)
				{
					base["Status"] = (_DeliveryStatus)value;
				}
			}
		}

		[DataMember]
		public string Recipients
		{
			get
			{
				return base["Recipients"].StringArrayJoin(",");
			}
			set
			{
				base["Recipients"] = value.ToArrayOfStrings();
			}
		}

		[DataMember]
		public string SearchText
		{
			get
			{
				return this.Recipients;
			}
			set
			{
				this.Recipients = value;
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			if (base.CanSetParameter("ByPassDelegateChecking"))
			{
				base["BypassDelegateChecking"] = true;
			}
			if (base.CanSetParameter("DetailLevel"))
			{
				base["DetailLevel"] = MessageTrackingDetailLevel.Verbose;
			}
			this.RecipientStatus = RecipientDeliveryStatus.All;
			base.ResultSize = 30;
		}

		public new const string RbacParameters = "?ResultSize&Identity&Status&Recipients";
	}
}
