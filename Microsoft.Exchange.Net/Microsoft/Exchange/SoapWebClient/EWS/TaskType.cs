using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class TaskType : ItemType
	{
		public int ActualWork;

		[XmlIgnore]
		public bool ActualWorkSpecified;

		public DateTime AssignedTime;

		[XmlIgnore]
		public bool AssignedTimeSpecified;

		public string BillingInformation;

		public int ChangeCount;

		[XmlIgnore]
		public bool ChangeCountSpecified;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Companies;

		public DateTime CompleteDate;

		[XmlIgnore]
		public bool CompleteDateSpecified;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Contacts;

		public TaskDelegateStateType DelegationState;

		[XmlIgnore]
		public bool DelegationStateSpecified;

		public string Delegator;

		public DateTime DueDate;

		[XmlIgnore]
		public bool DueDateSpecified;

		public int IsAssignmentEditable;

		[XmlIgnore]
		public bool IsAssignmentEditableSpecified;

		public bool IsComplete;

		[XmlIgnore]
		public bool IsCompleteSpecified;

		public bool IsRecurring;

		[XmlIgnore]
		public bool IsRecurringSpecified;

		public bool IsTeamTask;

		[XmlIgnore]
		public bool IsTeamTaskSpecified;

		public string Mileage;

		public string Owner;

		public double PercentComplete;

		[XmlIgnore]
		public bool PercentCompleteSpecified;

		public TaskRecurrenceType Recurrence;

		public DateTime StartDate;

		[XmlIgnore]
		public bool StartDateSpecified;

		public TaskStatusType Status;

		[XmlIgnore]
		public bool StatusSpecified;

		public string StatusDescription;

		public int TotalWork;

		[XmlIgnore]
		public bool TotalWorkSpecified;
	}
}
