using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsADConfigDriverException : ConfigurationSettingsException
	{
		public ConfigurationSettingsADConfigDriverException() : base(DirectoryStrings.ConfigurationSettingsADConfigDriverError)
		{
		}

		public ConfigurationSettingsADConfigDriverException(Exception innerException) : base(DirectoryStrings.ConfigurationSettingsADConfigDriverError, innerException)
		{
		}

		protected ConfigurationSettingsADConfigDriverException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
