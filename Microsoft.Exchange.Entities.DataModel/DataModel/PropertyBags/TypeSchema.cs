using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public abstract class TypeSchema
	{
		protected void RegisterPropertyDefinition(PropertyDefinition newProperty)
		{
			try
			{
				this.registeredProperties.Add(newProperty.Name, newProperty);
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidOperationException(string.Format("A property with the same name has already been registered (Name: {0}).", newProperty.Name), innerException);
			}
		}

		private readonly Dictionary<string, PropertyDefinition> registeredProperties = new Dictionary<string, PropertyDefinition>();
	}
}
