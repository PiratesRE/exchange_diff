using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Log : ILog
	{
		public Log(ILogEmitter logEmitter, LogLevel logLevel = LogLevel.LogDefault)
		{
			this.LogEmitter = logEmitter;
			this.LogLevel = logLevel;
		}

		protected ILogEmitter LogEmitter { get; set; }

		protected LogLevel LogLevel { get; set; }

		public bool IsEnabled(LogLevel level)
		{
			return this.LogLevel.HasFlag(level);
		}

		public virtual void Trace(string formatString, params object[] args)
		{
			if (this.IsEnabled(LogLevel.LogTrace))
			{
				this.LogEmitter.Emit(formatString, args);
			}
		}

		public virtual void Debug(string formatString, params object[] args)
		{
			if (this.IsEnabled(LogLevel.LogDebug))
			{
				this.LogEmitter.Emit(formatString, args);
			}
		}

		public virtual void Info(string formatString, params object[] args)
		{
			if (this.IsEnabled(LogLevel.LogInfo))
			{
				this.LogEmitter.Emit(formatString, args);
			}
		}

		public virtual void Warn(string formatString, params object[] args)
		{
			if (this.IsEnabled(LogLevel.LogWarn))
			{
				this.LogEmitter.Emit(formatString, args);
			}
		}

		public virtual void Error(string formatString, params object[] args)
		{
			if (this.IsEnabled(LogLevel.LogError))
			{
				this.LogEmitter.Emit(formatString, args);
			}
		}

		public virtual void Fatal(string formatString, params object[] args)
		{
			if (this.IsEnabled(LogLevel.LogFatal))
			{
				this.LogEmitter.Emit(formatString, args);
			}
		}

		public virtual void Fatal(Exception exception, string message = "")
		{
			if (this.IsEnabled(LogLevel.LogFatal))
			{
				string formatString = string.Format("{0}:{1}", message, exception.Message);
				this.LogEmitter.Emit(formatString, new object[0]);
			}
		}

		public virtual void Assert(bool condition, string formatString, params object[] args)
		{
			if (condition)
			{
				this.LogEmitter.Emit(formatString, args);
			}
		}

		public virtual void RetailAssert(bool condition, string formatString, params object[] args)
		{
			if (condition)
			{
				this.LogEmitter.Emit(formatString, args);
				ExAssert.RetailAssert(true, formatString, args);
			}
		}
	}
}
