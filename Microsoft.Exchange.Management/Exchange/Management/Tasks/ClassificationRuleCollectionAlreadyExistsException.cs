using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClassificationRuleCollectionAlreadyExistsException : ClassificationRuleCollectionValidationException
	{
		public ClassificationRuleCollectionAlreadyExistsException() : base(Strings.ClassificationRuleCollectionAlreadyExists)
		{
		}

		public ClassificationRuleCollectionAlreadyExistsException(Exception innerException) : base(Strings.ClassificationRuleCollectionAlreadyExists, innerException)
		{
		}

		protected ClassificationRuleCollectionAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
