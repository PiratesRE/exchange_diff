using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderUpdate
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderUpdateCommand : EasServerCommand<FolderUpdateRequest, FolderUpdateResponse, FolderUpdateStatus>
	{
		internal FolderUpdateCommand(EasConnectionSettings easConnectionSettings) : base(Command.FolderUpdate, easConnectionSettings)
		{
		}
	}
}
