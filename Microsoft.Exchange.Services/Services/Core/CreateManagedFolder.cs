using System;
using System.Security.Principal;
using System.Xml;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CreateManagedFolder : MultiStepServiceCommand<CreateManagedFolderRequest, BaseFolderType>
	{
		public CreateManagedFolder(CallContext callContext, CreateManagedFolderRequest request) : base(callContext, request)
		{
			this.folderNames = base.Request.FolderNames;
			ServiceCommandBase.ThrowIfNull(this.folderNames, "folderNames", "CreateManagedFolder::Execute");
			this.mailbox = base.Request.Mailbox;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			CreateManagedFolderResponse createManagedFolderResponse = new CreateManagedFolderResponse();
			createManagedFolderResponse.BuildForResults<BaseFolderType>(base.Results);
			return createManagedFolderResponse;
		}

		internal override int StepCount
		{
			get
			{
				return this.folderNames.Length;
			}
		}

		internal override void PreExecuteCommand()
		{
			string text;
			if (this.mailbox == null)
			{
				text = base.CallContext.GetEffectiveAccessingSmtpAddress();
				if (string.IsNullOrEmpty(text))
				{
					throw new MissingEmailAddressForManagedFolderException();
				}
			}
			else
			{
				text = CreateManagedFolder.ExtractEmailAddress(this.mailbox);
				if (string.IsNullOrEmpty(text))
				{
					throw new NonExistentMailboxException((CoreResources.IDs)4088802584U, string.Empty);
				}
			}
			ExchangePrincipal fromCache = ExchangePrincipalCache.GetFromCache(text, base.CallContext.ADRecipientSessionContext);
			RecipientIdentity recipientIdentity;
			if (!ADIdentityInformationCache.Singleton.TryGetRecipientIdentity(base.CallContext.EffectiveCallerSid, base.CallContext.ADRecipientSessionContext, out recipientIdentity))
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<string, SecurityIdentifier>(0L, "[CreatemanageFolder:CreateManagedFolders] In {0} access mode, we found that the caller needed delegate access to a mailbox, but the caller is actually a computer account and not a recipient and therefore cannot possibly have delegate rights to your mailbox.  Caller sid: {1}", (base.CallContext.MailboxAccessType == MailboxAccessType.Normal) ? "normal" : "S2S", base.CallContext.EffectiveCallerSid);
				throw new ServiceAccessDeniedException();
			}
			IADOrgPerson iadorgPerson = recipientIdentity.Recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<string, SecurityIdentifier>(0L, "[CreatemanageFolder:CreateManagedFolders] In {0} access mode, we found that the caller needed delegate access to a mailbox, but searching for the caller by sid returned no records.  Possibly a cross forest trust with no cross-forest contact to speak of.  Caller sid: {1}", (base.CallContext.MailboxAccessType == MailboxAccessType.Normal) ? "normal" : "S2S", base.CallContext.EffectiveCallerSid);
				throw new ServiceAccessDeniedException();
			}
			OptInFolders optInFolders = new OptInFolders(fromCache, iadorgPerson.LegacyExchangeDN, base.CallContext.EffectiveCaller.ClientSecurityContext, base.CallerBudget);
			this.idAndSession = base.IdConverter.ConvertDefaultFolderType(DefaultFolderType.Root, text);
			this.ids = optInFolders.CreateOrganizationalFolders(this.folderNames);
		}

		internal override ServiceResult<BaseFolderType> Execute()
		{
			ServiceResult<BaseFolderType> result;
			using (Folder folder = Folder.Bind(this.idAndSession.Session, this.ids[base.CurrentStep]))
			{
				BaseFolderType baseFolderType = new FolderType();
				base.LoadServiceObject(baseFolderType, folder, this.idAndSession, ServiceCommandBase.DefaultFolderResponseShape);
				result = new ServiceResult<BaseFolderType>(baseFolderType);
			}
			return result;
		}

		private static string ExtractEmailAddress(XmlElement mailboxElement)
		{
			string result = null;
			XmlNodeList xmlNodeList = mailboxElement.SelectNodes("t:EmailAddress[position() = 1]", ServiceXml.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				XmlElement textNodeParent = (XmlElement)xmlNodeList[0];
				result = ServiceXml.GetXmlTextNodeValue(textNodeParent);
			}
			return result;
		}

		private string[] folderNames;

		private XmlElement mailbox;

		private VersionedId[] ids;

		private IdAndSession idAndSession;
	}
}
