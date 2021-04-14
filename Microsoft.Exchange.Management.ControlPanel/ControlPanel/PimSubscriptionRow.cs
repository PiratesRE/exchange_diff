using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class PimSubscriptionRow : BaseRow
	{
		public PimSubscriptionRow(PimSubscriptionProxy subscription) : base(((AggregationSubscriptionIdentity)subscription.Identity).ToIdentity(subscription.EmailAddress.ToString()), subscription)
		{
			this.PimSubscriptionProxy = subscription;
		}

		public PimSubscriptionProxy PimSubscriptionProxy { get; private set; }

		[DataMember]
		public string EmailAddress
		{
			get
			{
				return this.PimSubscriptionProxy.EmailAddress.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		protected bool ShowWarningIcon
		{
			get
			{
				return this.PimSubscriptionProxy.IsWarningStatus || this.PimSubscriptionProxy.IsErrorStatus;
			}
		}

		[DataMember]
		public string StatusIcon
		{
			get
			{
				if (!this.ShowWarningIcon)
				{
					return string.Empty;
				}
				return "Warning.gif";
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string StatusIconAlt
		{
			get
			{
				if (!this.ShowWarningIcon)
				{
					return string.Empty;
				}
				return OwaOptionStrings.WarningAlt;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string StatusDescription
		{
			get
			{
				return this.PimSubscriptionProxy.StatusDescription;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SubscriptionType
		{
			get
			{
				return this.PimSubscriptionProxy.SubscriptionType.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsValid
		{
			get
			{
				return this.PimSubscriptionProxy.IsValid;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool SendAsEnabled
		{
			get
			{
				return this.PimSubscriptionProxy.SendAsState == SendAsState.Enabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string VerificationEmailState
		{
			get
			{
				return this.PimSubscriptionProxy.VerificationEmailState.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public string VerificationFeedbackString
		{
			get
			{
				string result = null;
				switch (this.PimSubscriptionProxy.VerificationEmailState)
				{
				case Microsoft.Exchange.Transport.Sync.Common.Subscription.VerificationEmailState.EmailSent:
					result = OwaOptionStrings.VerificationEmailSucceeded(this.EmailAddress);
					break;
				case Microsoft.Exchange.Transport.Sync.Common.Subscription.VerificationEmailState.EmailFailedToSend:
					result = OwaOptionStrings.VerificationEmailFailedToSend(this.EmailAddress);
					break;
				}
				return result;
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
