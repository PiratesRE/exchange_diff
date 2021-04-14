using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IListItem : IClientObject<ListItem>
	{
		object this[string fieldName]
		{
			get;
			set;
		}

		int Id { get; }

		string IdAsString { get; }

		IFile File { get; }

		IFolder Folder { get; }

		void BreakRoleInheritance(bool copyRoleAssignments, bool clearSubscopes);
	}
}
