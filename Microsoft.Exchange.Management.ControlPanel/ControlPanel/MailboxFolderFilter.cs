using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxFolderFilter : WebServiceParameters
	{
		public MailboxFolderFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base["Recurse"] = true;
			base["MailFolderOnly"] = true;
			base["ResultSize"] = int.MaxValue;
		}

		[DataMember]
		public FolderPickerType FolderPickerType { get; set; }

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MailboxFolder";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}

		public const string RbacParameters = "?Recurse&MailFolderOnly&ResultSize";
	}
}
