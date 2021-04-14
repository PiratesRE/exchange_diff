using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EmptyTaskListException : LocalizedException
	{
		public EmptyTaskListException() : base(Strings.EmptyTaskListException)
		{
		}

		public EmptyTaskListException(Exception innerException) : base(Strings.EmptyTaskListException, innerException)
		{
		}

		protected EmptyTaskListException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
