using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorFileHasNoTextContentException : LocalizedException
	{
		public ErrorFileHasNoTextContentException() : base(Strings.ErrorFileHasNoTextContent)
		{
		}

		public ErrorFileHasNoTextContentException(Exception innerException) : base(Strings.ErrorFileHasNoTextContent, innerException)
		{
		}

		protected ErrorFileHasNoTextContentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
