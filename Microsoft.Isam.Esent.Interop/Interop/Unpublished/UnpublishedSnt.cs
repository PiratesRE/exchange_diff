using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public static class UnpublishedSnt
	{
		public const JET_SNT OpenLog = (JET_SNT)1001;

		public const JET_SNT OpenCheckpoint = (JET_SNT)1002;

		public const JET_SNT MissingLog = (JET_SNT)1004;

		public const JET_SNT BeginUndo = (JET_SNT)1005;

		public const JET_SNT NotificationEvent = (JET_SNT)1006;

		public const JET_SNT SignalErrorCondition = (JET_SNT)1007;

		public const JET_SNT DbAttached = (JET_SNT)1008;

		public const JET_SNT DbDetached = (JET_SNT)1009;

		public const JET_SNT PagePatchRequest = (JET_SNT)1101;

		public const JET_SNT CorruptedPage = (JET_SNT)1102;
	}
}
