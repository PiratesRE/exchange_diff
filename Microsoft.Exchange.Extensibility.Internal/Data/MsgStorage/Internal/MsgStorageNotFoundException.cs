using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	[Serializable]
	public class MsgStorageNotFoundException : MsgStorageException
	{
		internal MsgStorageNotFoundException(MsgStorageErrorCode errorCode, string message, Exception exc) : base(errorCode, message, exc)
		{
		}

		protected MsgStorageNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
