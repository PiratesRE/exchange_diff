using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AutodiscoverCommand : EasPseudoCommand<AutodiscoverRequest, AutodiscoverResponse>
	{
		protected internal AutodiscoverCommand(EasConnectionSettings easConnectionSettings) : base(Command.Autodiscover, easConnectionSettings)
		{
			this.steps = new Dictionary<Step, IExecuteStep>
			{
				{
					Step.TryExistingEndpoint,
					new TryExistingEndpoint(easConnectionSettings)
				},
				{
					Step.TrySmtpAddress,
					new TrySmtpAddress(easConnectionSettings)
				},
				{
					Step.TryRemovingDomainPrefix,
					new TryRemovingDomainPrefix(easConnectionSettings)
				},
				{
					Step.TryAddingAutodiscoverPrefix,
					new TryAddingAutodiscoverPrefix(easConnectionSettings)
				},
				{
					Step.TryUnauthenticatedGet,
					new TryUnauthenticatedGet(easConnectionSettings)
				},
				{
					Step.TryDnsLookupOfSrvRecord,
					new TryDnsLookupOfSrvRecord(easConnectionSettings)
				}
			};
			this.results = new Dictionary<Step, string>(this.steps.Count);
		}

		internal override AutodiscoverResponse Execute(AutodiscoverRequest autodiscoverRequest)
		{
			AutodiscoverResponse result;
			lock (this.syncLock)
			{
				this.results.Clear();
				StepContext stepContext = new StepContext(autodiscoverRequest, base.EasConnectionSettings);
				Step step = Step.TryExistingEndpoint;
				while ((step & Step.Done) == Step.None)
				{
					base.EasConnectionSettings.Log.Debug("AutoDiscover Step: {0}", new object[]
					{
						step
					});
					Step step2 = this.steps[step].PrepareAndExecuteStep(stepContext);
					this.results[step] = string.Format("HttpStatus={0}{1}.", stepContext.HttpStatusCode, (stepContext.Error != null) ? (",Error=" + stepContext.Error.Message) : string.Empty);
					step = step2;
				}
				result = this.ProcessResponse(stepContext.Response);
			}
			return result;
		}

		private AutodiscoverResponse ProcessResponse(AutodiscoverResponse autodiscoverResponse)
		{
			if (autodiscoverResponse == null)
			{
				return this.CreateFailedAutodiscoverResponse();
			}
			if (autodiscoverResponse.Response != null && autodiscoverResponse.Response.Error == null)
			{
				string autodiscoveredDomain = autodiscoverResponse.AutodiscoveredDomain;
				base.EasConnectionSettings.Log.Info("Discovered Endpoint: {0}", new object[]
				{
					autodiscoveredDomain
				});
				if (!string.IsNullOrEmpty(autodiscoveredDomain))
				{
					base.EasConnectionSettings.EasEndpointSettings.MostRecentDomain = autodiscoveredDomain;
				}
			}
			autodiscoverResponse.ConvertStatusToEnum();
			return autodiscoverResponse;
		}

		private AutodiscoverResponse CreateFailedAutodiscoverResponse()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<Step, string> keyValuePair in this.results)
			{
				stringBuilder.AppendFormat("[{0}]{1}", keyValuePair.Key, keyValuePair.Value);
			}
			return new AutodiscoverResponse
			{
				AutodiscoverStatus = AutodiscoverStatus.EveryStepFailed,
				AutodiscoverSteps = stringBuilder.ToString()
			};
		}

		private readonly object syncLock = new object();

		private readonly Dictionary<Step, IExecuteStep> steps;

		private readonly Dictionary<Step, string> results;
	}
}
