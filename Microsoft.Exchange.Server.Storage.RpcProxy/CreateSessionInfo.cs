using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal struct CreateSessionInfo
	{
		public uint Flags;

		public string UserDn;

		public uint ConnectionMode;

		public uint CodePageId;

		public uint LocaleIdString;

		public uint LocaleIdSort;

		public short[] ClientVersion;

		public byte[] AuxiliaryIn;

		public Action<ErrorCode, uint> NotificationPendingCallback;
	}
}
