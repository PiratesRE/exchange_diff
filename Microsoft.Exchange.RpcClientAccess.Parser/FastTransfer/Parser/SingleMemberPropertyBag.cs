using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SingleMemberPropertyBag : IPropertyBag
	{
		public SingleMemberPropertyBag(PropertyTag requestedProperyTag)
		{
			if (requestedProperyTag.IsNamedProperty)
			{
				throw new ArgumentException("Should not be a named property", "requestedProperyTag");
			}
			this.requestedProperyTag = requestedProperyTag;
			this.ResetValue();
		}

		public PropertyValue PropertyValue
		{
			get
			{
				return this.propertyValue;
			}
		}

		AnnotatedPropertyValue IPropertyBag.GetAnnotatedProperty(PropertyTag propertyTag)
		{
			PropertyValue property = this.GetProperty(propertyTag);
			return new AnnotatedPropertyValue(propertyTag, property, null);
		}

		IEnumerable<AnnotatedPropertyValue> IPropertyBag.GetAnnotatedProperties()
		{
			return new AnnotatedPropertyValue[]
			{
				((IPropertyBag)this).GetAnnotatedProperty(this.propertyValue.PropertyTag)
			};
		}

		void IPropertyBag.SetProperty(PropertyValue propertyValue)
		{
			this.CheckCorrectProperty(propertyValue.PropertyTag);
			this.propertyValue = propertyValue;
		}

		void IPropertyBag.Delete(PropertyTag property)
		{
			this.CheckCorrectProperty(property);
			this.ResetValue();
		}

		Stream IPropertyBag.GetPropertyStream(PropertyTag property)
		{
			this.CheckCorrectProperty(property);
			return MemoryPropertyBag.WrapPropertyReadStream(MemoryPropertyBag.ConvertToRequestedType(this.propertyValue, property.PropertyType));
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
				throw new NotSupportedException("SingleMemberPropertyBag does not have a session and doesn't support named properties");
			}
		}

		private void CheckCorrectProperty(PropertyTag propertyTag)
		{
			if (propertyTag.PropertyId != this.requestedProperyTag.PropertyId)
			{
				throw new RopExecutionException(string.Format("SingleMemberPropertyBag can only store a value for property {0}. Asking for {1} is a misuse and a programming error", this.requestedProperyTag.PropertyId, propertyTag.PropertyId), ErrorCode.FxUnexpectedMarker);
			}
		}

		private void ResetValue()
		{
			this.propertyValue = PropertyValue.Error(this.requestedProperyTag.PropertyId, (ErrorCode)2147746063U);
		}

		private PropertyValue GetProperty(PropertyTag property)
		{
			this.CheckCorrectProperty(property);
			return MemoryPropertyBag.ConvertToRequestedType(this.propertyValue, property.PropertyType);
		}

		private readonly PropertyTag requestedProperyTag;

		private PropertyValue propertyValue;
	}
}
