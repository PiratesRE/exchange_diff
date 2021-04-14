using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Set", "BposPlacementConfiguration", SupportsShouldProcess = true)]
	public sealed class SetBposPlacementConfiguration : Task
	{
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public string Configuration
		{
			get
			{
				return (string)base.Fields["Configuration"];
			}
			set
			{
				base.Fields["Configuration"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetServiceInstanceMap;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				try
				{
					this.map = ServiceInstanceMapSerializer.ConvertXmlToServiceInstanceMap(this.Configuration);
				}
				catch (InvalidServiceInstanceMapXmlFormatException exception)
				{
					this.WriteError(exception, ErrorCategory.InvalidData, null, true);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			using (OnboardingService onboardingService = new MsoOnboardingService())
			{
				ResultCode? result = null;
				try
				{
					result = new ResultCode?(onboardingService.SetServiceInstanceMap(this.map));
				}
				catch (Exception exception)
				{
					this.WriteError(exception, ErrorCategory.ResourceUnavailable, null, true);
				}
				if (result != null && result.Value == ResultCode.Success)
				{
					this.Configuration = ServiceInstanceMapSerializer.ConvertServiceInstanceMapToXml(this.map);
					base.WriteObject(new BposPlacementConfiguration(this.Configuration));
				}
				else
				{
					string errorStringForResultcode = MsoOnboardingService.GetErrorStringForResultcode(result);
					this.WriteError(new CouldNotUpdateServiceInstanceMapException(errorStringForResultcode), ErrorCategory.InvalidData, null, true);
				}
			}
		}

		private ServiceInstanceMapValue map;
	}
}
