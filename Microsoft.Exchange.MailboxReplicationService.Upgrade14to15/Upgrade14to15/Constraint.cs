using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "Constraint", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.Common.DataContract")]
	public class Constraint : IExtensibleDataObject
	{
		public Constraint(string constraintName, string owner, DateTime fixByDate, ConstraintStatus status, bool isBlocking, string comment)
		{
			this.ConstraintName = constraintName;
			this.Owner = owner;
			this.FixByDate = fixByDate;
			this.Status = status;
			this.IsBlocking = isBlocking;
			this.Comment = comment;
		}

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
		public string Comment
		{
			get
			{
				return this.CommentField;
			}
			set
			{
				this.CommentField = value;
			}
		}

		[DataMember]
		public string ConstraintName
		{
			get
			{
				return this.ConstraintNameField;
			}
			set
			{
				this.ConstraintNameField = value;
			}
		}

		[DataMember]
		public DateTime FixByDate
		{
			get
			{
				return this.FixByDateField;
			}
			set
			{
				this.FixByDateField = value;
			}
		}

		[DataMember]
		public bool IsBlocking
		{
			get
			{
				return this.IsBlockingField;
			}
			set
			{
				this.IsBlockingField = value;
			}
		}

		[DataMember]
		public string Owner
		{
			get
			{
				return this.OwnerField;
			}
			set
			{
				this.OwnerField = value;
			}
		}

		[DataMember]
		public ConstraintStatus Status
		{
			get
			{
				return this.StatusField;
			}
			set
			{
				this.StatusField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string CommentField;

		private string ConstraintNameField;

		private DateTime FixByDateField;

		private bool IsBlockingField;

		private string OwnerField;

		private ConstraintStatus StatusField;
	}
}
