using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class UnsupportedPropertyForSortGroupException : StoragePermanentException
	{
		public UnsupportedPropertyForSortGroupException(LocalizedString message, PropertyDefinition unsupportedProperty) : base(message)
		{
			this.unsupportedProperty = unsupportedProperty;
		}

		protected UnsupportedPropertyForSortGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.unsupportedProperty = (PropertyDefinition)info.GetValue("unsupportedProperty", typeof(PropertyDefinition));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("unsupportedProperty", this.unsupportedProperty);
		}

		public PropertyDefinition UnsupportedProperty
		{
			get
			{
				return this.unsupportedProperty;
			}
		}

		private const string UnsupportedPropertyLabel = "unsupportedProperty";

		private PropertyDefinition unsupportedProperty;
	}
}
