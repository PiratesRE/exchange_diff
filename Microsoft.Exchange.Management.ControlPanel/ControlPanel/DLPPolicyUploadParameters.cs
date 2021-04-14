using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DLPPolicyUploadParameters : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-DLPPolicy";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		[DataMember]
		public Identity Identity
		{
			get
			{
				return (Identity)base["Identity"];
			}
			set
			{
				base["Identity"] = value;
			}
		}

		public byte[] TemplateData
		{
			get
			{
				return (byte[])base["TemplateData"];
			}
			set
			{
				base["TemplateData"] = value;
			}
		}

		[DataMember]
		public string State
		{
			get
			{
				return (string)base["State"];
			}
			set
			{
				base["State"] = value;
			}
		}

		[DataMember]
		public string Mode
		{
			get
			{
				return (string)base["Mode"];
			}
			set
			{
				base["Mode"] = value;
			}
		}
	}
}
