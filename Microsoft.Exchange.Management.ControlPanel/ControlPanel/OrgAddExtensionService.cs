using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class OrgAddExtensionService : AddExtensionService
	{
		public override Type SetParameterType
		{
			get
			{
				return typeof(OrgUploadExtensionParameter);
			}
		}

		protected override void AddParameters(PSCommand installCommand, WebServiceParameters param)
		{
			base.AddParameters(installCommand, param);
			installCommand.AddParameter("OrganizationApp");
		}
	}
}
