using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class AddTrustedSender : ServiceCommand<bool>
	{
		public AddTrustedSender(CallContext callContext, ItemId itemId) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(itemId, "itemId", "AddTrustedSender::AddTrustedSender");
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.itemId = itemId;
		}

		protected override bool InternalExecute()
		{
			return this.InternalAddTrustedSender(this.itemId);
		}

		private bool InternalAddTrustedSender(ItemId itemId)
		{
			JunkEmailRule junkEmailRule = this.session.JunkEmailRule;
			bool flag = false;
			bool result;
			using (Item item = Item.Bind(this.session, IdConverter.EwsIdToMessageStoreObjectId(itemId.Id)))
			{
				string value = (string)item[MessageItemSchema.SenderSmtpAddress];
				JunkEmailCollection.ValidationProblem validationProblem = JunkEmailCollection.ValidationProblem.NoError;
				try
				{
					validationProblem = junkEmailRule.TrustedSenderEmailCollection.TryAdd(value);
				}
				catch (JunkEmailValidationException ex)
				{
					validationProblem = ex.Problem;
				}
				finally
				{
					switch (validationProblem)
					{
					case JunkEmailCollection.ValidationProblem.NoError:
					case JunkEmailCollection.ValidationProblem.Duplicate:
						junkEmailRule.Save();
						flag = true;
						break;
					}
				}
				result = flag;
			}
			return result;
		}

		private readonly MailboxSession session;

		private ItemId itemId;
	}
}
