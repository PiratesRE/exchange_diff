using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class PropertyMapping
	{
		internal PropertyMapping(PropertyMappingKind kind, StorePropTag propertyTag, Column column, Func<Context, ISimplePropertyBag, object, ErrorCode> valueSetter, StreamGetterDelegate readStreamGetter, StreamGetterDelegate writeStreamGetter, bool primary, bool reservedPropId, bool list)
		{
			this.propertyTag = propertyTag;
			this.mappingKind = kind;
			this.column = column;
			this.valueSetter = valueSetter;
			this.readStreamGetter = readStreamGetter;
			this.writeStreamGetter = writeStreamGetter;
			this.primary = primary;
			this.reservedPropId = reservedPropId;
			this.list = list;
		}

		public StorePropTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public PropertyMappingKind MappingKind
		{
			get
			{
				return this.mappingKind;
			}
		}

		public Column Column
		{
			get
			{
				return this.column;
			}
		}

		public Func<Context, ISimplePropertyBag, object, ErrorCode> ValueSetter
		{
			get
			{
				return this.valueSetter;
			}
		}

		public StreamGetterDelegate ReadStreamGetter
		{
			get
			{
				return this.readStreamGetter;
			}
		}

		public StreamGetterDelegate WriteStreamGetter
		{
			get
			{
				return this.writeStreamGetter;
			}
		}

		public bool IsPrimary
		{
			get
			{
				return this.primary;
			}
		}

		public bool IsReservedPropId
		{
			get
			{
				return this.reservedPropId;
			}
		}

		public bool ShouldBeListed
		{
			get
			{
				return this.list;
			}
		}

		public abstract bool CanBeSet { get; }

		public abstract object GetPropertyValue(Context context, ISimpleReadOnlyPropertyBag bag);

		public abstract ErrorCode SetPropertyValue(Context context, ISimplePropertyBag bag, object value);

		public abstract bool IsPropertyChanged(Context context, ISimplePropertyBagWithChangeTracking bag);

		public virtual ErrorCode OpenPropertyReadStream(Context context, ISimplePropertyBag bag, out Stream readStream)
		{
			if (this.ReadStreamGetter == null)
			{
				readStream = null;
				return ErrorCode.CreateNotSupported((LID)47896U, this.PropertyTag.PropTag);
			}
			return this.ReadStreamGetter(context, bag, out readStream);
		}

		public virtual ErrorCode OpenPropertyWriteStream(Context context, ISimplePropertyBag bag, out Stream writeStream)
		{
			if (this.WriteStreamGetter == null)
			{
				writeStream = null;
				return ErrorCode.CreateNotSupported((LID)64280U, this.PropertyTag.PropTag);
			}
			return this.WriteStreamGetter(context, bag, out writeStream);
		}

		private readonly PropertyMappingKind mappingKind;

		private readonly StorePropTag propertyTag;

		private readonly Column column;

		private readonly Func<Context, ISimplePropertyBag, object, ErrorCode> valueSetter;

		private StreamGetterDelegate readStreamGetter;

		private StreamGetterDelegate writeStreamGetter;

		private bool primary;

		private bool reservedPropId;

		private bool list;
	}
}
