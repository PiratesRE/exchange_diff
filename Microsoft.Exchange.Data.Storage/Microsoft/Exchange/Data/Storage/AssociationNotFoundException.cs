using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AssociationNotFoundException : DataSourceOperationException
	{
		public AssociationNotFoundException(LocalizedString message) : base(message)
		{
		}

		public AssociationNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AssociationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
