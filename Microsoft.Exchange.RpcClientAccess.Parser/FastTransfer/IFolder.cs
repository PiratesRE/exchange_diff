using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFolder : IDisposable
	{
		IPropertyBag PropertyBag { get; }

		IEnumerable<IMessage> GetContents();

		IEnumerable<IMessage> GetAssociatedContents();

		IEnumerable<IFolder> GetFolders();

		IFolder CreateFolder();

		IMessage CreateMessage(bool isAssociatedMessage);

		void Save();

		bool IsContentAvailable { get; }

		string[] GetReplicaDatabases(out ushort localSiteDatabaseCount);

		StoreLongTermId GetLongTermId();
	}
}
