using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class GenericSyncPropertyFactory<T> : IGenericSyncPropertyFactory
	{
		internal GenericSyncPropertyFactory()
		{
			this.defaultValueSingle = SyncProperty<T>.NoChange;
			ISyncProperty syncProperty;
			if (!(typeof(T) == typeof(ProxyAddress)))
			{
				ISyncProperty noChange = SyncProperty<MultiValuedProperty<T>>.NoChange;
				syncProperty = noChange;
			}
			else
			{
				syncProperty = SyncProperty<ProxyAddressCollection>.NoChange;
			}
			this.defaultValueMVP = syncProperty;
		}

		public object Create(object value, bool multiValued)
		{
			if (!multiValued)
			{
				return new SyncProperty<T>((T)((object)value));
			}
			if (typeof(T) == typeof(ProxyAddress))
			{
				return new SyncProperty<ProxyAddressCollection>((ProxyAddressCollection)value);
			}
			return new SyncProperty<MultiValuedProperty<T>>((MultiValuedProperty<T>)value);
		}

		public object GetDefault(bool multiValued)
		{
			if (!multiValued)
			{
				return this.defaultValueSingle;
			}
			return this.defaultValueMVP;
		}

		private readonly ISyncProperty defaultValueSingle;

		private readonly ISyncProperty defaultValueMVP;
	}
}
