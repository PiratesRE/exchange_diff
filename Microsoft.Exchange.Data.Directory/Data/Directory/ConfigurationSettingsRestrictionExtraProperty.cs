using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsRestrictionExtraProperty : ConfigurationSettingsException
	{
		public ConfigurationSettingsRestrictionExtraProperty(string name, string propertyName) : base(DirectoryStrings.ConfigurationSettingsRestrictionExtraProperty(name, propertyName))
		{
			this.name = name;
			this.propertyName = propertyName;
		}

		public ConfigurationSettingsRestrictionExtraProperty(string name, string propertyName, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsRestrictionExtraProperty(name, propertyName), innerException)
		{
			this.name = name;
			this.propertyName = propertyName;
		}

		protected ConfigurationSettingsRestrictionExtraProperty(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("propertyName", this.propertyName);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private readonly string name;

		private readonly string propertyName;
	}
}
