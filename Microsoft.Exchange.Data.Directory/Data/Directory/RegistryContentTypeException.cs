using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RegistryContentTypeException : DataSourceOperationException
	{
		public RegistryContentTypeException() : base(DirectoryStrings.RegistryContentTypeException)
		{
		}

		public RegistryContentTypeException(Exception innerException) : base(DirectoryStrings.RegistryContentTypeException, innerException)
		{
		}

		protected RegistryContentTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
