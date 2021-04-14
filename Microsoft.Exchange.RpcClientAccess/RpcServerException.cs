using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcServerException : Exception
	{
		internal RpcServerException(string message, RpcErrorCode storeError) : base(message)
		{
			this.storeError = storeError;
		}

		internal RpcServerException(string message, RpcErrorCode storeError, Exception innerException) : base(message, innerException)
		{
			this.storeError = storeError;
		}

		public bool DropConnection
		{
			get
			{
				return this.dropConnection;
			}
			set
			{
				this.dropConnection = value;
			}
		}

		public override string Message
		{
			get
			{
				return string.Format("{0} (StoreError={1})", base.Message, this.storeError);
			}
		}

		internal RpcErrorCode StoreError
		{
			get
			{
				return this.storeError;
			}
		}

		private readonly RpcErrorCode storeError;

		private bool dropConnection;
	}
}
