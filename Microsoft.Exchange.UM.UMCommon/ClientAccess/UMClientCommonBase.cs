using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess
{
	internal abstract class UMClientCommonBase : DisposableBase
	{
		protected static UMClientAccessCountersInstance Counters
		{
			get
			{
				return UMClientCommonBase.counters;
			}
		}

		protected string TracePrefix
		{
			set
			{
				this.tracePrefix = value;
			}
		}

		public static void InitializePerformanceCounters(bool isWebService)
		{
			try
			{
				UMClientCommonBase.counters = UMClientAccessCounters.GetInstance(isWebService ? "Outlook" : "OWA");
				UMClientAccessCounters.ResetInstance(UMClientCommonBase.counters.Name);
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					UMClientCommonBase.counters.PID.RawValue = (long)currentProcess.Id;
				}
			}
			catch (InvalidOperationException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.ClientAccessTracer, 0, "Failed to initialize perf counters: {0}", new object[]
				{
					ex
				});
			}
		}

		public UMCallInfoEx GetCallInfo(string callId)
		{
			UMCallInfoEx result;
			try
			{
				this.DebugTrace("GetCallInfo({0})", new object[]
				{
					callId
				});
				string fqdn;
				string text;
				this.DecodeCallId(callId, out fqdn, out text);
				UMServerProxy server = UMServerManager.GetServer(fqdn);
				UMCallInfoEx callInfo = server.GetCallInfo(text);
				this.DebugTrace("GetCallInfo({0}): CallId:{1} CallState:{2}, EvtCause:{3}, RespCode:{4}, RespText:{5}", new object[]
				{
					callId,
					text,
					callInfo.CallState,
					callInfo.EventCause,
					callInfo.ResponseCode,
					callInfo.ResponseText
				});
				result = callInfo;
			}
			catch (LocalizedException exception)
			{
				this.LogException(exception);
				throw;
			}
			return result;
		}

		public void Disconnect(string callId)
		{
			try
			{
				this.DebugTrace("Disconnect({0})", new object[]
				{
					callId
				});
				string fqdn;
				string callId2;
				this.DecodeCallId(callId, out fqdn, out callId2);
				UMServerProxy server = UMServerManager.GetServer(fqdn);
				if (!UMServerManager.IsAuthorizedUMServer(server))
				{
					throw new FormatException("Invalid tokens in callId: " + callId);
				}
				UMCallInfoEx callInfo = server.GetCallInfo(callId2);
				if (callInfo == null || !Enum.IsDefined(typeof(UMCallState), callInfo.CallState))
				{
					throw new FormatException("Invalid tokens in callId: " + callId);
				}
				server.Disconnect(callId2);
			}
			catch (LocalizedException exception)
			{
				this.LogException(exception);
				throw;
			}
			catch (FormatException ex)
			{
				this.LogException(ex);
				throw new InvalidCallIdException(ex);
			}
		}

		protected abstract void DisposeMe();

		protected abstract string GetUserContext();

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeMe();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UMClientCommonBase>(this);
		}

		protected void DebugTrace(string formatString, params object[] formatObjects)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.ClientAccessTracer, this.GetHashCode(), this.tracePrefix + formatString, formatObjects);
		}

		protected void LogException(Exception exception)
		{
			this.DebugTrace("{0}", new object[]
			{
				exception
			});
			StackTrace stackTrace = new StackTrace();
			string name = stackTrace.GetFrame(1).GetMethod().Name;
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMClientAccessError, null, new object[]
			{
				name,
				this.GetUserContext(),
				exception.Message
			});
		}

		protected string EncodeCallId(string serverFqdn, string sessionId)
		{
			this.DebugTrace("EncodeCallId: ServerFqdn:{0} SessionID:{1}", new object[]
			{
				serverFqdn,
				sessionId
			});
			string s = sessionId + "|" + serverFqdn;
			string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(s), Base64FormattingOptions.None);
			this.DebugTrace("EncodeCallId(Base64): {0}", new object[]
			{
				text
			});
			return text;
		}

		private void DecodeCallId(string base64CallId, out string serverFqdn, out string sessionId)
		{
			serverFqdn = null;
			sessionId = null;
			try
			{
				this.DebugTrace("DecodeCallId(Base64CallId:{0})", new object[]
				{
					base64CallId
				});
				if (string.IsNullOrEmpty(base64CallId) || base64CallId.Length > 1420)
				{
					throw new FormatException();
				}
				byte[] array = Convert.FromBase64String(base64CallId);
				if (array == null || array.Length == 0)
				{
					throw new FormatException();
				}
				string @string = Encoding.UTF8.GetString(array);
				string[] array2 = @string.Split(new char[]
				{
					'|'
				});
				if (array2 == null || array2.Length != 2)
				{
					throw new FormatException("Invalid tokens in callId: " + @string);
				}
				serverFqdn = array2[1];
				sessionId = array2[0];
				this.DebugTrace("DecodeCallId: ServerFqdn:{0} SessionID:{1}", new object[]
				{
					serverFqdn,
					sessionId
				});
			}
			catch (FormatException innerException)
			{
				throw new InvalidCallIdException(innerException);
			}
		}

		private const int MaxBase64CallIdLength = 1420;

		private static UMClientAccessCountersInstance counters;

		private string tracePrefix = string.Empty;
	}
}
