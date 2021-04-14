using System;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class MrsConnectionsLog : ILog
	{
		public bool IsEnabled(LogLevel level)
		{
			if (level <= LogLevel.LogInfo)
			{
				switch (level)
				{
				case LogLevel.LogDebug:
					return MrsTracer.Provider.IsEnabled(TraceType.DebugTrace);
				case LogLevel.LogVerbose | LogLevel.LogDebug:
					break;
				case LogLevel.LogTrace:
					return MrsTracer.Provider.IsEnabled(TraceType.FunctionTrace);
				default:
					if (level == LogLevel.LogInfo)
					{
						return MrsTracer.Provider.IsEnabled(TraceType.DebugTrace);
					}
					break;
				}
			}
			else
			{
				if (level == LogLevel.LogWarn)
				{
					return MrsTracer.Provider.IsEnabled(TraceType.WarningTrace);
				}
				if (level == LogLevel.LogError)
				{
					return MrsTracer.Provider.IsEnabled(TraceType.ErrorTrace);
				}
			}
			return false;
		}

		public void Trace(string formatString, params object[] args)
		{
			MrsTracer.Provider.Function(string.Format(formatString, args), new object[0]);
		}

		public void Debug(string formatString, params object[] args)
		{
			MrsTracer.Provider.Debug(formatString, args);
		}

		public void Info(string formatString, params object[] args)
		{
			MrsTracer.Provider.Debug(formatString, args);
		}

		public void Warn(string formatString, params object[] args)
		{
			MrsTracer.Provider.Warning(formatString, args);
		}

		public void Error(string formatString, params object[] args)
		{
			MrsTracer.Provider.Error(formatString, args);
		}

		public void Fatal(string formatString, params object[] args)
		{
		}

		public void Fatal(Exception exception, string message = null)
		{
		}

		public void Assert(bool condition, string formatString, params object[] args)
		{
		}

		public void RetailAssert(bool condition, string formatString, params object[] args)
		{
			ExAssert.RetailAssert(condition, formatString);
		}
	}
}
