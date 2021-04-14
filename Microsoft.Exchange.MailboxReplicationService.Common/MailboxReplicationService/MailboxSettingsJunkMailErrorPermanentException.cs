using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxSettingsJunkMailErrorPermanentException : MailboxReplicationPermanentException
	{
		public MailboxSettingsJunkMailErrorPermanentException(string collectionName, string itemList, string validationError) : base(MrsStrings.MailboxSettingsJunkMailError(collectionName, itemList, validationError))
		{
			this.collectionName = collectionName;
			this.itemList = itemList;
			this.validationError = validationError;
		}

		public MailboxSettingsJunkMailErrorPermanentException(string collectionName, string itemList, string validationError, Exception innerException) : base(MrsStrings.MailboxSettingsJunkMailError(collectionName, itemList, validationError), innerException)
		{
			this.collectionName = collectionName;
			this.itemList = itemList;
			this.validationError = validationError;
		}

		protected MailboxSettingsJunkMailErrorPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.collectionName = (string)info.GetValue("collectionName", typeof(string));
			this.itemList = (string)info.GetValue("itemList", typeof(string));
			this.validationError = (string)info.GetValue("validationError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("collectionName", this.collectionName);
			info.AddValue("itemList", this.itemList);
			info.AddValue("validationError", this.validationError);
		}

		public string CollectionName
		{
			get
			{
				return this.collectionName;
			}
		}

		public string ItemList
		{
			get
			{
				return this.itemList;
			}
		}

		public string ValidationError
		{
			get
			{
				return this.validationError;
			}
		}

		private readonly string collectionName;

		private readonly string itemList;

		private readonly string validationError;
	}
}
