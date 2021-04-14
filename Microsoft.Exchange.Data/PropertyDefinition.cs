using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class PropertyDefinition
	{
		protected PropertyDefinition(string name, Type type)
		{
			this.name = name;
			this.type = type;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		public virtual ICollection<PropertyDefinition> RequiredPropertyDefinitionsWhenReading
		{
			get
			{
				if (this.requiredPropertyDefinitionsWhenReading == null)
				{
					this.requiredPropertyDefinitionsWhenReading = new List<PropertyDefinition>(1);
					this.requiredPropertyDefinitionsWhenReading.Add(this);
				}
				return this.requiredPropertyDefinitionsWhenReading;
			}
		}

		private string name;

		private Type type;

		private ICollection<PropertyDefinition> requiredPropertyDefinitionsWhenReading;
	}
}
