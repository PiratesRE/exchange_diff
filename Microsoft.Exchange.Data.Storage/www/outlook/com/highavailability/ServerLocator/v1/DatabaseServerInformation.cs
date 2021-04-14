using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace www.outlook.com.highavailability.ServerLocator.v1
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DatabaseServerInformation", Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/")]
	public class DatabaseServerInformation : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public Guid DatabaseGuid
		{
			get
			{
				return this.DatabaseGuidField;
			}
			set
			{
				this.DatabaseGuidField = value;
			}
		}

		[DataMember]
		public string ServerFqdn
		{
			get
			{
				return this.ServerFqdnField;
			}
			set
			{
				this.ServerFqdnField = value;
			}
		}

		[DataMember(Order = 2)]
		public DateTime RequestSentUtc
		{
			get
			{
				return this.RequestSentUtcField;
			}
			set
			{
				this.RequestSentUtcField = value;
			}
		}

		[DataMember(Order = 3)]
		public DateTime RequestReceivedUtc
		{
			get
			{
				return this.RequestReceivedUtcField;
			}
			set
			{
				this.RequestReceivedUtcField = value;
			}
		}

		[DataMember(Order = 4)]
		public DateTime ReplySentUtc
		{
			get
			{
				return this.ReplySentUtcField;
			}
			set
			{
				this.ReplySentUtcField = value;
			}
		}

		[DataMember(Order = 5)]
		public int ServerVersion
		{
			get
			{
				return this.ServerVersionField;
			}
			set
			{
				this.ServerVersionField = value;
			}
		}

		[DataMember(Order = 6)]
		public DateTime MountedTimeUtc
		{
			get
			{
				return this.MountedTimeUtcField;
			}
			set
			{
				this.MountedTimeUtcField = value;
			}
		}

		[DataMember(Order = 7)]
		public string LastMountedServerFqdn
		{
			get
			{
				return this.LastMountedServerFqdnField;
			}
			set
			{
				this.LastMountedServerFqdnField = value;
			}
		}

		[DataMember(Order = 8)]
		public long FailoverSequenceNumber
		{
			get
			{
				return this.FailoverSequenceNumberField;
			}
			set
			{
				this.FailoverSequenceNumberField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid DatabaseGuidField;

		private string ServerFqdnField;

		private DateTime RequestSentUtcField;

		private DateTime RequestReceivedUtcField;

		private DateTime ReplySentUtcField;

		private int ServerVersionField;

		private DateTime MountedTimeUtcField;

		private string LastMountedServerFqdnField;

		private long FailoverSequenceNumberField;
	}
}
