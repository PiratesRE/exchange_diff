using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsUnsupportedVersionException : ConfigurationSettingsException
	{
		public ConfigurationSettingsUnsupportedVersionException(string version) : base(DirectoryStrings.ConfigurationSettingsUnsupportedVersion(version))
		{
			this.version = version;
		}

		public ConfigurationSettingsUnsupportedVersionException(string version, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsUnsupportedVersion(version), innerException)
		{
			this.version = version;
		}

		protected ConfigurationSettingsUnsupportedVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.version = (string)info.GetValue("version", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("version", this.version);
		}

		public string Version
		{
			get
			{
				return this.version;
			}
		}

		private readonly string version;
	}
}
