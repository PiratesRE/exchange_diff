using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EventLogTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidEventCategoryInputException : LocalizedException
	{
		public InvalidEventCategoryInputException() : base(Strings.InvalidCategoryObject)
		{
		}

		public InvalidEventCategoryInputException(Exception innerException) : base(Strings.InvalidCategoryObject, innerException)
		{
		}

		protected InvalidEventCategoryInputException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
