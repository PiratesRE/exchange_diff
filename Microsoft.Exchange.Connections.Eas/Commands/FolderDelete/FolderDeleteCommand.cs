using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderDelete
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderDeleteCommand : EasServerCommand<FolderDeleteRequest, FolderDeleteResponse, FolderDeleteStatus>
	{
		internal FolderDeleteCommand(EasConnectionSettings easConnectionSettings) : base(Command.FolderDelete, easConnectionSettings)
		{
		}
	}
}
