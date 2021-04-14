using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MailboxDiscoverySearchRequest : DiscoverySearchBase
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxDiscoverySearchRequest.schema;
			}
		}

		internal override string ItemClass
		{
			get
			{
				return "IPM.Configuration.MailboxDiscoverySearchRequest";
			}
		}

		public ActionRequestType AsynchronousActionRequest
		{
			get
			{
				return (ActionRequestType)this[MailboxDiscoverySearchRequestSchema.AsynchronousActionRequest];
			}
			set
			{
				this[MailboxDiscoverySearchRequestSchema.AsynchronousActionRequest] = value;
			}
		}

		public string AsynchronousActionRbacContext
		{
			get
			{
				return (string)this[MailboxDiscoverySearchRequestSchema.AsynchronousActionRbacContext];
			}
			set
			{
				this[MailboxDiscoverySearchRequestSchema.AsynchronousActionRbacContext] = value;
			}
		}

		private static ObjectSchema schema = new MailboxDiscoverySearchRequestSchema();
	}
}
