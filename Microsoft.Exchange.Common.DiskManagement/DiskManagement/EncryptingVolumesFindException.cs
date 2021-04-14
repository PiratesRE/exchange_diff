using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EncryptingVolumesFindException : BitlockerUtilException
	{
		public EncryptingVolumesFindException(string error) : base(DiskManagementStrings.EncryptingVolumesFindError(error))
		{
			this.error = error;
		}

		public EncryptingVolumesFindException(string error, Exception innerException) : base(DiskManagementStrings.EncryptingVolumesFindError(error), innerException)
		{
			this.error = error;
		}

		protected EncryptingVolumesFindException(SerializationInfo info, StreamingContext context) : base(info, context)
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
