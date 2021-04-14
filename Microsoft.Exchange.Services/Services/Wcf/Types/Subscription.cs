using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class Subscription : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string DetailedStatus { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public string EmailAddress { get; set; }

		[DataMember]
		public Identity Identity { get; set; }

		[DataMember]
		public bool IsErrorStatus { get; set; }

		[DataMember]
		public bool IsValid { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string LastSuccessfulSync { get; set; }

		[DataMember]
		public string Name { get; set; }

		[IgnoreDataMember]
		public SendAsState SendAsState { get; set; }

		[DataMember(Name = "SendAsState", IsRequired = false, EmitDefaultValue = false)]
		public string SendAsStateString
		{
			get
			{
				return EnumUtilities.ToString<SendAsState>(this.SendAsState);
			}
			set
			{
				this.SendAsState = EnumUtilities.Parse<SendAsState>(value);
			}
		}

		[IgnoreDataMember]
		public AggregationStatus Status { get; set; }

		[DataMember(Name = "Status", IsRequired = false, EmitDefaultValue = false)]
		public string StatusString
		{
			get
			{
				return EnumUtilities.ToString<AggregationStatus>(this.Status);
			}
			set
			{
				this.Status = EnumUtilities.Parse<AggregationStatus>(value);
			}
		}

		[DataMember]
		public string StatusDescription { get; set; }

		[IgnoreDataMember]
		public AggregationSubscriptionType SubscriptionType { get; set; }

		[DataMember(Name = "SubscriptionType", IsRequired = false, EmitDefaultValue = false)]
		public string SubscriptionTypeString
		{
			get
			{
				return EnumUtilities.ToString<AggregationSubscriptionType>(this.SubscriptionType);
			}
			set
			{
				this.SubscriptionType = EnumUtilities.Parse<AggregationSubscriptionType>(value);
			}
		}
	}
}
