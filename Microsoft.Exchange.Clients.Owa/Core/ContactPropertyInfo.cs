using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class ContactPropertyInfo
	{
		public ContactPropertyInfo(PropertyDefinition propertyDefinition, string id, Strings.IDs label)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			this.label = label;
			this.id = id;
			this.propertyDefinition = propertyDefinition;
		}

		public Strings.IDs Label
		{
			get
			{
				return this.label;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public PropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
		}

		private Strings.IDs label;

		private string id;

		private PropertyDefinition propertyDefinition;
	}
}
