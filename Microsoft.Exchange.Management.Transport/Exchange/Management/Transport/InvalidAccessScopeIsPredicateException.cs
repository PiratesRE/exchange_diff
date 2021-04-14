using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidAccessScopeIsPredicateException : InvalidComplianceRulePredicateException
	{
		public InvalidAccessScopeIsPredicateException() : base(Strings.InvalidAccessScopeIsPredicate)
		{
		}

		public InvalidAccessScopeIsPredicateException(Exception innerException) : base(Strings.InvalidAccessScopeIsPredicate, innerException)
		{
		}

		protected InvalidAccessScopeIsPredicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
