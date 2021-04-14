using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClassificationRuleCollectionTimeoutException : LocalizedException
	{
		public ClassificationRuleCollectionTimeoutException() : base(Strings.ClassificationRuleCollectionTimeoutFailure)
		{
		}

		public ClassificationRuleCollectionTimeoutException(Exception innerException) : base(Strings.ClassificationRuleCollectionTimeoutFailure, innerException)
		{
		}

		protected ClassificationRuleCollectionTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
