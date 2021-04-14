using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Flags]
	public enum UserOptionsLearnabilityTypes
	{
		None = 0,
		Clutter = 1,
		ClutterDeleteAll = 2,
		PeopleCentricTriage = 4,
		ModernGroups = 8,
		ModernGroupsCompose = 16,
		DocCollabEditACopy = 32,
		PeopleCentricTriageReadingPane = 64,
		ModernGroupsComposeTNarrow = 128,
		HelpPanel = 256,
		ModernAttachments = 512
	}
}
