using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class PrivatePropertyBag : PropertyBag
	{
		protected PrivatePropertyBag(bool changeTrackingEnabled) : base(changeTrackingEnabled)
		{
		}

		protected override Dictionary<ushort, KeyValuePair<StorePropTag, object>> Properties
		{
			get
			{
				return this.properties;
			}
		}

		protected override bool PropertiesDirty
		{
			get
			{
				return this.propertiesDirty;
			}
			set
			{
				this.propertiesDirty = value;
			}
		}

		protected override void AssignPropertiesToUse(Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties)
		{
			this.properties = properties;
		}

		private Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties;

		private bool propertiesDirty;
	}
}
