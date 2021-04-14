using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Get", "BposPlacementConfiguration")]
	[OutputType(new Type[]
	{
		typeof(BposPlacementConfiguration)
	})]
	public sealed class GetBposPlacementConfiguration : Task
	{
		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			try
			{
				using (OnboardingService onboardingService = new MsoOnboardingService())
				{
					ServiceInstanceMapValue map = null;
					try
					{
						map = onboardingService.GetServiceInstanceMap();
					}
					catch (Exception exception)
					{
						this.WriteError(exception, ErrorCategory.ResourceUnavailable, null, true);
					}
					string configuration = ServiceInstanceMapSerializer.ConvertServiceInstanceMapToXml(map);
					base.WriteObject(new BposPlacementConfiguration(configuration));
				}
			}
			catch (CouldNotCreateMsoOnboardingServiceException exception2)
			{
				this.WriteError(exception2, ErrorCategory.ObjectNotFound, null, true);
			}
			catch (InvalidServiceInstanceMapXmlFormatException exception3)
			{
				this.WriteError(exception3, ErrorCategory.InvalidData, null, true);
			}
			catch (Exception exception4)
			{
				this.WriteError(exception4, ErrorCategory.InvalidOperation, null, true);
			}
		}
	}
}
