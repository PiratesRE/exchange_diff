using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class ESEBACK_CALLBACKS
	{
		public PfnErrESECBPrepareInstanceForBackup pfnPrepareInstance { get; set; }

		public PfnErrESECBDoneWithInstanceForBackup pfnDoneWithInstance { get; set; }

		public PfnErrESECBGetDatabasesInfo pfnGetDatabasesInfo { get; set; }

		public PfnErrESECBIsSGReplicated pfnIsSGReplicated { get; set; }

		public PfnErrESECBServerAccessCheck pfnServerAccessCheck { get; set; }

		public PfnErrESECBTrace pfnTrace { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ESEBACK_CALLBACKS()", new object[0]);
		}
	}
}
