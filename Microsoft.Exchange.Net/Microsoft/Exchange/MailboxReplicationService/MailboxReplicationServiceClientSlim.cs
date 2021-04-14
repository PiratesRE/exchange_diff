using System;
using System.Collections.Generic;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Diagnostics.Components.MailboxReplicationService;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class MailboxReplicationServiceClientSlim : WcfClientBase<IMailboxReplicationServiceSlim>
	{
		private MailboxReplicationServiceClientSlim(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public static void NotifyToSync(SyncNowNotificationFlags flags, Guid mailboxGuid, Guid mdbGuid)
		{
			try
			{
				using (MailboxReplicationServiceClientSlim mailboxReplicationServiceClientSlim = MailboxReplicationServiceClientSlim.Create())
				{
					SyncNowNotification item = new SyncNowNotification
					{
						MailboxGuid = mailboxGuid,
						MdbGuid = mdbGuid,
						Flags = (int)flags
					};
					mailboxReplicationServiceClientSlim.SyncNow(new List<SyncNowNotification>
					{
						item
					});
				}
			}
			catch (EndpointNotFoundTransientException arg)
			{
				ExTraceGlobals.MailboxReplicationCommonTracer.TraceWarning<EndpointNotFoundTransientException>(0L, "MRSClientSlim: MRS service is down: {0}", arg);
			}
		}

		internal static MailboxReplicationServiceClientSlim Create()
		{
			ExTraceGlobals.MailboxReplicationCommonTracer.TraceDebug<string>(0L, "MRSClientSlim: attempting to connect to local server '{0}'", ComputerInformation.DnsFullyQualifiedDomainName);
			string text = "net.pipe://localhost/Microsoft.Exchange.MailboxReplicationService";
			NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
			netNamedPipeBinding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
			EndpointAddress remoteAddress = new EndpointAddress(text);
			MailboxReplicationServiceClientSlim result = new MailboxReplicationServiceClientSlim(netNamedPipeBinding, remoteAddress);
			ExTraceGlobals.MailboxReplicationCommonTracer.TraceDebug<string>(0L, "MRSClientSlim: connected to '{0}'", text);
			return result;
		}

		internal void SyncNow(List<SyncNowNotification> notifications)
		{
			this.CallService(delegate()
			{
				this.Channel.SyncNow(notifications);
			});
		}

		private void CallService(Action serviceCall)
		{
			ExTraceGlobals.MailboxReplicationCommonTracer.TraceDebug<string>(0L, "MRSClientSlim: attempting to call: {0}", serviceCall.Method.Name);
			this.CallService(serviceCall, base.Endpoint.Address.ToString());
			ExTraceGlobals.MailboxReplicationCommonTracer.TraceDebug<string>(0L, "MRSClientSlim: completed the call: {0}", serviceCall.Method.Name);
		}
	}
}
