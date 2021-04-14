using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class FindPeopleResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("Persona", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public PersonaType[] People
		{
			get
			{
				return this.peopleField;
			}
			set
			{
				this.peopleField = value;
			}
		}

		public int TotalNumberOfPeopleInView
		{
			get
			{
				return this.totalNumberOfPeopleInViewField;
			}
			set
			{
				this.totalNumberOfPeopleInViewField = value;
			}
		}

		[XmlIgnore]
		public bool TotalNumberOfPeopleInViewSpecified
		{
			get
			{
				return this.totalNumberOfPeopleInViewFieldSpecified;
			}
			set
			{
				this.totalNumberOfPeopleInViewFieldSpecified = value;
			}
		}

		public int FirstMatchingRowIndex
		{
			get
			{
				return this.firstMatchingRowIndexField;
			}
			set
			{
				this.firstMatchingRowIndexField = value;
			}
		}

		[XmlIgnore]
		public bool FirstMatchingRowIndexSpecified
		{
			get
			{
				return this.firstMatchingRowIndexFieldSpecified;
			}
			set
			{
				this.firstMatchingRowIndexFieldSpecified = value;
			}
		}

		public int FirstLoadedRowIndex
		{
			get
			{
				return this.firstLoadedRowIndexField;
			}
			set
			{
				this.firstLoadedRowIndexField = value;
			}
		}

		[XmlIgnore]
		public bool FirstLoadedRowIndexSpecified
		{
			get
			{
				return this.firstLoadedRowIndexFieldSpecified;
			}
			set
			{
				this.firstLoadedRowIndexFieldSpecified = value;
			}
		}

		private PersonaType[] peopleField;

		private int totalNumberOfPeopleInViewField;

		private bool totalNumberOfPeopleInViewFieldSpecified;

		private int firstMatchingRowIndexField;

		private bool firstMatchingRowIndexFieldSpecified;

		private int firstLoadedRowIndexField;

		private bool firstLoadedRowIndexFieldSpecified;
	}
}
