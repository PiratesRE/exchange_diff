using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ConnectSubscription : Subscription
	{
		[IgnoreDataMember]
		public AggregationType AggregationType
		{
			get
			{
				return this.aggregationType;
			}
			set
			{
				this.aggregationType = value;
				base.TrackPropertyChanged("AggregationType");
			}
		}

		[DataMember(Name = "AggregationType")]
		public string AggregationTypeString
		{
			get
			{
				return this.aggregationType.ToString();
			}
			set
			{
				this.aggregationType = EnumUtilities.Parse<AggregationType>(value);
				base.TrackPropertyChanged("AggregationTypeString");
			}
		}

		[IgnoreDataMember]
		public AggregationSubscriptionType AggregationSubscriptionType
		{
			get
			{
				return this.aggregationSubscriptionType;
			}
			set
			{
				this.aggregationSubscriptionType = value;
				base.TrackPropertyChanged("AggregationSubscriptionType");
			}
		}

		[DataMember(Name = "AggregationSubscriptionType")]
		public string AggregationSubscriptionTypeString
		{
			get
			{
				return this.aggregationSubscriptionType.ToString();
			}
			set
			{
				this.aggregationSubscriptionType = EnumUtilities.Parse<AggregationSubscriptionType>(value);
				base.TrackPropertyChanged("AggregationSubscriptionTypeString");
			}
		}

		[IgnoreDataMember]
		public ConnectState ConnectState
		{
			get
			{
				return this.connectState;
			}
			set
			{
				this.connectState = value;
				base.TrackPropertyChanged("ConnectState");
			}
		}

		[DataMember(Name = "ConnectState")]
		public string ConnectStateString
		{
			get
			{
				return this.connectState.ToString();
			}
			set
			{
				this.connectState = EnumUtilities.Parse<ConnectState>(value);
				base.TrackPropertyChanged("ConnectStateString");
			}
		}

		private AggregationSubscriptionType aggregationSubscriptionType;

		private AggregationType aggregationType;

		private ConnectState connectState;
	}
}
