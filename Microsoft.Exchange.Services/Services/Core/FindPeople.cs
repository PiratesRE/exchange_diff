using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FindPeople : SingleStepServiceCommand<FindPeopleRequest, FindPeopleResult>
	{
		public FindPeople(CallContext callContext, FindPeopleRequest request) : base(callContext, request)
		{
			this.request = request;
			this.callContext = callContext;
			this.InitializeTracers();
			OwsLogRegistry.Register(FindPeople.FindPeopleActionName, typeof(FindPeopleMetadata), new Type[0]);
		}

		private static ADObjectId GetAddressListId(AddressListId addressListId)
		{
			Guid guid;
			if (Guid.TryParse(addressListId.Id, out guid))
			{
				return new ADObjectId(guid);
			}
			throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new FindPeopleResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<FindPeopleResult> Execute()
		{
			DateTime utcNow = DateTime.UtcNow;
			this.findPeopleImplementation = this.CreateFindPeopleImplementation();
			this.findPeopleImplementation.Validate();
			FindPeopleResult findPeopleResult = this.findPeopleImplementation.Execute();
			if (this.request.ShouldResolveOneOffEmailAddress)
			{
				if (this.request.ParentFolderId != null)
				{
					this.tracer.TraceError((long)this.GetHashCode(), "ShouldResolveOneOffEmailAddress is set to true but scope of the search is not set to both mailbox and directory.");
					throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
				}
				if (findPeopleResult.PersonaList.Length == 0)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "FindPeople:Execute method calling CreatePersonaIfQueryStringIsValidAddress.");
					findPeopleResult = this.CreatePersonaIfQueryStringIsValidAddress();
				}
			}
			ServiceResult<FindPeopleResult> result = new ServiceResult<FindPeopleResult>(findPeopleResult);
			DateTime utcNow2 = DateTime.UtcNow;
			this.findPeopleImplementation.Logger.Set(FindPeopleMetadata.CommandExecutionStart, utcNow.ToUniversalTime().ToString("o"));
			this.findPeopleImplementation.Logger.Set(FindPeopleMetadata.CommandExecutionEnd, utcNow2.ToUniversalTime().ToString("o"));
			return result;
		}

		protected override void LogTracesForCurrentRequest()
		{
			ServiceCommandBase.TraceLoggerFactory.Create(base.CallContext.HttpContext.Response.Headers).LogTraces(this.requestTracer);
		}

		private void InitializeTracers()
		{
			ITracer tracer;
			if (!base.IsRequestTracingEnabled)
			{
				ITracer instance = NullTracer.Instance;
				tracer = instance;
			}
			else
			{
				tracer = new InMemoryTracer(ExTraceGlobals.FindPeopleCallTracer.Category, ExTraceGlobals.FindPeopleCallTracer.TraceTag);
			}
			this.requestTracer = tracer;
			this.tracer = ExTraceGlobals.FindPeopleCallTracer.Compose(this.requestTracer);
		}

		private ADObjectId GetGlobalAddressListId()
		{
			IExchangePrincipal mailboxOwner = base.MailboxIdentityMailboxSession.MailboxOwner;
			return this.GetGlobalAddressListId(mailboxOwner.MailboxInfo.Configuration.AddressBookPolicy, this.callContext.EffectiveCaller.ClientSecurityContext, base.MailboxIdentityMailboxSession.GetADConfigurationSession(true, ConsistencyMode.IgnoreInvalid), base.MailboxIdentityMailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
		}

		private ADObjectId GetGlobalAddressListId(ADObjectId addressBookPolicyId, ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, IRecipientSession recipientSession)
		{
			if (!addressBookPolicyId.IsNullOrEmpty())
			{
				return DirectoryHelper.GetGlobalAddressListFromAddressBookPolicy(addressBookPolicyId, configurationSession);
			}
			return this.GetGlobalAddressListInAbsenceOfABP(clientSecurityContext, configurationSession, recipientSession);
		}

		private ADObjectId GetGlobalAddressListInAbsenceOfABP(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, IRecipientSession recipientSession)
		{
			AddressBookBase globalAddressList = AddressBookBase.GetGlobalAddressList(clientSecurityContext, configurationSession, recipientSession, null);
			if (globalAddressList != null)
			{
				return globalAddressList.Id;
			}
			return null;
		}

		private IdAndSession GetFolderIdAndSession()
		{
			ServiceCommandBase.ThrowIfNull(this.request.ParentFolderId, "ParentFolderId", "FindPeople::GetFolderIdAndSession");
			ServiceCommandBase.ThrowIfNull(this.request.ParentFolderId.BaseFolderId, "ParentFolderId", "FindPeople::GetFolderIdAndSession");
			return base.IdConverter.ConvertFolderIdToIdAndSession(this.request.ParentFolderId.BaseFolderId, IdConverter.ConvertOption.IgnoreChangeKey);
		}

		private FindPeopleImplementation CreateFindPeopleImplementation()
		{
			FindPeopleParameters parameters = new FindPeopleParameters
			{
				QueryString = this.request.QueryString,
				SortResults = this.request.SortOrder,
				Paging = this.request.Paging,
				Restriction = this.request.Restriction,
				AggregationRestriction = this.request.AggregationRestriction,
				PersonaShape = this.request.PersonaShape,
				CultureInfo = this.callContext.ClientCulture,
				Logger = this.callContext.ProtocolLog
			};
			if (this.request.QueryString == null)
			{
				return this.CreateFindPeopleBrowseImplementation(parameters);
			}
			return this.CreateFindPeopleSearchImplemenation(parameters);
		}

		private FindPeopleImplementation CreateFindPeopleBrowseImplementation(FindPeopleParameters parameters)
		{
			ServiceCommandBase.ThrowIfNull(this.request.ParentFolderId, "ParentFolderId", "FindPeople::CreateFindPeopleImplementation");
			AddressListId addressListId = this.request.ParentFolderId.BaseFolderId as AddressListId;
			if (addressListId != null)
			{
				MailboxSession mailboxSessionOrFail = this.GetMailboxSessionOrFail();
				return new BrowsePeopleInDirectory(parameters, this.callContext.ADRecipientSessionContext.OrganizationId, FindPeople.GetAddressListId(addressListId), mailboxSessionOrFail);
			}
			IdAndSession folderIdAndSession = this.GetFolderIdAndSession();
			if (folderIdAndSession.Session.IsPublicFolderSession)
			{
				if (this.request.ParentFolderId.BaseFolderId is FolderId || this.request.ParentFolderId.BaseFolderId is DistinguishedFolderId)
				{
					return new BrowsePeopleInPublicFolder(parameters, folderIdAndSession);
				}
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			else
			{
				if (ClientInfo.OWA.IsMatch(folderIdAndSession.Session.ClientInfoString))
				{
					MailboxSession mailboxSessionOrFail2 = this.GetMailboxSessionOrFail();
					StoreId defaultFolderId = mailboxSessionOrFail2.GetDefaultFolderId(DefaultFolderType.FromFavoriteSenders);
					if (defaultFolderId != null && defaultFolderId.Equals(folderIdAndSession.Id))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "FindPeople.CreateFindPeopleImplementation. Calling BrowsePeopleInMailFolder.");
						return new BrowsePeopleInMailFolder(parameters, mailboxSessionOrFail2, folderIdAndSession.Id, this.tracer);
					}
				}
				if (this.request.ParentFolderId.BaseFolderId is FolderId || this.request.ParentFolderId.BaseFolderId is DistinguishedFolderId)
				{
					MailboxSession mailboxSessionOrFail3 = this.GetMailboxSessionOrFail();
					return new BrowsePeopleInMailbox(parameters, mailboxSessionOrFail3, folderIdAndSession);
				}
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
		}

		private FindPeopleImplementation CreateFindPeopleSearchImplemenation(FindPeopleParameters parameters)
		{
			if (this.request.ParentFolderId == null)
			{
				MailboxSession mailboxSessionOrFail = this.GetMailboxSessionOrFail();
				return new SearchPeopleInMailboxAndDirectory(parameters, mailboxSessionOrFail, this.callContext.ADRecipientSessionContext.OrganizationId, mailboxSessionOrFail.GetDefaultFolderId(DefaultFolderType.Contacts), this.GetGlobalAddressListIdOrFail());
			}
			DistinguishedFolderId distinguishedFolderId = this.request.ParentFolderId.BaseFolderId as DistinguishedFolderId;
			if (distinguishedFolderId != null && distinguishedFolderId.Id == DistinguishedFolderIdName.directory)
			{
				MailboxSession mailboxSessionOrFail2 = this.GetMailboxSessionOrFail();
				return new SearchPeopleInDirectory(parameters, this.callContext.ADRecipientSessionContext.OrganizationId, this.GetGlobalAddressListIdOrFail(), mailboxSessionOrFail2, null);
			}
			AddressListId addressListId = this.request.ParentFolderId.BaseFolderId as AddressListId;
			if (addressListId != null)
			{
				MailboxSession mailboxSessionOrFail3 = this.GetMailboxSessionOrFail();
				return new SearchPeopleInDirectory(parameters, this.callContext.ADRecipientSessionContext.OrganizationId, FindPeople.GetAddressListId(addressListId), mailboxSessionOrFail3, null);
			}
			if (!(this.request.ParentFolderId.BaseFolderId is FolderId) && !(this.request.ParentFolderId.BaseFolderId is DistinguishedFolderId))
			{
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			IdAndSession folderIdAndSession = this.GetFolderIdAndSession();
			if (folderIdAndSession.Session.IsPublicFolderSession)
			{
				return new SearchPeopleInPublicFolder(parameters, folderIdAndSession);
			}
			MailboxSession mailboxSessionOrFail4 = this.GetMailboxSessionOrFail();
			return new SearchPeopleInMailbox(parameters, folderIdAndSession, new PeopleAggregationExtension(mailboxSessionOrFail4));
		}

		private MailboxSession GetMailboxSessionOrFail()
		{
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			if (mailboxIdentityMailboxSession == null || mailboxIdentityMailboxSession.MailboxOwner == null)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			return mailboxIdentityMailboxSession;
		}

		private ADObjectId GetGlobalAddressListIdOrFail()
		{
			ADObjectId globalAddressListId = this.GetGlobalAddressListId();
			if (globalAddressListId == null)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			return globalAddressListId;
		}

		private FindPeopleResult CreatePersonaIfQueryStringIsValidAddress()
		{
			string queryString = this.request.QueryString;
			if (string.IsNullOrWhiteSpace(queryString) || !SmtpAddress.IsValidSmtpAddress(queryString))
			{
				return FindPeopleResult.CreateSearchResult(Array<Persona>.Empty);
			}
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "FindPeople:CreatePersonaIfQueryStringIsValidAddress - QueryString is a valid smtp address {0}", queryString);
			EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper
			{
				Name = queryString,
				EmailAddress = queryString,
				RoutingType = "SMTP",
				MailboxType = MailboxHelper.MailboxTypeType.OneOff.ToString()
			};
			Persona persona = new Persona
			{
				PersonaId = IdConverter.PersonaIdFromPersonId(base.MailboxIdentityMailboxSession.MailboxGuid, PersonId.CreateNew()),
				DisplayName = queryString,
				EmailAddress = emailAddressWrapper,
				EmailAddresses = new EmailAddressWrapper[]
				{
					emailAddressWrapper
				},
				PersonaType = FindPeople.PersonaTypePerson
			};
			return FindPeopleResult.CreateSearchResult(new Persona[]
			{
				persona
			});
		}

		private static readonly string FindPeopleActionName = typeof(FindPeople).Name;

		private static readonly string PersonaTypePerson = PersonaTypeConverter.ToString(PersonType.Person);

		private readonly CallContext callContext;

		private readonly FindPeopleRequest request;

		private ITracer tracer = ExTraceGlobals.FindPeopleCallTracer;

		private ITracer requestTracer = NullTracer.Instance;

		private FindPeopleImplementation findPeopleImplementation;
	}
}
