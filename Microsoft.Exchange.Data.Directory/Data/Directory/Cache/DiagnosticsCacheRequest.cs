using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[KnownType(typeof(BaseDirectoryCacheRequest))]
	[DataContract]
	internal class DiagnosticsCacheRequest : BaseDirectoryCacheRequest, IExtensibleDataObject
	{
		public DiagnosticsCacheRequest() : base(TopologyProvider.LocalForestFqdn)
		{
		}
	}
}
