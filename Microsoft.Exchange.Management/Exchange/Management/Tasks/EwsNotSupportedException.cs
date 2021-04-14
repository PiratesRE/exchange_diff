using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EwsNotSupportedException : LocalizedException
	{
		public EwsNotSupportedException() : base(Strings.EwsNotSupportedException)
		{
		}

		public EwsNotSupportedException(Exception innerException) : base(Strings.EwsNotSupportedException, innerException)
		{
		}

		protected EwsNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
