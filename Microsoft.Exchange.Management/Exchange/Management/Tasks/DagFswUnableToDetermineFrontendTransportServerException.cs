using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagFswUnableToDetermineFrontendTransportServerException : LocalizedException
	{
		public DagFswUnableToDetermineFrontendTransportServerException() : base(Strings.DagFswUnableToDetermineFrontendTransportServerException)
		{
		}

		public DagFswUnableToDetermineFrontendTransportServerException(Exception innerException) : base(Strings.DagFswUnableToDetermineFrontendTransportServerException, innerException)
		{
		}

		protected DagFswUnableToDetermineFrontendTransportServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
