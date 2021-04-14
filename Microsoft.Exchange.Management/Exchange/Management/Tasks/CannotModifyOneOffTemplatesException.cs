using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotModifyOneOffTemplatesException : LocalizedException
	{
		public CannotModifyOneOffTemplatesException() : base(Strings.CannotModifyOneOffTemplates)
		{
		}

		public CannotModifyOneOffTemplatesException(Exception innerException) : base(Strings.CannotModifyOneOffTemplates, innerException)
		{
		}

		protected CannotModifyOneOffTemplatesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
