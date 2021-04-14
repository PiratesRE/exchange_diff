using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Providers;
using Microsoft.Exchange.Management.ReportingTask;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	public abstract class FfoReportingTask<TOutputObject> : Task where TOutputObject : new()
	{
		public FfoReportingTask()
		{
			this.outputObjectTypeName = typeof(TOutputObject).Name.ToString();
			this.Diagnostics = new Diagnostics(this.ComponentName);
			this.configDataProvider = new Lazy<IConfigDataProvider>(() => ServiceLocator.Current.GetService<IAuthenticationProvider>().CreateConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId));
		}

		[Parameter(Mandatory = false, Position = 0)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Expression Expression { get; set; }

		[Parameter(Mandatory = false)]
		public string ProbeTag
		{
			get
			{
				return this.probeTag;
			}
			set
			{
				this.Diagnostics.ActivityId = value;
				this.probeTag = value;
			}
		}

		internal Diagnostics Diagnostics { get; private set; }

		internal IConfigDataProvider ConfigSession
		{
			get
			{
				return this.configDataProvider.Value;
			}
		}

		public abstract string ComponentName { get; }

		public abstract string MonitorEventName { get; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			try
			{
				FaultInjection.FaultInjectionTracer.TraceTest(3355847997U);
				this.Authenticate();
				this.ValidateParameters();
			}
			catch (InvalidExpressionException ex)
			{
				this.Diagnostics.TraceError(string.Format("ValidationError-{0}", ex.Message));
				base.WriteError(ex, ErrorCategory.InvalidArgument, null);
			}
			catch (Exception exception)
			{
				this.Diagnostics.TraceException(string.Format("{0} validation failed", this.outputObjectTypeName), exception);
				base.WriteError(new FfoReportingException(), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
				FaultInjection.FaultInjectionTracer.TraceTest(3070635325U);
				IReadOnlyList<TOutputObject> readOnlyList = this.AggregateOutput();
				this.Diagnostics.Checkpoint("AggregateOutput");
				try
				{
					this.Diagnostics.StartTimer("WriteObject");
					foreach (!0 ! in readOnlyList)
					{
						object sendToPipeline = !;
						base.WriteObject(sendToPipeline);
					}
				}
				finally
				{
					this.Diagnostics.StopTimer("WriteObject");
				}
				if (readOnlyList.Count == 0)
				{
					this.PostProcessRecordValidation();
				}
				this.Diagnostics.SetHealthGreen(this.MonitorEventName, string.Format("{0} completed", this.outputObjectTypeName));
			}
			catch (InvalidExpressionException exception)
			{
				this.Diagnostics.SetHealthGreen(this.MonitorEventName, string.Format("{0} completed with validation error", this.outputObjectTypeName));
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (Exception ex)
			{
				this.Diagnostics.SetHealthRed(this.MonitorEventName, string.Format("{0} failed: {1}", this.outputObjectTypeName, ex.ToString()), ex);
				base.WriteError(new FfoReportingException(), ErrorCategory.InvalidOperation, null);
			}
		}

		protected abstract IReadOnlyList<TOutputObject> AggregateOutput();

		protected virtual void OnAuthenticateOrganization()
		{
			ServiceLocator.Current.GetService<IAuthenticationProvider>().ResolveOrganizationId(this.Organization, this);
		}

		protected void Authenticate()
		{
			try
			{
				this.Diagnostics.StartTimer("Authentication");
				if (this.Organization == null)
				{
					if (base.CurrentOrganizationId.OrganizationalUnit == null)
					{
						throw new InvalidExpressionException(Strings.InvalidOrganization);
					}
					this.Organization = new OrganizationIdParameter(base.CurrentOrganizationId.OrganizationalUnit.Name);
				}
				this.OnAuthenticateOrganization();
			}
			catch (ManagementObjectNotFoundException innerException)
			{
				throw new InvalidExpressionException(Strings.InvalidOrganization, innerException);
			}
			finally
			{
				this.Diagnostics.StopTimer("Authentication");
			}
		}

		protected virtual void CustomInternalValidate()
		{
		}

		private void PostProcessRecordValidation()
		{
			this.Diagnostics.Checkpoint("PostProcessRecordValidation");
			Schema.Utilities.ValidateParameters(this, () => this.ConfigSession, new HashSet<CmdletValidator.ValidatorTypes>
			{
				CmdletValidator.ValidatorTypes.Postprocessing,
				CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession
			});
		}

		private void ValidateParameters()
		{
			try
			{
				this.Diagnostics.StartTimer("Validation");
				FfoExpressionVisitor<TOutputObject>.Parse(this.Expression, this);
				Schema.Utilities.ValidateParameters(this, () => this.ConfigSession, new HashSet<CmdletValidator.ValidatorTypes>
				{
					CmdletValidator.ValidatorTypes.Preprocessing
				});
				this.CustomInternalValidate();
			}
			finally
			{
				this.Diagnostics.StopTimer("Validation");
			}
		}

		private readonly string outputObjectTypeName;

		private string probeTag;

		private Lazy<IConfigDataProvider> configDataProvider;
	}
}
