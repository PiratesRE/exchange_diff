using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidShadowRedundancyConfigurationException : LocalizedException
	{
		public InvalidShadowRedundancyConfigurationException(LocalizedString message) : base(message)
		{
		}

		public InvalidShadowRedundancyConfigurationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidShadowRedundancyConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
