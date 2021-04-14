using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_OPERATIONCONTEXT
	{
		public uint ulUserID;

		public byte nOperationID;

		public byte nOperationType;

		public byte nClientType;

		public byte fFlags;
	}
}
