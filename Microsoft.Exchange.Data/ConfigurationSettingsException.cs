using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsException : LocalizedException
	{
		public ConfigurationSettingsException(LocalizedString message) : base(message)
		{
		}

		public ConfigurationSettingsException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ConfigurationSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
