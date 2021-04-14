using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class PDLMembershipPropertyRule
	{
		public static bool UpdateProperties(ICorePropertyBag propertyBag)
		{
			ParticipantEntryId[] array;
			ParticipantEntryId[] array2;
			byte[][] array3;
			uint num;
			bool flag;
			DistributionList.GetEntryIds(propertyBag, out array, out array2, out array3, out num, out flag);
			return false;
		}

		public static readonly PropertyReference[] PropertyReferences = new PropertyReference[]
		{
			new PropertyReference(InternalSchema.Members, PropertyAccess.Read),
			new PropertyReference(InternalSchema.OneOffMembers, PropertyAccess.Read),
			new PropertyReference(InternalSchema.DLStream, PropertyAccess.Read)
		};
	}
}
