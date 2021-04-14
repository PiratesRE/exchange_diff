﻿using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMTypes;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "FiltersRequestTypeFilterConditionClause", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class FiltersRequestTypeFilterConditionClause
	{
		[XmlIgnore]
		public FilterKeyType Field
		{
			get
			{
				return this.internalField;
			}
			set
			{
				this.internalField = value;
				this.internalFieldSpecified = true;
			}
		}

		[XmlIgnore]
		public FilterOperatorType Operator
		{
			get
			{
				return this.internalOperator;
			}
			set
			{
				this.internalOperator = value;
				this.internalOperatorSpecified = true;
			}
		}

		[XmlIgnore]
		public StringWithCharSetType Value
		{
			get
			{
				if (this.internalValue == null)
				{
					this.internalValue = new StringWithCharSetType();
				}
				return this.internalValue;
			}
			set
			{
				this.internalValue = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Field", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public FilterKeyType internalField;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalFieldSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Operator", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public FilterOperatorType internalOperator;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalOperatorSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(StringWithCharSetType), ElementName = "Value", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public StringWithCharSetType internalValue;
	}
}
