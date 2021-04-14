using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class PopSubscriptionBaseParameter : PimSubscriptionParameter
	{
		[DataMember]
		public bool LeaveOnServer
		{
			get
			{
				return (bool)(base["LeaveOnServer"] ?? true);
			}
			set
			{
				base["LeaveOnServer"] = value;
			}
		}

		[DataMember]
		public string IncomingServer
		{
			get
			{
				return (string)base["IncomingServer"];
			}
			set
			{
				base["IncomingServer"] = value;
			}
		}

		[DataMember]
		public int IncomingPort
		{
			get
			{
				return (int)(base["IncomingPort"] ?? 0);
			}
			set
			{
				base["IncomingPort"] = value;
			}
		}

		[DataMember]
		public string IncomingSecurity
		{
			get
			{
				return (string)base["IncomingSecurity"];
			}
			set
			{
				base["IncomingSecurity"] = value;
			}
		}

		[DataMember]
		public string IncomingAuth
		{
			get
			{
				return (string)base["IncomingAuth"];
			}
			set
			{
				base["IncomingAuth"] = value;
			}
		}

		[DataMember]
		public string IncomingUserName
		{
			get
			{
				return (string)base["IncomingUserName"];
			}
			set
			{
				base["IncomingUserName"] = value;
			}
		}

		protected override string PasswordParameterName
		{
			get
			{
				return "IncomingPassword";
			}
		}
	}
}
