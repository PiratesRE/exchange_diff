using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ABGroupSchema : ABObjectSchema
	{
		public static readonly ABPropertyDefinition OwnerId = new ABPropertyDefinition("OwnerId", typeof(ABObjectId), PropertyDefinitionFlags.ReadOnly, null);

		public static readonly ABPropertyDefinition HiddenMembership = new ABPropertyDefinition("HiddenMembership", typeof(bool?), PropertyDefinitionFlags.ReadOnly, null);

		public static readonly ABPropertyDefinition MembersCount = new ABPropertyDefinition("MembersCount", typeof(int?), PropertyDefinitionFlags.ReadOnly, null);
	}
}
