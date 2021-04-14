using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ABRawEntry : IReadOnlyPropertyBag
	{
		protected ABRawEntry(ABSession ownerSession, ABPropertyDefinitionCollection propertyDefinitionCollection)
		{
			if (ownerSession == null)
			{
				throw new ArgumentNullException("ownerSession");
			}
			if (propertyDefinitionCollection == null)
			{
				throw new ArgumentNullException("propertyDefinitionCollection");
			}
			this.ownerSession = ownerSession;
			this.propertyDefinitionCollection = propertyDefinitionCollection;
		}

		public ABPropertyDefinitionCollection PropertyDefinitionCollection
		{
			get
			{
				return this.propertyDefinitionCollection;
			}
			internal set
			{
				this.propertyDefinitionCollection = value;
			}
		}

		object IReadOnlyPropertyBag.this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this[(ABPropertyDefinition)propertyDefinition];
			}
		}

		public virtual object this[ABPropertyDefinition property]
		{
			get
			{
				object result;
				if (!this.TryGetValue(property, out result))
				{
					throw new KeyNotFoundException(property.Name);
				}
				return result;
			}
		}

		public bool TryGetValue(ABPropertyDefinition property, out object value)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (!this.propertyDefinitionCollection.Contains(property))
			{
				throw new KeyNotFoundException(property.Name);
			}
			bool result = this.InternalTryGetValue(property, out value);
			if (value != null && !property.Type.IsInstanceOfType(value))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Provider '{0}' returned type '{1}' rather than '{2}' for property '{3}'", new object[]
				{
					this.ownerSession.ProviderName,
					(value == null) ? "null reference" : value.GetType().Name,
					property.Type.Name,
					property.Name
				}));
			}
			return result;
		}

		public bool TryGetValue<T>(ABPropertyDefinition property, out T value)
		{
			object obj;
			if (!this.TryGetValue(property, out obj))
			{
				value = default(T);
				return false;
			}
			value = (T)((object)obj);
			return true;
		}

		object[] IReadOnlyPropertyBag.GetProperties(ICollection<PropertyDefinition> propertyDefinitionCollection)
		{
			if (propertyDefinitionCollection == null)
			{
				throw new ArgumentNullException("propertyDefinitionCollection");
			}
			object[] array = new object[propertyDefinitionCollection.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitionCollection)
			{
				ABPropertyDefinition abpropertyDefinition = (ABPropertyDefinition)propertyDefinition;
				if (abpropertyDefinition == null)
				{
					throw new ArgumentException("Property definition collection contains null entries.", "propertyDefinitionCollection");
				}
				array[num++] = this[abpropertyDefinition];
			}
			return array;
		}

		protected virtual bool InternalTryGetValue(ABPropertyDefinition property, out object value)
		{
			value = null;
			return false;
		}

		private ABSession ownerSession;

		private ABPropertyDefinitionCollection propertyDefinitionCollection;
	}
}
