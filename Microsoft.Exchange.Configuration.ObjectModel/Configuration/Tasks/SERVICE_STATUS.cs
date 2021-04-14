using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal struct SERVICE_STATUS
	{
		public uint dwServiceType;

		public uint dwCurrentState;

		public uint dwControlsAccepted;

		public uint dwWin32ExitCode;

		public uint dwServiceSpecificExitCode;

		public uint dwCheckPoint;

		public uint dwWaitHint;
	}
}
