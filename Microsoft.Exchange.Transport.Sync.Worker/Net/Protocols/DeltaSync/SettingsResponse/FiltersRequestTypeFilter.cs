using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMTypes;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "FiltersRequestTypeFilter", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class FiltersRequestTypeFilter
	{
		[XmlIgnore]
		public int ExecutionOrder
		{
			get
			{
				return this.internalExecutionOrder;
			}
			set
			{
				this.internalExecutionOrder = value;
				this.internalExecutionOrderSpecified = true;
			}
		}

		[XmlIgnore]
		public BitType Enabled
		{
			get
			{
				return this.internalEnabled;
			}
			set
			{
				this.internalEnabled = value;
				this.internalEnabledSpecified = true;
			}
		}

		[XmlIgnore]
		public RunWhenType RunWhen
		{
			get
			{
				return this.internalRunWhen;
			}
			set
			{
				this.internalRunWhen = value;
				this.internalRunWhenSpecified = true;
			}
		}

		[XmlIgnore]
		public FiltersRequestTypeFilterCondition Condition
		{
			get
			{
				if (this.internalCondition == null)
				{
					this.internalCondition = new FiltersRequestTypeFilterCondition();
				}
				return this.internalCondition;
			}
			set
			{
				this.internalCondition = value;
			}
		}

		[XmlIgnore]
		public FiltersRequestTypeFilterActions Actions
		{
			get
			{
				if (this.internalActions == null)
				{
					this.internalActions = new FiltersRequestTypeFilterActions();
				}
				return this.internalActions;
			}
			set
			{
				this.internalActions = value;
			}
		}

		[XmlElement(ElementName = "ExecutionOrder", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalExecutionOrder;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalExecutionOrderSpecified;

		[XmlElement(ElementName = "Enabled", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public BitType internalEnabled;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalEnabledSpecified;

		[XmlElement(ElementName = "RunWhen", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public RunWhenType internalRunWhen;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalRunWhenSpecified;

		[XmlElement(Type = typeof(FiltersRequestTypeFilterCondition), ElementName = "Condition", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public FiltersRequestTypeFilterCondition internalCondition;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(FiltersRequestTypeFilterActions), ElementName = "Actions", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public FiltersRequestTypeFilterActions internalActions;
	}
}
