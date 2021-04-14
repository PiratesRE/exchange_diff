using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[DataContract(Name = "CommonFault", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[Serializable]
	public class CommonFault : IExtensibleDataObject
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
		public bool IsPermanentFailure
		{
			get
			{
				return this.IsPermanentFailureField;
			}
			set
			{
				this.IsPermanentFailureField = value;
			}
		}

		[DataMember(Order = 1)]
		public ServerErrorCode ErrorCode
		{
			get
			{
				return this.ErrorCodeField;
			}
			set
			{
				this.ErrorCodeField = value;
			}
		}

		[DataMember(Order = 2)]
		public Guid CorrelationId
		{
			get
			{
				return this.CorrelationIdField;
			}
			set
			{
				if (!this.CorrelationIdField.Equals(value))
				{
					this.CorrelationIdField = value;
					this.RaisePropertyChanged("CorrelationId");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private ExtensionDataObject extensionDataField;

		private bool IsPermanentFailureField;

		private ServerErrorCode ErrorCodeField;

		[OptionalField]
		private Guid CorrelationIdField;
	}
}
