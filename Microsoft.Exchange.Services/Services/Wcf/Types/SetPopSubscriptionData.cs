using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetPopSubscriptionData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
				base.TrackPropertyChanged("DisplayName");
			}
		}

		[DataMember]
		public string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
			set
			{
				this.emailAddress = value;
				base.TrackPropertyChanged("EmailAddress");
			}
		}

		[DataMember]
		public Identity Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				this.identity = value;
				base.TrackPropertyChanged("Identity");
			}
		}

		[IgnoreDataMember]
		public AuthenticationMechanism IncomingAuth
		{
			get
			{
				return this.incomingAuth;
			}
			set
			{
				this.incomingAuth = value;
				base.TrackPropertyChanged("IncomingAuth");
			}
		}

		[DataMember(Name = "IncomingAuth", IsRequired = false, EmitDefaultValue = false)]
		public string IncomingAuthString
		{
			get
			{
				return EnumUtilities.ToString<AuthenticationMechanism>(this.IncomingAuth);
			}
			set
			{
				this.IncomingAuth = EnumUtilities.Parse<AuthenticationMechanism>(value);
			}
		}

		[DataMember]
		public string IncomingPassword
		{
			get
			{
				return this.incomingPassword;
			}
			set
			{
				this.incomingPassword = value;
				base.TrackPropertyChanged("IncomingPassword");
			}
		}

		[DataMember]
		public int IncomingPort
		{
			get
			{
				return this.incomingPort;
			}
			set
			{
				this.incomingPort = value;
				base.TrackPropertyChanged("IncomingPort");
			}
		}

		[IgnoreDataMember]
		public SecurityMechanism IncomingSecurity
		{
			get
			{
				return this.incomingSecurity;
			}
			set
			{
				this.incomingSecurity = value;
				base.TrackPropertyChanged("IncomingSecurity");
			}
		}

		[DataMember(Name = "IncomingSecurity", IsRequired = false, EmitDefaultValue = false)]
		public string IncomingSecurityString
		{
			get
			{
				return EnumUtilities.ToString<SecurityMechanism>(this.IncomingSecurity);
			}
			set
			{
				this.IncomingSecurity = EnumUtilities.Parse<SecurityMechanism>(value);
			}
		}

		[DataMember]
		public string IncomingServer
		{
			get
			{
				return this.incomingServer;
			}
			set
			{
				this.incomingServer = value;
				base.TrackPropertyChanged("IncomingServer");
			}
		}

		[DataMember]
		public string IncomingUserName
		{
			get
			{
				return this.incomingUserName;
			}
			set
			{
				this.incomingUserName = value;
				base.TrackPropertyChanged("IncomingUserName");
			}
		}

		[DataMember]
		public bool LeaveOnServer
		{
			get
			{
				return this.leaveOnServer;
			}
			set
			{
				this.leaveOnServer = value;
				base.TrackPropertyChanged("LeaveOnServer");
			}
		}

		[DataMember]
		public bool ResendVerification
		{
			get
			{
				return this.resendVerification;
			}
			set
			{
				this.resendVerification = value;
				base.TrackPropertyChanged("ResendVerification");
			}
		}

		private string displayName;

		private string emailAddress;

		private Identity identity;

		private AuthenticationMechanism incomingAuth;

		private string incomingPassword;

		private int incomingPort;

		private SecurityMechanism incomingSecurity;

		private string incomingServer;

		private string incomingUserName;

		private bool leaveOnServer;

		private bool resendVerification;
	}
}
