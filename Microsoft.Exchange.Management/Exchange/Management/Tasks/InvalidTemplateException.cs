using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidTemplateException : LocalizedException
	{
		public InvalidTemplateException() : base(Strings.InvalidTemplate)
		{
		}

		public InvalidTemplateException(Exception innerException) : base(Strings.InvalidTemplate, innerException)
		{
		}

		protected InvalidTemplateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
