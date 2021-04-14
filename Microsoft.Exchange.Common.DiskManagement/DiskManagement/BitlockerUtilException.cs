using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BitlockerUtilException : LocalizedException
	{
		public BitlockerUtilException(string errorMsg) : base(DiskManagementStrings.BitlockerUtilError(errorMsg))
		{
			this.errorMsg = errorMsg;
		}

		public BitlockerUtilException(string errorMsg, Exception innerException) : base(DiskManagementStrings.BitlockerUtilError(errorMsg), innerException)
		{
			this.errorMsg = errorMsg;
		}

		protected BitlockerUtilException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string errorMsg;
	}
}
