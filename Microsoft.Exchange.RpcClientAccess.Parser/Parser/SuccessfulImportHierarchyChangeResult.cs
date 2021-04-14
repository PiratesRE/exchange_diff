using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulImportHierarchyChangeResult : RopResult
	{
		internal SuccessfulImportHierarchyChangeResult(StoreId folderId) : base(RopId.ImportHierarchyChange, ErrorCode.None, null)
		{
			this.folderId = folderId;
		}

		internal SuccessfulImportHierarchyChangeResult(Reader reader) : base(reader)
		{
			this.folderId = StoreId.Parse(reader);
		}

		internal StoreId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		internal static SuccessfulImportHierarchyChangeResult Parse(Reader reader)
		{
			return new SuccessfulImportHierarchyChangeResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.folderId.Serialize(writer);
		}

		private readonly StoreId folderId;
	}
}
