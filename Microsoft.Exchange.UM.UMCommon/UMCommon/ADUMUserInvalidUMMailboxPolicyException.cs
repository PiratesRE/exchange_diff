using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADUMUserInvalidUMMailboxPolicyException : LocalizedException
	{
		public ADUMUserInvalidUMMailboxPolicyException(string useraddress) : base(Strings.ADUMUserInvalidUMMailboxPolicyException(useraddress))
		{
			this.useraddress = useraddress;
		}

		public ADUMUserInvalidUMMailboxPolicyException(string useraddress, Exception innerException) : base(Strings.ADUMUserInvalidUMMailboxPolicyException(useraddress), innerException)
		{
			this.useraddress = useraddress;
		}

		protected ADUMUserInvalidUMMailboxPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.useraddress = (string)info.GetValue("useraddress", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("useraddress", this.useraddress);
		}

		public string Useraddress
		{
			get
			{
				return this.useraddress;
			}
		}

		private readonly string useraddress;
	}
}
