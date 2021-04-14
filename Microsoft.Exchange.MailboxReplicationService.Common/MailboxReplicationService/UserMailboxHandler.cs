using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class UserMailboxHandler<T> : RequestIndexEntryHandler<T> where T : class, IRequestIndexEntry, IAggregatedAccountConfigurationWrapper, new()
	{
		public override T Read(RequestIndexEntryProvider requestIndexEntryProvider, RequestIndexEntryObjectId identity)
		{
			ArgumentValidator.ThrowIfNull("targetExchangeGuid", identity.TargetExchangeGuid);
			ADRecipient adrecipient = requestIndexEntryProvider.Read<ADRecipient>((IRecipientSession session) => session.FindByExchangeGuidIncludingAlternate(identity.TargetExchangeGuid));
			if (adrecipient == null)
			{
				MrsTracer.Common.Warning("No ADRecipient found with ExchangeGuid '{0}' including alternates.", new object[]
				{
					identity.TargetExchangeGuid
				});
				return default(T);
			}
			ADUser aduser = adrecipient as ADUser;
			if (aduser == null)
			{
				MrsTracer.Common.Warning("'{0}' is not a user.", new object[]
				{
					adrecipient.Id.ToString()
				});
				return default(T);
			}
			T t = Activator.CreateInstance<T>();
			t.TargetUser = aduser;
			t.TargetExchangeGuid = identity.TargetExchangeGuid;
			t.TargetMDB = aduser.Database;
			T result = t;
			result.RequestGuid = identity.RequestGuid;
			result.SetExchangePrincipal();
			return result;
		}

		public override void Delete(RequestIndexEntryProvider requestIndexEntryProvider, T instance)
		{
			ArgumentValidator.ThrowIfNull("instance", instance);
			ArgumentValidator.ThrowIfNull("instance.TargetUser", instance.TargetUser);
			ArgumentValidator.ThrowIfNull("instance.TargetExchangeGuid", instance.TargetExchangeGuid);
			instance.SetExchangePrincipal();
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(instance.GetExchangePrincipal(), CultureInfo.InvariantCulture, "Client=MSExchangeMigration"))
			{
				instance.Delete(new MailboxStoreTypeProvider(instance.TargetUser)
				{
					MailboxSession = mailboxSession
				});
			}
		}

		public override void Save(RequestIndexEntryProvider requestIndexEntryProvider, T instance)
		{
			ArgumentValidator.ThrowIfNull("instance", instance);
			ArgumentValidator.ThrowIfNull("instance.TargetUser", instance.TargetUser);
			ArgumentValidator.ThrowIfNull("instance.TargetExchangeGuid", instance.TargetExchangeGuid);
			instance.SetExchangePrincipal();
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(instance.GetExchangePrincipal(), CultureInfo.InvariantCulture, "Client=MSExchangeMigration"))
			{
				instance.Save(new MailboxStoreTypeProvider(instance.TargetUser)
				{
					MailboxSession = mailboxSession
				});
			}
		}

		public override T CreateRequestIndexEntryFromRequestJob(RequestJobBase requestJob, RequestIndexId requestIndexId)
		{
			ArgumentValidator.ThrowIfNull("requestJob", requestJob);
			ArgumentValidator.ThrowIfNull("requestJob.TargetUser", requestJob.TargetUser);
			ArgumentValidator.ThrowIfNull("requestJob.TargetExchangeGuid", requestJob.TargetExchangeGuid);
			ArgumentValidator.ThrowIfInvalidValue<RequestJobBase>("requestJob.RequestType", requestJob, (RequestJobBase x) => x.RequestType == MRSRequestType.Sync);
			T result = Activator.CreateInstance<T>();
			result.UpdateData(requestJob);
			return result;
		}
	}
}
