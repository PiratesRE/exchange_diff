using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct Property
	{
		public Property(StorePropTag tag, object value)
		{
			this.tag = tag;
			this.value = value;
		}

		public bool IsError
		{
			get
			{
				return this.tag.PropType == PropertyType.Error;
			}
		}

		public StorePropTag Tag
		{
			get
			{
				return this.tag;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public static Property NotFoundError(StorePropTag tag)
		{
			return new Property(tag.ConvertToError(), Property.BoxedErrorCodeNotFound);
		}

		public static Property NotEnoughMemoryError(StorePropTag tag)
		{
			return new Property(tag.ConvertToError(), Property.BoxedErrorCodeNotEnoughMemory);
		}

		public void AppendToString(StringBuilder sb)
		{
			sb.Append("tag:[");
			sb.AppendAsString(this.Tag);
			sb.Append("] value:[");
			sb.AppendAsString(this.Value);
			sb.Append("]");
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(30);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		private static readonly object BoxedErrorCodeNotFound = ErrorCodeValue.NotFound;

		private static readonly object BoxedErrorCodeNotEnoughMemory = ErrorCodeValue.NotEnoughMemory;

		private readonly StorePropTag tag;

		private readonly object value;
	}
}
