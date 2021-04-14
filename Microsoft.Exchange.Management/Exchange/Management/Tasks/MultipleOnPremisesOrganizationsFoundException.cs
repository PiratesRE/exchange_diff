using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MultipleOnPremisesOrganizationsFoundException : LocalizedException
	{
		public MultipleOnPremisesOrganizationsFoundException() : base(Strings.MultipleOnPremisesOrganizationsFoundException)
		{
		}

		public MultipleOnPremisesOrganizationsFoundException(Exception innerException) : base(Strings.MultipleOnPremisesOrganizationsFoundException, innerException)
		{
		}

		protected MultipleOnPremisesOrganizationsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
