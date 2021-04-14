using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetPersonaModernGroupMembership : ServiceCommand<GetPersonaModernGroupMembershipResponse>
	{
		public GetPersonaModernGroupMembership(CallContext context, GetPersonaModernGroupMembershipRequest request) : base(context)
		{
			this.request = request;
			OwsLogRegistry.Register("GetPersonaModernGroupMembership", typeof(GetPersonaModernGroupMembershipMetadata), new Type[0]);
			request.ValidateRequest();
		}

		protected IRecipientSession ADSession
		{
			get
			{
				return base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			}
		}

		protected GetPersonaModernGroupMembershipResponse EmptyResponse
		{
			get
			{
				base.CallContext.ProtocolLog.Set(GetPersonaModernGroupMembershipMetadata.GroupCount, 0);
				return new GetPersonaModernGroupMembershipResponse
				{
					Groups = new Persona[0]
				};
			}
		}

		protected override GetPersonaModernGroupMembershipResponse InternalExecute()
		{
			ExTraceGlobals.ModernGroupsTracer.TraceDebug<string>((long)this.GetHashCode(), "GetPersonaModernGroupMembership.InternalExecute: Retrieving modern groups for user {0}.", this.request.SmtpAddress);
			ADRecipient adrecipient = this.ADSession.FindByProxyAddress(this.request.ProxyAddress);
			if (adrecipient == null)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceWarning<ProxyAddress>((long)this.GetHashCode(), "GetPersonaModernGroupMembership.InternalExecute: ADRecipient for proxy address {0} was not found.", this.request.ProxyAddress);
				return new GetPersonaModernGroupMembershipResponse();
			}
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.GroupMailbox),
				new ComparisonFilter(ComparisonOperator.Equal, IUnifiedGroupMailboxSchema.UnifiedGroupMembersLink, adrecipient.Id)
			});
			SortBy sortBy = new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending);
			List<ADRawEntry> list = this.ADSession.FindRecipient(null, QueryScope.SubTree, filter, sortBy, 0, GetPersonaModernGroupMembership.DefaultGroupProperties).ToList<ADRawEntry>();
			return new GetPersonaModernGroupMembershipResponse
			{
				Groups = list.ConvertAll<Persona>(new Converter<ADRawEntry, Persona>(GetPersonaModernGroupMembership.ConvertGroupMailboxToPersona)).ToArray()
			};
		}

		private static Persona ConvertGroupMailboxToPersona(ADRawEntry item)
		{
			string text = item[ADRecipientSchema.DisplayName] as string;
			return new Persona
			{
				DisplayName = text,
				Alias = (item[ADRecipientSchema.Alias] as string),
				PersonaType = PersonType.ModernGroup.ToString(),
				EmailAddress = new EmailAddressWrapper
				{
					Name = (text ?? string.Empty),
					EmailAddress = item[ADRecipientSchema.PrimarySmtpAddress].ToString(),
					RoutingType = "SMTP",
					MailboxType = MailboxHelper.MailboxTypeType.GroupMailbox.ToString()
				}
			};
		}

		internal static readonly ADPropertyDefinition[] DefaultGroupProperties = new ADPropertyDefinition[]
		{
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.Alias,
			ADRecipientSchema.PrimarySmtpAddress
		};

		private readonly GetPersonaModernGroupMembershipRequest request;
	}
}
