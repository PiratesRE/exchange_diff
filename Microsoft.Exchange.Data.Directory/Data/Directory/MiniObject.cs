using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public abstract class MiniObject : ADObject
	{
		public MiniObject()
		{
		}

		internal override void SetIsReadOnly(bool valueToSet)
		{
		}

		internal override void Initialize()
		{
			this.propertyBag.SetField(ADObjectSchema.ObjectState, ObjectState.Unchanged);
			this.propertyBag.ResetChangeTracking(ADObjectSchema.ObjectState);
			base.Initialize();
		}

		internal override object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
				object obj = null;
				if (!this.propertyBag.TryGetField(providerPropertyDefinition, ref obj))
				{
					if (!this.HasSupportingProperties(providerPropertyDefinition) && !providerPropertyDefinition.IsTaskPopulated)
					{
						throw new ValueNotPresentException(propertyDefinition.Name, (string)this.propertyBag[ADObjectSchema.Name]);
					}
					obj = this.propertyBag[providerPropertyDefinition];
				}
				return obj ?? providerPropertyDefinition.DefaultValue;
			}
			set
			{
				base[propertyDefinition] = value;
			}
		}

		internal bool HasSupportingProperties(ProviderPropertyDefinition propertyDefinition)
		{
			if (ADObjectSchema.IsCached == propertyDefinition || ADObjectSchema.WhenReadUTC == propertyDefinition || ADObjectSchema.OriginatingServer == propertyDefinition)
			{
				return true;
			}
			if (!propertyDefinition.IsCalculated)
			{
				return false;
			}
			foreach (ProviderPropertyDefinition key in propertyDefinition.SupportingProperties)
			{
				if (!this.propertyBag.Contains(key))
				{
					return false;
				}
			}
			return true;
		}
	}
}
