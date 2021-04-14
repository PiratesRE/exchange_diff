using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SetMonitoringContextAuxiliaryBlock : AuxiliaryBlock
	{
		public SetMonitoringContextAuxiliaryBlock() : base(1, AuxiliaryBlockTypes.SetMonitoringContext)
		{
		}

		internal SetMonitoringContextAuxiliaryBlock(Reader reader) : base(reader)
		{
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
		}
	}
}
