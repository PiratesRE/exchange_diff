using System;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;

namespace Microsoft.Exchange.Assistants
{
	internal class Base
	{
		internal EventLogger Logger
		{
			get
			{
				return SingletonEventLogger.Logger;
			}
		}

		public void CatchMeIfYouCan(CatchMe function, string nonLocalizedAssistantName)
		{
			try
			{
				Util.CatchMeIfYouCan(function, nonLocalizedAssistantName);
			}
			catch (AIException ex)
			{
				ExTraceGlobals.PFDTracer.TraceError<Base, AIException>((long)this.GetHashCode(), "{0}: Exception thrown: {1}", this, ex);
				this.LogEvent(AssistantsEventLogConstants.Tuple_GenericException, null, new object[]
				{
					ex.ToString()
				});
				if (ex is AIGrayException)
				{
					this.LogEvent(AssistantsEventLogConstants.Tuple_GrayException, ex.ToString(), new object[]
					{
						ex.ToString()
					});
				}
				throw;
			}
		}

		public virtual void ExportToQueryableObject(QueryableObject queryableObject)
		{
		}

		internal void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			this.Logger.LogEvent(tuple, periodicKey, messageArgs);
		}

		internal void TracePfd(string format, params object[] args)
		{
			ExTraceGlobals.PFDTracer.TracePfd((long)this.GetHashCode(), format, args);
		}
	}
}
