using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Conversations
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ItemWithoutConversationIdException : LocalizedException
	{
		public ItemWithoutConversationIdException(string user, string itemid) : base(Strings.ConversationsItemWithoutConversationId(user, itemid))
		{
			this.user = user;
			this.itemid = itemid;
		}

		public ItemWithoutConversationIdException(string user, string itemid, Exception innerException) : base(Strings.ConversationsItemWithoutConversationId(user, itemid), innerException)
		{
			this.user = user;
			this.itemid = itemid;
		}

		protected ItemWithoutConversationIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
			this.itemid = (string)info.GetValue("itemid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
			info.AddValue("itemid", this.itemid);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public string Itemid
		{
			get
			{
				return this.itemid;
			}
		}

		private readonly string user;

		private readonly string itemid;
	}
}
