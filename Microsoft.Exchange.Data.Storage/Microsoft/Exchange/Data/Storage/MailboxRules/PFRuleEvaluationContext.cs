using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class PFRuleEvaluationContext : RuleEvaluationContextBase
	{
		static PFRuleEvaluationContext()
		{
			string fqdn = LocalServer.GetServer().Fqdn;
			PFRuleEvaluationContext.localServerFqdn = fqdn;
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
				PFRuleEvaluationContext.localServerNetworkAddress = hostEntry.AddressList[0];
			}
			catch (SocketException ex)
			{
				ExTraceGlobals.SessionTracer.TraceError<string>(0L, "Start failed: {0}", ex.ToString());
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_PFRuleConfigGetLocalIPFailure, null, new object[]
				{
					ex
				});
				throw new InvalidRuleException(ex.Message, ex);
			}
		}

		protected PFRuleEvaluationContext(Folder folder, ICoreItem message, StoreSession session, ProxyAddress recipient, IADRecipientCache recipientCache, long mimeSize) : base(folder, null, session, recipient, recipientCache, mimeSize, PFRuleConfig.Instance, ExTraceGlobals.SessionTracer)
		{
			AcrPropertyBag acrPropertyBag = message.PropertyBag as AcrPropertyBag;
			if (acrPropertyBag != null)
			{
				this.messageContextStoreObject = acrPropertyBag.Context.StoreObject;
			}
			base.Message = new MessageItem(message, false);
			this.ruleHistory = base.Message.GetRuleHistory();
			base.LimitChecker = new LimitChecker(this);
			this.ruleConfig = PFRuleConfig.Instance;
		}

		protected PFRuleEvaluationContext(PFRuleEvaluationContext parentContext) : base(parentContext)
		{
		}

		public override string DefaultDomainName
		{
			get
			{
				if (this.defaultDomainName == null)
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 189, "DefaultDomainName", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Rules\\MailboxRules\\PFRuleEvaluationContext.cs");
					this.defaultDomainName = tenantOrTopologyConfigurationSession.GetDefaultAcceptedDomain().DomainName.Domain;
				}
				return this.defaultDomainName;
			}
		}

		public override List<KeyValuePair<string, string>> ExtraTrackingEventData
		{
			get
			{
				if (this.extraTrackingEventData == null)
				{
					this.extraTrackingEventData = new List<KeyValuePair<string, string>>();
				}
				return this.extraTrackingEventData;
			}
		}

		public override IsMemberOfResolver<string> IsMemberOfResolver
		{
			get
			{
				return this.ruleConfig.IsMemberOfResolver;
			}
		}

		public override string LocalServerFqdn
		{
			get
			{
				return PFRuleEvaluationContext.localServerFqdn;
			}
		}

		public override IPAddress LocalServerNetworkAddress
		{
			get
			{
				return PFRuleEvaluationContext.localServerNetworkAddress;
			}
		}

		public override ExEventLog.EventTuple OofHistoryCorruption
		{
			get
			{
				throw new InvalidOperationException("Access of OofHistoryCorruption property is invalid for public folder rules.");
			}
		}

		public override ExEventLog.EventTuple OofHistoryFolderMissing
		{
			get
			{
				throw new InvalidOperationException("Access of OofHistoryFolderMissing property is invalid for public folder rules.");
			}
		}

		public static PFRuleEvaluationContext Create(StoreObjectId folderId, ProxyAddress recipientProxyAddress, ICoreItem message, long mimeSize, PublicFolderSession session)
		{
			PFRuleEvaluationContext result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Folder folder = Folder.Bind(session, folderId, RuleEvaluationContextBase.AdditionalFolderProperties);
				disposeGuard.Add<Folder>(folder);
				PFMessageContext pfmessageContext = new PFMessageContext(folder, message, session, recipientProxyAddress ?? ProxyAddress.Parse(ProxyAddressPrefix.Smtp.PrimaryPrefix, session.MailboxPrincipal.MailboxInfo.PrimarySmtpAddress.ToString()), new ADRecipientCache<ADRawEntry>(PFRuleEvaluationContext.RecipientProperties, 0, session.MailboxPrincipal.MailboxInfo.OrganizationId), mimeSize);
				disposeGuard.Add<PFMessageContext>(pfmessageContext);
				pfmessageContext.traceFormatter = new TraceFormatter(false);
				session.ProhibitFolderRuleEvaluation = true;
				disposeGuard.Success();
				result = pfmessageContext;
			}
			return result;
		}

		public override MessageItem CreateMessageItem(PropertyDefinition[] prefetchProperties)
		{
			PublicFolderSession publicFolderSession = base.StoreSession as PublicFolderSession;
			MessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MessageItem messageItem = MessageItem.Create(publicFolderSession, publicFolderSession.GetInternalSubmissionFolderId());
				disposeGuard.Add<MessageItem>(messageItem);
				messageItem.Load(prefetchProperties);
				messageItem.SaveFlags |= PropertyBagSaveFlags.IgnoreAccessDeniedErrors;
				disposeGuard.Success();
				result = messageItem;
			}
			return result;
		}

		public override ISubmissionItem GenerateSubmissionItem(MessageItem item, WorkItem workItem)
		{
			return new PFSubmissionItem(this, item);
		}

		public override Folder GetDeletedItemsFolder()
		{
			throw new InvalidOperationException("Calling GetDeletedItemsFolder is invalid for public folder rules.");
		}

		public override void SetMailboxOwnerAsSender(MessageItem message)
		{
			PublicFolderSession publicFolderSession = base.StoreSession as PublicFolderSession;
			if (base.CurrentFolder.PropertyBag.GetValueOrDefault<bool>(FolderSchema.MailEnabled))
			{
				Exception ex = null;
				try
				{
					byte[] valueOrDefault = base.CurrentFolder.PropertyBag.GetValueOrDefault<byte[]>(FolderSchema.ProxyGuid);
					if (valueOrDefault != null && valueOrDefault.Length == 16)
					{
						IRecipientSession adrecipientSession = publicFolderSession.GetADRecipientSession(true, ConsistencyMode.PartiallyConsistent);
						ADRawEntry adrawEntry = adrecipientSession.Read(new ADObjectId(valueOrDefault)) as ADPublicFolder;
						if (adrawEntry != null)
						{
							message.From = new Participant(adrawEntry);
							return;
						}
					}
					ex = new ObjectNotFoundException(ServerStrings.ExItemNotFound);
				}
				catch (ADTransientException ex2)
				{
					ex = ex2;
				}
				catch (ADExternalException ex3)
				{
					ex = ex3;
				}
				catch (ADOperationException ex4)
				{
					ex = ex4;
				}
				catch (DataValidationException ex5)
				{
					ex = ex5;
				}
				catch (ObjectNotFoundException ex6)
				{
					ex = ex6;
				}
				if (ex != null)
				{
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_PFRuleSettingFromAddressFailure, base.CurrentFolder.StoreObjectId.ToHexEntryId(), new object[]
					{
						ex
					});
				}
			}
			message.From = (publicFolderSession.ConnectAsParticipant ?? new Participant(publicFolderSession.MailboxPrincipal));
		}

		public override void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			StorageGlobals.EventLogger.LogEvent(tuple, periodicKey, messageArgs);
		}

		public override ExTimeZone DetermineRecipientTimeZone()
		{
			if (this.timeZoneRetrieved)
			{
				base.TraceDebug<ExTimeZone>("TimeZone retrieved before, returning it. TimeZone: {0}", this.timeZone);
				return this.timeZone;
			}
			this.timeZoneRetrieved = true;
			this.timeZone = ExTimeZone.CurrentTimeZone;
			base.TraceDebug<Type, ExTimeZone>("Session is not MailboxSession, using server time zone instead. SessionType: {0}, TimeZone: {1}", base.StoreSession.GetType(), this.timeZone);
			return this.timeZone;
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
			((PublicFolderSession)base.StoreSession).ProhibitFolderRuleEvaluation = false;
			if (base.Message != null)
			{
				if (this.messageContextStoreObject != null)
				{
					base.Message.PropertyBag.Context.StoreObject = this.messageContextStoreObject;
				}
				base.Message.CoreObject = null;
				base.Message.Dispose();
			}
		}

		private static readonly ADPropertyDefinition[] RecipientProperties = new ADPropertyDefinition[]
		{
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.ThrottlingPolicy,
			ADRecipientSchema.SCLJunkEnabled,
			ADRecipientSchema.SCLJunkThreshold,
			IADMailStorageSchema.ExchangeGuid,
			IADMailStorageSchema.RulesQuota
		};

		private static string localServerFqdn;

		private static IPAddress localServerNetworkAddress;

		private List<KeyValuePair<string, string>> extraTrackingEventData;

		private string defaultDomainName;

		private PFRuleConfig ruleConfig;

		private bool timeZoneRetrieved;

		private ExTimeZone timeZone;

		private StoreObject messageContextStoreObject;
	}
}
