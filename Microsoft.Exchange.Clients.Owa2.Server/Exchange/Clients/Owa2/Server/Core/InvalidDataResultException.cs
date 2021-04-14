using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	internal class InvalidDataResultException : OwaPermanentException
	{
		public InvalidDataResultException(string errorMessage) : this(errorMessage, null)
		{
		}

		public InvalidDataResultException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
		{
		}
	}
}
