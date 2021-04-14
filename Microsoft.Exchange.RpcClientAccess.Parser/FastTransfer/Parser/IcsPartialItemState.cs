using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	internal class IcsPartialItemState
	{
		public PropertyGroupMapping GetCurrentMapping()
		{
			PropertyGroupMapping result;
			if (!this.propertyGroupMappings.TryGetValue(this.currentMappingId, out result))
			{
				throw new RopExecutionException(string.Format("Invalid property group mapping ID {0}", this.currentMappingId), (ErrorCode)2147942487U);
			}
			return result;
		}

		public bool AddMapping(PropertyGroupMapping mapping)
		{
			if (!this.propertyGroupMappings.ContainsKey(mapping.MappingId))
			{
				this.propertyGroupMappings.Add(mapping.MappingId, mapping);
				return true;
			}
			return false;
		}

		public int CurrentMappingId
		{
			get
			{
				return this.currentMappingId;
			}
			set
			{
				this.currentMappingId = value;
			}
		}

		private Dictionary<int, PropertyGroupMapping> propertyGroupMappings = new Dictionary<int, PropertyGroupMapping>(1);

		private int currentMappingId = -1;
	}
}
