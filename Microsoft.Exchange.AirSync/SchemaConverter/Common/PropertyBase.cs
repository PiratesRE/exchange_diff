using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal abstract class PropertyBase : IProperty
	{
		public int SchemaLinkId
		{
			get
			{
				return this.schemaLinkId;
			}
			set
			{
				if (-1 != this.schemaLinkId)
				{
					throw new ConversionException("Schema property already bound by schema link id");
				}
				this.schemaLinkId = value;
			}
		}

		public PropertyState State
		{
			get
			{
				return this.propertyState;
			}
			set
			{
				this.propertyState = value;
			}
		}

		public PropertyBase.PostProcessProperties PostProcessingDelegate { get; set; }

		public virtual void Unbind()
		{
			this.PostProcessingDelegate = null;
		}

		public abstract void CopyFrom(IProperty srcProperty);

		private PropertyState propertyState;

		private int schemaLinkId = -1;

		public delegate void PostProcessProperties(IProperty srcProperty, IList<IProperty> props);
	}
}
