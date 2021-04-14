using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsNotFoundForGroupException : ConfigurationSettingsException
	{
		public ConfigurationSettingsNotFoundForGroupException(string groupName, string key) : base(DirectoryStrings.ConfigurationSettingsNotFoundForGroup(groupName, key))
		{
			this.groupName = groupName;
			this.key = key;
		}

		public ConfigurationSettingsNotFoundForGroupException(string groupName, string key, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsNotFoundForGroup(groupName, key), innerException)
		{
			this.groupName = groupName;
			this.key = key;
		}

		protected ConfigurationSettingsNotFoundForGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupName = (string)info.GetValue("groupName", typeof(string));
			this.key = (string)info.GetValue("key", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupName", this.groupName);
			info.AddValue("key", this.key);
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		private readonly string groupName;

		private readonly string key;
	}
}
