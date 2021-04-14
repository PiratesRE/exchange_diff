using System;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal class CmdletErrorContext
	{
		internal CmdletErrorContext(Type exceptionType) : this(exceptionType, null, null)
		{
		}

		internal CmdletErrorContext(Type exceptionType, Type innerExceptionType) : this(exceptionType, innerExceptionType, null)
		{
		}

		internal CmdletErrorContext(Type exceptionType, Type innerExceptionType, string hostName)
		{
			if (exceptionType == null)
			{
				throw new ArgumentNullException("exceptionType");
			}
			this.exceptionType = exceptionType;
			this.innerExceptionType = innerExceptionType;
			this.hostName = hostName;
		}

		internal Type ExceptionType
		{
			get
			{
				return this.exceptionType;
			}
		}

		internal Type InnerExceptionType
		{
			get
			{
				return this.innerExceptionType;
			}
		}

		internal string HostName
		{
			get
			{
				return this.hostName;
			}
		}

		internal bool MatchesErrorContext(Exception exception, string hostName)
		{
			return this.exceptionType.IsInstanceOfType(exception) && (!(this.innerExceptionType != null) || exception.InnerException == null || this.innerExceptionType.IsInstanceOfType(exception.InnerException)) && (this.HostName == null || this.HostName.Equals(hostName));
		}

		private Type exceptionType;

		private Type innerExceptionType;

		private string hostName;
	}
}
