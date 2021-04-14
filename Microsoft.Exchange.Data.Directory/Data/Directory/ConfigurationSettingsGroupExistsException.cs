using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsGroupExistsException : ConfigurationSettingsException
	{
		public ConfigurationSettingsGroupExistsException(string name) : base(DirectoryStrings.ConfigurationSettingsGroupExists(name))
		{
			this.name = name;
		}

		public ConfigurationSettingsGroupExistsException(string name, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsGroupExists(name), innerException)
		{
			this.name = name;
		}

		protected ConfigurationSettingsGroupExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
