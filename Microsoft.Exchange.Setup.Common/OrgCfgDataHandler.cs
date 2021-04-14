using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class OrgCfgDataHandler : ConfigurationDataHandler
	{
		public OrgCfgDataHandler(ISetupContext context, string commandText, MonadConnection connection) : base(context, "", commandText, connection)
		{
		}

		public abstract bool WillDataHandlerDoAnyWork();
	}
}
