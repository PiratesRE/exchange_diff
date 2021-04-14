using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class NonIndexableItemDetailType
	{
		public ItemIdType ItemId
		{
			get
			{
				return this.itemIdField;
			}
			set
			{
				this.itemIdField = value;
			}
		}

		public ItemIndexErrorType ErrorCode
		{
			get
			{
				return this.errorCodeField;
			}
			set
			{
				this.errorCodeField = value;
			}
		}

		public string ErrorDescription
		{
			get
			{
				return this.errorDescriptionField;
			}
			set
			{
				this.errorDescriptionField = value;
			}
		}

		public bool IsPartiallyIndexed
		{
			get
			{
				return this.isPartiallyIndexedField;
			}
			set
			{
				this.isPartiallyIndexedField = value;
			}
		}

		public bool IsPermanentFailure
		{
			get
			{
				return this.isPermanentFailureField;
			}
			set
			{
				this.isPermanentFailureField = value;
			}
		}

		public string SortValue
		{
			get
			{
				return this.sortValueField;
			}
			set
			{
				this.sortValueField = value;
			}
		}

		public int AttemptCount
		{
			get
			{
				return this.attemptCountField;
			}
			set
			{
				this.attemptCountField = value;
			}
		}

		public DateTime LastAttemptTime
		{
			get
			{
				return this.lastAttemptTimeField;
			}
			set
			{
				this.lastAttemptTimeField = value;
			}
		}

		[XmlIgnore]
		public bool LastAttemptTimeSpecified
		{
			get
			{
				return this.lastAttemptTimeFieldSpecified;
			}
			set
			{
				this.lastAttemptTimeFieldSpecified = value;
			}
		}

		public string AdditionalInfo
		{
			get
			{
				return this.additionalInfoField;
			}
			set
			{
				this.additionalInfoField = value;
			}
		}

		private ItemIdType itemIdField;

		private ItemIndexErrorType errorCodeField;

		private string errorDescriptionField;

		private bool isPartiallyIndexedField;

		private bool isPermanentFailureField;

		private string sortValueField;

		private int attemptCountField;

		private DateTime lastAttemptTimeField;

		private bool lastAttemptTimeFieldSpecified;

		private string additionalInfoField;
	}
}
