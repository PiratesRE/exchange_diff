using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServersContainerNotFoundException : MandatoryContainerNotFoundException
	{
		public ServersContainerNotFoundException() : base(DirectoryStrings.ServersContainerNotFoundException)
		{
		}

		public ServersContainerNotFoundException(Exception innerException) : base(DirectoryStrings.ServersContainerNotFoundException, innerException)
		{
		}

		protected ServersContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
