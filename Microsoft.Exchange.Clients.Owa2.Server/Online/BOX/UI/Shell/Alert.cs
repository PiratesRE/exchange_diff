using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.UI.Shell
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "Alert", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.UI.Shell")]
	[DebuggerStepThrough]
	public class Alert : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public string ActionClick
		{
			get
			{
				return this.ActionClickField;
			}
			set
			{
				this.ActionClickField = value;
			}
		}

		[DataMember]
		public string ActionTarget
		{
			get
			{
				return this.ActionTargetField;
			}
			set
			{
				this.ActionTargetField = value;
			}
		}

		[DataMember]
		public string ActionText
		{
			get
			{
				return this.ActionTextField;
			}
			set
			{
				this.ActionTextField = value;
			}
		}

		[DataMember]
		public string ActionUrl
		{
			get
			{
				return this.ActionUrlField;
			}
			set
			{
				this.ActionUrlField = value;
			}
		}

		[DataMember]
		public string Message
		{
			get
			{
				return this.MessageField;
			}
			set
			{
				this.MessageField = value;
			}
		}

		[DataMember]
		public AlertPriority Priority
		{
			get
			{
				return this.PriorityField;
			}
			set
			{
				this.PriorityField = value;
			}
		}

		[DataMember]
		public string Title
		{
			get
			{
				return this.TitleField;
			}
			set
			{
				this.TitleField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string ActionClickField;

		private string ActionTargetField;

		private string ActionTextField;

		private string ActionUrlField;

		private string MessageField;

		private AlertPriority PriorityField;

		private string TitleField;
	}
}
