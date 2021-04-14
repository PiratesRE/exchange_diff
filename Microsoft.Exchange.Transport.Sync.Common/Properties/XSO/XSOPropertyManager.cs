using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Properties.XSO
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class XSOPropertyManager : IXSOPropertyManager
	{
		protected XSOPropertyManager()
		{
			this.allProperties = new List<PropertyDefinition>();
		}

		public PropertyDefinition[] AllProperties
		{
			get
			{
				if (this.cachedAllProperties == null || this.cachedAllProperties.Length != this.allProperties.Count)
				{
					this.cachedAllProperties = new PropertyDefinition[this.allProperties.Count];
					this.allProperties.CopyTo(this.cachedAllProperties);
				}
				return this.cachedAllProperties;
			}
		}

		public void AddPropertyDefinition(PropertyDefinition propertyDefinition)
		{
			SyncUtilities.ThrowIfArgumentNull("propertyDefinition", propertyDefinition);
			this.allProperties.Add(propertyDefinition);
		}

		private List<PropertyDefinition> allProperties;

		private PropertyDefinition[] cachedAllProperties;
	}
}
