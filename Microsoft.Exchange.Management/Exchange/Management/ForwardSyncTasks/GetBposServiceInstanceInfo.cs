using System;
using System.Management.Automation;
using System.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Get", "BposServiceInstanceInfo", DefaultParameterSetName = "Identity")]
	public sealed class GetBposServiceInstanceInfo : Task
	{
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public ServiceInstanceId Identity
		{
			get
			{
				return (ServiceInstanceId)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			try
			{
				ServiceInstanceInfoValue serviceInstanceInfoValue = null;
				using (OnboardingService onboardingService = new MsoOnboardingService())
				{
					serviceInstanceInfoValue = onboardingService.GetServiceInstanceInfo(this.Identity.ToString());
				}
				if (serviceInstanceInfoValue != null)
				{
					Uri backSyncUrl = null;
					if (serviceInstanceInfoValue.Endpoint != null)
					{
						foreach (ServiceEndpointValue serviceEndpointValue in serviceInstanceInfoValue.Endpoint)
						{
							if (string.Compare(serviceEndpointValue.Name, "BackSyncPSConnectionURI", true) == 0)
							{
								backSyncUrl = new Uri(serviceEndpointValue.Address);
								break;
							}
						}
					}
					bool authorityTransferIsSupported = false;
					if (serviceInstanceInfoValue.Any != null)
					{
						foreach (XmlElement xmlElement in serviceInstanceInfoValue.Any)
						{
							if (string.Compare(xmlElement.Name, "SupportsAuthorityTransfer", true) == 0)
							{
								authorityTransferIsSupported = true;
								break;
							}
						}
					}
					BposServiceInstanceInfo sendToPipeline = new BposServiceInstanceInfo(this.Identity, "BackSyncPSConnectionURI", backSyncUrl, authorityTransferIsSupported);
					base.WriteObject(sendToPipeline);
				}
			}
			catch (CouldNotCreateMsoOnboardingServiceException exception)
			{
				this.WriteError(exception, ErrorCategory.ObjectNotFound, null, true);
			}
			catch (InvalidServiceInstanceMapXmlFormatException exception2)
			{
				this.WriteError(exception2, ErrorCategory.InvalidData, null, true);
			}
			catch (Exception exception3)
			{
				this.WriteError(exception3, ErrorCategory.InvalidOperation, null, true);
			}
		}
	}
}
