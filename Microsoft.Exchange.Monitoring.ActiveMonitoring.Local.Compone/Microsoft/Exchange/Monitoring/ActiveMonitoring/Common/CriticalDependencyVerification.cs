using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal sealed class CriticalDependencyVerification
	{
		internal CriticalDependencyVerification(Trace trace, TracingContext traceContext)
		{
			this.trace = trace;
			this.traceContext = traceContext;
			this.dependencies = new ICriticalDependency[]
			{
				new EventLogCriticalDependency(this.trace, this.traceContext),
				new DNSCriticalDependency()
			};
		}

		public bool VerifyDependencies()
		{
			bool result = true;
			int num = this.dependencies.Length;
			for (int i = 0; i < num; i++)
			{
				ICriticalDependency criticalDependency = this.dependencies[i];
				DateTime d;
				DateTime d2;
				DateTime dateTime;
				DateTime dateTime2;
				criticalDependency.RetrieveHistory(out d, out d2, out dateTime, out dateTime2);
				string message = string.Format("Verifying critical dependency {0} of {1}: '{2}'. Last successful verification occurred {3}, last failed verification occurred {4}, last fix attempt occurred {5}, last escalation occurred {6}.", new object[]
				{
					i + 1,
					num,
					criticalDependency.Name,
					(d == DateTime.MinValue) ? "never" : d.ToString("u"),
					(d2 == DateTime.MinValue) ? "never" : d2.ToString("u"),
					(dateTime == DateTime.MinValue) ? "never" : dateTime.ToString("u"),
					(dateTime2 == DateTime.MinValue) ? "never" : dateTime2.ToString("u")
				});
				WTFDiagnostics.TraceInformation(this.trace, this.traceContext, message, null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 102);
				bool flag = this.RunTaskWithTimeout(new Func<bool>(criticalDependency.TestCriticalDependency), CriticalDependencyVerification.TestTimeLimit, criticalDependency.Name);
				if (flag)
				{
					WTFDiagnostics.TraceInformation<string>(this.trace, this.traceContext, "Critical dependency test '{0}' succeeded on first attempt.", criticalDependency.Name, null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 108);
					criticalDependency.RecordSuccessfulTest();
				}
				else
				{
					WTFDiagnostics.TraceWarning<string>(this.trace, this.traceContext, "Critical dependency test '{0}' failed on first attempt.", criticalDependency.Name, null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 115);
					criticalDependency.RecordFailedTest();
					if (DateTime.UtcNow.Subtract(CriticalDependencyVerification.FixRetryWindow).CompareTo(dateTime) > 0)
					{
						WTFDiagnostics.TraceInformation<string, string>(this.trace, this.traceContext, "Attempting to fix critical dependency '{0}'. Last fix attempt was {1}.", criticalDependency.Name, (dateTime == DateTime.MinValue) ? "never" : dateTime.ToString("u"), null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 123);
						criticalDependency.RecordFixAttempt();
						bool flag2 = this.RunTaskWithTimeout(new Func<bool>(criticalDependency.FixCriticalDependency), CriticalDependencyVerification.FixTimeLimit, criticalDependency.Name);
						if (flag2)
						{
							Thread.Sleep(criticalDependency.RetestDelay);
							bool flag3 = this.RunTaskWithTimeout(new Func<bool>(criticalDependency.TestCriticalDependency), CriticalDependencyVerification.TestTimeLimit, criticalDependency.Name);
							if (flag3)
							{
								WTFDiagnostics.TraceInformation<string>(this.trace, this.traceContext, "Critical dependency test '{0}' succeeded on second attempt.", criticalDependency.Name, null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 136);
								criticalDependency.RecordSuccessfulTest();
								goto IL_45A;
							}
							WTFDiagnostics.TraceWarning<string>(this.trace, this.traceContext, "Critical dependency test '{0}' failed on second attempt.", criticalDependency.Name, null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 143);
							criticalDependency.RecordFailedTest();
						}
					}
					else
					{
						WTFDiagnostics.TraceInformation<string, string>(this.trace, this.traceContext, "Not trying to fix critical dependency '{0}'. Last fix attempt was {1}.", criticalDependency.Name, (dateTime == DateTime.MinValue) ? "never" : dateTime.ToString("u"), null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 149);
					}
					result = false;
					if (d.CompareTo(dateTime2) <= 0)
					{
						WTFDiagnostics.TraceInformation<string, string, string>(this.trace, this.traceContext, "Skipping escalation for critical dependency '{0}' because we have not recorded a successful test result since the last escalation. Last success was {1}. Last escalation was {2}.", criticalDependency.Name, (d == DateTime.MinValue) ? "never" : d.ToString("u"), (dateTime2 == DateTime.MinValue) ? "never" : dateTime2.ToString("u"), null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 163);
						break;
					}
					if (DateTime.UtcNow.Subtract(CriticalDependencyVerification.EscalateRetryWindow).CompareTo(dateTime2) <= 0)
					{
						WTFDiagnostics.TraceInformation<string, string, string>(this.trace, this.traceContext, "Skipping escalation for critical dependency '{0}' because it has not been long enough since the last escalation. Last escalation was {1} and we need to wait until {2} before escalating again.", criticalDependency.Name, dateTime2.ToString("u"), dateTime2.Add(CriticalDependencyVerification.EscalateRetryWindow).ToString("u"), null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 174);
						break;
					}
					WTFDiagnostics.TraceInformation<string>(this.trace, this.traceContext, "Raising escalation for critical dependency '{0}'.", criticalDependency.Name, null, "VerifyDependencies", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 185);
					criticalDependency.RecordEscalation();
					criticalDependency.RaiseAlert(dateTime, this.trace, this.traceContext);
					break;
				}
				IL_45A:;
			}
			return result;
		}

		private bool RunTaskWithTimeout(Func<bool> func, TimeSpan timeout, string name)
		{
			bool result = false;
			Task<bool> task = Task<bool>.Factory.StartNew(func);
			try
			{
				if (task.Wait(timeout) && task.Status == TaskStatus.RanToCompletion)
				{
					result = task.Result;
				}
				else
				{
					WTFDiagnostics.TraceError<string>(this.trace, this.traceContext, "Critical dependency '{0}' timed out", name, null, "RunTaskWithTimeout", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 222);
				}
			}
			catch (AggregateException ex)
			{
				AggregateException ex2;
				ex2.Handle(delegate(Exception ex)
				{
					WTFDiagnostics.TraceError<string, string>(this.trace, this.traceContext, "Critical dependency '{0}' threw exception: {1}", name, ex.ToString(), null, "RunTaskWithTimeout", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\CriticalDependencyVerification.cs", 229);
					return true;
				});
			}
			return result;
		}

		private static TimeSpan TestTimeLimit = TimeSpan.FromSeconds(30.0);

		private static TimeSpan FixTimeLimit = TimeSpan.FromSeconds(30.0);

		private static TimeSpan FixRetryWindow = TimeSpan.FromHours(2.0);

		private static TimeSpan EscalateRetryWindow = TimeSpan.FromHours(24.0);

		private Trace trace;

		private TracingContext traceContext;

		private ICriticalDependency[] dependencies;
	}
}
