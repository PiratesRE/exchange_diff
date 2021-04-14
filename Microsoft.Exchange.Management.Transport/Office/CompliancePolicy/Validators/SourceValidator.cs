using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.Tasks;

namespace Microsoft.Office.CompliancePolicy.Validators
{
	internal abstract class SourceValidator
	{
		public SourceValidator(Task.TaskErrorLoggingDelegate writeErrorDelegate, Action<LocalizedString> writeWarningDelegate, Func<LocalizedString, bool> shouldContinueDelegate, ExecutionLog logger, string logTag, string tenantId, SourceValidator.Clients client)
		{
			if (client == SourceValidator.Clients.NewCompliancePolicy || client == SourceValidator.Clients.SetCompliancePolicy)
			{
				ArgumentValidator.ThrowIfNull("writeErrorDelegate", writeErrorDelegate);
				ArgumentValidator.ThrowIfNull("writeWarningDelegate", writeWarningDelegate);
				ArgumentValidator.ThrowIfNull("shouldContinueDelegate", shouldContinueDelegate);
			}
			this.writeErrorDelegate = writeErrorDelegate;
			this.writeWarningDelegate = writeWarningDelegate;
			this.shouldContinueDelegate = shouldContinueDelegate;
			this.logger = logger;
			this.logTag = logTag;
			this.tenantId = tenantId;
			this.client = client;
			this.logCorrelationId = Guid.NewGuid().ToString();
		}

		protected SourceValidator.Clients Client
		{
			get
			{
				return this.client;
			}
		}

		protected void WriteError(Exception exception, ErrorCategory errorCategory)
		{
			if (this.writeErrorDelegate != null)
			{
				this.writeErrorDelegate(exception, errorCategory, null);
			}
		}

		protected void WriteWarning(LocalizedString message)
		{
			if (this.writeWarningDelegate != null)
			{
				this.writeWarningDelegate(message);
			}
		}

		protected bool ShouldContinue(LocalizedString message)
		{
			return this.shouldContinueDelegate != null && this.shouldContinueDelegate(message);
		}

		protected void LogOneEntry(ExecutionLog.EventType eventType, string messageFormat, params object[] args)
		{
			this.LogOneEntry(eventType, null, messageFormat, args);
		}

		protected void LogOneEntry(ExecutionLog.EventType eventType, Exception exception, string messageFormat, params object[] args)
		{
			if (this.logger != null)
			{
				string contextData = string.Format(messageFormat, args);
				this.logger.LogOneEntry(this.client.ToString(), this.tenantId, this.logCorrelationId, eventType, this.logTag, contextData, exception, null);
			}
		}

		protected static int GetMaxLimitFromConfig(string maxLimitKey, int defaultLimit, int existingItemCount)
		{
			int intFromConfig = Utils.GetIntFromConfig(maxLimitKey, defaultLimit);
			int num = intFromConfig - existingItemCount;
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}

		internal static bool IsWideScope(string bindingParameter)
		{
			return string.Compare(bindingParameter, "All", StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		internal static PolicyBindingTypes GetBindingType(string bindingValue)
		{
			if (string.Compare("All", bindingValue, StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return PolicyBindingTypes.Tenant;
			}
			return PolicyBindingTypes.IndividualResource;
		}

		private readonly ExecutionLog logger;

		private readonly string logTag;

		private readonly string tenantId;

		private readonly SourceValidator.Clients client;

		private readonly string logCorrelationId;

		private readonly Task.TaskErrorLoggingDelegate writeErrorDelegate;

		private readonly Action<LocalizedString> writeWarningDelegate;

		private readonly Func<LocalizedString, bool> shouldContinueDelegate;

		internal enum Clients
		{
			SetCompliancePolicy,
			NewCompliancePolicy,
			UccPolicyUI
		}
	}
}
