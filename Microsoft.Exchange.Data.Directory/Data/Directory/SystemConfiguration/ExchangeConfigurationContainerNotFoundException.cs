using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeConfigurationContainerNotFoundException : MandatoryContainerNotFoundException
	{
		public ExchangeConfigurationContainerNotFoundException() : base(DirectoryStrings.ExchangeConfigurationContainerNotFoundException)
		{
		}

		public ExchangeConfigurationContainerNotFoundException(Exception innerException) : base(DirectoryStrings.ExchangeConfigurationContainerNotFoundException, innerException)
		{
		}

		protected ExchangeConfigurationContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
