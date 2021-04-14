using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.NspiClient;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NspiClient : BaseRpcClient<NspiAsyncRpcClient>, INspiClient, IRpcClient, IDisposable
	{
		public NspiClient(RpcBindingInfo bindingInfo) : base(new NspiAsyncRpcClient(bindingInfo))
		{
		}

		private bool NeedToUnbind
		{
			get
			{
				return this.nspiContextHandle != IntPtr.Zero;
			}
		}

		public IAsyncResult BeginBind(TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			return new NspiClient.BindRpcCallContext(base.RpcClient, timeout, asyncCallback, asyncState).Begin();
		}

		public NspiCallResult EndBind(IAsyncResult asyncResult)
		{
			return ((NspiClient.BindRpcCallContext)asyncResult).End(asyncResult, out this.nspiContextHandle, out this.serverGuid);
		}

		public IAsyncResult BeginUnbind(TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.NeedToUnbind)
			{
				return new NspiClient.UnbindRpcCallContext(base.RpcClient, this.nspiContextHandle, timeout, asyncCallback, asyncState).Begin();
			}
			EasyAsyncResult easyAsyncResult = new EasyAsyncResult(asyncCallback, asyncState);
			easyAsyncResult.InvokeCallback();
			return easyAsyncResult;
		}

		public NspiCallResult EndUnbind(IAsyncResult asyncResult)
		{
			if (this.NeedToUnbind)
			{
				IntPtr intPtr;
				NspiCallResult result = ((NspiClient.UnbindRpcCallContext)asyncResult).End(asyncResult, out intPtr);
				this.nspiContextHandle = intPtr;
				return result;
			}
			return NspiCallResult.CreateSuccessfulResult();
		}

		public IAsyncResult BeginGetHierarchyInfo(TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			return new NspiClient.GetHierarchyInfoRpcCallContext(base.RpcClient, this.nspiContextHandle, timeout, asyncCallback, asyncState).Begin();
		}

		public NspiCallResult EndGetHierarchyInfo(IAsyncResult asyncResult, out int version)
		{
			return ((NspiClient.GetHierarchyInfoRpcCallContext)asyncResult).End(asyncResult, out version);
		}

		public IAsyncResult BeginGetMatches(string primarySmtpAddress, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			return new NspiClient.GetMatchesRpcCallContext(base.RpcClient, this.nspiContextHandle, primarySmtpAddress, timeout, asyncCallback, asyncState).Begin();
		}

		public NspiCallResult EndGetMatches(IAsyncResult asyncResult, out int[] minimalIds)
		{
			return ((NspiClient.GetMatchesRpcCallContext)asyncResult).End(asyncResult, out minimalIds);
		}

		public IAsyncResult BeginQueryRows(int[] minimalIds, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			return new NspiClient.QueryRowsRpcCallContext(base.RpcClient, this.nspiContextHandle, minimalIds, timeout, asyncCallback, asyncState).Begin();
		}

		public NspiCallResult EndQueryRows(IAsyncResult asyncResult, out string homeMDB, out string userLegacyDN)
		{
			return ((NspiClient.QueryRowsRpcCallContext)asyncResult).End(asyncResult, out homeMDB, out userLegacyDN);
		}

		public IAsyncResult BeginDNToEph(string serverLegacyDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			return new NspiClient.DNToEphRpcCallContext(base.RpcClient, this.nspiContextHandle, serverLegacyDn, timeout, asyncCallback, asyncState).Begin();
		}

		public NspiCallResult EndDNToEph(IAsyncResult asyncResult, out int[] minimalIds)
		{
			return ((NspiClient.DNToEphRpcCallContext)asyncResult).End(asyncResult, out minimalIds);
		}

		public IAsyncResult BeginGetNetworkAddresses(int[] minimalIds, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			return new NspiClient.GetNetworkAddressesRpcCallContext(base.RpcClient, this.nspiContextHandle, minimalIds, timeout, asyncCallback, asyncState).Begin();
		}

		public NspiCallResult EndGetNetworkAddresses(IAsyncResult asyncResult, out string[] networkAddresses)
		{
			return ((NspiClient.GetNetworkAddressesRpcCallContext)asyncResult).End(asyncResult, out networkAddresses);
		}

		protected sealed override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiClient>(this);
		}

		protected sealed override void InternalDispose()
		{
			if (this.NeedToUnbind)
			{
				this.EndUnbind(this.BeginUnbind(Constants.DefaultRpcTimeout, null, null));
			}
			base.InternalDispose();
		}

		private static readonly int DefaultLcid = CultureInfo.CurrentUICulture.LCID;

		private static readonly int DefaultANSICodePage = CultureInfo.CurrentUICulture.TextInfo.ANSICodePage;

		private IntPtr nspiContextHandle;

		private Guid serverGuid;

		private abstract class NspiBaseRpcCallContext : RpcCallContext<NspiCallResult>
		{
			protected NspiBaseRpcCallContext(NspiAsyncRpcClient rpcClient, IntPtr contextHandle, NspiState nspiState, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnNullArgument(rpcClient, "rpcClient");
				this.rpcClient = rpcClient;
				this.contextHandle = contextHandle;
				this.nspiState = nspiState;
				if (this.nspiState == null)
				{
					this.nspiState = new NspiState();
					this.nspiState.SortLocale = NspiClient.DefaultLcid;
					this.nspiState.TemplateLocale = NspiClient.DefaultLcid;
					this.nspiState.CodePage = NspiClient.DefaultANSICodePage;
				}
			}

			protected NspiAsyncRpcClient NspiAsyncRpcClient
			{
				get
				{
					return this.rpcClient;
				}
			}

			protected NspiState NspiState
			{
				get
				{
					return this.nspiState;
				}
				set
				{
					this.nspiState = value;
				}
			}

			protected IntPtr ContextHandle
			{
				get
				{
					return this.contextHandle;
				}
				set
				{
					this.contextHandle = value;
				}
			}

			protected override NspiCallResult ConvertExceptionToResult(Exception exception)
			{
				RpcException ex = exception as RpcException;
				if (ex != null)
				{
					return this.OnRpcException(ex);
				}
				NspiDataException ex2 = exception as NspiDataException;
				if (ex2 != null)
				{
					return this.OnNspiDataException(ex2);
				}
				return null;
			}

			protected override NspiCallResult OnRpcException(RpcException rpcException)
			{
				return new NspiCallResult(rpcException);
			}

			protected NspiCallResult OnNspiDataException(NspiDataException nspiException)
			{
				return new NspiCallResult(nspiException);
			}

			private readonly NspiAsyncRpcClient rpcClient;

			private NspiState nspiState;

			private IntPtr contextHandle = IntPtr.Zero;
		}

		private sealed class BindRpcCallContext : NspiClient.NspiBaseRpcCallContext
		{
			public BindRpcCallContext(NspiAsyncRpcClient rpcClient, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(rpcClient, IntPtr.Zero, null, timeout, asyncCallback, asyncState)
			{
			}

			public NspiCallResult End(IAsyncResult asyncResult, out IntPtr contextHandle, out Guid serverGuid)
			{
				NspiCallResult result = base.GetResult();
				contextHandle = base.ContextHandle;
				serverGuid = this.serverGuid;
				return result;
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				ICancelableAsyncResult result = base.NspiAsyncRpcClient.BeginBind(null, null, NspiBindFlags.None, base.NspiState, new Guid?(this.serverGuid), asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override NspiCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				NspiCallResult result;
				try
				{
					Guid? guid;
					IntPtr contextHandle;
					NspiStatus nspiStatus = base.NspiAsyncRpcClient.EndBind(asyncResult, out guid, out contextHandle);
					if (nspiStatus == NspiStatus.Success)
					{
						base.ContextHandle = contextHandle;
						if (guid != null)
						{
							this.serverGuid = guid.Value;
						}
					}
					result = new NspiCallResult(nspiStatus);
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}

			private Guid serverGuid;
		}

		private sealed class UnbindRpcCallContext : NspiClient.NspiBaseRpcCallContext
		{
			public UnbindRpcCallContext(NspiAsyncRpcClient rpcClient, IntPtr contextHandle, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(rpcClient, contextHandle, null, timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnIntPtrZero(contextHandle, "contextHandle");
			}

			public NspiCallResult End(IAsyncResult asyncResult, out IntPtr contextHandle)
			{
				NspiCallResult result = base.GetResult();
				contextHandle = base.ContextHandle;
				return result;
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				ICancelableAsyncResult result = base.NspiAsyncRpcClient.BeginUnbind(null, base.ContextHandle, NspiUnbindFlags.None, asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override NspiCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				NspiCallResult result;
				try
				{
					IntPtr contextHandle;
					NspiStatus nspiStatus = base.NspiAsyncRpcClient.EndUnbind(asyncResult, out contextHandle);
					base.ContextHandle = contextHandle;
					if (nspiStatus == NspiStatus.UnbindSuccess)
					{
						result = new NspiCallResult(NspiStatus.Success);
					}
					else
					{
						result = new NspiCallResult(NspiStatus.GeneralFailure);
					}
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}
		}

		private sealed class GetHierarchyInfoRpcCallContext : NspiClient.NspiBaseRpcCallContext
		{
			public GetHierarchyInfoRpcCallContext(NspiAsyncRpcClient rpcClient, IntPtr contextHandle, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(rpcClient, contextHandle, null, timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnIntPtrZero(contextHandle, "contextHandle");
			}

			public NspiCallResult End(IAsyncResult asyncResult, out int version)
			{
				NspiCallResult result = base.GetResult();
				version = this.version;
				return result;
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				ICancelableAsyncResult result = base.NspiAsyncRpcClient.BeginGetHierarchyInfo(null, base.ContextHandle, NspiGetHierarchyInfoFlags.Unicode, base.NspiState, 0, asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override NspiCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				NspiCallResult result;
				try
				{
					PropertyValue[][] array = null;
					int num;
					int num2;
					NspiStatus nspiStatus = base.NspiAsyncRpcClient.EndGetHierarchyInfo(asyncResult, out num, out num2, out array);
					this.version = num2;
					result = new NspiCallResult(nspiStatus);
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}

			private int version;
		}

		private sealed class GetMatchesRpcCallContext : NspiClient.NspiBaseRpcCallContext
		{
			public GetMatchesRpcCallContext(NspiAsyncRpcClient rpcClient, IntPtr contextHandle, string primarySmtpAddress, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(rpcClient, contextHandle, null, timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnIntPtrZero(contextHandle, "contextHandle");
				Util.ThrowOnNullOrEmptyArgument(primarySmtpAddress, "primarySmtpAddress");
				this.primarySmtpAddress = primarySmtpAddress;
			}

			public NspiCallResult End(IAsyncResult asyncResult, out int[] minimalIds)
			{
				NspiCallResult result = base.GetResult();
				minimalIds = this.minimalIds;
				return result;
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				Restriction restriction = new PropertyRestriction(RelationOperator.Equals, PropertyTag.Anr, new PropertyValue?(new PropertyValue(PropertyTag.Anr, this.primarySmtpAddress)));
				ICancelableAsyncResult result = base.NspiAsyncRpcClient.BeginGetMatches(null, base.ContextHandle, NspiGetMatchesFlags.None, base.NspiState, null, 0, restriction, IntPtr.Zero, 100, NspiClient.GetMatchesRpcCallContext.MatchesPropertyTags, asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override NspiCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				NspiCallResult result;
				try
				{
					NspiState nspiState = null;
					PropertyValue[][] array = null;
					NspiStatus nspiStatus = base.NspiAsyncRpcClient.EndGetMatches(asyncResult, out nspiState, out this.minimalIds, out array);
					base.NspiState = nspiState;
					result = new NspiCallResult(nspiStatus);
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}

			private static readonly PropertyTag[] MatchesPropertyTags = new PropertyTag[]
			{
				new PropertyTag(805371934U),
				new PropertyTag(974585887U),
				new PropertyTag(973602847U),
				new PropertyTag(974716958U),
				new PropertyTag(972947487U),
				new PropertyTag(974520351U),
				new PropertyTag(973078559U),
				new PropertyTag(805437470U),
				new PropertyTag(268370178U),
				new PropertyTag(268304387U),
				new PropertyTag(956301315U),
				new PropertyTag(956628995U),
				new PropertyTag(267780354U),
				new PropertyTag(805503006U)
			};

			private readonly string primarySmtpAddress;

			private int[] minimalIds;
		}

		private sealed class QueryRowsRpcCallContext : NspiClient.NspiBaseRpcCallContext
		{
			public QueryRowsRpcCallContext(NspiAsyncRpcClient rpcClient, IntPtr contextHandle, int[] minimalIds, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(rpcClient, contextHandle, null, timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnIntPtrZero(contextHandle, "contextHandle");
				Util.ThrowOnNullArgument(minimalIds, "minimalIds");
				this.minimalIds = minimalIds;
			}

			public NspiCallResult End(IAsyncResult asyncResult, out string homeMDB, out string userLegacyDN)
			{
				NspiCallResult result = base.GetResult();
				homeMDB = this.homeMDB;
				userLegacyDN = this.userLegacyDN;
				return result;
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				ICancelableAsyncResult result = base.NspiAsyncRpcClient.BeginQueryRows(null, base.ContextHandle, NspiQueryRowsFlags.None, base.NspiState, this.minimalIds, 100, NspiClient.QueryRowsRpcCallContext.QueryRowsPropTags, asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override NspiCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				NspiCallResult result;
				try
				{
					NspiState nspiState = null;
					PropertyValue[][] array = null;
					NspiStatus nspiStatus = base.NspiAsyncRpcClient.EndQueryRows(asyncResult, out nspiState, out array);
					base.NspiState = nspiState;
					if (nspiStatus == NspiStatus.Success)
					{
						if (array == null)
						{
							return new NspiCallResult(new NspiDataException("QueryHomeMDB::QueryRows", "Rows is null"));
						}
						if (array.Length != 1)
						{
							return new NspiCallResult(new NspiDataException("QueryHomeMDB::QueryRows", string.Format("ExpectedRowCount = {0}, ActualRowCount = {1}", 1, array.Length)));
						}
						if (array[0].Length != NspiClient.QueryRowsRpcCallContext.QueryRowsPropTags.Length)
						{
							return new NspiCallResult(new NspiDataException("QueryHomeMDB::QueryRows", string.Format("ExpectedPropertyCount = {0}, ActualPropertyCount = {1}", NspiClient.QueryRowsRpcCallContext.QueryRowsPropTags.Length, array[0].Length)));
						}
						this.homeMDB = (string)array[0][3].Value;
						this.userLegacyDN = (string)array[0][1].Value;
					}
					result = new NspiCallResult(nspiStatus);
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}

			private const int IndexEmailAddressAnsi = 1;

			private const int IndexHomeMdb = 3;

			private static readonly PropertyTag[] QueryRowsPropTags = new PropertyTag[]
			{
				new PropertyTag(805371935U),
				new PropertyTag(805503006U),
				new PropertyTag(956301315U),
				new PropertyTag(2147876894U),
				new PropertyTag(237043970U),
				new PropertyTag(1712525342U),
				new PropertyTag(2148470814U),
				new PropertyTag(956628995U)
			};

			private readonly int[] minimalIds;

			private string homeMDB;

			private string userLegacyDN;
		}

		private class DNToEphRpcCallContext : NspiClient.NspiBaseRpcCallContext
		{
			public DNToEphRpcCallContext(NspiAsyncRpcClient rpcClient, IntPtr contextHandle, string serverLegacyDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
			{
				int[] array = new int[1];
				this.minimalIds = array;
				base..ctor(rpcClient, contextHandle, null, timeout, asyncCallback, asyncState);
				Util.ThrowOnIntPtrZero(contextHandle, "contextHandle");
				Util.ThrowOnNullOrEmptyArgument(serverLegacyDn, "serverLegacyDn");
				this.serverLegDn = serverLegacyDn;
			}

			public NspiCallResult End(IAsyncResult asyncResult, out int[] minimalIds)
			{
				NspiCallResult result = base.GetResult();
				minimalIds = this.minimalIds;
				return result;
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				ICancelableAsyncResult result = base.NspiAsyncRpcClient.BeginDNToEph(null, base.ContextHandle, NspiDNToEphFlags.None, new string[]
				{
					this.serverLegDn
				}, asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override NspiCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				NspiCallResult result;
				try
				{
					NspiStatus nspiStatus = base.NspiAsyncRpcClient.EndDNToEph(asyncResult, out this.minimalIds);
					result = new NspiCallResult(nspiStatus);
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}

			private readonly string serverLegDn;

			private int[] minimalIds;
		}

		private sealed class GetNetworkAddressesRpcCallContext : NspiClient.NspiBaseRpcCallContext
		{
			public GetNetworkAddressesRpcCallContext(NspiAsyncRpcClient rpcClient, IntPtr contextHandle, int[] minimalIds, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(rpcClient, contextHandle, null, timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnIntPtrZero(contextHandle, "contextHandle");
				base.NspiState.CurrentRecord = minimalIds[0];
			}

			public NspiCallResult End(IAsyncResult asyncResult, out string[] networkAddresses)
			{
				NspiCallResult result = base.GetResult();
				networkAddresses = this.networkAddresses;
				return result;
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				ICancelableAsyncResult result = base.NspiAsyncRpcClient.BeginGetProps(null, base.ContextHandle, NspiGetPropsFlags.None, base.NspiState, NspiClient.GetNetworkAddressesRpcCallContext.EmsABNetworkAddresses, asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override NspiCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				NspiCallResult result;
				try
				{
					int num = 0;
					PropertyValue[] array = null;
					NspiStatus nspiStatus = base.NspiAsyncRpcClient.EndGetProps(asyncResult, out num, out array);
					if (nspiStatus == NspiStatus.Success)
					{
						if (array == null || array.Length != 1)
						{
							throw new NspiDataException("GetNetworkAddresses::GetProps", string.Format("Properties = {0}, Count = {1}", (array != null) ? array.ToString() : "null", (array != null) ? array.Length : -1));
						}
						this.networkAddresses = (string[])array[0].Value;
					}
					result = new NspiCallResult(nspiStatus);
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}

			public static readonly PropertyTag EmsABNetworkAddress = new PropertyTag(2171605022U);

			public static readonly PropertyTag[] EmsABNetworkAddresses = new PropertyTag[]
			{
				NspiClient.GetNetworkAddressesRpcCallContext.EmsABNetworkAddress
			};

			private string[] networkAddresses = new string[0];
		}
	}
}
