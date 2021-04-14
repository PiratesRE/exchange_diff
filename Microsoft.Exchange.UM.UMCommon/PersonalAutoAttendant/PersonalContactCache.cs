using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class PersonalContactCache : IPersonalContactCache
	{
		private PersonalContactCache(UMSubscriber subscriber)
		{
			this.subscriber = subscriber;
			this.contactItemCache = new Dictionary<StoreObjectId, PersonalContactInfo>();
		}

		public static PersonalContactCache Create(UMSubscriber subscriber)
		{
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, subscriber.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, null, data, "PersonalContactCache::Create(_DisplayName)", new object[0]);
			return new PersonalContactCache(subscriber);
		}

		public void BuildCache()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalContactCache::BuildCache()", new object[0]);
			LatencyDetectionContext latencyDetectionContext = PAAUtils.BuildContactCacheFactory.CreateContext(CommonConstants.ApplicationVersion, CallId.Id ?? string.Empty, new IPerformanceDataProvider[]
			{
				RpcDataProvider.Instance,
				PerformanceContext.Current
			});
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.subscriber.CreateSessionLock())
			{
				using (ContactsFolder contactsFolder = ContactsFolder.Bind(mailboxSessionLock.Session, mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Contacts)))
				{
					using (QueryResult queryResult = contactsFolder.ItemQuery(ItemQueryType.None, null, null, PersonalContactInfo.ContactPhonePropertyDefinitions))
					{
						object[][] rows = queryResult.GetRows(100);
						while (rows != null && rows.Length > 0)
						{
							foreach (object[] result in rows)
							{
								PersonalContactInfo personalContactInfo = new PersonalContactInfo(mailboxSessionLock.Session.MailboxOwner.MailboxInfo.MailboxGuid, result, FoundByType.NotSpecified, true);
								if (this.contactItemCache == null)
								{
									this.contactItemCache = new Dictionary<StoreObjectId, PersonalContactInfo>();
								}
								StoreObjectId key = StoreObjectId.Deserialize(personalContactInfo.Id);
								this.contactItemCache[key] = personalContactInfo;
							}
							rows = queryResult.GetRows(100);
						}
					}
				}
			}
			TaskPerformanceData[] array = latencyDetectionContext.StopAndFinalizeCollection();
			TaskPerformanceData taskPerformanceData = array[0];
			PerformanceData end = taskPerformanceData.End;
			if (end != PerformanceData.Zero)
			{
				PerformanceData difference = taskPerformanceData.Difference;
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() PersonalContactCache::BuildCache() [RPCRequests = {0} RPCLatency = {1}", new object[]
				{
					difference.Count,
					difference.Milliseconds
				});
			}
		}

		public bool IsContactValid(StoreObjectId contactId)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalContactCache::IsContactValid()", new object[0]);
			PersonalContactInfo personalContactInfo = null;
			return this.contactItemCache.TryGetValue(contactId, out personalContactInfo);
		}

		public PersonalContactInfo GetContact(StoreObjectId contactId)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalContactCache::GetContact()", new object[0]);
			PersonalContactInfo result = null;
			this.contactItemCache.TryGetValue(contactId, out result);
			return result;
		}

		private UMSubscriber subscriber;

		private Dictionary<StoreObjectId, PersonalContactInfo> contactItemCache;
	}
}
