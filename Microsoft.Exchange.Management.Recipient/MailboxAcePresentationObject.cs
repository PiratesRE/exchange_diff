using System;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public class MailboxAcePresentationObject : AcePresentationObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxAcePresentationObject.schema;
			}
		}

		public MailboxAcePresentationObject(ActiveDirectoryAccessRule ace, ADObjectId identity) : base(ace, identity)
		{
			this.AccessRights = new MailboxRights[]
			{
				(MailboxRights)ace.ActiveDirectoryRights
			};
			base.ResetChangeTracking();
		}

		public MailboxAcePresentationObject()
		{
		}

		[Parameter(Mandatory = true, ParameterSetName = "AccessRights")]
		[Parameter(Mandatory = false, ParameterSetName = "Instance")]
		public MailboxRights[] AccessRights
		{
			get
			{
				return (MailboxRights[])this[MailboxAcePresentationObjectSchema.AccessRights];
			}
			set
			{
				this[MailboxAcePresentationObjectSchema.AccessRights] = value;
			}
		}

		private static MailboxAcePresentationObjectSchema schema = ObjectSchema.GetInstance<MailboxAcePresentationObjectSchema>();
	}
}
