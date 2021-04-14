using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidVolumeIdException : BitlockerUtilException
	{
		public InvalidVolumeIdException(string volumeId) : base(DiskManagementStrings.InvalidVolumeIdError(volumeId))
		{
			this.volumeId = volumeId;
		}

		public InvalidVolumeIdException(string volumeId, Exception innerException) : base(DiskManagementStrings.InvalidVolumeIdError(volumeId), innerException)
		{
			this.volumeId = volumeId;
		}

		protected InvalidVolumeIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeId = (string)info.GetValue("volumeId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeId", this.volumeId);
		}

		public string VolumeId
		{
			get
			{
				return this.volumeId;
			}
		}

		private readonly string volumeId;
	}
}
