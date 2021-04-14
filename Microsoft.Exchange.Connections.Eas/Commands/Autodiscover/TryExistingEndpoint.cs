using System;
using System.Net;
using Microsoft.Exchange.Connections.Eas.Commands.Settings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TryExistingEndpoint : AutodiscoverStep
	{
		internal TryExistingEndpoint(EasConnectionSettings easConnectionSettings) : base(easConnectionSettings, Step.TrySmtpAddress)
		{
		}

		public override Step ExecuteStep(StepContext stepContext)
		{
			AutodiscoverEndpoint mostRecentEndpoint = stepContext.EasConnectionSettings.EasEndpointSettings.MostRecentEndpoint;
			if (mostRecentEndpoint == null)
			{
				return base.NextStepOnFailure;
			}
			if (mostRecentEndpoint.IsPotentiallyReusable() && this.CheckVitality(stepContext))
			{
				stepContext.EasConnectionSettings.Log.Debug("Use the existing endpoint: {0}", new object[]
				{
					mostRecentEndpoint.Url
				});
				stepContext.HttpStatusCode = HttpStatusCode.OK;
				stepContext.Response = new AutodiscoverResponse
				{
					HttpStatus = HttpStatus.OK,
					AutodiscoverStatus = AutodiscoverStatus.Success
				};
				return Step.Succeeded;
			}
			stepContext.EasConnectionSettings.EasEndpointSettings.MostRecentDomain = null;
			return base.NextStepOnFailure;
		}

		protected override bool IsStepAllowable(StepContext stepContext)
		{
			return stepContext.Request.AutodiscoverOption != AutodiscoverOption.Probes;
		}

		private bool CheckVitality(StepContext stepContext)
		{
			SettingsCommand settingsCommand = new SettingsCommand(stepContext.EasConnectionSettings);
			bool result;
			try
			{
				settingsCommand.Execute(SettingsRequest.Default);
				result = true;
			}
			catch (LocalizedException ex)
			{
				stepContext.EasConnectionSettings.Log.Error("Failed to check vitality: {0}", new object[]
				{
					ex
				});
				result = false;
			}
			return result;
		}
	}
}
