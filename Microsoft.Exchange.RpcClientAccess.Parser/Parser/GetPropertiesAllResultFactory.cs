using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetPropertiesAllResultFactory : StandardResultFactory
	{
		internal GetPropertiesAllResultFactory(int maxSize, Encoding string8Encoding) : base(RopId.GetPropertiesAll)
		{
			this.maxSize = maxSize;
			this.string8Encoding = string8Encoding;
		}

		public RopResult CreateSuccessfulResult(PropertyValue[] propertyValues)
		{
			SuccessfulGetPropertiesAllResult successfulGetPropertiesAllResult = new SuccessfulGetPropertiesAllResult(propertyValues);
			successfulGetPropertiesAllResult.String8Encoding = this.string8Encoding;
			using (CountWriter countWriter = new CountWriter())
			{
				do
				{
					countWriter.Position = 0L;
					successfulGetPropertiesAllResult.Serialize(countWriter);
					if (countWriter.Position <= (long)this.maxSize)
					{
						goto IL_45;
					}
				}
				while (successfulGetPropertiesAllResult.RemoveLargestProperty());
				throw new BufferOutOfMemoryException();
				IL_45:;
			}
			return successfulGetPropertiesAllResult;
		}

		private readonly int maxSize;

		private readonly Encoding string8Encoding;
	}
}
