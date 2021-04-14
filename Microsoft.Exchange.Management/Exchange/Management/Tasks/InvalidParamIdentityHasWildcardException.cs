using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidParamIdentityHasWildcardException : LocalizedException
	{
		public InvalidParamIdentityHasWildcardException() : base(Strings.InvalidParamIdentityHasWildcardException)
		{
		}

		public InvalidParamIdentityHasWildcardException(Exception innerException) : base(Strings.InvalidParamIdentityHasWildcardException, innerException)
		{
		}

		protected InvalidParamIdentityHasWildcardException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
