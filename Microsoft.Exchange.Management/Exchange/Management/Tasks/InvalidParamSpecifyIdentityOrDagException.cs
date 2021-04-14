using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidParamSpecifyIdentityOrDagException : LocalizedException
	{
		public InvalidParamSpecifyIdentityOrDagException() : base(Strings.InvalidParamSpecifyIdentityOrDagException)
		{
		}

		public InvalidParamSpecifyIdentityOrDagException(Exception innerException) : base(Strings.InvalidParamSpecifyIdentityOrDagException, innerException)
		{
		}

		protected InvalidParamSpecifyIdentityOrDagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
