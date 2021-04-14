using System;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class EntitySets
	{
		public static readonly EdmEntityContainer EdmEntityContainer = new EdmEntityContainer(typeof(EntitySets).Namespace, "EntityContainer");

		public static readonly EdmSingleton Me = new EdmSingleton(EntitySets.EdmEntityContainer, "Me", User.EdmEntityType);

		public static readonly EdmEntitySet Users = new EdmEntitySet(EntitySets.EdmEntityContainer, "Users", User.EdmEntityType);
	}
}
