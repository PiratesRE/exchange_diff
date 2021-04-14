using System;
using System.Collections.Generic;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	public class QueryableLogTransactionInformation
	{
		public QueryableLogTransactionInformation(IEnumerable<ILogTransactionInformation> logTransactionInformationList)
		{
			List<string> list = new List<string>(2);
			this.ClientType = string.Empty;
			this.StoreOperation = string.Empty;
			foreach (ILogTransactionInformation logTransactionInformation in logTransactionInformationList)
			{
				LogTransactionInformationBlockType logTransactionInformationBlockType = (LogTransactionInformationBlockType)logTransactionInformation.Type();
				string item = logTransactionInformationBlockType.ToString();
				list.Add(item);
				switch (logTransactionInformationBlockType)
				{
				case LogTransactionInformationBlockType.Unknown:
				case LogTransactionInformationBlockType.ForTestPurposes:
				case LogTransactionInformationBlockType.Digest:
					break;
				case LogTransactionInformationBlockType.Identity:
				{
					LogTransactionInformationIdentity logTransactionInformationIdentity = (LogTransactionInformationIdentity)logTransactionInformation;
					this.MailboxNumber = logTransactionInformationIdentity.MailboxNumber;
					this.ClientType = logTransactionInformationIdentity.ClientType.ToString();
					break;
				}
				case LogTransactionInformationBlockType.AdminRpc:
				{
					LogTransactionInformationAdmin logTransactionInformationAdmin = (LogTransactionInformationAdmin)logTransactionInformation;
					this.StoreOperation = "AdminRpc." + logTransactionInformationAdmin.MethodId;
					break;
				}
				case LogTransactionInformationBlockType.MapiRpc:
				{
					LogTransactionInformationMapi logTransactionInformationMapi = (LogTransactionInformationMapi)logTransactionInformation;
					this.StoreOperation = "MapiRop." + logTransactionInformationMapi.RopId;
					break;
				}
				case LogTransactionInformationBlockType.Task:
				{
					LogTransactionInformationTask logTransactionInformationTask = (LogTransactionInformationTask)logTransactionInformation;
					this.StoreOperation = "Task." + logTransactionInformationTask.TaskTypeId;
					break;
				}
				default:
					throw new DiagnosticQueryException("Unexpected type");
				}
			}
			this.BlockTypes = list.ToArray();
		}

		public string[] BlockTypes { get; private set; }

		public int MailboxNumber { get; private set; }

		public string ClientType { get; private set; }

		public string StoreOperation { get; private set; }
	}
}
