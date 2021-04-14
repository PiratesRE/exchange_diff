using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Entities.EntitySets
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedExpressionException : NotSupportedException
	{
		public UnsupportedExpressionException(LocalizedString message) : base(message)
		{
		}

		public UnsupportedExpressionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected UnsupportedExpressionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
