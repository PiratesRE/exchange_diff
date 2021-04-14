using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClassificationRuleCollectionStorageException : LocalizedException
	{
		public ClassificationRuleCollectionStorageException() : base(Strings.ClassificationRuleCollectionStorageFailure)
		{
		}

		public ClassificationRuleCollectionStorageException(Exception innerException) : base(Strings.ClassificationRuleCollectionStorageFailure, innerException)
		{
		}

		protected ClassificationRuleCollectionStorageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
