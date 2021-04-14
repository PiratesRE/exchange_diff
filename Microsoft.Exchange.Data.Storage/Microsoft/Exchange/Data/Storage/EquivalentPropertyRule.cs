using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EquivalentPropertyRule : PropertyRule
	{
		public EquivalentPropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, params NativeStorePropertyDefinition[] properties) : base(name, onSetWriteEnforceLocationIdentifier, new PropertyReference[0])
		{
			if (properties.Length < 2)
			{
				throw new ArgumentException("At least 2 properties needed for EquivalentPropertyRule.");
			}
			List<PropertyDefinition> list = new List<PropertyDefinition>(properties.Length);
			list.Add(properties[0]);
			Type type = properties[0].Type;
			for (int i = 1; i < properties.Length; i++)
			{
				if (properties[i].Type != type)
				{
					throw new ArgumentException("Properties should be same type for EquivalentPropertyRule.");
				}
				if (list.Contains(properties[i]))
				{
					throw new ArgumentException("duplicate properties in collection");
				}
				list.Add(properties[i]);
			}
			base.ReadProperties = (base.WriteProperties = list);
		}

		protected override bool WriteEnforceRule(ICorePropertyBag propertyBag)
		{
			bool flag = false;
			object obj = null;
			foreach (PropertyDefinition propertyDefinition in base.ReadProperties)
			{
				if (propertyBag.IsPropertyDirty(propertyDefinition))
				{
					flag = true;
					obj = propertyBag.TryGetProperty(propertyDefinition);
					if (!PropertyError.IsPropertyNotFound(obj))
					{
						break;
					}
				}
			}
			if (flag)
			{
				foreach (PropertyDefinition property in base.WriteProperties)
				{
					propertyBag.SetOrDeleteProperty(property, obj);
				}
			}
			return flag;
		}
	}
}
