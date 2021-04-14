using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public abstract class VariantConfigurationComponent
	{
		protected VariantConfigurationComponent(string componentName)
		{
			this.ComponentName = componentName;
		}

		public string FileName
		{
			get
			{
				return "Settings\\" + this.ComponentName + ".settings.ini";
			}
		}

		public string ComponentName { get; private set; }

		public VariantConfigurationSection this[string name]
		{
			get
			{
				return this.sections[name];
			}
		}

		public IEnumerable<string> GetSections(bool includeInternal)
		{
			if (includeInternal)
			{
				return this.sections.Keys;
			}
			return from section in this.sections.Keys
			where this[section].IsPublic
			select section;
		}

		public bool Contains(string name, bool includeInternal)
		{
			return this.sections.ContainsKey(name) && (includeInternal || this[name].IsPublic);
		}

		protected void Add(VariantConfigurationSection section)
		{
			this.sections.Add(section.SectionName, section);
		}

		private Dictionary<string, VariantConfigurationSection> sections = new Dictionary<string, VariantConfigurationSection>(StringComparer.OrdinalIgnoreCase);
	}
}
