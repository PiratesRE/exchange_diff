using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EventLogTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NonUniqueEventCategoryInputException : LocalizedException
	{
		public NonUniqueEventCategoryInputException() : base(Strings.NonUniqueCategoryObject)
		{
		}

		public NonUniqueEventCategoryInputException(Exception innerException) : base(Strings.NonUniqueCategoryObject, innerException)
		{
		}

		protected NonUniqueEventCategoryInputException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
