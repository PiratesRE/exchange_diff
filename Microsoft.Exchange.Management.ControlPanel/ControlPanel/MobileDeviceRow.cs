using System;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MobileDeviceRow : BaseRow
	{
		public MobileDeviceRow(MobileDeviceConfiguration mobileDevice) : base(mobileDevice.ToIdentity(mobileDevice.DeviceFriendlyName), mobileDevice)
		{
			this.MobileDevice = mobileDevice;
		}

		public MobileDeviceConfiguration MobileDevice { get; set; }

		[DataMember]
		public string DeviceID
		{
			get
			{
				return this.MobileDevice.DeviceID ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceType
		{
			get
			{
				return this.MobileDevice.DeviceType ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceModel
		{
			get
			{
				return this.MobileDevice.DeviceModel ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DevicePhoneNumber
		{
			get
			{
				return this.MobileDevice.DevicePhoneNumber ?? OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DevicePhoneNumber_LtrSpan
		{
			get
			{
				if (!string.IsNullOrEmpty(this.MobileDevice.DevicePhoneNumber))
				{
					return string.Format("<span dir=\"ltr\">{0}</span>", HttpUtility.HtmlEncode(this.MobileDevice.DevicePhoneNumber));
				}
				return OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LastSyncAttemptTime
		{
			get
			{
				if (this.MobileDevice.LastSyncAttemptTime != null)
				{
					return this.MobileDevice.LastSyncAttemptTime.UtcToUserDateTimeString();
				}
				return OwaOptionStrings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public DateTime LastSyncAttemptUTCDateTime
		{
			get
			{
				if (this.MobileDevice.LastSyncAttemptTime == null)
				{
					return DateTime.MinValue;
				}
				return this.MobileDevice.LastSyncAttemptTime.Value;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool DeviceStatusIsOK
		{
			get
			{
				return this.MobileDevice.Status == DeviceRemoteWipeStatus.DeviceOk;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceStatus
		{
			get
			{
				return LocalizedDescriptionAttribute.FromEnumForOwaOption(this.MobileDevice.Status.GetType(), this.MobileDevice.Status);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeviceAccessState
		{
			get
			{
				if (!this.DeviceStatusIsOK)
				{
					return this.DeviceStatus;
				}
				return LocalizedDescriptionAttribute.FromEnumForOwaOption(this.MobileDevice.DeviceAccessState.GetType(), this.MobileDevice.DeviceAccessState);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool HasRecoveryPassword
		{
			get
			{
				return !string.IsNullOrEmpty(this.MobileDevice.RecoveryPassword);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RecoveryPassword
		{
			get
			{
				return this.MobileDevice.RecoveryPassword;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsRemoteWipeSupported
		{
			get
			{
				return this.MobileDevice.IsRemoteWipeSupported;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsLoggingRunning { get; set; }

		private const string PhoneNumberLtrFmt = "<span dir=\"ltr\">{0}</span>";
	}
}
