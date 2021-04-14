using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class PhysicalColumnPropertyMapping : PropertyMapping
	{
		internal PhysicalColumnPropertyMapping(StorePropTag propertyTag, Column column, Func<Context, ISimplePropertyBag, object, ErrorCode> valueSetter, StreamGetterDelegate readStreamGetter, StreamGetterDelegate writeStreamGetter, PhysicalColumn physicalColumn, bool canBeSet, bool primary, bool reservedPropId, bool list, bool tailSet) : base(PropertyMappingKind.PhysicalColumn, propertyTag, column, valueSetter, readStreamGetter, writeStreamGetter, primary, reservedPropId, list)
		{
			this.physicalColumn = physicalColumn;
			this.canBeSet = canBeSet;
			this.tailSet = tailSet;
		}

		public override bool CanBeSet
		{
			get
			{
				return this.canBeSet || base.ValueSetter != null;
			}
		}

		public PhysicalColumn PhysicalColumn
		{
			get
			{
				return this.physicalColumn;
			}
		}

		public override object GetPropertyValue(Context context, ISimpleReadOnlyPropertyBag bag)
		{
			return bag.GetPhysicalColumnValue(context, this.physicalColumn);
		}

		public override ErrorCode SetPropertyValue(Context context, ISimplePropertyBag bag, object value)
		{
			if (!this.CanBeSet)
			{
				return ErrorCode.CreateNoAccess((LID)39704U, base.PropertyTag.PropTag);
			}
			if (base.ValueSetter != null)
			{
				ErrorCode first = ErrorCode.NoError;
				first = base.ValueSetter(context, bag, value);
				if (first != ErrorCode.NoError || !this.tailSet)
				{
					return first.Propagate((LID)39744U);
				}
			}
			if (value == null && !this.physicalColumn.IsNullable)
			{
				return ErrorCode.CreateNoAccess((LID)56088U, base.PropertyTag.PropTag);
			}
			int num;
			if (value != null && (this.physicalColumn.Type == typeof(string) || this.physicalColumn.Type == typeof(byte[])) && this.physicalColumn.TryGetColumnMaxSize(out num))
			{
				if (this.physicalColumn.Type == typeof(string))
				{
					string text = (string)value;
					if (text.Length > num)
					{
						return ErrorCode.CreateTooBig((LID)59472U);
					}
				}
				else
				{
					byte[] array = (byte[])value;
					if (array.Length > num)
					{
						return ErrorCode.CreateTooBig((LID)34896U);
					}
				}
			}
			bag.SetPhysicalColumn(context, this.physicalColumn, value);
			return ErrorCode.NoError;
		}

		public override bool IsPropertyChanged(Context context, ISimplePropertyBagWithChangeTracking bag)
		{
			return bag.IsPhysicalColumnChanged(context, this.physicalColumn);
		}

		public override ErrorCode OpenPropertyReadStream(Context context, ISimplePropertyBag bag, out Stream readStream)
		{
			if (base.ReadStreamGetter != null)
			{
				return base.ReadStreamGetter(context, bag, out readStream);
			}
			if (!this.physicalColumn.StreamSupport)
			{
				readStream = null;
				return ErrorCode.CreateNotSupported((LID)60184U, base.PropertyTag.PropTag);
			}
			return bag.OpenPhysicalColumnReadStream(context, this.physicalColumn, out readStream);
		}

		public override ErrorCode OpenPropertyWriteStream(Context context, ISimplePropertyBag bag, out Stream writeStream)
		{
			if (base.WriteStreamGetter != null)
			{
				return base.WriteStreamGetter(context, bag, out writeStream);
			}
			if (!this.physicalColumn.StreamSupport)
			{
				writeStream = null;
				return ErrorCode.CreateNotSupported((LID)45848U, base.PropertyTag.PropTag);
			}
			return bag.OpenPhysicalColumnWriteStream(context, this.physicalColumn, out writeStream);
		}

		private readonly bool canBeSet;

		private readonly bool tailSet;

		private PhysicalColumn physicalColumn;
	}
}
