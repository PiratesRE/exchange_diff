using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidContentDateFromAndContentDateToPredicateException : InvalidComplianceRulePredicateException
	{
		public InvalidContentDateFromAndContentDateToPredicateException() : base(Strings.InvalidContentDateFromAndContentDateToPredicate)
		{
		}

		public InvalidContentDateFromAndContentDateToPredicateException(Exception innerException) : base(Strings.InvalidContentDateFromAndContentDateToPredicate, innerException)
		{
		}

		protected InvalidContentDateFromAndContentDateToPredicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
