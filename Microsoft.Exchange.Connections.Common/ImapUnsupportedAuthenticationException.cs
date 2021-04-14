using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ImapUnsupportedAuthenticationException : OperationLevelTransientException
	{
		public ImapUnsupportedAuthenticationException(string authErrorMsg, string authMechanismName, RetryPolicy retryPolicy) : base(CXStrings.ImapUnsupportedAuthenticationErrorMsg(authErrorMsg, authMechanismName, retryPolicy))
		{
			this.authErrorMsg = authErrorMsg;
			this.authMechanismName = authMechanismName;
			this.retryPolicy = retryPolicy;
		}

		public ImapUnsupportedAuthenticationException(string authErrorMsg, string authMechanismName, RetryPolicy retryPolicy, Exception innerException) : base(CXStrings.ImapUnsupportedAuthenticationErrorMsg(authErrorMsg, authMechanismName, retryPolicy), innerException)
		{
			this.authErrorMsg = authErrorMsg;
			this.authMechanismName = authMechanismName;
			this.retryPolicy = retryPolicy;
		}

		protected ImapUnsupportedAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.authErrorMsg = (string)info.GetValue("authErrorMsg", typeof(string));
			this.authMechanismName = (string)info.GetValue("authMechanismName", typeof(string));
			this.retryPolicy = (RetryPolicy)info.GetValue("retryPolicy", typeof(RetryPolicy));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("authErrorMsg", this.authErrorMsg);
			info.AddValue("authMechanismName", this.authMechanismName);
			info.AddValue("retryPolicy", this.retryPolicy);
		}

		public string AuthErrorMsg
		{
			get
			{
				return this.authErrorMsg;
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

		private readonly string authErrorMsg;

		private readonly string authMechanismName;

		private readonly RetryPolicy retryPolicy;
	}
}
