using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthMailboxNotFoundException : LocalizedException
	{
		public CasHealthMailboxNotFoundException(string userPrincipalName) : base(Strings.CasHealthMailboxNotFound(userPrincipalName))
		{
			this.userPrincipalName = userPrincipalName;
		}

		public CasHealthMailboxNotFoundException(string userPrincipalName, Exception innerException) : base(Strings.CasHealthMailboxNotFound(userPrincipalName), innerException)
		{
			this.userPrincipalName = userPrincipalName;
		}

		protected CasHealthMailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userPrincipalName = (string)info.GetValue("userPrincipalName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userPrincipalName", this.userPrincipalName);
		}

		public string UserPrincipalName
		{
			get
			{
				return this.userPrincipalName;
			}
		}

		private readonly string userPrincipalName;
	}
}
