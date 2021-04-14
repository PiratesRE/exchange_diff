using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidConfigurationObjectTypeException : TroubleshootingCmdletException
	{
		public InvalidConfigurationObjectTypeException(LocalizedString message) : base(message)
		{
		}

		public InvalidConfigurationObjectTypeException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidConfigurationObjectTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
