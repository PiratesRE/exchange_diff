using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.XropService;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class WebServiceCallStateConnect : WebServiceCallState<ConnectRequest, ConnectResponse>
	{
		public WebServiceCallStateConnect(WebServiceUserInformation userInformation, IExchangeAsyncDispatch exchangeAsyncDispatch, AsyncCallback asyncCallback, object asyncState, IntPtr contextHandle, WindowsIdentity userIdentity) : base(userInformation, exchangeAsyncDispatch, asyncCallback, asyncState)
		{
			this.contextHandle = contextHandle;
			this.userIdentity = userIdentity;
		}

		internal IntPtr ContextHandle
		{
			get
			{
				return this.contextHandle;
			}
		}

		protected override string Name
		{
			get
			{
				return "Connect";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ConnectXropTracer;
			}
		}

		protected override void InternalBegin(ConnectRequest request)
		{
			if (request == null)
			{
				throw new ServerInvalidArgumentException("request", null);
			}
			this.icxrLink = request.IcxrLink;
			this.timeStamp = request.TimeStamp;
			this.clientVersion = request.ClientVersion;
			if (this.contextHandle != IntPtr.Zero)
			{
				throw new ServerInvalidBindingException(string.Format("Connect being called when we already have a stored context handle; stored={0}.", (uint)this.contextHandle.ToInt64()), null);
			}
			if (string.IsNullOrEmpty(request.UserDN))
			{
				throw new ServerInvalidArgumentException("request.UserDN", null);
			}
			if (request.ClientVersion == null || request.ClientVersion.Length != 6)
			{
				throw new ServerInvalidArgumentException("request.ClientVersion", null);
			}
			ArraySegment<byte> segmentExtendedAuxIn = WebServiceCall.BuildRequestSegment(request.AuxIn);
			ArraySegment<byte> responseAuxSegment = WebServiceCall.GetResponseAuxSegment((int)request.AuxOutMaxSize, out this.auxOut);
			short[] array = WebServiceCallStateConnect.NormalizeVersion(request.ClientVersion);
			string text = request.UserDN;
			if (LegacyDnHelper.IsFederatedSystemAttendant(text))
			{
				if (!LegacyDnHelper.IsValidClientFederatedSystemAttendant(text))
				{
					throw new UnknownUserException("Federated System Attendant isn't valid");
				}
				if ((request.Flags & 1U) == 0U)
				{
					throw new InvalidParameterException("Cannot use federated system attendant without UseAdminPrivilege");
				}
				text = string.Format("{0}/cn={1}", text, base.UserInformation.Domain);
			}
			string protocolSequence;
			if (string.IsNullOrEmpty(base.UserInformation.EmailAddress))
			{
				protocolSequence = RpcDispatch.WebServiceProtocolSequencePrefix + base.UserInformation.Domain;
			}
			else
			{
				protocolSequence = string.Format("{0}{1}[{0}{2}]", RpcDispatch.WebServiceProtocolSequencePrefix, base.UserInformation.Domain, base.UserInformation.EmailAddress);
			}
			this.webServiceClientBinding = new WebServiceClientBinding(protocolSequence, this.userIdentity);
			base.ExchangeAsyncDispatch.BeginConnect(null, this.webServiceClientBinding, text, (int)request.Flags, (int)request.ConMod, (int)request.Cpid, (int)request.LcidString, (int)request.LcidSort, array, segmentExtendedAuxIn, responseAuxSegment, new CancelableAsyncCallback(base.Complete), this);
		}

		protected override void InternalBeginCleanup(bool isSuccessful)
		{
			if (!isSuccessful && this.auxOut != null)
			{
				WebServiceCall.ReleaseBuffer(this.auxOut);
				this.auxOut = null;
			}
		}

		protected override ConnectResponse InternalEnd(ICancelableAsyncResult asyncResult)
		{
			ConnectResponse connectResponse = new ConnectResponse();
			TimeSpan timeSpan;
			int retry;
			TimeSpan timeSpan2;
			string dnprefix;
			string displayName;
			short[] version;
			ArraySegment<byte> segment;
			connectResponse.ErrorCode = (uint)base.ExchangeAsyncDispatch.EndConnect(asyncResult, out this.contextHandle, out timeSpan, out retry, out timeSpan2, out dnprefix, out displayName, out version, out segment);
			if (connectResponse.ErrorCode == 0U)
			{
				connectResponse.Context = (uint)this.contextHandle.ToInt64();
				if (this.icxrLink == 4294967295U)
				{
					connectResponse.TimeStamp = (uint)ExDateTime.Now.ToFileTimeUtc();
				}
				else
				{
					connectResponse.TimeStamp = this.timeStamp;
				}
				connectResponse.Icxr = (ushort)(this.contextHandle.ToInt64() & 65535L);
				connectResponse.PollsMax = (uint)timeSpan.Milliseconds;
				connectResponse.Retry = (uint)retry;
				connectResponse.RetryDelay = (uint)timeSpan2.Milliseconds;
				connectResponse.DNPrefix = dnprefix;
				connectResponse.DisplayName = displayName;
			}
			else if (this.contextHandle != IntPtr.Zero)
			{
				throw new ArgumentException("contextHandle should be zero");
			}
			connectResponse.ServerVersion = WebServiceCallStateConnect.LegacyVersion(version);
			connectResponse.BestVersion = this.clientVersion;
			connectResponse.AuxOut = WebServiceCall.BuildResponseArray(segment);
			return connectResponse;
		}

		protected override void InternalEndCleanup()
		{
			if (this.auxOut != null)
			{
				WebServiceCall.ReleaseBuffer(this.auxOut);
				this.auxOut = null;
			}
		}

		protected override ConnectResponse InternalFailure(uint serviceCode)
		{
			return new ConnectResponse
			{
				ServiceCode = serviceCode
			};
		}

		private static short[] NormalizeVersion(byte[] version)
		{
			if (version == null || version.Length != 6)
			{
				throw new ArgumentException("version");
			}
			short[] array = new short[4];
			if ((version[3] & 128) != 0)
			{
				array[0] = (short)version[0];
				array[1] = (short)version[1];
				array[2] = (short)((int)version[2] + (int)version[3] * 256 & 32767);
				array[3] = (short)((int)version[4] + (int)version[5] * 256);
			}
			else
			{
				array[0] = (short)((int)version[0] + (int)version[1] * 256);
				array[1] = 0;
				array[2] = (short)((int)version[2] + (int)version[3] * 256);
				array[3] = (short)((int)version[4] + (int)version[5] * 256);
			}
			return array;
		}

		private static byte[] LegacyVersion(short[] version)
		{
			if (version == null || version.Length != 4)
			{
				throw new ArgumentException("version");
			}
			ushort num = (ushort)((int)((ushort)version[0]) << 8 | (int)((ushort)version[1]));
			ushort num2 = (ushort)version[2] | 32768;
			ushort num3 = (ushort)version[3] + 4000;
			return new byte[]
			{
				(byte)(num & 255),
				(byte)(num >> 8 & 255),
				(byte)(num2 & 255),
				(byte)(num2 >> 8 & 255),
				(byte)(num3 & 255),
				(byte)(num3 >> 8 & 255)
			};
		}

		private readonly WindowsIdentity userIdentity;

		private IntPtr contextHandle;

		private uint icxrLink;

		private uint timeStamp;

		private byte[] clientVersion;

		private byte[] auxOut;

		private WebServiceClientBinding webServiceClientBinding;
	}
}
