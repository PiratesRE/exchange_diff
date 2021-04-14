using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderCreate
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderCreateCommand : EasServerCommand<FolderCreateRequest, FolderCreateResponse, FolderCreateStatus>
	{
		internal FolderCreateCommand(EasConnectionSettings easConnectionSettings) : base(Command.FolderCreate, easConnectionSettings)
		{
		}
	}
}
