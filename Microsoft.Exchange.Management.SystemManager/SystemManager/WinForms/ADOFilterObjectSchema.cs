using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ADOFilterObjectSchema : ObjectSchema
	{
		public ADOFilterObjectSchema(IList<PropertyDefinition> definitions)
		{
			this.allFilterableProperties = new ReadOnlyCollection<PropertyDefinition>(definitions);
		}

		public override ReadOnlyCollection<PropertyDefinition> AllFilterableProperties
		{
			get
			{
				return this.allFilterableProperties;
			}
		}

		private ReadOnlyCollection<PropertyDefinition> allFilterableProperties;
	}
}
