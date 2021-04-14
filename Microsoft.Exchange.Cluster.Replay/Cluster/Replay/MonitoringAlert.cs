using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class MonitoringAlert
	{
		protected static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		protected MonitoringAlert(string identity, Guid identityGuid)
		{
			this.m_identity = identity;
			this.m_identityGuid = identityGuid;
			this.Init();
		}

		protected virtual TimeSpan DatabaseHealthCheckGreenTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckGreenTransitionSuppressionInSec);
			}
		}

		protected virtual TimeSpan DatabaseHealthCheckGreenPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckGreenPeriodicIntervalInSec);
			}
		}

		protected virtual TimeSpan DatabaseHealthCheckRedTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckRedTransitionSuppressionInSec);
			}
		}

		protected virtual TimeSpan DatabaseHealthCheckRedPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckRedPeriodicIntervalInSec);
			}
		}

		protected virtual bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public string Identity
		{
			get
			{
				return this.m_identity;
			}
		}

		public Guid IdentityGuid
		{
			get
			{
				return this.m_identityGuid;
			}
		}

		public TransientErrorInfo.ErrorType CurrentAlertState { get; private set; }

		public string ErrorMessage { get; private set; }

		public string ErrorMessageWithoutFullStatus { get; private set; }

		public void RaiseAppropriateAlertIfNecessary(IHealthValidationResultMinimal result)
		{
			if (!this.IsEnabled)
			{
				MonitoringAlert.Tracer.TraceDebug<string>((long)this.GetHashCode(), "MonitoringAlert: RaiseAppropriateAlertIfNecessary() for '{0}' is skipped because the alert is disabled!", this.Identity);
				this.m_resetEligible = false;
				return;
			}
			this.m_resetEligible = true;
			TransientErrorInfo.ErrorType errorType;
			if (this.IsValidationSuccessful(result))
			{
				if (this.m_alertSuppression.ReportSuccessPeriodic(out errorType))
				{
					this.RaiseAppropriateEvent(errorType, result);
				}
			}
			else if (this.m_alertSuppression.ReportFailurePeriodic(out errorType))
			{
				this.RaiseAppropriateEvent(errorType, result);
			}
			this.CurrentAlertState = errorType;
			this.ErrorMessage = result.ErrorMessage;
			this.ErrorMessageWithoutFullStatus = result.ErrorMessageWithoutFullStatus;
		}

		public virtual void ResetState()
		{
			if (this.m_resetEligible)
			{
				MonitoringAlert.Tracer.TraceDebug<string>((long)this.GetHashCode(), "MonitoringAlert: ResetState() called! Alert state for '{0}' is being overwritten to 'Unknown'.", this.Identity);
				this.Init();
				this.m_resetEligible = false;
			}
		}

		protected virtual bool IsValidationSuccessful(IHealthValidationResultMinimal result)
		{
			return result.IsValidationSuccessful;
		}

		protected abstract void RaiseGreenEvent(IHealthValidationResultMinimal result);

		protected abstract void RaiseRedEvent(IHealthValidationResultMinimal result);

		private void Init()
		{
			this.m_alertSuppression = new TransientErrorInfoPeriodic(this.DatabaseHealthCheckGreenTransitionSuppression, this.DatabaseHealthCheckGreenPeriodicInterval, this.DatabaseHealthCheckRedTransitionSuppression, this.DatabaseHealthCheckRedPeriodicInterval, TransientErrorInfo.ErrorType.Unknown);
			this.CurrentAlertState = TransientErrorInfo.ErrorType.Unknown;
			this.ErrorMessage = null;
			this.ErrorMessageWithoutFullStatus = null;
		}

		private void RaiseAppropriateEvent(TransientErrorInfo.ErrorType currentState, IHealthValidationResultMinimal result)
		{
			if (currentState == TransientErrorInfo.ErrorType.Success)
			{
				this.RaiseGreenEvent(result);
				return;
			}
			if (currentState == TransientErrorInfo.ErrorType.Failure)
			{
				this.RaiseRedEvent(result);
			}
		}

		protected const string MonitorStateRegkey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States";

		private readonly string m_identity;

		private readonly Guid m_identityGuid;

		private bool m_resetEligible = true;

		private TransientErrorInfoPeriodic m_alertSuppression;
	}
}
