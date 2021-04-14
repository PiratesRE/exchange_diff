using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ImapAuthenticationException : OperationLevelPermanentException
	{
		public ImapAuthenticationException(string imapAuthenticationErrorMsg, string authMechanismName, RetryPolicy retryPolicy) : base(CXStrings.ImapAuthenticationErrorMsg(imapAuthenticationErrorMsg, authMechanismName, retryPolicy))
		{
			this.imapAuthenticationErrorMsg = imapAuthenticationErrorMsg;
			this.authMechanismName = authMechanismName;
			this.retryPolicy = retryPolicy;
		}

		public ImapAuthenticationException(string imapAuthenticationErrorMsg, string authMechanismName, RetryPolicy retryPolicy, Exception innerException) : base(CXStrings.ImapAuthenticationErrorMsg(imapAuthenticationErrorMsg, authMechanismName, retryPolicy), innerException)
		{
			this.imapAuthenticationErrorMsg = imapAuthenticationErrorMsg;
			this.authMechanismName = authMechanismName;
			this.retryPolicy = retryPolicy;
		}

		protected ImapAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.imapAuthenticationErrorMsg = (string)info.GetValue("imapAuthenticationErrorMsg", typeof(string));
			this.authMechanismName = (string)info.GetValue("authMechanismName", typeof(string));
			this.retryPolicy = (RetryPolicy)info.GetValue("retryPolicy", typeof(RetryPolicy));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("imapAuthenticationErrorMsg", this.imapAuthenticationErrorMsg);
			info.AddValue("authMechanismName", this.authMechanismName);
			info.AddValue("retryPolicy", this.retryPolicy);
		}

		public string ImapAuthenticationErrorMsg
		{
			get
			{
				return this.imapAuthenticationErrorMsg;
			}
		}

		public string AuthMechanismName
		{
			get
			{
				return this.authMechanismName;
			}
		}

		public RetryPolicy RetryPolicy
		{
			get
			{
				return this.retryPolicy;
			}
		}

		private readonly string imapAuthenticationErrorMsg;

		private readonly string authMechanismName;

		private readonly RetryPolicy retryPolicy;
	}
}
