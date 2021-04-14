using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ManifestItemChange : ManifestChangeBase
	{
		internal ManifestItemChange(bool isNewItem, PropValue[] headerPropertyValues, PropValue[] propertyValues)
		{
			this.isNewItem = isNewItem;
			this.propertyValues = Util.MergeArrays<PropValue>(new ICollection<PropValue>[]
			{
				headerPropertyValues,
				propertyValues
			});
		}

		public bool IsNewItem
		{
			get
			{
				return this.isNewItem;
			}
		}

		public PropValue[] PropertyValues
		{
			get
			{
				return this.propertyValues;
			}
		}

		private readonly bool isNewItem;

		private readonly PropValue[] propertyValues;
	}
}
