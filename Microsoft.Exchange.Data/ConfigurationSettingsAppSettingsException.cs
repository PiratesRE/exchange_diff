using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsAppSettingsException : ConfigurationSettingsException
	{
		public ConfigurationSettingsAppSettingsException() : base(DataStrings.ConfigurationSettingsAppSettingsError)
		{
		}

		public ConfigurationSettingsAppSettingsException(Exception innerException) : base(DataStrings.ConfigurationSettingsAppSettingsError, innerException)
		{
		}

		protected ConfigurationSettingsAppSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
