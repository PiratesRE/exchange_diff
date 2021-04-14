using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public abstract class UMVersionedRpcRequest
	{
		public UMVersionedRpcRequest()
		{
			this.version = this.CurrentVersion;
		}

		public Version Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		internal virtual Version CurrentVersion
		{
			get
			{
				return UMVersionedRpcRequest.Version10;
			}
		}

		internal static UMRpcResponse ExecuteRequest(UMVersionedRpcRequest request)
		{
			UMRpcResponse result = null;
			try
			{
				if (request.Version == null || request.Version.Major != request.CurrentVersion.Major || request.Version.Minor > request.CurrentVersion.Minor)
				{
					throw new UMRPCIncompabibleVersionException();
				}
				result = request.Execute();
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "{0}.ExecuteRequest: {1}", new object[]
				{
					request,
					ex
				});
				request.LogErrorEvent(ex);
				UMRPCComponentBase.HandleException(ex);
				throw new UMRpcException(ex);
			}
			return result;
		}

		internal abstract UMRpcResponse Execute();

		internal abstract string GetFriendlyName();

		protected abstract void LogErrorEvent(Exception ex);

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type:{0}({1}) RequestVersion:{2} CurrentVersion:{3} HashCode:{4}", new object[]
			{
				base.GetType().Name,
				this.GetFriendlyName(),
				this.Version,
				this.CurrentVersion,
				this.GetHashCode()
			});
		}

		public static readonly Version Version10 = new Version(1, 0);

		private Version version;
	}
}
