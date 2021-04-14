using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExRpcConnectionFactory : IExRpcConnectionFactory
	{
		public ExRpcConnectionFactory(ICrossServerConnectionPolicy crossServerPolicy)
		{
			if (crossServerPolicy == null)
			{
				throw new ArgumentNullException("crossServerPolicy");
			}
			this.crossServerPolicy = crossServerPolicy;
		}

		public ExRpcConnection Create(ExRpcConnectionInfo connectionInfo)
		{
			this.crossServerPolicy.Apply(connectionInfo);
			ExRpcModule.Bind();
			ExRpcConnection result = null;
			WebServiceConnection webServiceConnection = null;
			IExRpcConnectionInterface exRpcConnectionInterface = null;
			ConnectFlag ulConnectFlags = connectionInfo.ConnectFlags | ConnectFlag.NoUnderCoverConnection;
			ExRpcConnectionCreateFlag exRpcConnectionCreateFlag = connectionInfo.CreateFlags;
			try
			{
				if ((exRpcConnectionCreateFlag & ExRpcConnectionCreateFlag.WebServices) != ExRpcConnectionCreateFlag.None)
				{
					webServiceConnection = new WebServiceConnection(connectionInfo.XropClient);
					exRpcConnectionCreateFlag &= ~(ExRpcConnectionCreateFlag.XorMagicUp | ExRpcConnectionCreateFlag.XorMagicDown);
				}
				try
				{
					IExRpcManageInterface exRpcManageInterface = null;
					try
					{
						exRpcManageInterface = ExRpcModule.GetExRpcManage();
						int num = exRpcManageInterface.ConnectEx2((int)exRpcConnectionCreateFlag, (int)ulConnectFlags, connectionInfo.ServerDn, (connectionInfo.MdbGuid != Guid.Empty) ? connectionInfo.MdbGuid.ToByteArray() : null, connectionInfo.UserDn, connectionInfo.UserName, connectionInfo.Domain, connectionInfo.Password, connectionInfo.HttpProxyServerName, connectionInfo.ConnectionModulation, connectionInfo.LocaleIdForReturnedString, connectionInfo.LocaleIdForSort, connectionInfo.CodePageId, connectionInfo.ReconnectIntervalInMins, connectionInfo.RpcBufferSize, connectionInfo.AuxBufferSize, (connectionInfo.ClientSessionInfo == null) ? 0 : connectionInfo.ClientSessionInfo.Length, connectionInfo.ClientSessionInfo, (webServiceConnection != null) ? webServiceConnection.NativeConnectDelegate : IntPtr.Zero, (webServiceConnection != null) ? webServiceConnection.NativeExecuteDelegate : IntPtr.Zero, (webServiceConnection != null) ? webServiceConnection.NativeDisconnectDelegate : IntPtr.Zero, (int)connectionInfo.ConnectionTimeout.TotalMilliseconds, (int)connectionInfo.CallTimeout.TotalMilliseconds, out exRpcConnectionInterface);
						if (num != 0)
						{
							MapiExceptionHelper.ThrowIfError("Unable to make connection to the server.", num, (SafeExInterfaceHandle)exRpcManageInterface, (webServiceConnection != null) ? webServiceConnection.LastException : null);
						}
					}
					finally
					{
						exRpcManageInterface.DisposeIfValid();
					}
					result = new ExRpcConnection(exRpcConnectionInterface, webServiceConnection, connectionInfo.IsCrossServer, connectionInfo.CallTimeout);
					exRpcConnectionInterface = null;
					webServiceConnection = null;
				}
				finally
				{
					exRpcConnectionInterface.DisposeIfValid();
				}
			}
			finally
			{
				if (webServiceConnection != null)
				{
					webServiceConnection.Dispose();
				}
			}
			return result;
		}

		private ICrossServerConnectionPolicy crossServerPolicy;
	}
}
