using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Tnef;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	public struct MsgStoragePropertyReader
	{
		internal MsgStoragePropertyReader(MsgStorageReader reader)
		{
			this.Reader = reader;
		}

		public bool ReadNextProperty()
		{
			return this.Reader.SubStorageReader.ReadNextProperty();
		}

		public TnefPropertyTag PropertyTag
		{
			get
			{
				return this.Reader.SubStorageReader.PropertyTag;
			}
		}

		public TnefPropertyType PropertyType
		{
			get
			{
				return this.Reader.SubStorageReader.PropertyTag.TnefType;
			}
		}

		public bool IsNamedProperty
		{
			get
			{
				return this.Reader.SubStorageReader.PropertyTag.IsNamed;
			}
		}

		public TnefNameId PropertyNameId
		{
			get
			{
				TnefPropertyTag propertyTag = this.Reader.SubStorageReader.PropertyTag;
				if (!propertyTag.IsNamed)
				{
					throw new InvalidOperationException(MsgStorageStrings.NotANamedProperty);
				}
				TnefNameId result;
				if (!this.Reader.NamedPropertyList.TryGetValue(propertyTag.Id, out result))
				{
					throw new MsgStorageException(MsgStorageErrorCode.NamedPropertyNotFound, MsgStorageStrings.CorruptData);
				}
				return result;
			}
		}

		public bool IsMultiValuedProperty
		{
			get
			{
				return this.Reader.SubStorageReader.PropertyTag.IsMultiValued;
			}
		}

		public bool IsLargeValue
		{
			get
			{
				return this.Reader.SubStorageReader.IsLargeValue();
			}
		}

		public object ReadValue()
		{
			return this.Reader.SubStorageReader.ReadPropertyValue();
		}

		public Stream GetValueReadStream()
		{
			return this.Reader.SubStorageReader.OpenPropertyStream();
		}

		public Stream GetValueReadStream(TnefPropertyTag propertyTag)
		{
			return this.Reader.SubStorageReader.OpenPropertyStream(propertyTag);
		}

		public MsgStorageReader GetEmbeddedMessageReader()
		{
			return this.Reader.SubStorageReader.OpenAttachedMessage();
		}

		public int AttachMethod
		{
			get
			{
				return this.Reader.SubStorageReader.AttachMethod;
			}
		}

		internal MsgStorageReader Reader;
	}
}
