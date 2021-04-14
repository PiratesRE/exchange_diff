using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksException : BitlockerUtilException
	{
		public Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksException(string volume, string mountPoint, string eventXML) : base(DiskManagementStrings.Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksError(volume, mountPoint, eventXML))
		{
			this.volume = volume;
			this.mountPoint = mountPoint;
			this.eventXML = eventXML;
		}

		public Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksException(string volume, string mountPoint, string eventXML, Exception innerException) : base(DiskManagementStrings.Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksError(volume, mountPoint, eventXML), innerException)
		{
			this.volume = volume;
			this.mountPoint = mountPoint;
			this.eventXML = eventXML;
		}

		protected Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volume = (string)info.GetValue("volume", typeof(string));
			this.mountPoint = (string)info.GetValue("mountPoint", typeof(string));
			this.eventXML = (string)info.GetValue("eventXML", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volume", this.volume);
			info.AddValue("mountPoint", this.mountPoint);
			info.AddValue("eventXML", this.eventXML);
		}

		public string Volume
		{
			get
			{
				return this.volume;
			}
		}

		public string MountPoint
		{
			get
			{
				return this.mountPoint;
			}
		}

		public string EventXML
		{
			get
			{
				return this.eventXML;
			}
		}

		private readonly string volume;

		private readonly string mountPoint;

		private readonly string eventXML;
	}
}
