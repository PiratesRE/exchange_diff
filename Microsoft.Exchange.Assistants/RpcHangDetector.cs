using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class RpcHangDetector : HangDetector
	{
		private RpcHangDetector()
		{
		}

		private string CallStack { get; set; }

		public static RpcHangDetector Create()
		{
			return new RpcHangDetector
			{
				Timeout = Configuration.HangDetectionTimeout,
				Period = Configuration.HangDetectionPeriod
			};
		}

		protected override void OnHangDetected(int hangsDetected)
		{
			if (hangsDetected == 0)
			{
				try
				{
					base.MonitoredThread.Suspend();
					try
					{
						this.CallStack = new StackTrace(base.MonitoredThread, true).ToString();
					}
					finally
					{
						base.MonitoredThread.Resume();
					}
				}
				catch
				{
					this.CallStack = "Unknown";
				}
				SingletonEventLogger.Logger.LogEvent(AssistantsEventLogConstants.Tuple_ShutdownAssistantsThreadHanging, null, new object[]
				{
					base.DatabaseName,
					base.AssistantName,
					base.InvokeTime.ToLocalTime(),
					this.CallStack
				});
				return;
			}
			SingletonEventLogger.Logger.LogEvent(AssistantsEventLogConstants.Tuple_ShutdownAssistantsThreadHangPersisted, null, new object[]
			{
				base.DatabaseName,
				base.AssistantName,
				base.InvokeTime.ToLocalTime(),
				this.CallStack
			});
			string text = string.Format("Hung detected for assistant {0} on database {1}. Shutting down service by calling Process.Kill.", base.AssistantName, base.DatabaseName);
			StackTrace stackTrace = new StackTrace(1);
			MethodBase method = stackTrace.GetFrame(0).GetMethod();
			AssemblyName name = method.DeclaringType.Assembly.GetName();
			int hashCode = (method.Name + text + base.AssistantName).GetHashCode();
			Thread.Sleep(TimeSpan.FromSeconds(10.0));
			ExWatson.SendGenericWatsonReport("E12", ExWatson.RealApplicationVersion.ToString(), ExWatson.RealAppName, name.Version.ToString(), name.Name, text, this.CallStack, hashCode.ToString("x"), method.Name, this.GetCrashingMessage());
			Process.GetCurrentProcess().Kill();
		}

		private string GetCrashingMessage()
		{
			StringBuilder stringBuilder = new StringBuilder("Assistant ");
			stringBuilder.Append(base.AssistantName ?? "Unknown");
			stringBuilder.Append(" timed out during shutdown\r\n");
			stringBuilder.Append(AIBreadcrumbs.Instance.GenerateReport());
			return stringBuilder.ToString();
		}
	}
}
