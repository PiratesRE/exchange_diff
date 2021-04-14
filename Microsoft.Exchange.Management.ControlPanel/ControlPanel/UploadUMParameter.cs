using System;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UploadUMParameter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Import-UMPrompt";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		public Stream PromptFileStream
		{
			get
			{
				return (Stream)base["PromptFileStream"];
			}
			set
			{
				base["PromptFileStream"] = value;
			}
		}

		[DataMember]
		public Identity UMDialPlan
		{
			get
			{
				return (Identity)base["UMDialPlan"];
			}
			set
			{
				base["UMDialPlan"] = value;
			}
		}

		[DataMember]
		public Identity UMAutoAttendant
		{
			get
			{
				return (Identity)base["UMAutoAttendant"];
			}
			set
			{
				base["UMAutoAttendant"] = value;
			}
		}

		[DataMember]
		public string PromptFileName
		{
			get
			{
				return (string)base["PromptFileName"];
			}
			set
			{
				base["PromptFileName"] = value;
			}
		}
	}
}
