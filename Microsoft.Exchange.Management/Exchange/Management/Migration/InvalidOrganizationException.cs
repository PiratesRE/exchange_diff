using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidOrganizationException : LocalizedException
	{
		public InvalidOrganizationException() : base(Strings.ErrorInvalidOrganization)
		{
		}

		public InvalidOrganizationException(Exception innerException) : base(Strings.ErrorInvalidOrganization, innerException)
		{
		}

		protected InvalidOrganizationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
