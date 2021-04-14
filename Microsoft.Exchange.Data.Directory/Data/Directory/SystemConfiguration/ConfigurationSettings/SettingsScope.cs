using System;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[XmlInclude(typeof(SettingsDagScope))]
	[XmlInclude(typeof(SettingsProcessScope))]
	[XmlInclude(typeof(SettingsServerScope))]
	[XmlInclude(typeof(SettingsUserScope))]
	[XmlInclude(typeof(SettingsDatabaseScope))]
	[XmlInclude(typeof(SettingsForestScope))]
	[XmlInclude(typeof(SettingsGenericScope))]
	[XmlInclude(typeof(SettingsOrganizationScope))]
	[Serializable]
	public abstract class SettingsScope : XMLSerializableBase
	{
		public SettingsScope()
		{
			this.ScopeId = Guid.NewGuid();
		}

		public SettingsScope(string subType, string nameMatch) : this()
		{
			this.Restriction = new SettingsScopeRestriction(subType, nameMatch);
		}

		public SettingsScope(Guid? guidMatch) : this()
		{
			this.Restriction = new SettingsScopeRestriction(guidMatch.Value);
		}

		public SettingsScope(string nameMatch, string minVersion, string maxVersion) : this()
		{
			this.Restriction = new SettingsScopeRestriction(nameMatch, minVersion, maxVersion);
		}

		[XmlElement(ElementName = "Id")]
		public Guid ScopeId { get; set; }

		[XmlElement(ElementName = "Rs")]
		public SettingsScopeRestriction Restriction { get; set; }

		internal abstract int DefaultPriority { get; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[{0}", base.GetType().Name);
			if (this.Restriction != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(this.Restriction.ToString());
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		internal abstract QueryFilter ConstructScopeFilter(IConfigSchema schema);

		internal abstract void Validate(IConfigSchema schema);
	}
}
