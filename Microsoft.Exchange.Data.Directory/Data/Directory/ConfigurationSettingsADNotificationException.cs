using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsADNotificationException : ConfigurationSettingsException
	{
		public ConfigurationSettingsADNotificationException() : base(DirectoryStrings.ConfigurationSettingsADNotificationError)
		{
		}

		public ConfigurationSettingsADNotificationException(Exception innerException) : base(DirectoryStrings.ConfigurationSettingsADNotificationError, innerException)
		{
		}

		protected ConfigurationSettingsADNotificationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
