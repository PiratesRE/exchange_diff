using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UmUserProblem : LocalizedException
	{
		public UmUserProblem() : base(Strings.UserProblem)
		{
		}

		public UmUserProblem(Exception innerException) : base(Strings.UserProblem, innerException)
		{
		}

		protected UmUserProblem(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
