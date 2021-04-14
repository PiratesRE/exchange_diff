using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public class LoadBalancingMiniRecipient : MiniRecipient
	{
		internal LoadBalancingMiniRecipient(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		public LoadBalancingMiniRecipient()
		{
		}

		public UserConfigXML ConfigXML
		{
			get
			{
				return (UserConfigXML)this[MiniRecipientSchema.ConfigurationXML];
			}
		}

		public string MailboxMoveBatchName
		{
			get
			{
				return (string)this[LoadBalancingMiniRecipientSchema.MailboxMoveBatchName];
			}
		}

		public RequestStatus MailboxMoveStatus
		{
			get
			{
				return (RequestStatus)this[LoadBalancingMiniRecipientSchema.MailboxMoveStatus];
			}
		}

		public RequestFlags MailboxMoveFlags
		{
			get
			{
				return (RequestFlags)this[LoadBalancingMiniRecipientSchema.MailboxMoveFlags];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return LoadBalancingMiniRecipient.schema;
			}
		}

		private static readonly LoadBalancingMiniRecipientSchema schema = ObjectSchema.GetInstance<LoadBalancingMiniRecipientSchema>();
	}
}
