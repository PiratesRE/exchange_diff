using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationPersistableDictionary
	{
		public MigrationPersistableDictionary()
		{
			this.propertyBag = new PersistableDictionary();
		}

		protected PersistableDictionary PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public string Serialize()
		{
			return this.propertyBag.Serialize();
		}

		protected void DeserializeData(string serializedData)
		{
			this.propertyBag = PersistableDictionary.Create(serializedData);
		}

		protected T Get<T>(string key)
		{
			return this.propertyBag.Get<T>(key);
		}

		protected void Set<T>(string key, T value)
		{
			this.propertyBag.Set<T>(key, value);
		}

		protected T? GetNullable<T>(string key) where T : struct
		{
			if (!this.propertyBag.Contains(key))
			{
				return null;
			}
			return this.propertyBag.Get<T?>(key);
		}

		protected void SetNullable<T>(string key, T? value) where T : struct
		{
			if (value != null)
			{
				this.propertyBag.Set<T>(key, value.Value);
				return;
			}
			this.propertyBag.Remove(key);
		}

		private PersistableDictionary propertyBag;
	}
}
