using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class Location
	{
		public string DisplayName { get; set; }

		internal static readonly LazyMember<EdmComplexType> EdmComplexType = new LazyMember<EdmComplexType>(delegate()
		{
			EdmComplexType edmComplexType = new EdmComplexType(typeof(Location).Namespace, typeof(Location).Name);
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "DisplayName", EdmCoreModel.Instance.GetString(true)));
			return edmComplexType;
		});
	}
}
