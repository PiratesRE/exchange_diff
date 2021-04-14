using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.DirSync
{
	[Serializable]
	public abstract class ADDirSyncResult : ADRawEntry
	{
		internal ADDirSyncResult(ADPropertyBag propertyBag) : base(propertyBag)
		{
		}

		protected ADDirSyncResult()
		{
		}

		public bool IsDeleted
		{
			get
			{
				return (bool)this.propertyBag[ADDirSyncResultSchema.IsDeleted];
			}
		}

		public bool IsNew
		{
			get
			{
				return this.Contains(ADDirSyncResultSchema.WhenCreated);
			}
		}

		public bool IsRenamed
		{
			get
			{
				return this.Contains(ADDirSyncResultSchema.Name) && !this.IsNew;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return (ADObjectId)this.propertyBag[ADDirSyncResultSchema.Id];
			}
		}

		public string DistinguishedName
		{
			get
			{
				ADObjectId adobjectId = (ADObjectId)this.Identity;
				if (adobjectId != null)
				{
					return adobjectId.DistinguishedName;
				}
				return string.Empty;
			}
		}

		internal override ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				return null;
			}
		}

		internal override object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.GetPropertyValue<object>(propertyDefinition);
			}
			set
			{
				base[propertyDefinition] = value;
			}
		}

		internal ADDirSyncProperty<T> GetPropertyValue<T>(PropertyDefinition property)
		{
			if (this.Contains((ProviderPropertyDefinition)property))
			{
				return new ADDirSyncProperty<T>((T)((object)this.propertyBag[property]));
			}
			return ADDirSyncProperty<T>.NoChange;
		}

		internal IDictionary<ADPropertyDefinition, object> GetChangedProperties()
		{
			ArrayList arrayList = new ArrayList(this.propertyBag.Keys);
			arrayList.Remove(ADDirSyncResultSchema.Id);
			arrayList.Remove(ADObjectSchema.ObjectState);
			return this.GetChangedProperties(arrayList);
		}

		internal IDictionary<ADPropertyDefinition, object> GetChangedProperties(ICollection properties)
		{
			return ADDirSyncHelper.GetChangedProperties(properties, this.propertyBag);
		}

		internal abstract ADDirSyncResult CreateInstance(PropertyBag propertyBag);

		internal override bool SkipFullPropertyValidation(ProviderPropertyDefinition propertyDefinition)
		{
			return true;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
		}

		private bool Contains(ProviderPropertyDefinition property)
		{
			return ADDirSyncHelper.ContainsProperty(this.propertyBag, property);
		}
	}
}
