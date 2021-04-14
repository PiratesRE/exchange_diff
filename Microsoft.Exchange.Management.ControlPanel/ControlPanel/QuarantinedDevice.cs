using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class QuarantinedDevice : BaseRow
	{
		public QuarantinedDevice(MobileDevice device) : base(device)
		{
			this.device = device;
		}

		[DataMember]
		public string UserName
		{
			get
			{
				return this.device.Id.Parent.Parent.Name;
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
				return this.device.DeviceType;
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
				return this.device.DeviceModel;
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
				if (this.device.FirstSyncTime != null)
				{
					return this.device.FirstSyncTime.UtcToUserDateTimeString();
				}
				return Strings.NotAvailable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private readonly MobileDevice device;
	}
}
