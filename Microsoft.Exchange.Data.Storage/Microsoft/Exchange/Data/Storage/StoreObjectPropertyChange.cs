using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StoreObjectPropertyChange
	{
		public StoreObjectPropertyChange(PropertyDefinition propertyDefinition, object oldValue, object newValue) : this(propertyDefinition, oldValue, newValue, false)
		{
		}

		public StoreObjectPropertyChange(PropertyDefinition propertyDefinition, object oldValue, object newValue, bool isPropertyValidated)
		{
			this.propertyDefinition = propertyDefinition;
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.isPropertyValidated = isPropertyValidated;
		}

		public PropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
			set
			{
				this.propertyDefinition = value;
			}
		}

		public object OldValue
		{
			get
			{
				return this.oldValue;
			}
			set
			{
				this.oldValue = value;
			}
		}

		public object NewValue
		{
			get
			{
				return this.newValue;
			}
			set
			{
				this.newValue = value;
			}
		}

		public bool IsPropertyValidated
		{
			get
			{
				return this.isPropertyValidated;
			}
			set
			{
				this.isPropertyValidated = value;
			}
		}

		private PropertyDefinition propertyDefinition;

		private object oldValue;

		private object newValue;

		private bool isPropertyValidated;
	}
}
