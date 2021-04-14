using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport
{
	internal class XExch50Blob : LazyBytes
	{
		public XExch50Blob(DataRow row, DataColumn column) : base(row, column)
		{
		}

		public XExch50Blob(DataRow row, BlobCollection blobCollection, byte blobCollectionKey) : base(row, blobCollection, blobCollectionKey)
		{
		}

		public XExch50Blob()
		{
		}
	}
}
