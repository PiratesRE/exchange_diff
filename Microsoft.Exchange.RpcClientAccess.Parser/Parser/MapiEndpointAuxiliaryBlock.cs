using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class MapiEndpointAuxiliaryBlock : AuxiliaryBlock
	{
		public MapiEndpointAuxiliaryBlock(MapiEndpointProcessType mapiEndpointProcessType, string endpointFqdn) : base((endpointFqdn == null) ? 1 : 2, AuxiliaryBlockTypes.MapiEndpoint)
		{
			this.mapiEndpointProcessType = mapiEndpointProcessType;
			this.endpointFqdn = endpointFqdn;
		}

		internal MapiEndpointAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.mapiEndpointProcessType = (MapiEndpointProcessType)reader.ReadByte();
			if (base.Version >= 2)
			{
				this.endpointFqdn = reader.ReadAsciiString(StringFlags.Sized);
			}
		}

		public MapiEndpointProcessType MapiEndpointProcessType
		{
			get
			{
				return this.mapiEndpointProcessType;
			}
		}

		public string EndpointFqdn
		{
			get
			{
				return this.endpointFqdn;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.mapiEndpointProcessType);
			if (base.Version >= 2)
			{
				writer.WriteAsciiString(this.endpointFqdn, StringFlags.Sized);
			}
		}

		private readonly MapiEndpointProcessType mapiEndpointProcessType;

		private readonly string endpointFqdn;
	}
}
