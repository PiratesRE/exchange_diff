using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Search.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IndexStatusRegistryNotFoundException : IndexStatusException
	{
		public IndexStatusRegistryNotFoundException() : base(Strings.IndexStatusRegistryNotFound)
		{
		}

		public IndexStatusRegistryNotFoundException(Exception innerException) : base(Strings.IndexStatusRegistryNotFound, innerException)
		{
		}

		protected IndexStatusRegistryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
