using System;
using Microsoft.Exchange.Data;
using Microsoft.WindowsAzure.ActiveDirectory;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public sealed class AADUserPresentationObject : AADDirectoryObjectPresentationObject
	{
		internal AADUserPresentationObject(User user) : base(user)
		{
			this.DisplayName = user.displayName;
			this.MailNickname = user.mailNickname;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<AADUserPresentationObjectSchema>();
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[AADUserPresentationObjectSchema.DisplayName];
			}
			set
			{
				this[AADUserPresentationObjectSchema.DisplayName] = value;
			}
		}

		public string MailNickname
		{
			get
			{
				return (string)this[AADUserPresentationObjectSchema.MailNickname];
			}
			set
			{
				this[AADUserPresentationObjectSchema.MailNickname] = value;
			}
		}
	}
}
