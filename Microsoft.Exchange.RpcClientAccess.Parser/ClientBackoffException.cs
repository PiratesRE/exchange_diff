using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ClientBackoffException : Exception
	{
		public ClientBackoffException(string message) : base(message)
		{
		}

		public ClientBackoffException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ClientBackoffException(string message, byte logonId, uint duration) : base(message)
		{
			this.backoffInformation = new BackoffInformation(logonId, duration, Array<BackoffRopData>.Empty, Array<byte>.Empty);
		}

		public ClientBackoffException(string message, byte logonId, uint duration, BackoffRopData[] backoffRopData, byte[] additionalData) : base(message)
		{
			this.backoffInformation = new BackoffInformation(logonId, duration, backoffRopData, additionalData);
		}

		public bool IsRepeatingBackoff { get; set; }

		internal bool IsSpecificBackoff
		{
			get
			{
				return this.backoffInformation != null;
			}
		}

		internal BackoffInformation BackoffInformation
		{
			get
			{
				return this.backoffInformation;
			}
		}

		private readonly BackoffInformation backoffInformation;
	}
}
