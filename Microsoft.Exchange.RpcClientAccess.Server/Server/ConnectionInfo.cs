using System;
using System.Net;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class ConnectionInfo : WatsonHelper.IProvideWatsonReportData
	{
		public ConnectionInfo(Activity activity, SecurityIdentifier securityIndentifier, string userDn, ConnectionFlags connectionFlags, LocaleInfo localeInfo, MapiVersion clientVersion, IPAddress clientIpAddress, IPAddress serverIpAddress, string protocolSequence, bool isEncrypted, OrganizationId organizationId, string rpcServerTarget, bool isAnonymous, DispatchOptions dispatchOptions)
		{
			this.Activity = activity;
			this.securityIndentifier = securityIndentifier;
			this.UserDn = userDn;
			this.ConnectionFlags = connectionFlags;
			this.LocaleInfo = localeInfo;
			this.ClientVersion = clientVersion;
			this.ClientIpAddress = clientIpAddress;
			this.ServerIpAddress = serverIpAddress;
			this.ProtocolSequence = protocolSequence;
			this.IsEncrypted = isEncrypted;
			this.OrganizationId = organizationId;
			this.RpcServerTarget = rpcServerTarget;
			this.IsAnonymous = isAnonymous;
			this.DispatchOptions = (dispatchOptions ?? new DispatchOptions());
			activity.RegisterWatsonReportDataProvider(WatsonReportActionType.Connection, this);
			ExTraceGlobals.ConnectRpcTracer.TraceDebug(0, Activity.TraceId, "Connecting {0} acting as '{1}' from {2}/{3}", new object[]
			{
				this.securityIndentifier,
				this.UserDn,
				this.ProtocolSequence,
				this.ClientIpAddress
			});
		}

		string WatsonHelper.IProvideWatsonReportData.GetWatsonReportString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendFormat("UserDn: {0}\r\n", this.UserDn);
			stringBuilder.AppendFormat("ClientVersion: v", new object[0]);
			stringBuilder.Append(this.ClientVersion);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("ClientIpAddress: {0}\r\n", this.ClientIpAddress);
			stringBuilder.AppendFormat("Protocol: {0}\r\n", this.ProtocolSequence);
			stringBuilder.AppendFormat("UserSid: {0}\r\n", this.securityIndentifier);
			stringBuilder.Append(this.LocaleInfo);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("RpcServerTarget: {0}\r\n", this.RpcServerTarget);
			stringBuilder.AppendFormat("IsEncrypted: {0}\r\n", this.IsEncrypted);
			stringBuilder.AppendFormat("IsAnonymous: {0}\r\n", this.IsAnonymous);
			return stringBuilder.ToString();
		}

		private readonly SecurityIdentifier securityIndentifier;

		internal readonly Activity Activity;

		internal readonly string UserDn;

		internal readonly ConnectionFlags ConnectionFlags;

		internal readonly LocaleInfo LocaleInfo;

		internal readonly MapiVersion ClientVersion;

		internal readonly IPAddress ClientIpAddress;

		internal readonly IPAddress ServerIpAddress;

		internal readonly string ProtocolSequence;

		internal readonly bool IsEncrypted;

		internal readonly OrganizationId OrganizationId;

		internal readonly string RpcServerTarget;

		internal readonly bool IsAnonymous;

		internal readonly DispatchOptions DispatchOptions;
	}
}
