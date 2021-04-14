using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EncryptableVolumeArgNullException : BitlockerUtilException
	{
		public EncryptableVolumeArgNullException(string methodName) : base(DiskManagementStrings.EncryptableVolumeArgNullError(methodName))
		{
			this.methodName = methodName;
		}

		public EncryptableVolumeArgNullException(string methodName, Exception innerException) : base(DiskManagementStrings.EncryptableVolumeArgNullError(methodName), innerException)
		{
			this.methodName = methodName;
		}

		protected EncryptableVolumeArgNullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.methodName = (string)info.GetValue("methodName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("methodName", this.methodName);
		}

		public string MethodName
		{
			get
			{
				return this.methodName;
			}
		}

		private readonly string methodName;
	}
}
