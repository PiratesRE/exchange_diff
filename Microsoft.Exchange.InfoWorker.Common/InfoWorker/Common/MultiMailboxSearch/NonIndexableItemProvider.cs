using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal abstract class NonIndexableItemProvider
	{
		public NonIndexableItemProvider(IRecipientSession recipientSession, ExTimeZone timeZone, CallerInfo callerInfo, OrganizationId orgId, string[] mailboxes, bool searchArchiveOnly)
		{
			Util.ThrowOnNull(recipientSession, "recipientSession");
			Util.ThrowOnNull(timeZone, "timeZone");
			Util.ThrowOnNull(callerInfo, "callerInfo");
			Util.ThrowOnNull(orgId, "orgId");
			Util.ThrowOnNull(mailboxes, "mailboxes");
			this.recipientSession = recipientSession;
			this.timeZone = timeZone;
			this.callerInfo = callerInfo;
			this.orgId = orgId;
			this.mailboxes = mailboxes;
			this.searchArchiveOnly = searchArchiveOnly;
			this.failedMailboxes = new Dictionary<string, string>(1);
			this.alreadyProxy = false;
		}

		internal Dictionary<string, string> FailedMailboxes
		{
			get
			{
				return this.failedMailboxes;
			}
		}

		internal void ExecuteSearch()
		{
			this.InternalExecuteSearch();
		}

		internal static List<ADRawEntry> FindMailboxesInAD(IRecipientSession recipientSession, string[] legacyExchangeDNs, List<string> notFoundMailboxes)
		{
			List<ADRawEntry> list = new List<ADRawEntry>(legacyExchangeDNs.Length);
			Result<ADRawEntry>[] array = recipientSession.FindByLegacyExchangeDNs(legacyExchangeDNs, MailboxInfo.PropertyDefinitionCollection);
			if (array != null && array.Length > 0)
			{
				Dictionary<string, ADRawEntry> dictionary = new Dictionary<string, ADRawEntry>(array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					ADRawEntry data = array[i].Data;
					if (data != null)
					{
						string key = (string)data[ADRecipientSchema.LegacyExchangeDN];
						if (!dictionary.ContainsKey(key))
						{
							dictionary.Add(key, data);
						}
						foreach (string text in legacyExchangeDNs)
						{
							if (!dictionary.ContainsKey(text) && Util.CheckLegacyDnExistInProxyAddresses(text, data))
							{
								dictionary.Add(text, data);
								break;
							}
						}
					}
				}
				foreach (string text2 in legacyExchangeDNs)
				{
					if (dictionary.ContainsKey(text2))
					{
						list.Add(dictionary[text2]);
					}
					else
					{
						notFoundMailboxes.Add(text2);
					}
				}
			}
			else
			{
				notFoundMailboxes.AddRange(legacyExchangeDNs);
			}
			return list;
		}

		protected void AddFailedMailbox(string legacyDn, string error)
		{
			if (this.failedMailboxes.ContainsKey(legacyDn))
			{
				this.failedMailboxes[legacyDn] = error;
				return;
			}
			this.failedMailboxes.Add(legacyDn, error);
		}

		protected void ExecuteSearchWebService()
		{
			this.ewsClient = Factory.Current.CreateNonIndexableDiscoveryEwsClient(this.groupId, new MailboxInfo[]
			{
				this.mailboxInfo
			}, this.timeZone, this.callerInfo);
			this.alreadyProxy = true;
			string legacyExchangeDN = this.mailboxInfo.LegacyExchangeDN;
			Exception exception = null;
			Util.HandleExceptions(delegate
			{
				int num = 3;
				while (num-- > 0)
				{
					try
					{
						ServiceRemoteException exception = null;
						this.InternalExecuteSearchWebService();
						break;
					}
					catch (ServiceResponseException ex)
					{
						ServiceRemoteException exception = ex;
						if (!WebServiceMailboxSearchGroup.TransientServiceErrors.Contains(ex.ErrorCode))
						{
							break;
						}
					}
					catch (ServiceRemoteException exception)
					{
						ServiceRemoteException exception = exception;
					}
					catch (WebServiceProxyInvalidResponseException exception2)
					{
						ServiceRemoteException exception = exception2;
						break;
					}
				}
			}, delegate(GrayException ge)
			{
				exception = new WebServiceProxyInvalidResponseException(Strings.UnexpectedError, ge);
			});
			if (exception != null)
			{
				this.AddFailedMailbox(legacyExchangeDN, exception.Message);
				this.HandleExecuteSearchWebServiceFailed(legacyExchangeDN);
			}
		}

		protected void PerformMailboxDiscovery(ADRawEntry adRawEntry, MailboxType mailboxType, out GroupId groupId, out MailboxInfo mailboxInfo)
		{
			List<MailboxInfo> list = new List<MailboxInfo>(1);
			list.Add(new MailboxInfo(adRawEntry, mailboxType));
			IEwsEndpointDiscovery ewsEndpointDiscovery = Factory.Current.GetEwsEndpointDiscovery(list, this.orgId, this.callerInfo);
			long num = 0L;
			long num2 = 0L;
			Dictionary<GroupId, List<MailboxInfo>> source = ewsEndpointDiscovery.FindEwsEndpoints(out num, out num2);
			KeyValuePair<GroupId, List<MailboxInfo>> keyValuePair = source.First<KeyValuePair<GroupId, List<MailboxInfo>>>();
			groupId = keyValuePair.Key;
			mailboxInfo = keyValuePair.Value[0];
		}

		protected abstract void InternalExecuteSearch();

		protected abstract void InternalExecuteSearchWebService();

		protected abstract void HandleExecuteSearchWebServiceFailed(string legacyDn);

		protected readonly IRecipientSession recipientSession;

		protected readonly ExTimeZone timeZone;

		protected readonly CallerInfo callerInfo;

		protected readonly OrganizationId orgId;

		protected readonly string[] mailboxes;

		protected readonly bool searchArchiveOnly;

		protected readonly Dictionary<string, string> failedMailboxes;

		protected GroupId groupId;

		protected MailboxInfo mailboxInfo;

		protected INonIndexableDiscoveryEwsClient ewsClient;

		protected bool alreadyProxy;
	}
}
