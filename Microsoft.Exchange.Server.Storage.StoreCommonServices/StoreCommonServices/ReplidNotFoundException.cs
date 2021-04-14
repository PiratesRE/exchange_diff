using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class ReplidNotFoundException : StoreException
	{
		public ReplidNotFoundException(LID lid, ushort replid) : base(lid, ErrorCodeValue.NotFound)
		{
			this.replid = replid;
		}

		public ReplidNotFoundException(LID lid, ushort replid, Exception innerException) : base(lid, ErrorCodeValue.NotFound, string.Empty, innerException)
		{
			this.replid = replid;
		}

		public ushort Replid
		{
			get
			{
				return this.replid;
			}
		}

		private const string ReplidSerializationLabel = "replid";

		private ushort replid;
	}
}
