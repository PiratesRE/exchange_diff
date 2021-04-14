using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class PimSubscription : PimSubscriptionRow
	{
		public PimSubscription(PimSubscriptionProxy subscription) : base(subscription)
		{
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return base.PimSubscriptionProxy.DisplayName;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LastSuccessfulSyncText
		{
			get
			{
				string arg;
				if (base.PimSubscriptionProxy.LastSuccessfulSync != null)
				{
					arg = base.PimSubscriptionProxy.LastSuccessfulSync.UtcToUserDateTimeString();
				}
				else
				{
					arg = OwaOptionStrings.NeverSyncText;
				}
				string result = string.Empty;
				if (base.PimSubscriptionProxy.IsSuccessStatus)
				{
					result = string.Format(OwaOptionStrings.LastSynchronization, arg);
				}
				else
				{
					result = string.Format(OwaOptionStrings.LastSuccessfulSync, arg);
				}
				return result;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DetailedStatus
		{
			get
			{
				return base.PimSubscriptionProxy.DetailedStatus;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CurrentStatusClass
		{
			get
			{
				if (!base.ShowWarningIcon)
				{
					return "PropertyDiv HideSyncFailedRow";
				}
				return "PropertyDiv ShowSyncFailedRow";
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string WarningText
		{
			get
			{
				if (base.ShowWarningIcon)
				{
					return base.StatusDescription;
				}
				return string.Empty;
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
