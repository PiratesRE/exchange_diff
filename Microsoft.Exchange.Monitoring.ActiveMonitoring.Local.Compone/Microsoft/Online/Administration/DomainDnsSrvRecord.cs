using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DomainDnsSrvRecord", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class DomainDnsSrvRecord : DomainDnsRecord
	{
		[DataMember]
		public string NameTarget
		{
			get
			{
				return this.NameTargetField;
			}
			set
			{
				this.NameTargetField = value;
			}
		}

		[DataMember]
		public int? Port
		{
			get
			{
				return this.PortField;
			}
			set
			{
				this.PortField = value;
			}
		}

		[DataMember]
		public int? Priority
		{
			get
			{
				return this.PriorityField;
			}
			set
			{
				this.PriorityField = value;
			}
		}

		[DataMember]
		public string Protocol
		{
			get
			{
				return this.ProtocolField;
			}
			set
			{
				this.ProtocolField = value;
			}
		}

		[DataMember]
		public string Service
		{
			get
			{
				return this.ServiceField;
			}
			set
			{
				this.ServiceField = value;
			}
		}

		[DataMember]
		public int? Weight
		{
			get
			{
				return this.WeightField;
			}
			set
			{
				this.WeightField = value;
			}
		}

		private string NameTargetField;

		private int? PortField;

		private int? PriorityField;

		private string ProtocolField;

		private string ServiceField;

		private int? WeightField;
	}
}
