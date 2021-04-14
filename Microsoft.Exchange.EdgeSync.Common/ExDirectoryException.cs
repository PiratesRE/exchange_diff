using System;
using System.DirectoryServices.Protocols;

namespace Microsoft.Exchange.EdgeSync
{
	internal class ExDirectoryException : Exception
	{
		public ExDirectoryException(DirectoryOperationException e) : base(e.Message, e)
		{
			if (e.Response != null)
			{
				this.resultCode = e.Response.ResultCode;
			}
		}

		public ExDirectoryException(Exception e) : base(e.Message, e)
		{
		}

		public ExDirectoryException(string message, Exception e) : base(message, e)
		{
		}

		public ExDirectoryException(ResultCode resultCode, string message) : base(message)
		{
			this.resultCode = resultCode;
		}

		public ResultCode ResultCode
		{
			get
			{
				return this.resultCode;
			}
		}

		private ResultCode resultCode;
	}
}
