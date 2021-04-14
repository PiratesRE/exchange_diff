using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class DirectoryPropertyXmlInvitationTicket : DirectoryPropertyXml
	{
		public override IList GetValues()
		{
			throw new NotImplementedException();
		}

		public sealed override void SetValues(IList values)
		{
			throw new NotImplementedException();
		}

		[XmlElement("Value", Order = 0)]
		public XmlValueInvitationTicket[] Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		private XmlValueInvitationTicket[] valueField;
	}
}
