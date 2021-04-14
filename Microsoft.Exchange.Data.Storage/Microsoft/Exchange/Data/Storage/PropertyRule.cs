using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class PropertyRule
	{
		protected internal IEnumerable<PropertyDefinition> ReadProperties { get; protected set; }

		protected internal IEnumerable<PropertyDefinition> WriteProperties { get; protected set; }

		protected internal Action<ILocationIdentifierSetter> OnSetWriteEnforceLocationIdentifier { get; private set; }

		protected PropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, params PropertyReference[] references) : this(name, onSetWriteEnforceLocationIdentifier, (IEnumerable<PropertyReference>)references)
		{
		}

		protected PropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, IEnumerable<PropertyReference> references)
		{
			this.Name = name;
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			List<PropertyDefinition> list2 = new List<PropertyDefinition>();
			foreach (PropertyReference propertyReference in references)
			{
				if (propertyReference.AccessType == PropertyAccess.None)
				{
					throw new ArgumentException("invalid type in property reference");
				}
				if (list.Contains(propertyReference.Property) || list2.Contains(propertyReference.Property))
				{
					throw new ArgumentException("duplicate properties in collection");
				}
				if ((propertyReference.AccessType & PropertyAccess.Read) == PropertyAccess.Read)
				{
					list.Add(propertyReference.Property);
				}
				if ((propertyReference.AccessType & PropertyAccess.Write) == PropertyAccess.Write)
				{
					list2.Add(propertyReference.Property);
				}
			}
			this.ReadProperties = list.ToArray();
			this.WriteProperties = list2.ToArray();
			this.OnSetWriteEnforceLocationIdentifier = onSetWriteEnforceLocationIdentifier;
		}

		internal bool WriteEnforce(ICorePropertyBag propertyBag)
		{
			bool flag = this.WriteEnforceRule(propertyBag);
			if (flag && this.OnSetWriteEnforceLocationIdentifier != null)
			{
				this.OnSetWriteEnforceLocationIdentifier(propertyBag);
			}
			return flag;
		}

		protected abstract bool WriteEnforceRule(ICorePropertyBag propertyBag);

		public override string ToString()
		{
			return this.Name ?? string.Empty;
		}

		public static bool CheckCircularDependency(IEnumerable<PropertyRule> rules)
		{
			List<PropertyDefinition> readList = new List<PropertyDefinition>();
			List<PropertyDefinition> writeList = new List<PropertyDefinition>();
			foreach (PropertyRule propertyRule in rules)
			{
				if (!propertyRule.ReadProperties.Any((PropertyDefinition p) => writeList.Contains(p)))
				{
					if (!propertyRule.WriteProperties.Any((PropertyDefinition p) => readList.Contains(p)))
					{
						if (!propertyRule.WriteProperties.Any((PropertyDefinition p) => writeList.Contains(p)))
						{
							readList.AddRange(propertyRule.ReadProperties);
							writeList.AddRange(propertyRule.WriteProperties);
							continue;
						}
					}
				}
				return false;
			}
			return true;
		}

		public readonly string Name;
	}
}
