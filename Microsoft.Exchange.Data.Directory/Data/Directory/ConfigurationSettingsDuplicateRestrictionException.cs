using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsDuplicateRestrictionException : ConfigurationSettingsException
	{
		public ConfigurationSettingsDuplicateRestrictionException(string name, string groupName) : base(DirectoryStrings.ConfigurationSettingsDuplicateRestriction(name, groupName))
		{
			this.name = name;
			this.groupName = groupName;
		}

		public ConfigurationSettingsDuplicateRestrictionException(string name, string groupName, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsDuplicateRestriction(name, groupName), innerException)
		{
			this.name = name;
			this.groupName = groupName;
		}

		protected ConfigurationSettingsDuplicateRestrictionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.groupName = (string)info.GetValue("groupName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("groupName", this.groupName);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		private readonly string name;

		private readonly string groupName;
	}
}
