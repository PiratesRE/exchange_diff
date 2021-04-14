using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public struct PropertyChangeTracker<TObject, TPropertyDefinition> : IPropertyChangeTracker<TPropertyDefinition>
	{
		public PropertyChangeTracker(TObject trackingObject, Func<TObject, TPropertyDefinition, bool> isPropertySet)
		{
			this = default(PropertyChangeTracker<TObject, TPropertyDefinition>);
			this.TrackingObject = trackingObject;
			this.isPropertySet = isPropertySet;
		}

		public TObject TrackingObject { get; private set; }

		public bool IsPropertySet(TPropertyDefinition property)
		{
			return this.isPropertySet(this.TrackingObject, property);
		}

		private readonly Func<TObject, TPropertyDefinition, bool> isPropertySet;
	}
}
