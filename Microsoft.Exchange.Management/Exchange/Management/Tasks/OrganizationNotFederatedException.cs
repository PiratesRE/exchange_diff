using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OrganizationNotFederatedException : LocalizedException
	{
		public OrganizationNotFederatedException() : base(Strings.OrganizationNotFederatedException)
		{
		}

		public OrganizationNotFederatedException(Exception innerException) : base(Strings.OrganizationNotFederatedException, innerException)
		{
		}

		protected OrganizationNotFederatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
