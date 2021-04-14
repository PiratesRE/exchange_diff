using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class IdentityReplaceMechanism : TranslationMechanismBase
	{
		public IdentityReplaceMechanism(LocalizedString messageWithoutDisplayName) : base(messageWithoutDisplayName, false)
		{
		}

		protected override string TranslationWithDisplayName(Identity id, string originalMessage)
		{
			return originalMessage.Replace(id.RawIdentity, id.DisplayName);
		}
	}
}
