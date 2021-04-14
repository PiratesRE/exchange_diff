using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class DistributionListType : ItemType
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

		public string FileAs
		{
			get
			{
				return this.fileAsField;
			}
			set
			{
				this.fileAsField = value;
			}
		}

		public ContactSourceType ContactSource
		{
			get
			{
				return this.contactSourceField;
			}
			set
			{
				this.contactSourceField = value;
			}
		}

		[XmlIgnore]
		public bool ContactSourceSpecified
		{
			get
			{
				return this.contactSourceFieldSpecified;
			}
			set
			{
				this.contactSourceFieldSpecified = value;
			}
		}

		[XmlArrayItem("Member", IsNullable = false)]
		public MemberType[] Members
		{
			get
			{
				return this.membersField;
			}
			set
			{
				this.membersField = value;
			}
		}

		private string displayNameField;

		private string fileAsField;

		private ContactSourceType contactSourceField;

		private bool contactSourceFieldSpecified;

		private MemberType[] membersField;
	}
}
