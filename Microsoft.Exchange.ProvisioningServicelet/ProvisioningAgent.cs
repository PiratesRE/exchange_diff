using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ProvisioningServicelet;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Servicelets.Provisioning.Messages;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	internal abstract class ProvisioningAgent : IDisposable
	{
		public ProvisioningAgent(IProvisioningData data, ProvisioningAgentContext agentContext)
		{
			MigrationUtil.ThrowOnNullArgument(data, "data");
			MigrationUtil.ThrowOnNullArgument(agentContext, "agentContext");
			this.isUserInputError = false;
			this.data = data;
			this.agentContext = agentContext;
			this.timeStarted = ExDateTime.Now;
			this.timeFinished = null;
		}

		public Error Error
		{
			get
			{
				return this.error;
			}
		}

		public ExDateTime TimeStarted
		{
			get
			{
				return this.timeStarted;
			}
		}

		public ExDateTime? TimeFinished
		{
			get
			{
				return this.timeFinished;
			}
		}

		public IProvisioningData ProvisioningData
		{
			get
			{
				return this.data;
			}
		}

		public ProvisioningAgentContext AgentContext
		{
			get
			{
				return this.agentContext;
			}
		}

		public virtual IMailboxData MailboxData
		{
			get
			{
				return null;
			}
		}

		public int GroupMemberProvisioned { get; protected set; }

		public int GroupMemberSkipped { get; protected set; }

		public bool IsUserInputError
		{
			get
			{
				return this.isUserInputError;
			}
		}

		public void Dispose()
		{
		}

		public void Work()
		{
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				Thread.CurrentThread.CurrentCulture = this.AgentContext.CultureInfo;
				Thread.CurrentThread.CurrentUICulture = this.AgentContext.CultureInfo;
				this.timeStarted = ExDateTime.Now;
				this.IncrementPerfCounterForAttempt();
				this.error = this.CreateRecipient();
				if (this.error == null)
				{
					this.IncrementPerfCounterForCompletion();
					this.timeFinished = new ExDateTime?(ExDateTime.Now);
					return;
				}
				if (this.error.IsUserInputError)
				{
					this.isUserInputError = true;
					ExTraceGlobals.WorkerTracer.TraceError<Error>(17732, (long)this.GetHashCode(), "non-fatal error {0}. Processing of the request will continue", this.error);
					this.IncrementPerfCounterForFailure();
					return;
				}
				ExTraceGlobals.WorkerTracer.TraceError<Error>(17736, (long)this.GetHashCode(), "fatal error {0}. Processing of the request will not continue", this.error);
				this.IncrementPerfCounterForTransientFailure();
				this.AgentContext.EventLog.LogEvent(MSExchangeProvisioningEventLogConstants.Tuple_NonFatalProcessingError, string.Empty, new object[]
				{
					this.AgentContext.TenantOrganization,
					this.ProvisioningData.ToString(),
					this.error
				});
			}, (object exception) => true, ReportOptions.None);
		}

		protected static Exception FilterTaskException(Exception ex)
		{
			if (ex is CommandNotFoundException)
			{
				ex = new PermissionDeniedException(Strings.PermissionDenied);
			}
			else if (ex is ThrowTerminatingErrorException && (ex.InnerException is ManagementObjectNotFoundException || ex.InnerException is ManagementObjectAlreadyExistsException || ex.InnerException is ManagementObjectDuplicateException || ex.InnerException is ManagementObjectAmbiguousException))
			{
				ex = ex.InnerException;
			}
			return ex;
		}

		protected abstract Error CreateRecipient();

		protected T RunPSCommand<T>(PSCommand command, RunspaceProxy runspaceProxy, out Error errors)
		{
			ExTraceGlobals.WorkerTracer.TraceFunction(17800, (long)this.GetHashCode(), "RunPSCommand");
			PowerShellProxy powerShellProxy = new PowerShellProxy(runspaceProxy, command);
			Collection<T> source = powerShellProxy.Invoke<T>();
			if (powerShellProxy.Failed)
			{
				errors = new Error(powerShellProxy.Errors[0], command.Commands[0].CommandText);
				return default(T);
			}
			errors = null;
			return source.FirstOrDefault<T>();
		}

		protected T SafeRunPSCommand<T>(PSCommand command, RunspaceProxy runspaceProxy, out Error errors, ProvisioningAgent.ErrorMessageOperation errorMessageOperation, uint? faultTraceId)
		{
			T result;
			try
			{
				if (faultTraceId != null)
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest(faultTraceId.Value);
				}
				result = this.RunPSCommand<T>(command, runspaceProxy, out errors);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3840290109U);
				errors = new Error(ProvisioningAgent.FilterTaskException(ex), null, command.Commands[0].CommandText);
				if (errorMessageOperation != null)
				{
					errors.Message = errorMessageOperation(ex.Message);
				}
				result = default(T);
			}
			return result;
		}

		protected IEnumerable<T> RunPSCommandAndReturnAll<T>(PSCommand command, RunspaceProxy runspaceProxy, out Error errors)
		{
			ExTraceGlobals.WorkerTracer.TraceFunction(17800, (long)this.GetHashCode(), "RunPSCommandAndReturnAll");
			PowerShellProxy powerShellProxy = new PowerShellProxy(runspaceProxy, command);
			Collection<T> result = powerShellProxy.Invoke<T>();
			if (powerShellProxy.Failed)
			{
				errors = new Error(powerShellProxy.Errors[0], command.Commands[0].CommandText);
				return null;
			}
			errors = null;
			return result;
		}

		protected bool PopulateParamsToPSCommand(PSCommand command, string[][] parameterMap, Dictionary<string, object> parameters)
		{
			ExTraceGlobals.WorkerTracer.TraceFunction(17808, (long)this.GetHashCode(), "PopulateParamsToPSCommand");
			bool result = false;
			if (parameterMap != null)
			{
				foreach (string[] array in parameterMap)
				{
					string key = array[0];
					object obj;
					if (parameters.TryGetValue(key, out obj) && obj != null)
					{
						result = true;
						string parameterName = string.IsNullOrEmpty(array[1]) ? array[0] : array[1];
						command.AddParameter(parameterName, obj);
					}
				}
			}
			return result;
		}

		protected virtual void IncrementPerfCounterForCompletion()
		{
			BulkUserProvisioningCounters.NumberOfRecipientsCreated.Increment();
			BulkUserProvisioningCounters.RateOfRecipientsCreated.Increment();
		}

		protected virtual void IncrementPerfCounterForFailure()
		{
			BulkUserProvisioningCounters.NumberOfRecipientsFailed.Increment();
		}

		protected virtual void IncrementPerfCounterForTransientFailure()
		{
			BulkUserProvisioningCounters.NumberOfRequestsWithTransientError.Increment();
		}

		protected virtual void IncrementPerfCounterForAttempt()
		{
			BulkUserProvisioningCounters.NumberOfRecipientsAttempted.Increment();
			BulkUserProvisioningCounters.RateOfRecipientsAttempted.Increment();
			BulkUserProvisioningCounters.LastRecipientAttemptedTimestamp.RawValue = DateTime.UtcNow.Ticks;
		}

		protected virtual void UpdateProxyAddressesParameter(MailEnabledRecipient recipient)
		{
			if (this.ProvisioningData.Parameters.ContainsKey(ADRecipientSchema.EmailAddresses.Name))
			{
				string[] array = this.ProvisioningData.Parameters[ADRecipientSchema.EmailAddresses.Name] as string[];
				if (recipient.EmailAddresses != null)
				{
					string[] array2 = recipient.EmailAddresses.ToStringArray();
					if (array2 != null)
					{
						if (array == null)
						{
							array = array2;
						}
						else
						{
							string primaryProxyAddress = array.LastOrDefault((string address) => address.StartsWith("SMTP:", StringComparison.Ordinal));
							if (string.IsNullOrEmpty(primaryProxyAddress))
							{
								primaryProxyAddress = array2.LastOrDefault((string address) => address.StartsWith("SMTP:", StringComparison.Ordinal));
							}
							if (string.IsNullOrEmpty(primaryProxyAddress))
							{
								array = array.Union(array2, StringComparer.OrdinalIgnoreCase).ToArray<string>();
							}
							else
							{
								IEnumerable<string> enumerable = array.Union(array2, StringComparer.OrdinalIgnoreCase);
								enumerable = from address in enumerable
								where !address.Equals(primaryProxyAddress, StringComparison.OrdinalIgnoreCase)
								select address.ToLower();
								enumerable = enumerable.Union(new string[]
								{
									primaryProxyAddress
								}, StringComparer.Ordinal);
								array = enumerable.ToArray<string>();
							}
						}
					}
				}
				this.ProvisioningData.Parameters[ADRecipientSchema.EmailAddresses.Name] = array;
			}
		}

		protected void RemoveSmtpProxyAddressesWithAcceptedDomain()
		{
			if (this.ProvisioningData.Parameters.ContainsKey(ADRecipientSchema.EmailAddresses.Name))
			{
				IEnumerable<string> enumerable = this.ProvisioningData.Parameters[ADRecipientSchema.EmailAddresses.Name] as string[];
				if (enumerable != null)
				{
					PSCommand command = new PSCommand().AddCommand("Get-AcceptedDomain");
					this.PopulateParamsToPSCommand(command, ProvisioningAgent.getAcceptedDomainParameterMap, this.ProvisioningData.Parameters);
					Error error;
					IEnumerable<AcceptedDomain> enumerable2 = this.RunPSCommandAndReturnAll<AcceptedDomain>(command, this.AgentContext.Runspace, out error);
					if (error != null || enumerable2 == null)
					{
						return;
					}
					foreach (AcceptedDomain acceptedDomain in enumerable2)
					{
						if ((acceptedDomain.DomainType == AcceptedDomainType.Authoritative || acceptedDomain.DomainType == AcceptedDomainType.InternalRelay) && acceptedDomain.DomainName != null && acceptedDomain.DomainName.SmtpDomain != null)
						{
							string domainPart = "@" + acceptedDomain.DomainName.SmtpDomain.Domain;
							enumerable = from address in enumerable
							where !address.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase) || !address.EndsWith(domainPart, StringComparison.OrdinalIgnoreCase)
							select address;
						}
					}
					this.ProvisioningData.Parameters[ADRecipientSchema.EmailAddresses.Name] = enumerable.ToArray<string>();
				}
			}
		}

		protected void RemoveSmtpProxyAddressesWithExternalDomain()
		{
			if (this.ProvisioningData.Parameters.ContainsKey(ADRecipientSchema.EmailAddresses.Name))
			{
				IEnumerable<string> enumerable = this.ProvisioningData.Parameters[ADRecipientSchema.EmailAddresses.Name] as string[];
				if (enumerable != null)
				{
					PSCommand command = new PSCommand().AddCommand("Get-AcceptedDomain");
					this.PopulateParamsToPSCommand(command, ProvisioningAgent.getAcceptedDomainParameterMap, this.ProvisioningData.Parameters);
					Error error;
					IEnumerable<AcceptedDomain> enumerable2 = this.RunPSCommandAndReturnAll<AcceptedDomain>(command, this.AgentContext.Runspace, out error);
					if (error != null || enumerable2 == null)
					{
						return;
					}
					IEnumerable<string> enumerable3 = enumerable;
					foreach (AcceptedDomain acceptedDomain in enumerable2)
					{
						if ((acceptedDomain.DomainType == AcceptedDomainType.Authoritative || acceptedDomain.DomainType == AcceptedDomainType.InternalRelay) && acceptedDomain.DomainName != null && acceptedDomain.DomainName.SmtpDomain != null)
						{
							string domainPart = "@" + acceptedDomain.DomainName.SmtpDomain.Domain;
							enumerable3 = from address in enumerable3
							where address.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase) && !address.EndsWith(domainPart, StringComparison.OrdinalIgnoreCase)
							select address;
						}
					}
					enumerable = enumerable.Except(enumerable3, StringComparer.OrdinalIgnoreCase);
					this.ProvisioningData.Parameters[ADRecipientSchema.EmailAddresses.Name] = enumerable.ToArray<string>();
				}
			}
		}

		private static readonly string[][] getAcceptedDomainParameterMap = new string[][]
		{
			new string[]
			{
				"Organization",
				string.Empty
			}
		};

		private ProvisioningAgentContext agentContext;

		private IProvisioningData data;

		private Error error;

		private ExDateTime timeStarted;

		private ExDateTime? timeFinished;

		private bool isUserInputError;

		protected delegate LocalizedString ErrorMessageOperation(string message);
	}
}
