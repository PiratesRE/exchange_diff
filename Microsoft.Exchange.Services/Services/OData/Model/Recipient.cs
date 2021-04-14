using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class Recipient
	{
		public string Name { get; set; }

		public string Address { get; set; }

		internal static readonly LazyMember<EdmComplexType> EdmComplexType = new LazyMember<EdmComplexType>(delegate()
		{
			EdmComplexType edmComplexType = new EdmComplexType(typeof(Recipient).Namespace, typeof(Recipient).Name);
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Name", EdmCoreModel.Instance.GetString(true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Address", EdmCoreModel.Instance.GetString(true)));
			return edmComplexType;
		});
	}
}
