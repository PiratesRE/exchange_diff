using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetMime : ServiceCommand<string>
	{
		public GetMime(CallContext callContext, ItemId itemId) : base(callContext)
		{
			this.itemId = itemId;
		}

		protected override string InternalExecute()
		{
			IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(this.itemId);
			MessageItem messageItem = Item.BindAsMessage(idAndSession.Session, idAndSession.Id);
			messageItem.Load(StoreObjectSchema.ContentConversionProperties);
			OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(base.CallContext.DefaultDomain.DomainName.ToString());
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext);
			outboundConversionOptions.UserADSession = UserContextUtilities.CreateADRecipientSession(base.CallContext.ClientCulture.LCID, true, ConsistencyMode.IgnoreInvalid, false, userContext, true, base.CallContext.Budget);
			outboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			outboundConversionOptions.AllowPartialStnefConversion = true;
			outboundConversionOptions.DemoteBcc = true;
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ItemConversion.ConvertItemToMime(messageItem, memoryStream, outboundConversionOptions);
				memoryStream.Position = 0L;
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Read(array, 0, array.Length);
				@string = Encoding.ASCII.GetString(array);
			}
			return @string;
		}

		private ItemId itemId;
	}
}
