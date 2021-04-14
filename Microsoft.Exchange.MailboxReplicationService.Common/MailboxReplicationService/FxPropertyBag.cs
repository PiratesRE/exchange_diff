using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxPropertyBag : IPropertyBag
	{
		public FxPropertyBag(ISession session)
		{
			this.values = new Dictionary<PropertyTag, object>(10);
			this.session = session;
		}

		public ISession Session
		{
			get
			{
				return this.session;
			}
		}

		internal object this[PropertyTag propertyTag]
		{
			get
			{
				object result;
				if (!this.values.TryGetValue(propertyTag, out result))
				{
					return null;
				}
				return result;
			}
			set
			{
				this.values[propertyTag] = value;
			}
		}

		AnnotatedPropertyValue IPropertyBag.GetAnnotatedProperty(PropertyTag propertyTag)
		{
			NamedProperty namedProperty = null;
			object obj = this[propertyTag];
			PropertyValue propertyValue = (obj != null) ? new PropertyValue(propertyTag, obj) : PropertyValue.Error(propertyTag.PropertyId, (ErrorCode)2147746063U);
			if (propertyTag.IsNamedProperty)
			{
				this.session.TryResolveToNamedProperty(propertyTag, out namedProperty);
				if (namedProperty == null)
				{
					propertyValue = PropertyValue.Error(propertyTag.PropertyId, (ErrorCode)2147746063U);
				}
			}
			return new AnnotatedPropertyValue(propertyTag, propertyValue, namedProperty);
		}

		IEnumerable<AnnotatedPropertyValue> IPropertyBag.GetAnnotatedProperties()
		{
			foreach (KeyValuePair<PropertyTag, object> kvp in this.values)
			{
				KeyValuePair<PropertyTag, object> keyValuePair = kvp;
				yield return ((IPropertyBag)this).GetAnnotatedProperty(keyValuePair.Key);
			}
			yield break;
		}

		void IPropertyBag.Delete(PropertyTag property)
		{
			throw new NotSupportedException();
		}

		Stream IPropertyBag.GetPropertyStream(PropertyTag propertyTag)
		{
			throw new NotSupportedException();
		}

		Stream IPropertyBag.SetPropertyStream(PropertyTag propertyTag, long dataSizeEstimate)
		{
			throw new NotSupportedException();
		}

		void IPropertyBag.SetProperty(PropertyValue propertyValue)
		{
			throw new NotSupportedException();
		}

		private readonly IDictionary<PropertyTag, object> values;

		private readonly ISession session;
	}
}
