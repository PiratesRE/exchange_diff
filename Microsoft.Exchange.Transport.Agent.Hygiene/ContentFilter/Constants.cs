using System;

namespace Microsoft.Exchange.Transport.Agent.ContentFilter
{
	internal static class Constants
	{
		public static readonly Guid ContentFilterWrapperGuid = new Guid("5850470B-ADFD-4EF8-B68A-AFF968721A46");

		public static readonly uint[] ExpectedMessageFailureHResults = new uint[]
		{
			839232U,
			2147942487U,
			2147942511U,
			2147943513U,
			2147944414U,
			2147946736U,
			2148077569U,
			2148077570U,
			2148077571U,
			2148077572U,
			2148077573U,
			2148077574U,
			2148077575U,
			2148077576U,
			2148077577U,
			2148077578U,
			2148077579U,
			2148077580U,
			2148077581U,
			2148077582U,
			2148077583U,
			2148077584U,
			2148077585U,
			2148081665U,
			2148081666U,
			2148081667U,
			2148081668U,
			2148081669U,
			2148081670U,
			2148081671U,
			2148081672U,
			2148081673U,
			2148081674U,
			2148081675U,
			2148081676U,
			2148081677U,
			2148081678U,
			2148081679U,
			2148081680U,
			2148081681U,
			2148081682U,
			2148081683U,
			2148081684U,
			2148081696U,
			2148081697U,
			2148081698U,
			2148081699U,
			2148081700U,
			2148081701U,
			2148081702U,
			2148081703U,
			2148081704U,
			2148081705U,
			2148081706U,
			2148081707U,
			2148081708U,
			2148322819U,
			2148322820U,
			2148322828U,
			2148322993U,
			2148322998U,
			2148322999U,
			2148323000U,
			2148323056U,
			2148323059U,
			2148323060U,
			2148323061U,
			2148323062U,
			2148323063U
		};

		internal static class ComErrorCodes
		{
			public const int InsufficientBuffer = -2147024774;

			public const int AlreadyInitialized = -2147023649;

			public const int ExSMimeInitialization = -1067253755;

			public const int Win32ErrorDiskFull = -2147024784;
		}

		public static class ComInteropPropertyIds
		{
			public const int Error = 0;

			public const int RequestType = 1;

			public const int RequestResult = 2;

			public const int ScanCompleteResult = 3;

			public const int SCL = 4;

			public const int UnmodifiedSCL = 5;

			public const int CustomWordBufferLength = 6;

			public const int CustomWords = 7;

			public const int PRD = 8;

			public const int SenderIdResult = 9;

			public const int FilterDirectory = 10;

			public const int OutlookEmailPostmarkValidationEnabled = 11;

			public const int RecipientsBufferLength = 12;

			public const int Recipients = 13;

			public const int Diagnostics = 14;

			public const int PCL = 15;

			public const int Postmark = 16;

			public const int PremiumSKUEnabled = 17;

			public const int FailureHResult = 18;

			public const int FailureFunctionID = 19;
		}

		public static class RequestTypes
		{
			public static readonly byte[] Initialize = BitConverter.GetBytes(1);

			public static readonly byte[] ScanMessage = BitConverter.GetBytes(2);

			public static readonly byte[] Shutdown = BitConverter.GetBytes(3);
		}

		internal enum FailureFunctionIDs
		{
			NoError,
			MessageStreamQueryInterface,
			RecipientBufferAllocation,
			SmartScreenEvaluate,
			SmartScreenResultGetSCL,
			SmartScreenResultGetUnmodifiedSCL,
			SmartScreenResultGetPCL,
			SmartScreenResultGetDiagnostics,
			SmartScreenResultGetPresolveStatus,
			PropertyBagPutSCL,
			PropertyBagPutUnmodifiedSCL,
			PropertyBagPutDiagnostics,
			PropertyBagPutPCL,
			PropertyBagPutPostmark
		}
	}
}
