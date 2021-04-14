using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlRoot(ElementName = "OpenAsAdminOrSystemService", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class OpenAsAdminOrSystemServiceType
	{
		[XmlElement]
		public ConnectingSIDType ConnectingSID
		{
			get
			{
				return this.connectingSIDField;
			}
			set
			{
				this.connectingSIDField = value;
			}
		}

		[XmlAttribute]
		public SpecialLogonType LogonType
		{
			get
			{
				return this.logonTypeField;
			}
			set
			{
				this.logonTypeField = value;
			}
		}

		[XmlAttribute]
		public int BudgetType
		{
			get
			{
				return this.budgetTypeField;
			}
			set
			{
				this.budgetTypeField = value;
			}
		}

		[XmlIgnore]
		public bool BudgetTypeSpecified
		{
			get
			{
				return this.budgetTypeSpecifiedField;
			}
			set
			{
				this.budgetTypeSpecifiedField = value;
			}
		}

		private int budgetTypeField;

		private bool budgetTypeSpecifiedField;

		private SpecialLogonType logonTypeField;

		private ConnectingSIDType connectingSIDField;
	}
}
