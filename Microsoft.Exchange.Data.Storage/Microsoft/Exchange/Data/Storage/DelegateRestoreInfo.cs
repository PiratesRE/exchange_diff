using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct DelegateRestoreInfo
	{
		public IList<IExchangePrincipal> Principals;

		public string[] Names;

		public byte[][] Ids;

		public int[] Flags;

		public int[] Flags2;

		public bool BossWantsCopy;

		public bool BossWantsInfo;

		public bool DontMailDelegate;

		public IDictionary<StoreObjectId, IDictionary<ADRecipient, MemberRights>> FolderPermissions;

		public IList<ADObjectId> SendOnBehalf;

		public Rule DelegateRule;
	}
}
