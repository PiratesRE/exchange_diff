using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class AtomResultPaneProfile : ResultPaneProfile
	{
		public override bool HasPermission()
		{
			return NodeProfile.CanAddAtomResultPane(base.Type);
		}
	}
}
