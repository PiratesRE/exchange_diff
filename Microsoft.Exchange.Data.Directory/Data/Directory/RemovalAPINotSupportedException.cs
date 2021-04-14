using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemovalAPINotSupportedException : DataSourceOperationException
	{
		public RemovalAPINotSupportedException() : base(DirectoryStrings.ErrorRemovalNotSupported)
		{
		}

		public RemovalAPINotSupportedException(Exception innerException) : base(DirectoryStrings.ErrorRemovalNotSupported, innerException)
		{
		}

		protected RemovalAPINotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
