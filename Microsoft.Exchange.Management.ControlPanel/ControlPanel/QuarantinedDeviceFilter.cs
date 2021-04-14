using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class QuarantinedDeviceFilter : ResultSizeFilter
	{
		public QuarantinedDeviceFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MobileDevice";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base.ResultSize = 500;
			base["Filter"] = "DeviceAccessState -eq 'Quarantined'";
			base["ActiveSync"] = new SwitchParameter(true);
		}

		public new const string RbacParameters = "?ResultSize&Filter&ActiveSync";
	}
}
