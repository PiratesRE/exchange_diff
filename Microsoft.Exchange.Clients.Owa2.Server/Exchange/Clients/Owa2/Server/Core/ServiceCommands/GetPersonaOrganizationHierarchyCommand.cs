using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class GetPersonaOrganizationHierarchyCommand : ServiceCommand<GetPersonaOrganizationHierarchyResponse>
	{
		public GetPersonaOrganizationHierarchyCommand(CallContext callContext, string galObjectGuid, EmailAddressWrapper emailAddress) : base(callContext)
		{
			this.adRecipientSession = base.CallContext.ADRecipientSessionContext.GetGALScopedADRecipientSession(base.CallContext.EffectiveCaller.ClientSecurityContext);
			this.galObjectGuid = galObjectGuid;
			this.emailAddress = emailAddress;
		}

		protected override GetPersonaOrganizationHierarchyResponse InternalExecute()
		{
			return this.GetOrganizationHierarchy();
		}

		private GetPersonaOrganizationHierarchyResponse GetOrganizationHierarchy()
		{
			GetPersonaOrganizationHierarchyResponse getPersonaOrganizationHierarchyResponse = null;
			ADRecipient adrecipient = null;
			Guid guid;
			if (Guid.TryParse(this.galObjectGuid, out guid))
			{
				ADObjectId adobjectId = new ADObjectId(guid);
				adrecipient = this.adRecipientSession.FindByObjectGuid(adobjectId.ObjectGuid);
			}
			else if (this.emailAddress != null && !string.IsNullOrEmpty(this.emailAddress.EmailAddress))
			{
				ProxyAddress proxyAddress = null;
				if (ProxyAddress.TryParse(this.emailAddress.EmailAddress, out proxyAddress))
				{
					adrecipient = this.adRecipientSession.FindByProxyAddress(proxyAddress);
				}
				else
				{
					this.WriteTrace("Invalid EmailAddress was specified. - {0}", new object[]
					{
						this.emailAddress.EmailAddress
					});
				}
			}
			else
			{
				this.WriteTrace("Either GALGuid or EmailAddress must be specified.", new object[0]);
			}
			if (adrecipient != null)
			{
				IADOrgPerson iadorgPerson = adrecipient as IADOrgPerson;
				if (iadorgPerson != null)
				{
					Persona[] managementChain = null;
					Persona manager = null;
					this.GetManagementChain(iadorgPerson, out managementChain, out manager);
					Persona[] peers = this.GetPeers(iadorgPerson);
					Persona[] directReports = this.GetDirectReports(iadorgPerson);
					getPersonaOrganizationHierarchyResponse = new GetPersonaOrganizationHierarchyResponse();
					getPersonaOrganizationHierarchyResponse.ManagementChain = managementChain;
					getPersonaOrganizationHierarchyResponse.Peers = peers;
					getPersonaOrganizationHierarchyResponse.DirectReports = directReports;
					getPersonaOrganizationHierarchyResponse.Manager = manager;
				}
			}
			else
			{
				this.WriteTrace("GAL Recipient not found.", new object[0]);
			}
			return getPersonaOrganizationHierarchyResponse;
		}

		private void GetManagementChain(IADOrgPerson orgPerson, out Persona[] management, out Persona manager)
		{
			object[][] managementChainView = orgPerson.GetManagementChainView(false, GetPersonaOrganizationHierarchyCommand.propertiesToFetch);
			this.WriteTrace("managementChain.Count={0}", new object[]
			{
				(managementChainView == null) ? "0" : managementChainView.Length.ToString()
			});
			management = null;
			manager = null;
			if (managementChainView != null && managementChainView.Length > 1)
			{
				manager = this.GetPersonaFromADObject(managementChainView[managementChainView.Length - 2]);
				this.WriteTrace("Manager name={0} / Manager ID={1}", new object[]
				{
					manager.DisplayName,
					manager.ADObjectId.ToString()
				});
				if (managementChainView.Length > 2)
				{
					management = new Persona[managementChainView.Length - 2];
					for (int i = 0; i < managementChainView.Length - 2; i++)
					{
						management[i] = this.GetPersonaFromADObject(managementChainView[i]);
						this.WriteTrace("Manager name={0} / Manager ID=", new object[]
						{
							management[i].DisplayName,
							management[i].ADObjectId.ToString()
						});
					}
				}
			}
		}

		private Persona[] GetDirectReports(IADOrgPerson orgPerson)
		{
			object[][] directReportsView = orgPerson.GetDirectReportsView(GetPersonaOrganizationHierarchyCommand.propertiesToFetch);
			this.WriteTrace("personaReports.Count={0}", new object[]
			{
				(directReportsView == null) ? "0" : directReportsView.Length.ToString()
			});
			Persona[] result = null;
			if (directReportsView != null && directReportsView.Length > 0)
			{
				result = Array.ConvertAll<object[], Persona>(directReportsView, (object[] personaReport) => this.GetPersonaFromADObject(personaReport));
			}
			return result;
		}

		private Persona[] GetPeers(IADOrgPerson orgPerson)
		{
			Persona[] result = null;
			if (orgPerson.Manager != null)
			{
				ADRecipient adrecipient = this.adRecipientSession.Read(orgPerson.Manager);
				if (adrecipient != null)
				{
					IADOrgPerson iadorgPerson = (IADOrgPerson)adrecipient;
					object[][] directReportsView = iadorgPerson.GetDirectReportsView(GetPersonaOrganizationHierarchyCommand.propertiesToFetch);
					this.WriteTrace("peers.Count={0}", new object[]
					{
						(directReportsView == null) ? "0" : directReportsView.Length.ToString()
					});
					if (directReportsView != null && directReportsView.Length > 1)
					{
						result = Array.ConvertAll<object[], Persona>(directReportsView, (object[] personaReport) => this.GetPersonaFromADObject(personaReport));
					}
				}
				else
				{
					this.WriteTrace("manager not found to get peers list", new object[0]);
				}
			}
			return result;
		}

		private Persona GetPersonaFromADObject(object[] values)
		{
			ADObjectId adobjectId = (ADObjectId)values[0];
			string displayName = values[1].ToString();
			string text = values[2].ToString();
			string text2 = values[3].ToString();
			Persona persona = new Persona
			{
				Attributions = new Attribution[]
				{
					GetPersonaOrganizationHierarchyCommand.galAttribution
				},
				ADObjectId = adobjectId.ObjectGuid,
				DisplayName = displayName,
				Title = text,
				Titles = new StringAttributedValue[]
				{
					new StringAttributedValue(text, new string[]
					{
						"0"
					})
				},
				EmailAddress = new EmailAddressWrapper
				{
					EmailAddress = text2,
					RoutingType = "SMTP",
					MailboxType = MailboxHelper.MailboxTypeType.Mailbox.ToString()
				}
			};
			persona.PersonaId = IdConverter.PersonaIdFromADObjectId(persona.ADObjectId);
			return persona;
		}

		private void WriteTrace(string message, params object[] args)
		{
			ExTraceGlobals.GetPersonaOrganizationHierarchyTracer.TraceDebug((long)this.GetHashCode(), message, args);
		}

		private static readonly PropertyDefinition[] propertiesToFetch = new PropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.DisplayName,
			ADOrgPersonSchema.Title,
			ADRecipientSchema.PrimarySmtpAddress
		};

		private static readonly Attribution galAttribution = new Attribution("0", null, WellKnownNetworkNames.GAL);

		private readonly IRecipientSession adRecipientSession;

		private readonly string galObjectGuid;

		private readonly EmailAddressWrapper emailAddress;
	}
}
