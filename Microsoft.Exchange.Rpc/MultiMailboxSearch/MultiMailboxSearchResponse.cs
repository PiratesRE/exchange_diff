using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal sealed class MultiMailboxSearchResponse : MultiMailboxResponseBase
	{
		internal MultiMailboxSearchResponse(int version, Dictionary<string, List<MultiMailboxSearchRefinersResult>> refinersOutput, long totalResultCount, long totalResultSize) : base(version)
		{
			this.refinersOutput = refinersOutput;
			this.totalResultCount = totalResultCount;
			this.totalResultSize = totalResultSize;
		}

		internal MultiMailboxSearchResponse(Dictionary<string, List<MultiMailboxSearchRefinersResult>> refinersOutput, long totalResultCount, long totalResultSize)
		{
			this.refinersOutput = refinersOutput;
			this.totalResultCount = totalResultCount;
			this.totalResultSize = totalResultSize;
		}

		internal MultiMailboxSearchResponse(int version) : base(version)
		{
			this.refinersOutput = new Dictionary<string, List<MultiMailboxSearchRefinersResult>>(0);
		}

		internal MultiMailboxSearchResponse()
		{
			this.refinersOutput = new Dictionary<string, List<MultiMailboxSearchRefinersResult>>(0);
		}

		internal Dictionary<string, List<MultiMailboxSearchRefinersResult>> RefinerOutput
		{
			get
			{
				return this.refinersOutput;
			}
		}

		internal long TotalResultCount
		{
			get
			{
				return this.totalResultCount;
			}
		}

		internal long TotalResultSize
		{
			get
			{
				return this.totalResultSize;
			}
		}

		internal PageReference PagingReference
		{
			get
			{
				return this.pageReference;
			}
			set
			{
				this.pageReference = value;
			}
		}

		[SuppressMessage("Exchange.Security", "EX0043:DoNotUseBinarySoapFormatter", Justification = "Suppress warning in current code base.The usage has already been verified.")]
		internal static MultiMailboxSearchResponse DeSerialize(byte[] bytes)
		{
			if (bytes != null && bytes.Length > 0)
			{
				MemoryStream memoryStream = null;
				BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
				try
				{
					MultiMailboxSearchResponse result;
					try
					{
						memoryStream = new MemoryStream(bytes);
						return binaryFormatter.Deserialize(memoryStream) as MultiMailboxSearchResponse;
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
		internal static byte[] Serialize(MultiMailboxSearchResponse response)
		{
			if (response == null)
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
					ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null).Serialize(memoryStream, response);
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

		private PageReference pageReference;

		private readonly Dictionary<string, List<MultiMailboxSearchRefinersResult>> refinersOutput;

		private readonly long totalResultCount;

		private readonly long totalResultSize;
	}
}
