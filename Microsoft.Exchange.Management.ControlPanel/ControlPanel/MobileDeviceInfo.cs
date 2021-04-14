using System;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MobileDeviceInfo
	{
		public MobileDeviceInfo(MobileDeviceConfiguration configuration)
		{
			this.Identity = ((ADObjectId)configuration.Identity).ToIdentity();
			this.DeviceType = configuration.DeviceType;
			this.DeviceModel = configuration.DeviceModel;
			this.DeviceAccessState = ((configuration.Status == DeviceRemoteWipeStatus.DeviceOk) ? LocalizedDescriptionAttribute.FromEnum(configuration.DeviceAccessState.GetType(), configuration.DeviceAccessState) : LocalizedDescriptionAttribute.FromEnum(configuration.Status.GetType(), configuration.Status));
			this.DevicePhoneNumber_LtrSpan = (string.IsNullOrEmpty(configuration.DevicePhoneNumber) ? Strings.NotAvailable : string.Format("<span dir=\"ltr\">{0}</span>", HttpUtility.HtmlEncode(configuration.DevicePhoneNumber)));
			this.IsRemoteWipeSupported = configuration.IsRemoteWipeSupported;
			this.DeviceStatusIsOK = (configuration.Status == DeviceRemoteWipeStatus.DeviceOk);
			this.ClientType = configuration.ClientType.ToString();
		}

		internal MobileDeviceInfo()
		{
		}

		[DataMember]
		public Identity Identity { get; set; }

		[DataMember]
		public string DeviceType { get; set; }

		[DataMember]
		public string DeviceModel { get; set; }

		[DataMember]
		public string DeviceAccessState { get; set; }

		[DataMember]
		public string DevicePhoneNumber_LtrSpan { get; set; }

		[DataMember]
		public bool IsRemoteWipeSupported { get; set; }

		[DataMember]
		public bool DeviceStatusIsOK { get; set; }

		[DataMember]
		public string PendingCommand { get; set; }

		[DataMember]
		public string ClientType { get; set; }

		public string StatusDisplayString { get; set; }

		public override bool Equals(object obj)
		{
			MobileDeviceInfo mobileDeviceInfo = obj as MobileDeviceInfo;
			return mobileDeviceInfo != null && mobileDeviceInfo.Identity == this.Identity;
		}

		public override int GetHashCode()
		{
			return this.GetHashCode();
		}

		private const string PhoneNumberLtrFmt = "<span dir=\"ltr\">{0}</span>";
	}
}
