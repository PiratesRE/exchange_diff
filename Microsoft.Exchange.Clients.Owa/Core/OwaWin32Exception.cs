using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaWin32Exception : OwaPermanentException
	{
		public OwaWin32Exception(int lastError, string message, Exception innerException) : base(string.Format("{0}, GetLastError()={1}", string.IsNullOrEmpty(message) ? "<n/a>" : message, lastError.ToString()), innerException)
		{
			this.lastError = lastError;
		}

		public OwaWin32Exception(int lastError, string message) : this(lastError, message, null)
		{
		}

		public int LastError
		{
			get
			{
				return this.lastError;
			}
		}

		private int lastError;
	}
}
