using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	internal class SearchErrorInfo
	{
		public SearchErrorInfo(int errorCode, Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			this.m_errorCode = errorCode;
			this.m_message = exception.Message;
			this.m_exception = exception;
		}

		public SearchErrorInfo(int errorCode, string message)
		{
			this.m_errorCode = errorCode;
			this.m_message = message;
			this.m_exception = null;
		}

		public SearchErrorInfo()
		{
			this.m_errorCode = 0;
			this.m_message = null;
			this.m_exception = null;
		}

		public int ErrorCode
		{
			get
			{
				return this.m_errorCode;
			}
			set
			{
				this.m_errorCode = value;
			}
		}

		public bool Succeeded
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return ((this.m_errorCode >= 0) ? 1 : 0) != 0;
			}
		}

		public bool Failed
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return ((this.m_errorCode < 0) ? 1 : 0) != 0;
			}
		}

		public string Message
		{
			get
			{
				return this.m_message;
			}
			set
			{
				this.m_message = value;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.m_exception;
			}
			set
			{
				this.m_exception = value;
			}
		}

		private int m_errorCode;

		private string m_message;

		private Exception m_exception;

		public const int IMS_S_OK = 0;

		public const int IMS_E_UNKNOWN_ERROR = -2147220991;

		public const int IMS_E_SEARCH_NOT_FOUND = -2147220990;

		public const int IMS_E_WRONG_TARGET_SERVER = -2147220989;

		public const int IMS_E_SEARCH_ARGUMENT = -2147220988;

		public const int IMS_E_OBJECT_NOT_FOUND = -2147220987;

		public const int IMS_E_STORE_PERMANENT_ERROR = -2147220986;

		public const int IMS_E_STORE_TRANSIENT_ERROR = -2147220985;

		public const int IMS_E_AQS_PARSER_ERROR = -2147220984;

		public const int IMS_E_SERVER_SHUTDOWN = -2147220983;

		public const int IMS_E_ACCESS_ERROR = -2147220982;

		public const int IMS_E_EMPTY_QUERY_ERROR = -2147220981;

		public const int IMS_E_REMOVE_ONGOING_SEARCH = -2147220980;

		public const int IMS_E_OVER_BUDGET_ERROR = -2147220979;

		public const int IMS_E_DISABLED_ERROR = -2147220978;

		public const int IMS_S_SEARCH_ALREADY_STARTED = 262657;

		public const int IMS_S_SEARCH_NOT_STARTED = 262658;
	}
}
