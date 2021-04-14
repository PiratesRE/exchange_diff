using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RopExecutionException : Exception
	{
		public RopExecutionException(string message, ErrorCode error) : base(RopExecutionException.GetErrorMessage(message, error))
		{
			this.ErrorCode = error;
		}

		public RopExecutionException(string message, ErrorCode error, Exception innerException) : base(RopExecutionException.GetErrorMessage(message, error), innerException)
		{
			this.ErrorCode = error;
		}

		public override bool Equals(object obj)
		{
			RopExecutionException ex = obj as RopExecutionException;
			return ex != null && ex.ErrorCode == this.ErrorCode;
		}

		public override int GetHashCode()
		{
			return (int)this.ErrorCode;
		}

		private static string GetErrorMessage(string message, ErrorCode error)
		{
			if (message != null)
			{
				return string.Format("{0}. Error code = {1} (0x{1:X})", message, error);
			}
			return error.ToString();
		}

		public readonly ErrorCode ErrorCode;
	}
}
