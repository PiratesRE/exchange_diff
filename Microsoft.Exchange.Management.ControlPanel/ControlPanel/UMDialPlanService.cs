using System;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class UMDialPlanService : UMBasePromptService, IUMDialPlanService, IUploadHandler
	{
	}
}
