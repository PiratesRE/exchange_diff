using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSpecifyBothAllDomainsAndDomainException : LocalizedException
	{
		public CannotSpecifyBothAllDomainsAndDomainException() : base(Strings.CannotSpecifyBothAllDomainsAndDomain)
		{
		}

		public CannotSpecifyBothAllDomainsAndDomainException(Exception innerException) : base(Strings.CannotSpecifyBothAllDomainsAndDomain, innerException)
		{
		}

		protected CannotSpecifyBothAllDomainsAndDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
