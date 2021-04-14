using System;
using System.IO;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public struct VariantConfigurationSection
	{
		internal VariantConfigurationSection(string settingsFile, string name, Type type, bool isPublic)
		{
			this.FileName = settingsFile;
			this.SectionName = name;
			this.Type = type;
			this.IsPublic = isPublic;
		}

		public string Name
		{
			get
			{
				return Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(this.FileName));
			}
		}

		public VariantConfigurationOverride CreateOverride(params string[] parameters)
		{
			if (this.FileName.EndsWith(".flight.ini", StringComparison.InvariantCultureIgnoreCase))
			{
				return new VariantConfigurationFlightOverride(this.SectionName, parameters);
			}
			return new VariantConfigurationSettingOverride(this.Name, this.SectionName, parameters);
		}

		public readonly string FileName;

		public readonly string SectionName;

		public readonly Type Type;

		public readonly bool IsPublic;
	}
}
