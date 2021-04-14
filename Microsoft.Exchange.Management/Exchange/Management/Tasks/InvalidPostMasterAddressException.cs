using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidPostMasterAddressException : LocalizedException
	{
		public InvalidPostMasterAddressException() : base(Strings.ErrorInvalidPostMasterAddress)
		{
		}

		public InvalidPostMasterAddressException(Exception innerException) : base(Strings.ErrorInvalidPostMasterAddress, innerException)
		{
		}

		protected InvalidPostMasterAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
