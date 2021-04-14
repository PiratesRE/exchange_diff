using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.AirSync
{
	internal class AirSyncContext : IAirSyncContext, IReadOnlyPropertyBag, INotificationManagerContext, ISyncLogger
	{
		internal AirSyncContext(HttpContext httpContext)
		{
			this.httpContext = httpContext;
			this.request = new AirSyncRequest(this, httpContext.Request);
			this.response = new AirSyncResponse(this, httpContext.Response);
			if (ConditionalRegistrationCache.Singleton.PropertyIsActive(AirSyncConditionalHandlerSchema.PerCallTracing))
			{
				this.perCallTracer = new StringBuilder();
			}
		}

		public Participant GetFullParticipant(string legDN)
		{
			Participant result;
			if (this.cachedParticipants.TryGetValue(legDN, out result))
			{
				this.participantCacheHits++;
				return result;
			}
			this.participantCacheMisses++;
			return null;
		}

		public void CacheParticipant(string legDN, Participant fullParticipant)
		{
			this.cachedParticipants.TryAdd(legDN, fullParticipant);
		}

		public string GetParticipantCacheData()
		{
			return string.Format("H:{0}, M:{1}", this.participantCacheHits, this.participantCacheMisses);
		}

		public bool PerCallTracingEnabled
		{
			get
			{
				return this.perCallTracer != null;
			}
		}

		IAirSyncUser IAirSyncContext.User
		{
			get
			{
				return this.user;
			}
			set
			{
				this.user = value;
			}
		}

		string IAirSyncContext.TaskDescription
		{
			get
			{
				return string.Format("{0}:{1}", ((IAirSyncContext)this).Request.CommandType, (this.user == null) ? "<no user>" : this.user.Name);
			}
		}

		TimeTracker IAirSyncContext.Tracker { get; set; }

		IPrincipal IAirSyncContext.Principal
		{
			get
			{
				return this.httpContext.User;
			}
			set
			{
				this.httpContext.User = value;
			}
		}

		string IAirSyncContext.WindowsLiveId
		{
			get
			{
				string text = this.httpContext.GetMemberName();
				if (string.IsNullOrEmpty(text))
				{
					text = this.httpContext.Request.Headers["X-WLID-MemberName"];
				}
				return text;
			}
		}

		Dictionary<EasFeature, bool> IAirSyncContext.FlightingOverrides
		{
			get
			{
				if (this.flightingOverrides == null)
				{
					if (GlobalSettings.AllowFlightingOverrides)
					{
						string text = this.httpContext.Request.Headers["X-EAS-FlightingOverrides"];
						if (!string.IsNullOrEmpty(text))
						{
							string[] array = text.Split(new char[]
							{
								','
							});
							foreach (string text2 in array)
							{
								string[] array3 = text2.Split(new char[]
								{
									'='
								});
								EasFeature key;
								bool value;
								if (array3.Length == 2 && Enum.TryParse<EasFeature>(array3[0], out key) && bool.TryParse(array3[1], out value))
								{
									if (this.flightingOverrides == null)
									{
										this.flightingOverrides = new Dictionary<EasFeature, bool>();
									}
									this.flightingOverrides[key] = value;
								}
							}
						}
					}
					if (this.flightingOverrides == null)
					{
						this.flightingOverrides = AirSyncContext.EmptyOverrides;
					}
				}
				return this.flightingOverrides;
			}
		}

		IAirSyncRequest IAirSyncContext.Request
		{
			get
			{
				return this.request;
			}
		}

		IAirSyncResponse IAirSyncContext.Response
		{
			get
			{
				return this.response;
			}
		}

		ProtocolLogger IAirSyncContext.ProtocolLogger
		{
			get
			{
				return this.protocolLogger;
			}
		}

		DeviceBehavior IAirSyncContext.DeviceBehavior { get; set; }

		public void Information(Trace tracer, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.InfoTrace, message, new object[0]);
			}
			tracer.Information(id, message);
		}

		public void Information(Trace tracer, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.InfoTrace, formatString, args);
			}
			tracer.Information(id, formatString, args);
		}

		public void Information<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.InfoTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.Information<T0>(id, formatString, arg0);
		}

		public void Information<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.InfoTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.Information<T0, T1>(id, formatString, arg0, arg1);
		}

		public void Information<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.InfoTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.Information<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceDebug(Trace tracer, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, message, new object[0]);
			}
			tracer.TraceDebug(id, message);
		}

		public void TraceDebug(Trace tracer, int lid, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, message, new object[0]);
			}
			tracer.TraceDebug(lid, id, message);
		}

		public void TraceDebug(Trace tracer, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, formatString, args);
			}
			tracer.TraceDebug(id, formatString, args);
		}

		public void TraceDebug<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TraceDebug<T0>(id, formatString, arg0);
		}

		public void TraceDebug(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, formatString, args);
			}
			tracer.TraceDebug(lid, id, formatString, args);
		}

		public void TraceDebug<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TraceDebug<T0>(lid, id, formatString, arg0);
		}

		public void TraceDebug<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TraceDebug<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceDebug<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TraceDebug<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceDebug<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TraceDebug<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceDebug<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.DebugTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TraceDebug<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceError(Trace tracer, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, message, new object[0]);
			}
			tracer.TraceError(id, message);
		}

		public void TraceError(Trace tracer, int lid, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, message, new object[0]);
			}
			tracer.TraceError(lid, id, message);
		}

		public void TraceError(Trace tracer, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, formatString, args);
			}
			tracer.TraceError(id, formatString, args);
		}

		public void TraceError<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TraceError<T0>(id, formatString, arg0);
		}

		public void TraceError(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, formatString, args);
			}
			tracer.TraceError(lid, id, formatString, args);
		}

		public void TraceError<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TraceError<T0>(lid, id, formatString, arg0);
		}

		public void TraceError<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TraceError<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceError<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TraceError<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceError<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TraceError<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceError<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.ErrorTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TraceError<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceFunction(Trace tracer, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, message, new object[0]);
			}
			tracer.TraceFunction(id, message);
		}

		public void TraceFunction(Trace tracer, int lid, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, message, new object[0]);
			}
			tracer.TraceFunction(lid, id, message);
		}

		public void TraceFunction(Trace tracer, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, formatString, args);
			}
			tracer.TraceFunction(id, formatString, args);
		}

		public void TraceFunction<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TraceFunction<T0>(id, formatString, arg0);
		}

		public void TraceFunction(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, formatString, args);
			}
			tracer.TraceFunction(lid, id, formatString, args);
		}

		public void TraceFunction<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TraceFunction<T0>(lid, id, formatString, arg0);
		}

		public void TraceFunction<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TraceFunction<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceFunction<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TraceFunction<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceFunction<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TraceFunction<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceFunction<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.FunctionTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TraceFunction<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TracePfd(Trace tracer, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, message, new object[0]);
			}
			tracer.TracePfd(id, message);
		}

		public void TracePfd(Trace tracer, int lid, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, message, new object[0]);
			}
			tracer.TracePfd(lid, id, message);
		}

		public void TracePfd(Trace tracer, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, formatString, args);
			}
			tracer.TracePfd(id, formatString, args);
		}

		public void TracePfd<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TracePfd<T0>(id, formatString, arg0);
		}

		public void TracePfd(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, formatString, args);
			}
			tracer.TracePfd(lid, id, formatString, args);
		}

		public void TracePfd<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TracePfd<T0>(lid, id, formatString, arg0);
		}

		public void TracePfd<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TracePfd<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TracePfd<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TracePfd<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TracePfd<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TracePfd<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TracePfd<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.PfdTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TracePfd<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceWarning(Trace tracer, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, message, new object[0]);
			}
			tracer.TraceWarning(id, message);
		}

		public void TraceWarning(Trace tracer, int lid, long id, string message)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, message, new object[0]);
			}
			tracer.TraceWarning(lid, id, message);
		}

		public void TraceWarning(Trace tracer, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, formatString, args);
			}
			tracer.TraceWarning(id, formatString, args);
		}

		public void TraceWarning<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TraceWarning<T0>(id, formatString, arg0);
		}

		public void TraceWarning(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, formatString, args);
			}
			tracer.TraceWarning(lid, id, formatString, args);
		}

		public void TraceWarning<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, formatString, new object[]
				{
					arg0
				});
			}
			tracer.TraceWarning<T0>(lid, id, formatString, arg0);
		}

		public void TraceWarning<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TraceWarning<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceWarning<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			tracer.TraceWarning<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceWarning<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TraceWarning<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceWarning<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.PerCallTracingEnabled)
			{
				this.PerCallTrace(TraceType.WarningTrace, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			tracer.TraceWarning<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		private string GetLogPrefix(TraceType traceType)
		{
			return string.Format("[{0}/{1}/{2}] ", DateTime.UtcNow, Thread.CurrentThread.ManagedThreadId, ((AirSyncContext.EasTraceType)traceType).ToString());
		}

		private void PerCallTrace(TraceType traceType, string formatString, params object[] args)
		{
			if (this.PerCallTracingEnabled)
			{
				string logPrefix = this.GetLogPrefix(traceType);
				if (args != null && args.Length > 0)
				{
					this.perCallTracer.AppendLine(logPrefix + string.Format(formatString, args));
					return;
				}
				this.perCallTracer.AppendLine(logPrefix + formatString);
			}
		}

		void IAirSyncContext.PrepareToHang()
		{
			this.user.PrepareToHang();
			((IAirSyncContext)this).Request.PrepareToHang();
			((IAirSyncContext)this).ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.TimeHang, ExDateTime.UtcNow);
		}

		void IAirSyncContext.WriteActivityContextData()
		{
			string activityContextData = ((IAirSyncContext)this).GetActivityContextData();
			if (!string.IsNullOrEmpty(activityContextData))
			{
				this.protocolLogger.SetValue(ProtocolLoggerData.ActivityContextData, activityContextData);
			}
			if (GlobalSettings.WriteActivityContextDiagnostics)
			{
				((IAirSyncContext)this).Response.AppendHeader("X-ActivityContextDiagnostics", string.IsNullOrEmpty(activityContextData) ? "<empty>" : activityContextData);
			}
		}

		string IAirSyncContext.GetActivityContextData()
		{
			return "ActivityID=" + ((INotificationManagerContext)this).ActivityScope.ActivityId.ToString() + ";" + LogRowFormatter.FormatCollection(WorkloadManagementLogger.FormatWlmActivity(((INotificationManagerContext)this).ActivityScope, true));
		}

		void IAirSyncContext.SetDiagnosticValue(PropertyDefinition propDef, object value)
		{
			this.diagnosticsProperties[propDef] = value;
		}

		void IAirSyncContext.ClearDiagnosticValue(PropertyDefinition propDef)
		{
			object obj;
			this.diagnosticsProperties.TryRemove(propDef, out obj);
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			object[] array = new object[propertyDefinitionArray.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitionArray)
			{
				array[num++] = this[propertyDefinition];
			}
			return array;
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				object result;
				try
				{
					object obj = null;
					if (this.diagnosticsProperties.TryGetValue(propertyDefinition, out obj))
					{
						result = obj;
					}
					else if (propertyDefinition == ConditionalHandlerSchema.SmtpAddress)
					{
						result = ((((IAirSyncContext)this).User == null) ? null : ((IAirSyncContext)this).User.SmtpAddress);
					}
					else if (propertyDefinition == ConditionalHandlerSchema.DisplayName)
					{
						result = ((((IAirSyncContext)this).User == null) ? null : ((IAirSyncContext)this).User.DisplayName);
					}
					else if (propertyDefinition == ConditionalHandlerSchema.TenantName)
					{
						result = ((((IAirSyncContext)this).User == null || ((IAirSyncContext)this).User.WindowsLiveId == null) ? null : SmtpAddress.Parse(((IAirSyncContext)this).User.WindowsLiveId).Domain);
					}
					else if (propertyDefinition == ConditionalHandlerSchema.WindowsLiveId)
					{
						result = ((IAirSyncContext)this).WindowsLiveId;
					}
					else if (propertyDefinition == ConditionalHandlerSchema.MailboxServer)
					{
						result = ((IAirSyncContext)this).User.ExchangePrincipal.MailboxInfo.Location.ServerFqdn;
					}
					else if (propertyDefinition == ConditionalHandlerSchema.MailboxDatabase)
					{
						result = ((((IAirSyncContext)this).User == null) ? null : ((IAirSyncContext)this).User.ExchangePrincipal.MailboxInfo.GetDatabaseGuid());
					}
					else if (propertyDefinition == ConditionalHandlerSchema.MailboxServerVersion)
					{
						result = ((((IAirSyncContext)this).User == null) ? null : ((IAirSyncContext)this).User.ExchangePrincipal.MailboxInfo.Location.ServerVersion);
					}
					else if (propertyDefinition == ConditionalHandlerSchema.IsMonitoringUser)
					{
						result = ((((IAirSyncContext)this).User == null) ? null : ((IAirSyncContext)this).User.IsMonitoringTestUser);
					}
					else if (propertyDefinition == ConditionalHandlerSchema.BudgetDelay)
					{
						double num;
						if (this.TryGetActivityContextStat(ActivityOperationType.UserDelay, out num))
						{
							result = num;
						}
						else
						{
							result = null;
						}
					}
					else if (propertyDefinition == ConditionalHandlerSchema.BudgetUsed)
					{
						double num2;
						if (this.TryGetActivityContextStat(ActivityOperationType.BudgetUsed, out num2))
						{
							result = num2;
						}
						else
						{
							result = null;
						}
					}
					else if (propertyDefinition == ConditionalHandlerSchema.BudgetLockedOut)
					{
						if (((IAirSyncContext)this).User != null)
						{
							ITokenBucket budgetTokenBucket = ((IAirSyncContext)this).User.GetBudgetTokenBucket();
							result = (budgetTokenBucket != null && budgetTokenBucket.Locked);
						}
						else
						{
							result = null;
						}
					}
					else if (propertyDefinition == ConditionalHandlerSchema.BudgetLockedUntil)
					{
						if (((IAirSyncContext)this).User != null)
						{
							ITokenBucket budgetTokenBucket2 = ((IAirSyncContext)this).User.GetBudgetTokenBucket();
							result = ((budgetTokenBucket2 != null) ? budgetTokenBucket2.LockedUntilUtc : new DateTime?(DateTime.MinValue));
						}
						else
						{
							result = null;
						}
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.DeviceId)
					{
						result = ((((INotificationManagerContext)this).DeviceIdentity != null) ? ((INotificationManagerContext)this).DeviceIdentity.DeviceId : null);
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.DeviceType)
					{
						result = ((((INotificationManagerContext)this).DeviceIdentity != null) ? ((INotificationManagerContext)this).DeviceIdentity.DeviceType : null);
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.ProtocolVersion)
					{
						result = ((IAirSyncContext)this).Request.Version;
					}
					else if (propertyDefinition == ConditionalHandlerSchema.ActivityId)
					{
						result = ((INotificationManagerContext)this).ActivityScope.ActivityId;
					}
					else if (propertyDefinition == ConditionalHandlerSchema.Cmd)
					{
						result = ((IAirSyncContext)this).Request.CommandType.ToString();
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.WbXmlRequestSize)
					{
						result = ((IAirSyncContext)this).Request.GetRawHttpRequest().TotalBytes;
					}
					else if (propertyDefinition == ConditionalHandlerSchema.ElapsedTime)
					{
						result = ExDateTime.UtcNow - ((INotificationManagerContext)this).RequestTime;
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.HttpStatus)
					{
						result = ((IAirSyncContext)this).Response.HttpStatusCode;
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.EasStatus)
					{
						int num3 = 0;
						if (this.protocolLogger.TryGetValue<int>(ProtocolLoggerData.StatusCode, out num3))
						{
							result = num3;
						}
						else
						{
							result = null;
						}
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.ProtocolError)
					{
						string text;
						if (this.protocolLogger.TryGetValue<string>(ProtocolLoggerData.Error, out text))
						{
							result = text;
						}
						else
						{
							result = null;
						}
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.Traces)
					{
						result = AirSyncInMemoryTraceHandler.GetInstance().GetExchangeDiagnosticsInfoData(default(DiagnosableParameters)).TraceData;
					}
					else if (propertyDefinition == ConditionalHandlerSchema.PostWlmElapsed)
					{
						TimeSpan timeSpan;
						if (((IAirSyncContext)this).TryGetElapsed(ConditionalHandlerIntermediateSchema.PostWlmStartTime, out timeSpan))
						{
							result = timeSpan;
						}
						else
						{
							result = null;
						}
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.ProxyElapsed)
					{
						TimeSpan timeSpan2;
						if (((IAirSyncContext)this).TryGetElapsed(ConditionalHandlerIntermediateSchema.ProxyStartTime, out timeSpan2))
						{
							result = timeSpan2;
						}
						else
						{
							result = null;
						}
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.ProxyFromServer)
					{
						result = (((IAirSyncContext)this).Request.WasProxied ? ((IAirSyncContext)this).Request.UserHostName : null);
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.WasProxied)
					{
						result = ((IAirSyncContext)this).Request.WasProxied;
					}
					else if (propertyDefinition == AirSyncConditionalHandlerSchema.TimeTracker)
					{
						result = ((IAirSyncContext)this).Tracker.ToString();
					}
					else
					{
						if (propertyDefinition == AirSyncConditionalHandlerSchema.XmlRequest)
						{
							try
							{
								XmlDocument xmlDocument = this.request.LoadRequestDocument();
								return (xmlDocument != null) ? AirSyncUtility.BuildOuterXml(xmlDocument) : null;
							}
							catch (Exception ex)
							{
								return ex.ToString();
							}
						}
						if (propertyDefinition == AirSyncConditionalHandlerSchema.XmlResponse)
						{
							result = ((((IAirSyncContext)this).Response != null && ((IAirSyncContext)this).Response.XmlDocument != null) ? AirSyncUtility.BuildOuterXml(((IAirSyncContext)this).Response.XmlDocument) : null);
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.ProtocolLoggerData)
						{
							string str = ((IAirSyncContext)this).ProtocolLogger.ToString();
							result = HttpUtility.UrlDecode(str);
						}
						else if (propertyDefinition == ConditionalHandlerSchema.ThrottlingPolicyName)
						{
							result = ((IAirSyncContext)this).GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.GetShortIdentityString());
						}
						else if (propertyDefinition == ConditionalHandlerSchema.MaxConcurrency)
						{
							result = ((IAirSyncContext)this).GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.EasMaxConcurrency);
						}
						else if (propertyDefinition == ConditionalHandlerSchema.MaxBurst)
						{
							result = ((IAirSyncContext)this).GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.EasMaxBurst);
						}
						else if (propertyDefinition == ConditionalHandlerSchema.CutoffBalance)
						{
							result = ((IAirSyncContext)this).GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.EasCutoffBalance);
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.EasMaxDevices)
						{
							result = ((IAirSyncContext)this).GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.EasMaxDevices);
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.EasMaxDeviceDeletesPerMonth)
						{
							result = ((IAirSyncContext)this).GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.EasMaxDeviceDeletesPerMonth);
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.EasMaxInactivityForDeviceCleanup)
						{
							result = ((IAirSyncContext)this).GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.EasMaxInactivityForDeviceCleanup);
						}
						else if (propertyDefinition == ConditionalHandlerSchema.ThrottlingPolicyScope)
						{
							result = ((IAirSyncContext)this).GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.ThrottlingPolicyScope);
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.RequestHeaders)
						{
							result = ((IAirSyncContext)this).Request.GetHeadersAsString();
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.ResponseHeaders)
						{
							result = ((IAirSyncContext)this).Response.GetHeadersAsString();
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.UserWLMData)
						{
							result = UserWorkloadManagerHandler.GetInstance().GetExchangeDiagnosticsInfoData(DiagnosableParameters.Create("dumpcache", false, true, string.Empty));
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.EmptyRequest)
						{
							result = ((IAirSyncContext)this).Request.IsEmpty;
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.PerCallTracing)
						{
							result = ((this.perCallTracer == null) ? null : this.perCallTracer.ToString());
						}
						else if (propertyDefinition == AirSyncConditionalHandlerSchema.IsConsumerOrganizationUser)
						{
							if (((IAirSyncContext)this).User != null)
							{
								result = ((IAirSyncContext)this).User.IsConsumerOrganizationUser;
							}
							else
							{
								result = null;
							}
						}
						else
						{
							result = null;
						}
					}
				}
				catch (AirSyncPermanentException arg)
				{
					AirSyncDiagnostics.TraceDebug<string, AirSyncPermanentException>(ExTraceGlobals.DiagnosticsTracer, this, "[AirSyncContext.this[]] Caught exception trying to retrieve diagnostics property '{0}'.  Ignoring.  Exception: {1}", propertyDefinition.Name, arg);
					result = null;
				}
				return result;
			}
		}

		object IAirSyncContext.GetThrottlingPolicyValue(Func<IThrottlingPolicy, object> func)
		{
			if (((IAirSyncContext)this).User == null || ((IAirSyncContext)this).User.Budget == null)
			{
				return null;
			}
			return func(((IAirSyncContext)this).User.Budget.ThrottlingPolicy);
		}

		bool IAirSyncContext.TryGetElapsed(PropertyDefinition startTime, out TimeSpan elapsed)
		{
			elapsed = TimeSpan.Zero;
			object obj = this[startTime];
			if (obj != null)
			{
				ExDateTime dt = (ExDateTime)obj;
				elapsed = ExDateTime.UtcNow - dt;
				return true;
			}
			return false;
		}

		private bool TryGetActivityContextStat(ActivityOperationType operationType, out double value)
		{
			value = 0.0;
			IEnumerable<KeyValuePair<OperationKey, OperationStatistics>> statistics = ((INotificationManagerContext)this).ActivityScope.Statistics;
			foreach (KeyValuePair<OperationKey, OperationStatistics> keyValuePair in statistics)
			{
				if (keyValuePair.Key.ActivityOperationType == operationType)
				{
					TotalOperationStatistics totalOperationStatistics = keyValuePair.Value as TotalOperationStatistics;
					if (totalOperationStatistics != null)
					{
						value = totalOperationStatistics.Total;
						return true;
					}
					AverageOperationStatistics averageOperationStatistics = keyValuePair.Value as AverageOperationStatistics;
					if (averageOperationStatistics != null)
					{
						value = (double)averageOperationStatistics.CumulativeAverage;
						return true;
					}
				}
			}
			return false;
		}

		BudgetKey INotificationManagerContext.BudgetKey
		{
			get
			{
				return ((IAirSyncContext)this).User.BudgetKey;
			}
		}

		string INotificationManagerContext.SmtpAddress
		{
			get
			{
				return ((IAirSyncContext)this).User.SmtpAddress;
			}
		}

		DeviceIdentity INotificationManagerContext.DeviceIdentity
		{
			get
			{
				return ((IAirSyncContext)this).Request.DeviceIdentity;
			}
		}

		Guid INotificationManagerContext.MdbGuid
		{
			get
			{
				return ((IAirSyncContext)this).User.ExchangePrincipal.MailboxInfo.GetDatabaseGuid();
			}
		}

		int INotificationManagerContext.MailboxPolicyHash
		{
			get
			{
				PolicyData policyData = ADNotificationManager.GetPolicyData(((IAirSyncContext)this).User);
				if (policyData != null)
				{
					return policyData.GetHashCode(((IAirSyncContext)this).Request.Version);
				}
				return 0;
			}
		}

		uint INotificationManagerContext.PolicyKey
		{
			get
			{
				if (((IAirSyncContext)this).Request.PolicyKey != null)
				{
					return ((IAirSyncContext)this).Request.PolicyKey.Value;
				}
				return 0U;
			}
		}

		int INotificationManagerContext.AirSyncVersion
		{
			get
			{
				return ((IAirSyncContext)this).Request.Version;
			}
		}

		ExDateTime INotificationManagerContext.RequestTime
		{
			get
			{
				return new ExDateTime(ExTimeZone.UtcTimeZone, this.httpContext.Timestamp.ToUniversalTime());
			}
		}

		Guid INotificationManagerContext.MailboxGuid
		{
			get
			{
				return ((IAirSyncContext)this).User.MailboxGuid;
			}
		}

		CommandType INotificationManagerContext.CommandType
		{
			get
			{
				return ((IAirSyncContext)this).Request.CommandType;
			}
		}

		IActivityScope INotificationManagerContext.ActivityScope { get; set; }

		IAccountValidationContext INotificationManagerContext.AccountValidationContext
		{
			get
			{
				if (this.httpContext.Items.Contains("AccountValidationContext"))
				{
					return (IAccountValidationContext)this.httpContext.Items["AccountValidationContext"];
				}
				return null;
			}
		}

		private static readonly Dictionary<EasFeature, bool> EmptyOverrides = new Dictionary<EasFeature, bool>();

		private Dictionary<EasFeature, bool> flightingOverrides;

		private ConcurrentDictionary<string, Participant> cachedParticipants = new ConcurrentDictionary<string, Participant>();

		private HttpContext httpContext;

		private IAirSyncRequest request;

		private IAirSyncResponse response;

		private StringBuilder perCallTracer;

		private IAirSyncUser user;

		private ProtocolLogger protocolLogger = new ProtocolLogger();

		private int participantCacheHits;

		private int participantCacheMisses;

		private ConcurrentDictionary<PropertyDefinition, object> diagnosticsProperties = new ConcurrentDictionary<PropertyDefinition, object>();

		private enum EasTraceType
		{
			None,
			DbgT,
			Dbg = 1,
			Wrn,
			WrnT = 2,
			Err,
			ErrT = 3,
			FtlT,
			Ftl = 4,
			Inf,
			InfT = 5,
			Prf,
			PrfT = 6,
			Fnc,
			FncT = 7,
			PfdT,
			Pfd = 8,
			Flt,
			FltI = 9
		}
	}
}
