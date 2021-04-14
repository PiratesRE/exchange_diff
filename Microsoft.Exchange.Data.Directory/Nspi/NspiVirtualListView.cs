using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Nspi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiVirtualListView
	{
		internal NspiVirtualListView(IConfigurationSession session, int codePage, ADObjectId addressListId, PropertyDefinition[] properties)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (addressListId == null)
			{
				throw new ArgumentNullException("addressListId");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			this.session = session;
			this.codePage = codePage;
			this.addressListId = addressListId;
			this.properties = properties;
			this.currentRow = -1;
			this.estimatedRowCount = -1;
		}

		public int CurrentRow
		{
			get
			{
				return this.currentRow;
			}
		}

		public int EstimatedRowCount
		{
			get
			{
				if (this.estimatedRowCount == -1)
				{
					this.estimatedRowCount = NspiVirtualListView.GetEstimatedRowCount(this.session, this.addressListId.ObjectGuid);
				}
				return this.estimatedRowCount;
			}
		}

		public ADRawEntry[] GetPropertyBags(int offset, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count == 0)
			{
				return Array<ADRawEntry>.Empty;
			}
			Encoding encoding = Encoding.GetEncoding(this.codePage);
			NspiPropertyMap nspiPropertyMap = NspiPropertyMap.Create(this.properties, encoding);
			PropRowSet propRowSet = null;
			try
			{
				using (NspiRpcClientConnection nspiRpcClientConnection = this.session.GetNspiRpcClientConnection())
				{
					int addressListEphemeralId = NspiVirtualListView.GetAddressListEphemeralId(nspiRpcClientConnection, this.addressListId.ObjectGuid);
					if (addressListEphemeralId == 0)
					{
						return null;
					}
					NspiState nspiState = new NspiState
					{
						SortIndex = SortIndex.DisplayName,
						ContainerId = addressListEphemeralId,
						CurrentRecord = 0,
						Delta = offset,
						CodePage = this.codePage,
						TemplateLocale = this.session.Lcid,
						SortLocale = this.session.Lcid
					};
					using (SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(nspiState.GetBytesToMarshal()))
					{
						SafeRpcMemoryHandle safeRpcMemoryHandle2 = null;
						try
						{
							IntPtr intPtr = safeRpcMemoryHandle.DangerousGetHandle();
							nspiState.MarshalToNative(intPtr);
							int num = nspiRpcClientConnection.RpcClient.UpdateStat(intPtr);
							if (num != 0)
							{
								throw new NspiFailureException(num);
							}
							num = nspiRpcClientConnection.RpcClient.QueryRows(0, intPtr, null, count, nspiPropertyMap.NspiProperties, out safeRpcMemoryHandle2);
							if (num != 0)
							{
								throw new NspiFailureException(num);
							}
							nspiState.MarshalFromNative(intPtr);
							this.currentRow = nspiState.Position;
							this.estimatedRowCount = nspiState.TotalRecords;
							if (safeRpcMemoryHandle2 != null)
							{
								propRowSet = new PropRowSet(safeRpcMemoryHandle2, true);
							}
						}
						finally
						{
							if (safeRpcMemoryHandle2 != null)
							{
								safeRpcMemoryHandle2.Dispose();
							}
						}
					}
				}
			}
			catch (RpcException ex)
			{
				throw new DataSourceOperationException(DirectoryStrings.NspiRpcError(ex.Message), ex);
			}
			if (propRowSet == null)
			{
				return null;
			}
			return nspiPropertyMap.Convert(propRowSet);
		}

		internal static int GetEstimatedRowCount(IConfigurationSession session, Guid addressListObjectGuid)
		{
			int result;
			try
			{
				using (NspiRpcClientConnection nspiRpcClientConnection = session.GetNspiRpcClientConnection())
				{
					int addressListEphemeralId = NspiVirtualListView.GetAddressListEphemeralId(nspiRpcClientConnection, addressListObjectGuid);
					if (addressListEphemeralId == 0)
					{
						result = -1;
					}
					else
					{
						NspiState nspiState = new NspiState
						{
							ContainerId = addressListEphemeralId,
							CodePage = 1252,
							TemplateLocale = 1033,
							SortLocale = 1033
						};
						using (SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(nspiState.GetBytesToMarshal()))
						{
							IntPtr intPtr = safeRpcMemoryHandle.DangerousGetHandle();
							nspiState.MarshalToNative(intPtr);
							int num = nspiRpcClientConnection.RpcClient.UpdateStat(intPtr);
							if (num != 0)
							{
								throw new NspiFailureException(num);
							}
							nspiState.MarshalFromNative(intPtr);
						}
						result = nspiState.TotalRecords;
					}
				}
			}
			catch (RpcException ex)
			{
				throw new DataSourceOperationException(DirectoryStrings.NspiRpcError(ex.Message), ex);
			}
			return result;
		}

		private static int GetAddressListEphemeralId(NspiRpcClientConnection nspiRpcClientConnection, Guid addressListObjectGuid)
		{
			string text = LegacyDN.FormatLegacyDnFromGuid(Guid.Empty, addressListObjectGuid);
			int[] array;
			int num = nspiRpcClientConnection.RpcClient.DNToEph(new string[]
			{
				text
			}, out array);
			if (num != 0)
			{
				throw new NspiFailureException(num);
			}
			return array[0];
		}

		private readonly IConfigurationSession session;

		private readonly int codePage;

		private readonly ADObjectId addressListId;

		private readonly PropertyDefinition[] properties;

		private int currentRow;

		private int estimatedRowCount;
	}
}
