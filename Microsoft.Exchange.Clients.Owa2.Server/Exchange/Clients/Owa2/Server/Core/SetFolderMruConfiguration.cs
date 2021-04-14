using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SetFolderMruConfiguration : ServiceCommand<bool>
	{
		public SetFolderMruConfiguration(CallContext callContext, TargetFolderMruConfiguration folderMruConfiguration) : base(callContext)
		{
			this.folderMruConfiguration = folderMruConfiguration;
		}

		protected override bool InternalExecute()
		{
			if (this.folderMruConfiguration.FolderMruEntries == null || this.folderMruConfiguration.FolderMruEntries.Length == 0)
			{
				return false;
			}
			this.folderMruConfiguration.Save(CallContext.Current);
			return true;
		}

		private TargetFolderMruConfiguration folderMruConfiguration;
	}
}
