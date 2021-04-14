using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbMountNotAllowedDueToRegistryConfigurationException : AmServerException
	{
		public AmDbMountNotAllowedDueToRegistryConfigurationException() : base(ServerStrings.AmDbMountNotAllowedDueToRegistryConfigurationException)
		{
		}

		public AmDbMountNotAllowedDueToRegistryConfigurationException(Exception innerException) : base(ServerStrings.AmDbMountNotAllowedDueToRegistryConfigurationException, innerException)
		{
		}

		protected AmDbMountNotAllowedDueToRegistryConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
