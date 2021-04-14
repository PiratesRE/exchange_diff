using System;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Rpc.MultiMailboxSearch;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ediscovery.Probes
{
	public class EdiscoveryProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				this.InternalDoWork();
			}
			catch (Exception ex)
			{
				if (!(ex.GetBaseException() is MapiExceptionIllegalCrossServerConnection) && !(ex is WrongServerException))
				{
					throw;
				}
				ProbeResult result = base.Result;
				result.FailureContext += "\r\nPassive Failure Case, the MDB was offline\r\n";
			}
		}

		internal void InternalDoWork()
		{
			Guid guid = new Guid(base.Definition.Attributes["MailboxDatabaseGuid"]);
			Guid mailboxGuid = new Guid(base.Definition.Attributes["MailboxGuid"]);
			string mailboxDatabaseServerFQDN = base.Definition.Attributes["MailboxDatabaseServerFQDN"];
			using (ExRpcAdmin exRpcAdmin = EdiscoveryProbeHelper.CreateRPCAdminConnection(mailboxDatabaseServerFQDN, guid))
			{
				byte[] searchRequest = MultiMailboxKeywordStatsRequest.Serialize(new MultiMailboxKeywordStatsRequest
				{
					MailboxInfos = new MultiMailboxSearchMailboxInfo[]
					{
						new MultiMailboxSearchMailboxInfo(mailboxGuid)
					},
					Keywords = EdiscoveryProbeHelper.ConvertQueryFilterToRestriction("{fc697f5b-7943-4f20-a6da-47380bf6cd97}+{768d984f-6fe4-4b79-8b5d-7dc52ee0ca62}", exRpcAdmin, base.Definition.Account)
				});
				byte[] multiMailboxSearchKeywordStats = exRpcAdmin.GetMultiMailboxSearchKeywordStats(guid, searchRequest);
				MultiMailboxKeywordStatsResult[] array;
				if (!EdiscoveryProbeHelper.IsSearchResponseValid(multiMailboxSearchKeywordStats, out array))
				{
					if (array == null)
					{
						throw new Exception(Strings.NullSearchResponseExceptionMessage);
					}
					throw new Exception(string.Format(Strings.InvalidSearchResultsExceptionMessage, array[0].Count));
				}
			}
		}
	}
}
