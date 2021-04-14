using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class VolumeLockedFindException : BitlockerUtilException
	{
		public VolumeLockedFindException(string volumeId, string error) : base(DiskManagementStrings.VolumeLockedFindError(volumeId, error))
		{
			this.volumeId = volumeId;
			this.error = error;
		}

		public VolumeLockedFindException(string volumeId, string error, Exception innerException) : base(DiskManagementStrings.VolumeLockedFindError(volumeId, error), innerException)
		{
			this.volumeId = volumeId;
			this.error = error;
		}

		protected VolumeLockedFindException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeId = (string)info.GetValue("volumeId", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeId", this.volumeId);
			info.AddValue("error", this.error);
		}

		public string VolumeId
		{
			get
			{
				return this.volumeId;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string volumeId;

		private readonly string error;
	}
}
