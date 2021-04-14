using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MobileDevice : MobileDeviceRow
	{
		public MobileDevice(MobileDeviceConfiguration mobileDevice) : base(mobileDevice)
		{
		}

		[DataMember]
		public string Caption
		{
			get
			{
				return OwaOptionStrings.MobileDeviceDetailTitle;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string FirstSyncTime
		{
			get
			{
				if (base.MobileDevice.FirstSyncTime != null)
				{
					return base.MobileDevice.FirstSyncTime.UtcToUserDateTimeString();
				}
				return OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LastPolicyUpdateTime
		{
			get
			{
				if (base.MobileDevice.LastPolicyUpdateTime != null)
				{
					return base.MobileDevice.LastPolicyUpdateTime.UtcToUserDateTimeString();
				}
				return OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LastSuccessSync
		{
			get
			{
				if (base.MobileDevice.LastSuccessSync != null)
				{
					return base.MobileDevice.LastSuccessSync.UtcToUserDateTimeString();
				}
				return OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int NumberOfFoldersSynced
		{
			get
			{
				return base.MobileDevice.NumberOfFoldersSynced;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceMobileOperator
		{
			get
			{
				return base.MobileDevice.DeviceMobileOperator ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceAccessStateDescription
		{
			get
			{
				return LocalizedDescriptionAttribute.FromEnumForOwaOption(base.MobileDevice.DeviceAccessState.GetType(), base.MobileDevice.DeviceAccessState);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceAccessStateReasonDescription
		{
			get
			{
				string text = LocalizedDescriptionAttribute.FromEnumForOwaOption(base.MobileDevice.DeviceAccessStateReason.GetType(), base.MobileDevice.DeviceAccessStateReason);
				if (base.MobileDevice.DeviceAccessControlRule != null)
				{
					text = text + " - " + base.MobileDevice.DeviceAccessControlRule;
				}
				return text;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceAccessStateReasonLink
		{
			get
			{
				switch (base.MobileDevice.DeviceAccessStateReason)
				{
				case DeviceAccessStateReason.Policy:
					return "http://go.microsoft.com/fwlink/p/?LinkId=262798";
				case DeviceAccessStateReason.UserAgentsChanges:
				case DeviceAccessStateReason.RecentCommands:
				case DeviceAccessStateReason.Watsons:
				case DeviceAccessStateReason.OutOfBudgets:
				case DeviceAccessStateReason.SyncCommands:
				case DeviceAccessStateReason.CommandFrequency:
					return "http://go.microsoft.com/fwlink/p/?LinkId=262797";
				}
				return "http://go.microsoft.com/fwlink/?LinkId=264210";
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DevicePolicyAppliedDescription
		{
			get
			{
				return ((base.MobileDevice.DevicePolicyApplied != null) ? base.MobileDevice.DevicePolicyApplied.ToString() : OwaOptionStrings.NotAvailable) + " - " + LocalizedDescriptionAttribute.FromEnumForOwaOption(base.MobileDevice.DevicePolicyApplicationStatus.GetType(), base.MobileDevice.DevicePolicyApplicationStatus);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceActiveSyncVersion
		{
			get
			{
				return base.MobileDevice.ClientVersion ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceUserAgent
		{
			get
			{
				return base.MobileDevice.DeviceUserAgent ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceWipeSentTime
		{
			get
			{
				if (base.MobileDevice.DeviceWipeSentTime != null)
				{
					return base.MobileDevice.DeviceWipeSentTime.UtcToUserDateTimeString();
				}
				return OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string IssuedCommandDescription
		{
			get
			{
				if (base.MobileDevice.DeviceWipeRequestTime != null || base.MobileDevice.DeviceWipeSentTime != null)
				{
					string str = (base.MobileDevice.DeviceWipeSentTime != null) ? OwaOptionStrings.PendingWipeCommandSentLabel : OwaOptionStrings.PendingWipeCommandIssuedLabel;
					DateTime? deviceWipeSentTime = base.MobileDevice.DeviceWipeSentTime;
					DateTime? dateTimeWithoutTimeZone = (deviceWipeSentTime != null) ? new DateTime?(deviceWipeSentTime.GetValueOrDefault()) : base.MobileDevice.DeviceWipeRequestTime;
					return str + " " + dateTimeWithoutTimeZone.UtcToUserDateTimeString();
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceWipeRequestTime
		{
			get
			{
				if (base.MobileDevice.DeviceWipeRequestTime != null)
				{
					return base.MobileDevice.DeviceWipeRequestTime.UtcToUserDateTimeString();
				}
				return OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceWipeAckTime
		{
			get
			{
				if (base.MobileDevice.DeviceWipeAckTime != null)
				{
					return base.MobileDevice.DeviceWipeAckTime.UtcToUserDateTimeString();
				}
				return OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public uint? LastPingHeartbeat
		{
			get
			{
				return base.MobileDevice.LastPingHeartbeat;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceImei
		{
			get
			{
				return base.MobileDevice.DeviceImei ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceFriendlyName
		{
			get
			{
				return base.MobileDevice.DeviceFriendlyName ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceOS
		{
			get
			{
				return base.MobileDevice.DeviceOS ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceOSLanguage
		{
			get
			{
				return base.MobileDevice.DeviceOSLanguage ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string StatusNote
		{
			get
			{
				return base.MobileDevice.StatusNote;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
