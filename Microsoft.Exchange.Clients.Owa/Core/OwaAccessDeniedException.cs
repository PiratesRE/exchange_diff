using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaAccessDeniedException : OwaPermanentException
	{
		public OwaAccessDeniedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaAccessDeniedException(string message) : base(message)
		{
		}

		public OwaAccessDeniedException(string message, bool isWebPartFailure) : base(message)
		{
			this.isWebPartFailure = isWebPartFailure;
		}

		public bool IsWebPartFailure
		{
			get
			{
				return this.isWebPartFailure;
			}
		}

		private bool isWebPartFailure;
	}
}
