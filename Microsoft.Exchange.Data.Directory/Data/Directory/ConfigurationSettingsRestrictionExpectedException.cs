using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigurationSettingsRestrictionExpectedException : ConfigurationSettingsException
	{
		public ConfigurationSettingsRestrictionExpectedException(string name) : base(DirectoryStrings.ConfigurationSettingsRestrictionExpected(name))
		{
			this.name = name;
		}

		public ConfigurationSettingsRestrictionExpectedException(string name, Exception innerException) : base(DirectoryStrings.ConfigurationSettingsRestrictionExpected(name), innerException)
		{
			this.name = name;
		}

		protected ConfigurationSettingsRestrictionExpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
