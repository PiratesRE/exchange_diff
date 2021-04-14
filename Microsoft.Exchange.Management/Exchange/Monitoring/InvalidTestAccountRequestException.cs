using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidTestAccountRequestException : LocalizedException
	{
		public InvalidTestAccountRequestException() : base(Strings.InvalidTestAccountRequest)
		{
		}

		public InvalidTestAccountRequestException(Exception innerException) : base(Strings.InvalidTestAccountRequest, innerException)
		{
		}

		protected InvalidTestAccountRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
