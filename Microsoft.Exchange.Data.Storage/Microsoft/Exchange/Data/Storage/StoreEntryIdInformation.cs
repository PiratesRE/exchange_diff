using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class StoreEntryIdInformation
	{
		internal StoreEntryIdInformation()
		{
		}

		public string MailboxLegacyDN { get; internal set; }

		public string ServerName { get; internal set; }

		public string ProviderFileName { get; internal set; }

		public byte[] ProviderFileNameBytes { get; internal set; }

		public byte Version { get; internal set; }

		public byte[] WrappedStoreGuid { get; internal set; }

		public byte[] StoreGuid { get; internal set; }

		public byte[] Flags { get; internal set; }

		public uint FlagsInt { get; internal set; }

		public byte[] StoreEntryIdBytes { get; internal set; }

		public bool IsPublic { get; internal set; }
	}
}
