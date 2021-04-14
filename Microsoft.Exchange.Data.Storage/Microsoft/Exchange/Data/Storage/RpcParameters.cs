using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcParameters
	{
		public RpcParameters()
		{
			this.parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public RpcParameters(byte[] data)
		{
			if (data == null || data.Length <= 0)
			{
				throw new ArgumentNullException("data");
			}
			this.parameters = RpcCommon.ConvertByteArrayToRpcParameters(data);
		}

		public byte[] Serialize()
		{
			return RpcCommon.ConvertRpcParametersToByteArray(this.parameters);
		}

		protected void SetParameterValue(string name, object value)
		{
			this.parameters[name] = value;
		}

		protected object GetParameterValue(string name)
		{
			object result = null;
			if (!this.parameters.TryGetValue(name, out result))
			{
				throw new ArgumentNullException("RPC parameter is missing for " + name);
			}
			return result;
		}

		private readonly Dictionary<string, object> parameters;
	}
}
