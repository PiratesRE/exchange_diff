using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ItemBody
	{
		public BodyType ContentType { get; set; }

		public string Content { get; set; }

		internal static readonly LazyMember<EdmComplexType> EdmComplexType = new LazyMember<EdmComplexType>(delegate()
		{
			EdmComplexType edmComplexType = new EdmComplexType(typeof(ItemBody).Namespace, typeof(ItemBody).Name);
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "ContentType", new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(BodyType)), true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, "Content", EdmCoreModel.Instance.GetString(true)));
			return edmComplexType;
		});
	}
}
