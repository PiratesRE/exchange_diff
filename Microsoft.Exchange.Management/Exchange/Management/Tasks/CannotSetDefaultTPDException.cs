using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSetDefaultTPDException : LocalizedException
	{
		public CannotSetDefaultTPDException() : base(Strings.CannotSetDefaultTPD)
		{
		}

		public CannotSetDefaultTPDException(Exception innerException) : base(Strings.CannotSetDefaultTPD, innerException)
		{
		}

		protected CannotSetDefaultTPDException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
