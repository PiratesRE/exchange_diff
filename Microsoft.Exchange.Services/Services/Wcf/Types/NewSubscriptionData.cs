using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewSubscriptionData : OptionsPropertyChangeTracker
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
		public bool Force
		{
			get
			{
				return this.force;
			}
			set
			{
				this.force = value;
				base.TrackPropertyChanged("Force");
			}
		}

		[DataMember]
		public bool Hotmail
		{
			get
			{
				return this.hotmail;
			}
			set
			{
				this.hotmail = value;
				base.TrackPropertyChanged("Hotmail");
			}
		}

		[DataMember]
		public bool Imap
		{
			get
			{
				return this.imap;
			}
			set
			{
				this.imap = value;
				base.TrackPropertyChanged("Imap");
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				base.TrackPropertyChanged("Name");
			}
		}

		[DataMember]
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
				base.TrackPropertyChanged("Password");
			}
		}

		[DataMember]
		public bool Pop
		{
			get
			{
				return this.pop;
			}
			set
			{
				this.pop = value;
				base.TrackPropertyChanged("Pop");
			}
		}

		private string displayName;

		private string emailAddress;

		private bool force;

		private bool hotmail;

		private bool imap;

		private string name;

		private string password;

		private bool pop;
	}
}
