using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors
{
	internal sealed class StorageBodyPropertyAccessor<TItem> : StoragePropertyAccessor<TItem, ItemBody> where TItem : IItem
	{
		public StorageBodyPropertyAccessor() : base(false, PropertyChangeMetadata.PropertyGroup.Body, null)
		{
		}

		protected override bool PerformTryGetValue(TItem container, out ItemBody value)
		{
			char[] buffer = new char[32768];
			value = container.GetEntityBody(buffer);
			return true;
		}

		protected override void PerformSet(TItem container, ItemBody value)
		{
			value.SetOnStorageItem(container, !container.IsNew);
		}

		private const int RpcMaxBufferSize = 32768;
	}
}
