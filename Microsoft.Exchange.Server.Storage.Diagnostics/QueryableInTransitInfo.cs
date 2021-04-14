using System;
using System.Collections.Generic;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class QueryableInTransitInfo
	{
		public QueryableInTransitInfo(MailboxState state, InTransitStatus inTransitStatus, List<object> logons)
		{
			this.DatabaseGuid = state.DatabaseGuid;
			this.MailboxGuid = state.MailboxGuid;
			this.MailboxNumber = state.MailboxNumber;
			this.MailboxType = state.MailboxType;
			this.Quarantined = state.Quarantined;
			this.Status = state.Status;
			this.TenantHint = state.TenantHint;
			this.InTransitStatus = inTransitStatus.ToString();
			if (logons != null)
			{
				this.ClientInfos = new ClientInfo[logons.Count];
				int num = 0;
				foreach (object obj in logons)
				{
					MapiLogon mapiLogon = obj as MapiLogon;
					if (mapiLogon != null && mapiLogon.Session != null)
					{
						ClientInfo clientInfo = new ClientInfo();
						clientInfo.ApplicationId = mapiLogon.Session.ApplicationId;
						clientInfo.ClientVersion = QueryableInTransitInfo.CreateClientVersion(mapiLogon.Session.ClientVersion);
						clientInfo.ClientMachineName = mapiLogon.Session.ClientMachineName;
						clientInfo.ClientProcessName = mapiLogon.Session.ClientProcessName;
						clientInfo.ConnectTime = mapiLogon.Session.ConnectTime;
						this.ClientInfos[num++] = clientInfo;
					}
				}
			}
		}

		public Guid DatabaseGuid { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public int MailboxNumber { get; private set; }

		public MailboxInfo.MailboxType MailboxType { get; private set; }

		public bool Quarantined { get; private set; }

		public MailboxStatus Status { get; private set; }

		public TenantHint TenantHint { get; private set; }

		public string InTransitStatus { get; private set; }

		public ClientInfo[] ClientInfos { get; private set; }

		public static string CreateClientVersion(Microsoft.Exchange.Protocols.MAPI.Version version)
		{
			return string.Format("{0}.{1:00}.{2:0000}.{3:000}", new object[]
			{
				version.ProductMajor,
				version.ProductMinor,
				version.BuildMajor,
				version.BuildMinor
			});
		}
	}
}
