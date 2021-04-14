using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class GetUserPhotoParameters : WebServiceParameters
	{
		public SwitchParameter Preview
		{
			get
			{
				object obj = base["Preview"];
				if (obj != null)
				{
					return (SwitchParameter)obj;
				}
				return new SwitchParameter(false);
			}
			set
			{
				base["Preview"] = value;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-UserPhoto";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}
	}
}
