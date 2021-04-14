using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class FileDeleteResponder : ResponderWorkItem
	{
		internal string FileDeleteTargets { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, string fileDeleteTargets, ServiceHealthStatus responderTargetState, bool enabled = true, int timeoutSeconds = 60, string throttleGroupName = null)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = FileDeleteResponder.AssemblyPath;
			responderDefinition.TypeName = FileDeleteResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = monitorName;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.MaxRetryAttempts = 10;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 10;
			responderDefinition.Enabled = enabled;
			responderDefinition.TimeoutSeconds = timeoutSeconds;
			responderDefinition.Attributes["FileDeleteTargets"] = fileDeleteTargets;
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.DeleteFile, monitorName, null);
			return responderDefinition;
		}

		protected void InitializeServiceAttributes(AttributeHelper attributeHelper)
		{
			this.FileDeleteTargets = attributeHelper.GetString("FileDeleteTargets", true, null);
		}

		protected virtual void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.InitializeServiceAttributes(attributeHelper);
		}

		protected void InternalFileDelete(CancellationToken cancellationToken)
		{
			Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
			{
				string[] array = this.FileDeleteTargets.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					int num = text.LastIndexOf(Path.DirectorySeparatorChar);
					if (num < 0)
					{
						WTFDiagnostics.TraceWarning(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("Skipping '{0}'. Cannot locate trailing slash.", text), null, "InternalFileDelete", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\FileDeleteResponder.cs", 140);
					}
					else
					{
						string text2 = text.Substring(0, num);
						string text3 = text.Substring(num + 1);
						if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3) && Directory.Exists(text2))
						{
							StringBuilder stringBuilder = new StringBuilder();
							StringBuilder stringBuilder2 = new StringBuilder();
							try
							{
								string[] files = Directory.GetFiles(text2, text3);
								foreach (string text4 in files)
								{
									try
									{
										File.Delete(text4);
										stringBuilder2.AppendLine(text4);
									}
									catch (Exception ex)
									{
										stringBuilder.AppendLine(string.Format("{0} : {1}", text4, ex.ToString()));
									}
								}
								string text5 = stringBuilder.ToString();
								if (text5.Length > 0)
								{
									WTFDiagnostics.TraceWarning(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, text5, null, "InternalFileDelete", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\FileDeleteResponder.cs", 176);
								}
								string message = "No files deleted";
								if (stringBuilder2.Length > 0)
								{
									message = string.Format("Deleted file list{0}{1}", Environment.NewLine, stringBuilder2.ToString());
								}
								WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, message, null, "InternalFileDelete", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\FileDeleteResponder.cs", 188);
							}
							catch (Exception ex2)
							{
								WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format(ex2.Message, new object[0]), null, "InternalFileDelete", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\FileDeleteResponder.cs", 195);
							}
						}
					}
				}
			});
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes();
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.DeleteFile, this.FileDeleteTargets, this, false, cancellationToken, null);
			recoveryActionRunner.Execute(delegate()
			{
				this.InternalFileDelete(cancellationToken);
			});
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(FileDeleteResponder).FullName;

		internal static class AttributeNames
		{
			internal const string FileDeleteTargets = "FileDeleteTargets";

			internal const string throttleGroupName = "throttleGroupName";
		}

		internal class DefaultValues
		{
			internal const int TimeoutInSeconds = 60;

			internal const string ServiceName = "Exchange";

			internal const bool Enabled = true;
		}
	}
}
