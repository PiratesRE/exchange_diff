using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoRMSServersFoundException : LocalizedException
	{
		public NoRMSServersFoundException() : base(Strings.NoRMSServersFoundException)
		{
		}

		public NoRMSServersFoundException(Exception innerException) : base(Strings.NoRMSServersFoundException, innerException)
		{
		}

		protected NoRMSServersFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
