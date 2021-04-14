using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "Role", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class Role : IExtensibleDataObject
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
		public string Description
		{
			get
			{
				return this.DescriptionField;
			}
			set
			{
				this.DescriptionField = value;
			}
		}

		[DataMember]
		public bool? IsEnabled
		{
			get
			{
				return this.IsEnabledField;
			}
			set
			{
				this.IsEnabledField = value;
			}
		}

		[DataMember]
		public bool? IsSystem
		{
			get
			{
				return this.IsSystemField;
			}
			set
			{
				this.IsSystemField = value;
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.NameField;
			}
			set
			{
				this.NameField = value;
			}
		}

		[DataMember]
		public Guid? ObjectId
		{
			get
			{
				return this.ObjectIdField;
			}
			set
			{
				this.ObjectIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string DescriptionField;

		private bool? IsEnabledField;

		private bool? IsSystemField;

		private string NameField;

		private Guid? ObjectIdField;
	}
}
