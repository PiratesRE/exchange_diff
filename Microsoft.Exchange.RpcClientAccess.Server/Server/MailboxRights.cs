using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal static class MailboxRights
	{
		public const uint Owner = 1U;

		private const uint SendAs = 2U;

		private const uint PrimaryUser = 4U;

		public static readonly Guid SendAsExtendedRightGuid = WellKnownGuid.SendAsExtendedRightGuid;

		public static readonly Guid ReceiveAsExtendedRightGuid = WellKnownGuid.ReceiveAsExtendedRightGuid;

		public static readonly Guid UserObjectType = new Guid("bf967aba-0de6-11d0-a285-00aa003049e2");

		public static readonly NativeMethods.GENERIC_MAPPING GenericRightsMapping = new NativeMethods.GENERIC_MAPPING
		{
			GenericRead = 131072U,
			GenericWrite = 196608U,
			GenericExecute = 131072U,
			GenericAll = 2031623U
		};
	}
}
