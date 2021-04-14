using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public struct JET_OPERATIONCONTEXT : IContentEquatable<JET_OPERATIONCONTEXT>
	{
		public uint UserID { get; set; }

		public byte OperationID { get; set; }

		public byte OperationType { get; set; }

		public byte ClientType { get; set; }

		public byte Flags { get; set; }

		public bool ContentEquals(JET_OPERATIONCONTEXT other)
		{
			return this.UserID == other.UserID && this.OperationID == other.OperationID && this.OperationType == other.OperationType && this.ClientType == other.ClientType && this.Flags == other.Flags;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_OPERATIONCONTEXT({0}:{1}:{2}:{3}:{4}:0x{5:x2})", new object[]
			{
				this.UserID,
				this.OperationID,
				this.OperationType,
				this.ClientType,
				this.Flags
			});
		}

		internal NATIVE_OPERATIONCONTEXT GetNativeOperationContext()
		{
			return new NATIVE_OPERATIONCONTEXT
			{
				ulUserID = this.UserID,
				nOperationID = this.OperationID,
				nOperationType = this.OperationType,
				nClientType = this.ClientType,
				fFlags = this.Flags
			};
		}
	}
}
