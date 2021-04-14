using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class FingerprintUploadParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-Fingerprint";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		public byte[] FileData
		{
			get
			{
				return (byte[])base["FileData"];
			}
			set
			{
				base["FileData"] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)base["Description"];
			}
			set
			{
				base["Description"] = value;
			}
		}
	}
}
