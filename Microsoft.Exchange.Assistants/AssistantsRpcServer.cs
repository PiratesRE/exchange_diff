using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class AssistantsRpcServer : AssistantsRpcServerBase
	{
		public static void StartServer(SecurityIdentifier exchangeServersSid)
		{
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			FileSystemAccessRule accessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.ReadData, AccessControlType.Allow);
			SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
			FileSystemAccessRule rule = new FileSystemAccessRule(identity, FileSystemRights.ReadData, AccessControlType.Allow);
			FileSystemAccessRule rule2 = new FileSystemAccessRule(exchangeServersSid, FileSystemRights.ReadData, AccessControlType.Allow);
			FileSecurity fileSecurity = new FileSecurity();
			fileSecurity.SetOwner(securityIdentifier);
			fileSecurity.SetAccessRule(accessRule);
			fileSecurity.AddAccessRule(rule);
			fileSecurity.AddAccessRule(rule2);
			RpcServerBase.RegisterServer(typeof(AssistantsRpcServer), fileSecurity, 1);
		}

		public static void RegisterAssistant(string assistantName, TimeBasedAssistantController controller)
		{
			AssistantsRpcServer.Tracer.TraceDebug<string>(0L, "Assistant {0} registered", assistantName);
			lock (AssistantsRpcServer.timeBasedAssistants)
			{
				AssistantsRpcServer.timeBasedAssistants.Add(assistantName, controller);
			}
		}

		public static void DeregisterAssistant(string assistantName)
		{
			AssistantsRpcServer.Tracer.TraceDebug<string>(0L, "Assistant {0} deregistered", assistantName);
			lock (AssistantsRpcServer.timeBasedAssistants)
			{
				AssistantsRpcServer.timeBasedAssistants.Remove(assistantName);
			}
		}

		public override void RunNow(string assistantName, ValueType mailboxGuid, ValueType mdbGuid)
		{
			ExAssert.RetailAssert(false, "RunNow must not be invoked. Use RunNowHR instead.");
		}

		public override void Halt(string assistantName)
		{
			ExAssert.RetailAssert(false, "Halt must not be invoked. Use HaltHR instead.");
		}

		public override int RunNowHR(string assistantName, ValueType mailboxGuid, ValueType mdbGuid)
		{
			AssistantsRpcServer.Tracer.TraceDebug<string, ValueType, ValueType>((long)this.GetHashCode(), "RunNowHR requested for assistant={0}, mailbox={1}, database={2}", assistantName, mailboxGuid, mdbGuid);
			return this.RunNowWithParamsHR(assistantName, mailboxGuid, mdbGuid, null);
		}

		public override int RunNowWithParamsHR(string assistantName, ValueType mailboxGuid, ValueType mdbGuid, string parameters)
		{
			AssistantsRpcServer.Tracer.TraceDebug((long)this.GetHashCode(), "RunNowWithParamsHR requested for assistant={0}, mailbox={1}, database={2}, parameters={3}", new object[]
			{
				assistantName,
				mailboxGuid,
				mdbGuid,
				string.IsNullOrEmpty(parameters) ? "<null>" : parameters
			});
			return AssistantsRpcServer.Execute(delegate
			{
				TimeBasedAssistantController controller = AssistantsRpcServer.GetController(assistantName);
				controller.RunNow((Guid)mailboxGuid, (Guid)mdbGuid, parameters);
			}, assistantName);
		}

		public override int HaltHR(string assistantName)
		{
			AssistantsRpcServer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "HaltHR requested for assistant={0}", assistantName);
			return AssistantsRpcServer.Execute(delegate
			{
				TimeBasedAssistantController controller = AssistantsRpcServer.GetController(assistantName);
				controller.Halt();
			}, assistantName);
		}

		private static TimeBasedAssistantController GetController(string assistantName)
		{
			TimeBasedAssistantController timeBasedAssistantController;
			AssistantsRpcServer.timeBasedAssistants.TryGetValue(assistantName, out timeBasedAssistantController);
			if (timeBasedAssistantController == null)
			{
				AssistantsRpcServer.Tracer.TraceError<string>(0L, "Assistant {0} unknown", assistantName);
				throw new UnknownAssistantException(assistantName);
			}
			return timeBasedAssistantController;
		}

		private static int Execute(GrayException.UserCodeDelegate function, string assistantName)
		{
			Exception exception = null;
			AssistantsRpcServer.Tracer.TraceDebug<string>(0L, "Executing the RPC request for assistant {0}.", assistantName);
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						function();
					}
					catch (MapiExceptionMdbOffline exception3)
					{
						exception = exception3;
					}
					catch (MapiExceptionNotFound exception4)
					{
						exception = exception4;
					}
					catch (MailboxOrDatabaseNotSpecifiedException exception5)
					{
						exception = exception5;
					}
					catch (UnknownAssistantException exception6)
					{
						exception = exception6;
					}
					catch (UnknownDatabaseException exception7)
					{
						exception = exception7;
					}
					catch (TransientException exception8)
					{
						exception = exception8;
					}
				});
			}
			catch (GrayException exception)
			{
				GrayException exception9;
				exception = exception9;
			}
			catch (Exception exception2)
			{
				exception = exception2;
				ExWatson.SendReportAndCrashOnAnotherThread(exception2);
			}
			if (exception != null)
			{
				return AssistantsRpcServer.LogExceptionAndGetHR(exception, assistantName);
			}
			return 0;
		}

		private static int LogExceptionAndGetHR(Exception ex, string assistantName)
		{
			AssistantsRpcServer.Tracer.TraceError<string, Exception>(0L, "LogExceptionAndGetHR: RPC request failed. for assistant={0}, exception={1}", assistantName, ex);
			SingletonEventLogger.Logger.LogEvent(AssistantsEventLogConstants.Tuple_RpcError, null, new object[]
			{
				"Execute",
				ex
			});
			return AssistantsRpcErrorCode.GetHRFromException(ex);
		}

		private static readonly Trace Tracer = ExTraceGlobals.AssistantsRpcServerTracer;

		private static Dictionary<string, TimeBasedAssistantController> timeBasedAssistants = new Dictionary<string, TimeBasedAssistantController>();
	}
}
