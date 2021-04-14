using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LockedVolumesFindException : BitlockerUtilException
	{
		public LockedVolumesFindException(string error) : base(DiskManagementStrings.LockedVolumesFindError(error))
		{
			this.error = error;
		}

		public LockedVolumesFindException(string error, Exception innerException) : base(DiskManagementStrings.LockedVolumesFindError(error), innerException)
		{
			this.error = error;
		}

		protected LockedVolumesFindException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
