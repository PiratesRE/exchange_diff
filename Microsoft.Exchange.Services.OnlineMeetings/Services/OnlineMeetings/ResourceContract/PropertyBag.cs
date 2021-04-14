using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal class PropertyBag : Collection<Property>
	{
		public PropertyBag()
		{
		}

		public PropertyBag(IList<Property> list) : base(list)
		{
		}
	}
}
