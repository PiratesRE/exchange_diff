using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorFileTypeIsUnsupportedException : LocalizedException
	{
		public ErrorFileTypeIsUnsupportedException() : base(Strings.ErrorFileTypeIsUnsupported)
		{
		}

		public ErrorFileTypeIsUnsupportedException(Exception innerException) : base(Strings.ErrorFileTypeIsUnsupported, innerException)
		{
		}

		protected ErrorFileTypeIsUnsupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
