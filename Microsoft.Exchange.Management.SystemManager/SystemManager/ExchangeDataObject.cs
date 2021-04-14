using System;
using System.ComponentModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager
{
	public abstract class ExchangeDataObject : IConfigurable, INotifyPropertyChanged
	{
		public ExchangeDataObject()
		{
			this.propertyBag = new ADPropertyBag();
			this.objectState = ObjectState.Unchanged;
		}

		internal PropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		internal virtual object this[ProviderPropertyDefinition key]
		{
			get
			{
				return this.PropertyBag[key];
			}
			set
			{
				bool flag = object.Equals(this.PropertyBag[key], value);
				this.propertyBag[key] = value;
				if (!flag)
				{
					this.OnPropertyChanged(new PropertyChangedEventArgs(key.Name));
				}
			}
		}

		public virtual object this[string key]
		{
			get
			{
				foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
				{
					ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
					if (providerPropertyDefinition.Name.Equals(key))
					{
						return this.PropertyBag[providerPropertyDefinition];
					}
				}
				return null;
			}
			set
			{
				foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
				{
					ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
					if (providerPropertyDefinition.Name.Equals(key))
					{
						this.PropertyBag[providerPropertyDefinition] = value;
						break;
					}
				}
			}
		}

		internal abstract ObjectSchema Schema { get; }

		public virtual ObjectId Identity
		{
			get
			{
				return null;
			}
		}

		public virtual ValidationError[] Validate()
		{
			return ValidationError.None;
		}

		public bool IsValid
		{
			get
			{
				return 0 == this.Validate().Length;
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				if (this.objectState == ObjectState.Changed)
				{
					return ObjectState.Changed;
				}
				foreach (object obj in this.propertyBag.Keys)
				{
					ProviderPropertyDefinition key = (ProviderPropertyDefinition)obj;
					if (this.propertyBag.IsChanged(key))
					{
						this.objectState = ObjectState.Changed;
						return ObjectState.Changed;
					}
				}
				return ObjectState.Unchanged;
			}
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			ExchangeDataObject exchangeDataObject = source as ExchangeDataObject;
			if (exchangeDataObject == null)
			{
				throw new ArgumentOutOfRangeException("source", "Dev Error: Copying changes from invalid object type");
			}
			foreach (object obj in exchangeDataObject.propertyBag.Keys)
			{
				ProviderPropertyDefinition key = (ProviderPropertyDefinition)obj;
				if (exchangeDataObject.propertyBag.IsChanged(key))
				{
					this[key] = exchangeDataObject[key];
				}
			}
		}

		public void CopyFrom(ExchangeDataObject source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.propertyBag = (PropertyBag)source.PropertyBag.Clone();
			this.ResetChangeTracking();
		}

		public void ResetChangeTracking()
		{
			this.propertyBag.ResetChangeTracking();
			this.objectState = ObjectState.Unchanged;
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler propertyChangedEventHandler = (PropertyChangedEventHandler)this.Events[ExchangeDataObject.EventPropertyChanged];
			if (propertyChangedEventHandler != null)
			{
				propertyChangedEventHandler(this, e);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				this.Events.AddHandler(ExchangeDataObject.EventPropertyChanged, value);
			}
			remove
			{
				this.Events.RemoveHandler(ExchangeDataObject.EventPropertyChanged, value);
			}
		}

		protected EventHandlerList Events
		{
			get
			{
				return this.events;
			}
		}

		private PropertyBag propertyBag;

		private ObjectState objectState;

		private static readonly object EventPropertyChanged = new object();

		private EventHandlerList events = new EventHandlerList();
	}
}
