using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "GetHeaderInfoResponse", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetHeaderInfoResponse : Response
	{
		[DataMember]
		public ClientVersionHeader ClientVersionHeader
		{
			get
			{
				return this.ClientVersionHeaderField;
			}
			set
			{
				this.ClientVersionHeaderField = value;
			}
		}

		[DataMember]
		public string ClientVersionHeaderName
		{
			get
			{
				return this.ClientVersionHeaderNameField;
			}
			set
			{
				this.ClientVersionHeaderNameField = value;
			}
		}

		[DataMember]
		public Context ContextHeader
		{
			get
			{
				return this.ContextHeaderField;
			}
			set
			{
				this.ContextHeaderField = value;
			}
		}

		[DataMember]
		public string ContextHeaderName
		{
			get
			{
				return this.ContextHeaderNameField;
			}
			set
			{
				this.ContextHeaderNameField = value;
			}
		}

		[DataMember]
		public ContractVersionHeader ContractVersionHeader
		{
			get
			{
				return this.ContractVersionHeaderField;
			}
			set
			{
				this.ContractVersionHeaderField = value;
			}
		}

		[DataMember]
		public string ContractVersionHeaderName
		{
			get
			{
				return this.ContractVersionHeaderNameField;
			}
			set
			{
				this.ContractVersionHeaderNameField = value;
			}
		}

		[DataMember]
		public string HeaderNameSpace
		{
			get
			{
				return this.HeaderNameSpaceField;
			}
			set
			{
				this.HeaderNameSpaceField = value;
			}
		}

		[DataMember]
		public string IdentityHeaderName
		{
			get
			{
				return this.IdentityHeaderNameField;
			}
			set
			{
				this.IdentityHeaderNameField = value;
			}
		}

		[DataMember]
		public UserIdentityHeader ReturnValue
		{
			get
			{
				return this.ReturnValueField;
			}
			set
			{
				this.ReturnValueField = value;
			}
		}

		[DataMember]
		public TrackingHeader TrackingHeader
		{
			get
			{
				return this.TrackingHeaderField;
			}
			set
			{
				this.TrackingHeaderField = value;
			}
		}

		[DataMember]
		public string TrackingHeaderName
		{
			get
			{
				return this.TrackingHeaderNameField;
			}
			set
			{
				this.TrackingHeaderNameField = value;
			}
		}

		private ClientVersionHeader ClientVersionHeaderField;

		private string ClientVersionHeaderNameField;

		private Context ContextHeaderField;

		private string ContextHeaderNameField;

		private ContractVersionHeader ContractVersionHeaderField;

		private string ContractVersionHeaderNameField;

		private string HeaderNameSpaceField;

		private string IdentityHeaderNameField;

		private UserIdentityHeader ReturnValueField;

		private TrackingHeader TrackingHeaderField;

		private string TrackingHeaderNameField;
	}
}
