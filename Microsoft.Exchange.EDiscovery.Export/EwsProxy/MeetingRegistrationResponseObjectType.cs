using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(DeclineItemType))]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(AcceptItemType))]
	[XmlInclude(typeof(TentativelyAcceptItemType))]
	[Serializable]
	public class MeetingRegistrationResponseObjectType : WellKnownResponseObjectType
	{
		public DateTime ProposedStart
		{
			get
			{
				return this.proposedStartField;
			}
			set
			{
				this.proposedStartField = value;
			}
		}

		[XmlIgnore]
		public bool ProposedStartSpecified
		{
			get
			{
				return this.proposedStartFieldSpecified;
			}
			set
			{
				this.proposedStartFieldSpecified = value;
			}
		}

		public DateTime ProposedEnd
		{
			get
			{
				return this.proposedEndField;
			}
			set
			{
				this.proposedEndField = value;
			}
		}

		[XmlIgnore]
		public bool ProposedEndSpecified
		{
			get
			{
				return this.proposedEndFieldSpecified;
			}
			set
			{
				this.proposedEndFieldSpecified = value;
			}
		}

		private DateTime proposedStartField;

		private bool proposedStartFieldSpecified;

		private DateTime proposedEndField;

		private bool proposedEndFieldSpecified;
	}
}
