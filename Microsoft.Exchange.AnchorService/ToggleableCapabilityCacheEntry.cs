using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ToggleableCapabilityCacheEntry : CapabilityCacheEntry
	{
		public ToggleableCapabilityCacheEntry(ActiveAnchorContext context, ADUser user) : base(context, user)
		{
		}

		public override void Activate()
		{
			base.ADProvider.AddCapability(base.ObjectId, this.ActiveCapability);
		}

		public override void Deactivate()
		{
			base.ADProvider.RemoveCapability(base.ObjectId, this.ActiveCapability);
		}
	}
}
