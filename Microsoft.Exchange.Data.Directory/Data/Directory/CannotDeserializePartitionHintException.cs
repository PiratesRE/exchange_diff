using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotDeserializePartitionHintException : DataSourceOperationException
	{
		public CannotDeserializePartitionHintException(LocalizedString message) : base(message)
		{
		}

		public CannotDeserializePartitionHintException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CannotDeserializePartitionHintException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
