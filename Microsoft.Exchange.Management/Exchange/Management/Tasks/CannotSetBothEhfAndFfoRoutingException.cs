using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSetBothEhfAndFfoRoutingException : LocalizedException
	{
		public CannotSetBothEhfAndFfoRoutingException() : base(Strings.CannotSetBothEhfAndFfoRoutingId)
		{
		}

		public CannotSetBothEhfAndFfoRoutingException(Exception innerException) : base(Strings.CannotSetBothEhfAndFfoRoutingId, innerException)
		{
		}

		protected CannotSetBothEhfAndFfoRoutingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
