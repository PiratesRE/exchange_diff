using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsGroupNotFoundException : ConfigurationSettingsException
	{
		public ConfigurationSettingsGroupNotFoundException(string name) : base(DirectoryStrings.ConfigurationSettingsGroupNotFound(name))
		{
			this.name = name;
		}

		public ConfigurationSettingsGroupNotFoundException(string name, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsGroupNotFound(name), innerException)
		{
			this.name = name;
		}

		protected ConfigurationSettingsGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
