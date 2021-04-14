using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Probes
{
	public class DatabaseAvailabilityProbeBase : ProbeWorkItem
	{
		protected bool ActiveCopy
		{
			get
			{
				return this.activeCopy;
			}
			set
			{
				this.activeCopy = value;
			}
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition definition, Dictionary<string, string> propertyBag)
		{
			StoreDiscovery.PopulateProbeDefinition(definition as ProbeDefinition, propertyBag["TargetResource"], base.GetType(), definition.Name, TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0));
			MailboxDatabase mailboxDatabaseFromName = DirectoryAccessor.Instance.GetMailboxDatabaseFromName(propertyBag["TargetResource"]);
			definition.TargetExtension = mailboxDatabaseFromName.Guid.ToString();
			definition.Attributes["SystemMailboxGuid"] = DirectoryAccessor.Instance.GetSystemMailboxGuid(mailboxDatabaseFromName.Guid).ToString();
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>
			{
				new PropertyInformation("Identity", Strings.DatabaseAvailabilityHelpString, true)
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			Guid empty = Guid.Empty;
			string targetExtension = base.Definition.TargetExtension;
			string text = base.Definition.Attributes["SystemMailboxGuid"];
			this.databaseName = base.Definition.TargetResource;
			base.Result.StateAttribute1 = this.databaseName;
			base.Result.StateAttribute2 = targetExtension;
			base.Result.StateAttribute5 = text;
			if (string.IsNullOrWhiteSpace(targetExtension))
			{
				base.Result.StateAttribute12 = typeof(DatabaseGuidNotFoundException).FullName;
				throw new DatabaseGuidNotFoundException();
			}
			this.databaseGuid = new Guid(targetExtension);
			if (string.IsNullOrWhiteSpace(text))
			{
				base.Result.StateAttribute12 = typeof(SystemMailboxGuidNotFoundException).FullName;
				throw new SystemMailboxGuidNotFoundException();
			}
			this.systemMailboxGuid = new Guid(text);
			this.PerformValidation();
		}

		private void PerformValidation()
		{
			DateTime utcNow = DateTime.UtcNow;
			DatabaseCopyConnectivity.DatabaseAvailable result = DatabaseCopyConnectivity.DatabaseAvailable.Failure;
			WTFDiagnostics.TraceInformation<Guid>(ExTraceGlobals.StoreTracer, base.TraceContext, "Starting database availability check against database {0}", this.databaseGuid, null, "PerformValidation", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseAvailabilityProbeBase.cs", 157);
			try
			{
				DatabaseCopyConnectivity databaseCopyConnectivity = new DatabaseCopyConnectivity(this.databaseGuid);
				Exception unhandledExceptionFromValidate = null;
				Action delegateGetDiagnosticInfo = delegate()
				{
					try
					{
						result = databaseCopyConnectivity.Validate(this.systemMailboxGuid, this.activeCopy);
					}
					catch (Exception unhandledExceptionFromValidate)
					{
						unhandledExceptionFromValidate = unhandledExceptionFromValidate;
					}
				};
				IAsyncResult asyncResult = delegateGetDiagnosticInfo.BeginInvoke(delegate(IAsyncResult r)
				{
					delegateGetDiagnosticInfo.EndInvoke(r);
				}, null);
				if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds((double)base.Definition.TimeoutSeconds)))
				{
					if (result != DatabaseCopyConnectivity.DatabaseAvailable.NoOp)
					{
						base.Result.StateAttribute3 = ((databaseCopyConnectivity.IsActive != null) ? databaseCopyConnectivity.IsActive.ToString() : "Unknown");
						throw new TimeoutException(Strings.DatabaseAvailabilityTimeout);
					}
				}
				else if (unhandledExceptionFromValidate != null)
				{
					base.Result.StateAttribute3 = ((databaseCopyConnectivity.IsActive != null) ? databaseCopyConnectivity.IsActive.ToString() : "Unknown");
					base.Result.StateAttribute4 = result.ToString();
					if (unhandledExceptionFromValidate is NullReferenceException)
					{
						throw new DatabaseValidationNullRefException(this.databaseGuid.ToString());
					}
					throw unhandledExceptionFromValidate;
				}
				base.Result.StateAttribute3 = ((databaseCopyConnectivity.IsActive != null) ? databaseCopyConnectivity.IsActive.ToString() : "Unknown");
				base.Result.StateAttribute4 = result.ToString();
				if (result != DatabaseCopyConnectivity.DatabaseAvailable.NoOp)
				{
					base.Result.StateAttribute6 = databaseCopyConnectivity.Latency.TotalMilliseconds;
					LocalizedException exception = databaseCopyConnectivity.Exception;
					if (result != DatabaseCopyConnectivity.DatabaseAvailable.Success)
					{
						base.Result.StateAttribute11 = databaseCopyConnectivity.DiagnosticContext;
						if (exception != null)
						{
							WTFDiagnostics.TraceError<string, string, double, string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, exception.ToString(), base.Result.StateAttribute1, result.ToString(), base.Result.SampleValue, base.Result.StateAttribute2, base.Result.StateAttribute3, null, "PerformValidation", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseAvailabilityProbeBase.cs", 251);
							throw exception;
						}
						throw new Exception(Strings.DatabaseAvailabilityFailure(this.databaseGuid.ToString()));
					}
					else
					{
						WTFDiagnostics.TraceInformation<string, double, string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, base.Result.StateAttribute1, result.ToString(), base.Result.SampleValue, base.Result.StateAttribute2, base.Result.StateAttribute3, null, "PerformValidation", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseAvailabilityProbeBase.cs", 269);
					}
				}
			}
			catch (Exception ex)
			{
				if (string.IsNullOrWhiteSpace(base.Result.StateAttribute11))
				{
					base.Result.StateAttribute11 = ex.ToString();
				}
				base.Result.StateAttribute12 = ((ex.InnerException != null) ? ex.InnerException.GetType().FullName : ex.GetType().FullName);
				if (result == DatabaseCopyConnectivity.DatabaseAvailable.Failure)
				{
					throw;
				}
			}
			finally
			{
				base.Result.SampleValue = (double)((int)(DateTime.UtcNow - utcNow).TotalMilliseconds);
				if (result != DatabaseCopyConnectivity.DatabaseAvailable.NoOp)
				{
					StxLoggerBase.GetLoggerInstance(StxLogType.DatabaseAvailability).BeginAppend(this.databaseName, result == DatabaseCopyConnectivity.DatabaseAvailable.Success, (base.Result.StateAttribute6 != 0.0) ? TimeSpan.FromMilliseconds(base.Result.StateAttribute6) : TimeSpan.Zero, 0, base.Result.StateAttribute12, string.IsNullOrWhiteSpace(base.Result.StateAttribute3) ? "Unknown" : base.Result.StateAttribute3, base.Result.StateAttribute11, base.Result.ExecutionStartTime.ToString("O"), string.Empty);
				}
			}
		}

		private const string DatabaseCopyStateUnknown = "Unknown";

		private bool activeCopy;

		private Guid databaseGuid;

		private Guid systemMailboxGuid;

		private string databaseName;
	}
}
