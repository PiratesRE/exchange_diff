using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthUserNotFoundException : LocalizedException
	{
		public CasHealthUserNotFoundException(string userPrincipalName, string errorString) : base(Strings.CasHealthUserNotFound(userPrincipalName, errorString))
		{
			this.userPrincipalName = userPrincipalName;
			this.errorString = errorString;
		}

		public CasHealthUserNotFoundException(string userPrincipalName, string errorString, Exception innerException) : base(Strings.CasHealthUserNotFound(userPrincipalName, errorString), innerException)
		{
			this.userPrincipalName = userPrincipalName;
			this.errorString = errorString;
		}

		protected CasHealthUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userPrincipalName = (string)info.GetValue("userPrincipalName", typeof(string));
			this.errorString = (string)info.GetValue("errorString", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userPrincipalName", this.userPrincipalName);
			info.AddValue("errorString", this.errorString);
		}

		public string UserPrincipalName
		{
			get
			{
				return this.userPrincipalName;
			}
		}

		public string ErrorString
		{
			get
			{
				return this.errorString;
			}
		}

		private readonly string userPrincipalName;

		private readonly string errorString;
	}
}
