using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HA.Services
{
	[DataContract(Name = "DatabaseServerInformationFault", Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/")]
	public class DatabaseServerInformationFault
	{
		public DatabaseServerInformationFault()
		{
		}

		public DatabaseServerInformationFault(DatabaseServerInformationFaultType code, Exception ex) : this()
		{
			this.m_errorCode = code;
			this.m_type = ex.GetType().FullName;
			this.m_message = ex.Message;
			this.m_stackTrace = ex.StackTrace;
		}

		[DataMember(Name = "ErrorCode", IsRequired = false, Order = 0)]
		public DatabaseServerInformationFaultType ErrorCode
		{
			get
			{
				return this.m_errorCode;
			}
			set
			{
				this.m_errorCode = value;
			}
		}

		[DataMember(Name = "Type", IsRequired = false, Order = 1)]
		public string Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}

		[DataMember(Name = "Message", IsRequired = false, Order = 2)]
		public string Message
		{
			get
			{
				return this.m_message;
			}
			set
			{
				this.m_message = value;
			}
		}

		[DataMember(Name = "StackTrace", IsRequired = false, Order = 3)]
		public string StackTrace
		{
			get
			{
				return this.m_stackTrace;
			}
			set
			{
				this.m_stackTrace = value;
			}
		}

		private DatabaseServerInformationFaultType m_errorCode;

		private string m_type;

		private string m_message;

		private string m_stackTrace;
	}
}
