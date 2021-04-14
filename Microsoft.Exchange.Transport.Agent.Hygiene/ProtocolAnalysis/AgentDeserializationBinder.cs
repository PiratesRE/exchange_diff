using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	internal sealed class AgentDeserializationBinder : SerializationBinder
	{
		public override Type BindToType(string assemblyName, string typeName)
		{
			return Type.GetType(string.Format(CultureInfo.InvariantCulture, "{0}, {1}", new object[]
			{
				typeName,
				assemblyName
			}));
		}
	}
}
