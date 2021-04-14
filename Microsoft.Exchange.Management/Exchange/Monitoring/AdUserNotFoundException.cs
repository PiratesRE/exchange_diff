using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdUserNotFoundException : LocalizedException
	{
		public AdUserNotFoundException(string userPrincipalName, string errorMessage) : base(Strings.ErrorAdUserNotFound(userPrincipalName, errorMessage))
		{
			this.userPrincipalName = userPrincipalName;
			this.errorMessage = errorMessage;
		}

		public AdUserNotFoundException(string userPrincipalName, string errorMessage, Exception innerException) : base(Strings.ErrorAdUserNotFound(userPrincipalName, errorMessage), innerException)
		{
			this.userPrincipalName = userPrincipalName;
			this.errorMessage = errorMessage;
		}

		protected AdUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userPrincipalName = (string)info.GetValue("userPrincipalName", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userPrincipalName", this.userPrincipalName);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string UserPrincipalName
		{
			get
			{
				return this.userPrincipalName;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string userPrincipalName;

		private readonly string errorMessage;
	}
}
