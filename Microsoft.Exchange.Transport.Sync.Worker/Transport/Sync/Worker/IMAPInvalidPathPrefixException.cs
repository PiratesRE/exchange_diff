using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Worker
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class IMAPInvalidPathPrefixException : Exception
	{
		public IMAPInvalidPathPrefixException(string message, string pathPrefix) : this(message, null, pathPrefix)
		{
		}

		public IMAPInvalidPathPrefixException(string message, Exception innerException, string pathPrefix) : base(message, innerException)
		{
			this.pathPrefix = pathPrefix;
		}

		public IMAPInvalidPathPrefixException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public string PathPrefix
		{
			get
			{
				return this.pathPrefix;
			}
		}

		private readonly string pathPrefix;
	}
}
