using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public abstract class MailboxPolicy : ADLegacyVersionableObject
	{
		internal abstract bool CheckForAssociatedUsers();

		internal int MailboxPolicyFlags
		{
			get
			{
				return (int)this[MailboxPolicySchema.MailboxPolicyFlags];
			}
			set
			{
				this[MailboxPolicySchema.MailboxPolicyFlags] = value;
			}
		}

		public virtual bool IsDefault
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
	}
}
