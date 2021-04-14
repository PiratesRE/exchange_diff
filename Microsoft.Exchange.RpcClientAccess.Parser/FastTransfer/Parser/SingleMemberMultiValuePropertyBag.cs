using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SingleMemberMultiValuePropertyBag : IPropertyBag
	{
		public SingleMemberMultiValuePropertyBag(PropertyTag requestedPropertyTag, int elementCount, IPropertyBag propertyBag)
		{
			this.requestedPropertyTag = requestedPropertyTag;
			this.multiValues = SingleMemberMultiValuePropertyBag.CreateArrayForMultiValue(this.requestedPropertyTag, elementCount);
			this.elementPropertyTag = new PropertyTag(this.requestedPropertyTag.PropertyId, this.requestedPropertyTag.ElementPropertyType);
			this.propertyBag = propertyBag;
			this.elementIndex = 0;
			if (elementCount <= 0)
			{
				this.FlushProperty();
			}
		}

		public PropertyTag ElementPropertyTag
		{
			get
			{
				return this.elementPropertyTag;
			}
		}

		private int ElementCount
		{
			get
			{
				return this.multiValues.Length;
			}
		}

		public int ElementIndex
		{
			set
			{
				this.elementIndex = value;
			}
		}

		AnnotatedPropertyValue IPropertyBag.GetAnnotatedProperty(PropertyTag propertyTag)
		{
			throw new NotSupportedException("SingleMemberMultiValuePropertyBag does not support IPropertyBag.GetAnnotatedProperty");
		}

		IEnumerable<AnnotatedPropertyValue> IPropertyBag.GetAnnotatedProperties()
		{
			throw new NotSupportedException("SingleMemberMultiValuePropertyBag does not support IPropertyBag.GetAnnotatedProperties");
		}

		void IPropertyBag.SetProperty(PropertyValue propertyValue)
		{
			if (this.elementIndex >= this.ElementCount)
			{
				throw new RopExecutionException("Cannot set more elements then defined in constructor.  This is misuse and a programming error.", ErrorCode.FxUnexpectedMarker);
			}
			this.CheckCorrectProperty(propertyValue.PropertyTag);
			this.multiValues.SetValue(propertyValue.Value, this.elementIndex);
			if (this.elementIndex + 1 >= this.ElementCount)
			{
				this.FlushProperty();
			}
		}

		void IPropertyBag.Delete(PropertyTag property)
		{
			throw new NotSupportedException("SingleMemberMultiValuePropertyBag does not support IPropertyBag.Delete.");
		}

		Stream IPropertyBag.GetPropertyStream(PropertyTag property)
		{
			throw new NotSupportedException("SingleMemberMultiValuePropertyBag does not support IPropertyBag.GetPropertyStream.");
		}

		Stream IPropertyBag.SetPropertyStream(PropertyTag property, long dataSizeEstimate)
		{
			this.CheckCorrectProperty(property);
			return MemoryPropertyBag.WrapPropertyWriteStream(this, property, dataSizeEstimate);
		}

		ISession IPropertyBag.Session
		{
			get
			{
				throw new NotSupportedException("SingleMemberMultiValuePropertyBag does not have a session and doesn't support named properties.");
			}
		}

		private static Array CreateArrayForMultiValue(PropertyTag propertyTag, int count)
		{
			PropertyType propertyType = propertyTag.PropertyType;
			if (propertyType <= PropertyType.MultiValueUnicode)
			{
				switch (propertyType)
				{
				case PropertyType.MultiValueInt16:
					return new short[count];
				case PropertyType.MultiValueInt32:
					return new int[count];
				case PropertyType.MultiValueFloat:
					return new float[count];
				case PropertyType.MultiValueDouble:
					return new double[count];
				case PropertyType.MultiValueCurrency:
				case PropertyType.MultiValueAppTime:
					break;
				default:
					if (propertyType == PropertyType.MultiValueInt64)
					{
						return new long[count];
					}
					switch (propertyType)
					{
					case PropertyType.MultiValueString8:
					case PropertyType.MultiValueUnicode:
						return new string[count];
					}
					break;
				}
			}
			else
			{
				if (propertyType == PropertyType.MultiValueSysTime)
				{
					return new ExDateTime[count];
				}
				if (propertyType == PropertyType.MultiValueGuid)
				{
					return new Guid[count];
				}
				if (propertyType == PropertyType.MultiValueBinary)
				{
					return new byte[count][];
				}
			}
			throw new NotSupportedException();
		}

		private void CheckCorrectProperty(PropertyTag propertyTag)
		{
			if (propertyTag != this.elementPropertyTag)
			{
				throw new RopExecutionException(string.Format("SingleMemberMultiValuePropertyBag can only store a value for property {0}. Trying to set {1} is a misuse and a programming error.", this.requestedPropertyTag, propertyTag), ErrorCode.FxUnexpectedMarker);
			}
		}

		private void FlushProperty()
		{
			PropertyTag propertyTag = this.requestedPropertyTag;
			if (propertyTag.ElementPropertyType == PropertyType.String8)
			{
				propertyTag = propertyTag.ChangeElementPropertyType(PropertyType.Unicode);
			}
			this.propertyBag.SetProperty(new PropertyValue(propertyTag, this.multiValues));
		}

		private readonly PropertyTag requestedPropertyTag;

		private readonly PropertyTag elementPropertyTag;

		private readonly Array multiValues;

		private readonly IPropertyBag propertyBag;

		private int elementIndex;
	}
}
