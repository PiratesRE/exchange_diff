using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ADErrorRecord
	{
		internal HandlingType HandlingType
		{
			get
			{
				return this.handlingType;
			}
			set
			{
				this.handlingType = value;
			}
		}

		internal LdapError LdapError
		{
			get
			{
				return this.ldapError;
			}
			set
			{
				this.ldapError = value;
			}
		}

		internal string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		internal int NativeError
		{
			get
			{
				return this.nativeError;
			}
			set
			{
				this.nativeError = value;
			}
		}

		internal int JetError
		{
			get
			{
				return this.jetError;
			}
			set
			{
				this.jetError = value;
			}
		}

		internal bool IsDownError
		{
			get
			{
				return this.isDownError;
			}
			set
			{
				this.isDownError = value;
			}
		}

		internal bool IsFatalError
		{
			get
			{
				return this.isFatalError;
			}
			set
			{
				this.isFatalError = value;
			}
		}

		internal bool IsSearchError
		{
			get
			{
				return this.isSearchError;
			}
			set
			{
				this.isSearchError = value;
			}
		}

		internal bool IsTimeoutError
		{
			get
			{
				return this.isTimeoutError;
			}
			set
			{
				this.isTimeoutError = value;
			}
		}

		internal bool IsServerSideTimeoutError
		{
			get
			{
				return this.isServerSideTimeoutError;
			}
			set
			{
				this.isServerSideTimeoutError = value;
			}
		}

		internal bool IsModificationError
		{
			get
			{
				return this.isModificationError;
			}
			set
			{
				this.isModificationError = value;
			}
		}

		internal Exception InnerException
		{
			get
			{
				return this.innerException;
			}
			set
			{
				this.innerException = value;
			}
		}

		private HandlingType handlingType;

		private LdapError ldapError;

		private string message = string.Empty;

		private int nativeError;

		private int jetError;

		private bool isDownError;

		private bool isFatalError;

		private bool isSearchError;

		private bool isTimeoutError;

		private bool isServerSideTimeoutError;

		private bool isModificationError;

		private Exception innerException;
	}
}
