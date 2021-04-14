using System;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Send")]
	internal class RespondToMessageRequest : EntityActionRequest<Message>
	{
		public RespondToMessageRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public MessageResponseType ResponseType { get; protected set; }

		public string Comment { get; protected set; }

		public Recipient[] ToRecipients { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			object obj;
			if (base.Parameters.TryGetValue("Comment", out obj))
			{
				this.Comment = (string)obj;
			}
			string actionName;
			if ((actionName = base.ActionName) != null)
			{
				if (actionName == "Reply")
				{
					this.ResponseType = MessageResponseType.Reply;
					return;
				}
				if (actionName == "ReplyAll")
				{
					this.ResponseType = MessageResponseType.ReplyAll;
					return;
				}
				if (!(actionName == "Forward"))
				{
					return;
				}
				this.ResponseType = MessageResponseType.Forward;
				object obj2;
				if (base.Parameters.TryGetValue("ToRecipients", out obj2))
				{
					this.ToRecipients = RecipientsODataConverter.ODataCollectionValueToRecipients((ODataCollectionValue)obj2);
				}
			}
		}

		public override void Validate()
		{
			base.Validate();
			if (this.ResponseType == MessageResponseType.Forward)
			{
				ValidationHelper.ValidateParameterEmpty("ToRecipients", this.ToRecipients);
			}
		}

		public override ODataCommand GetODataCommand()
		{
			return new RespondToMessageCommand(this);
		}
	}
}
