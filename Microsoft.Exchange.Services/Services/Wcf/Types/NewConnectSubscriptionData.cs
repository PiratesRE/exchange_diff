using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewConnectSubscriptionData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string AppAuthorizationCode
		{
			get
			{
				return this.appAuthorizationCode;
			}
			set
			{
				this.appAuthorizationCode = value;
				base.TrackPropertyChanged("AppAuthorizationCode");
			}
		}

		[IgnoreDataMember]
		public ConnectSubscriptionType ConnectSubscriptionType { get; set; }

		[DataMember]
		public string OAuthVerifier
		{
			get
			{
				return this.oAuthVerifier;
			}
			set
			{
				this.oAuthVerifier = value;
				base.TrackPropertyChanged("OAuthVerifier");
			}
		}

		[DataMember]
		public string RequestToken
		{
			get
			{
				return this.requestToken;
			}
			set
			{
				this.requestToken = value;
				base.TrackPropertyChanged("RequestToken");
			}
		}

		[DataMember]
		public string RequestSecret
		{
			get
			{
				return this.requestSecret;
			}
			set
			{
				this.requestSecret = value;
				base.TrackPropertyChanged("RequestSecret");
			}
		}

		[DataMember(Name = "ConnectSubscriptionType", EmitDefaultValue = false)]
		public string ConnectSubscriptionTypeString
		{
			get
			{
				return EnumUtilities.ToString<ConnectSubscriptionType>(this.ConnectSubscriptionType);
			}
			set
			{
				this.ConnectSubscriptionType = EnumUtilities.Parse<ConnectSubscriptionType>(value);
			}
		}

		[DataMember]
		public string RedirectUri
		{
			get
			{
				return this.redirectUri;
			}
			set
			{
				this.redirectUri = value;
				base.TrackPropertyChanged("RedirectUri");
			}
		}

		private string appAuthorizationCode;

		private string oAuthVerifier;

		private string redirectUri;

		private string requestSecret;

		private string requestToken;
	}
}
