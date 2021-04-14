using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidStatusCodeException : InvalidOperationException
	{
		public InvalidStatusCodeException(string message, uint statusCode) : base(message)
		{
			this.statusCode = statusCode;
		}

		public uint StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		private readonly uint statusCode;
	}
}
