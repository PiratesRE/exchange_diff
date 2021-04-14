using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetPropertiesSpecificResultFactory : StandardResultFactory
	{
		internal GetPropertiesSpecificResultFactory(int maxSize, Encoding string8Encoding) : base(RopId.GetPropertiesSpecific)
		{
			this.maxSize = maxSize;
			this.string8Encoding = string8Encoding;
		}

		public RopResult CreateSuccessfulResult(PropertyTag[] columns, PropertyValue[] propertyValues)
		{
			SuccessfulGetPropertiesSpecificResult successfulGetPropertiesSpecificResult = new SuccessfulGetPropertiesSpecificResult(columns, propertyValues);
			successfulGetPropertiesSpecificResult.String8Encoding = this.string8Encoding;
			using (CountWriter countWriter = new CountWriter())
			{
				do
				{
					countWriter.Position = 0L;
					successfulGetPropertiesSpecificResult.Serialize(countWriter);
					if (countWriter.Position <= (long)this.maxSize)
					{
						goto IL_46;
					}
				}
				while (successfulGetPropertiesSpecificResult.RemoveLargestProperty());
				throw new BufferOutOfMemoryException();
				IL_46:;
			}
			return successfulGetPropertiesSpecificResult;
		}

		private readonly int maxSize;

		private readonly Encoding string8Encoding;
	}
}
