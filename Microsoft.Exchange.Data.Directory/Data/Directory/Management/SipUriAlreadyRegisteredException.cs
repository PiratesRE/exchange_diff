using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SipUriAlreadyRegisteredException : LocalizedException
	{
		public SipUriAlreadyRegisteredException(string sipUri, string user) : base(DirectoryStrings.SipUriAlreadyRegistered(sipUri, user))
		{
			this.sipUri = sipUri;
			this.user = user;
		}

		public SipUriAlreadyRegisteredException(string sipUri, string user, Exception innerException) : base(DirectoryStrings.SipUriAlreadyRegistered(sipUri, user), innerException)
		{
			this.sipUri = sipUri;
			this.user = user;
		}

		protected SipUriAlreadyRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sipUri = (string)info.GetValue("sipUri", typeof(string));
			this.user = (string)info.GetValue("user", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sipUri", this.sipUri);
			info.AddValue("user", this.user);
		}

		public string SipUri
		{
			get
			{
				return this.sipUri;
			}
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		private readonly string sipUri;

		private readonly string user;
	}
}
