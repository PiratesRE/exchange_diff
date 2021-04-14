using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_ESEBACK_CALLBACKS
	{
		public NATIVE_PfnErrESECBPrepareInstanceForBackup pfnPrepareInstance;

		public NATIVE_PfnErrESECBDoneWithInstanceForBackup pfnDoneWithInstance;

		public NATIVE_PfnErrESECBGetDatabasesInfo pfnGetDatabasesInfo;

		public NATIVE_PfnErrESECBFreeDatabasesInfo pfnFreeDatabasesInfo;

		public NATIVE_PfnErrESECBIsSGReplicated pfnIsSGReplicated;

		public NATIVE_PfnErrESECBFreeShipLogInfo pfnFreeShipLogInfo;

		public NATIVE_PfnErrESECBServerAccessCheck pfnServerAccessCheck;

		public NATIVE_PfnErrESECBTrace pfnTrace;
	}
}
