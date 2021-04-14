using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsDatabaseNotFoundException : ConfigurationSettingsException
	{
		public ConfigurationSettingsDatabaseNotFoundException(string id) : base(DirectoryStrings.ConfigurationSettingsDatabaseNotFound(id))
		{
			this.id = id;
		}

		public ConfigurationSettingsDatabaseNotFoundException(string id, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsDatabaseNotFound(id), innerException)
		{
			this.id = id;
		}

		protected ConfigurationSettingsDatabaseNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly string id;
	}
}
