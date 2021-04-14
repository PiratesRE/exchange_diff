using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetLikers : ServiceCommand<GetLikersResponseMessage>
	{
		public GetLikers(CallContext callContext, GetLikersRequest request) : base(callContext)
		{
			this.request = request;
			this.callContext = callContext;
		}

		protected override GetLikersResponseMessage InternalExecute()
		{
			this.request.Validate();
			GetLikersResponseMessage getLikersResponseMessage = new GetLikersResponseMessage();
			GetLikersResponseMessage result;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.callContext.MailboxIdentityPrincipal, this.callContext.ClientCulture, StoreSessionCacheBase.BuildMapiApplicationId(this.callContext, null)))
			{
				IList<Participant> likers;
				using (MessageItem messageItem = MessageItem.Bind(mailboxSession, IdConverter.EwsIdToMessageStoreObjectId(this.request.ItemId.Id), Likers.RequiredProperties))
				{
					likers = messageItem.Likers;
				}
				List<string> list = new List<string>();
				List<ProxyAddress> list2 = new List<ProxyAddress>();
				foreach (Participant participant in likers)
				{
					string routingType;
					if ((routingType = participant.RoutingType) != null)
					{
						if (!(routingType == "EX"))
						{
							if (routingType == "SMTP")
							{
								list2.Add(ProxyAddress.Parse(participant.SmtpEmailAddress));
							}
						}
						else
						{
							list.Add(participant.EmailAddress);
						}
					}
				}
				List<Persona> list3 = new List<Persona>();
				IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
				if (list.Any<string>())
				{
					list3.AddRange(Persona.LoadFromLegacyDNs(mailboxSession, adrecipientSession, list.ToArray(), Persona.DefaultPersonaShape));
				}
				if (list2.Any<ProxyAddress>())
				{
					list3.AddRange(Persona.LoadFromProxyAddresses(mailboxSession, adrecipientSession, list2.ToArray(), Persona.DefaultPersonaShape));
				}
				getLikersResponseMessage.Personas = list3.ToArray();
				result = getLikersResponseMessage;
			}
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.GetLikersTracer;

		private readonly GetLikersRequest request;

		private readonly CallContext callContext;
	}
}
