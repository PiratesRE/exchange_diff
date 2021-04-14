using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MountPointsFindException : BitlockerUtilException
	{
		public MountPointsFindException(string error) : base(DiskManagementStrings.MountpointsFindError(error))
		{
			this.error = error;
		}

		public MountPointsFindException(string error, Exception innerException) : base(DiskManagementStrings.MountpointsFindError(error), innerException)
		{
			this.error = error;
		}

		protected MountPointsFindException(SerializationInfo info, StreamingContext context) : base(info, context)
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
