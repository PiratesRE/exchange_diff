using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToAccessIrmConfigurationException : LocalizedException
	{
		public FailedToAccessIrmConfigurationException() : base(Strings.FailedToAccessIrmConfiguration)
		{
		}

		public FailedToAccessIrmConfigurationException(Exception innerException) : base(Strings.FailedToAccessIrmConfiguration, innerException)
		{
		}

		protected FailedToAccessIrmConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
