using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "RestoreUserError", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	public class RestoreUserError : IExtensibleDataObject
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
		public Guid? ConflictingObjectId
		{
			get
			{
				return this.ConflictingObjectIdField;
			}
			set
			{
				this.ConflictingObjectIdField = value;
			}
		}

		[DataMember]
		public string CurrentValue
		{
			get
			{
				return this.CurrentValueField;
			}
			set
			{
				this.CurrentValueField = value;
			}
		}

		[DataMember]
		public string ErrorId
		{
			get
			{
				return this.ErrorIdField;
			}
			set
			{
				this.ErrorIdField = value;
			}
		}

		[DataMember]
		public string ErrorType
		{
			get
			{
				return this.ErrorTypeField;
			}
			set
			{
				this.ErrorTypeField = value;
			}
		}

		[DataMember]
		public string ObjectType
		{
			get
			{
				return this.ObjectTypeField;
			}
			set
			{
				this.ObjectTypeField = value;
			}
		}

		[DataMember]
		public string SuggestedValue
		{
			get
			{
				return this.SuggestedValueField;
			}
			set
			{
				this.SuggestedValueField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid? ConflictingObjectIdField;

		private string CurrentValueField;

		private string ErrorIdField;

		private string ErrorTypeField;

		private string ObjectTypeField;

		private string SuggestedValueField;
	}
}
