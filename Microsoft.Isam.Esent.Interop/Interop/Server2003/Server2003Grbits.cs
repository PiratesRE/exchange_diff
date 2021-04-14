using System;

namespace Microsoft.Isam.Esent.Interop.Server2003
{
	public static class Server2003Grbits
	{
		public const AttachDatabaseGrbit DeleteUnicodeIndexes = (AttachDatabaseGrbit)1024;

		public const ColumndefGrbit ColumnDeleteOnZero = (ColumndefGrbit)131072;

		public const TempTableGrbit ForwardOnly = (TempTableGrbit)64;

		public const EnumerateColumnsGrbit EnumerateIgnoreUserDefinedDefault = (EnumerateColumnsGrbit)1048576;

		public const CommitTransactionGrbit WaitAllLevel0Commit = (CommitTransactionGrbit)8;
	}
}
