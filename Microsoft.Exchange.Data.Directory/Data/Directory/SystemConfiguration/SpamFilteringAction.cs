using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum SpamFilteringAction
	{
		[LocDescription(DirectoryStrings.IDs.SpamFilteringActionJmf)]
		MoveToJmf,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringActionAddXHeader)]
		AddXHeader,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringActionModifySubject)]
		ModifySubject,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringActionRedirect)]
		Redirect,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringActionDelete)]
		Delete,
		[LocDescription(DirectoryStrings.IDs.SpamFilteringActionQuarantine)]
		Quarantine
	}
}
