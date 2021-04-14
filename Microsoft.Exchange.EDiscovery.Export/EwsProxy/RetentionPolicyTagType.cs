using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[Serializable]
	public class RetentionPolicyTagType
	{
		public string DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public string RetentionId
		{
			get
			{
				return this.retentionIdField;
			}
			set
			{
				this.retentionIdField = value;
			}
		}

		public int RetentionPeriod
		{
			get
			{
				return this.retentionPeriodField;
			}
			set
			{
				this.retentionPeriodField = value;
			}
		}

		public ElcFolderType Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		public RetentionActionType RetentionAction
		{
			get
			{
				return this.retentionActionField;
			}
			set
			{
				this.retentionActionField = value;
			}
		}

		public string Description
		{
			get
			{
				return this.descriptionField;
			}
			set
			{
				this.descriptionField = value;
			}
		}

		public bool IsVisible
		{
			get
			{
				return this.isVisibleField;
			}
			set
			{
				this.isVisibleField = value;
			}
		}

		public bool OptedInto
		{
			get
			{
				return this.optedIntoField;
			}
			set
			{
				this.optedIntoField = value;
			}
		}

		public bool IsArchive
		{
			get
			{
				return this.isArchiveField;
			}
			set
			{
				this.isArchiveField = value;
			}
		}

		private string displayNameField;

		private string retentionIdField;

		private int retentionPeriodField;

		private ElcFolderType typeField;

		private RetentionActionType retentionActionField;

		private string descriptionField;

		private bool isVisibleField;

		private bool optedIntoField;

		private bool isArchiveField;
	}
}
