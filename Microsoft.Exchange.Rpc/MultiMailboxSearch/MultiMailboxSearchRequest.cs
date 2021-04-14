using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal sealed class MultiMailboxSearchRequest : MultiMailboxRequestBase
	{
		internal MultiMailboxSearchRequest(int version) : base(version)
		{
			this.refinersEnabled = false;
		}

		internal MultiMailboxSearchRequest() : base(MultiMailboxSearchBase.CurrentVersion)
		{
		}

		internal bool RefinersEnabled
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.refinersEnabled;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.refinersEnabled = value;
			}
		}

		internal int RefinerResultsTrimCount
		{
			get
			{
				return this.refinerResultsTrimCount;
			}
			set
			{
				this.refinerResultsTrimCount = value;
			}
		}

		internal string Query
		{
			get
			{
				return this.query;
			}
			set
			{
				this.query = value;
			}
		}

		internal byte[] SortCriteria
		{
			get
			{
				return this.sortCriteria;
			}
			set
			{
				this.sortCriteria = value;
			}
		}

		internal Sorting SortingOrder
		{
			get
			{
				return this.sortingOrder;
			}
			set
			{
				this.sortingOrder = value;
			}
		}

		internal PagingInfo Paging
		{
			get
			{
				return this.pagingInfo;
			}
			set
			{
				this.pagingInfo = value;
			}
		}

		internal byte[] Restriction
		{
			get
			{
				return this.restriction;
			}
			set
			{
				this.restriction = value;
			}
		}

		[SuppressMessage("Exchange.Security", "EX0043:DoNotUseBinarySoapFormatter", Justification = "Suppress warning in current code base.The usage has already been verified.")]
		internal static MultiMailboxSearchRequest DeSerialize(byte[] bytes)
		{
			if (bytes != null && bytes.Length > 0)
			{
				MemoryStream memoryStream = null;
				BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
				try
				{
					MultiMailboxSearchRequest result;
					try
					{
						memoryStream = new MemoryStream(bytes);
						return binaryFormatter.Deserialize(memoryStream) as MultiMailboxSearchRequest;
					}
					catch (SerializationException)
					{
						result = null;
					}
					return result;
				}
				finally
				{
					if (null != memoryStream)
					{
						memoryStream.Close();
					}
				}
			}
			return null;
		}

		[SuppressMessage("Exchange.Security", "EX0043:DoNotUseBinarySoapFormatter", Justification = "Suppress warning in current code base.The usage has already been verified.")]
		internal static byte[] Serialize(MultiMailboxSearchRequest request)
		{
			if (request == null)
			{
				return new byte[0];
			}
			MemoryStream memoryStream = null;
			byte[] result;
			try
			{
				byte[] array;
				try
				{
					memoryStream = new MemoryStream();
					ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null).Serialize(memoryStream, request);
					return memoryStream.ToArray();
				}
				catch (SerializationException)
				{
					array = new byte[0];
				}
				result = array;
			}
			finally
			{
				if (null != memoryStream)
				{
					memoryStream.Close();
				}
			}
			return result;
		}

		private string query;

		private PagingInfo pagingInfo;

		private Sorting sortingOrder;

		private byte[] restriction;

		private bool refinersEnabled;

		private int refinerResultsTrimCount;

		private byte[] sortCriteria;
	}
}
