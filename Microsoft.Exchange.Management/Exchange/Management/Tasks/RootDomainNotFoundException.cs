using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RootDomainNotFoundException : LocalizedException
	{
		public RootDomainNotFoundException() : base(Strings.RootDomainNotFoundException)
		{
		}

		public RootDomainNotFoundException(Exception innerException) : base(Strings.RootDomainNotFoundException, innerException)
		{
		}

		protected RootDomainNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
