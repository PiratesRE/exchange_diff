using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsSharedIdentityLocalComputerNotFoundException : LocalizedException
	{
		public RmsSharedIdentityLocalComputerNotFoundException() : base(Strings.RmsSharedIdentityLocalComputerNotFound)
		{
		}

		public RmsSharedIdentityLocalComputerNotFoundException(Exception innerException) : base(Strings.RmsSharedIdentityLocalComputerNotFound, innerException)
		{
		}

		protected RmsSharedIdentityLocalComputerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
