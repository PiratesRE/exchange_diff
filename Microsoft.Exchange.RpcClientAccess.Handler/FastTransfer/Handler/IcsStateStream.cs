using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IcsStateStream
	{
		internal IcsStateStream(IPropertyBag propertyBag)
		{
			this.propertyBag = propertyBag;
		}

		public byte[] GetStateValue(PropertyTag statePropertyTag)
		{
			PropertyValue propertyValue = this.propertyBag.GetAnnotatedProperty(statePropertyTag).PropertyValue;
			if (!propertyValue.IsNotFound)
			{
				return propertyValue.GetValueAssert<byte[]>();
			}
			return Array<byte>.Empty;
		}

		public void SetStateValue(PropertyTag statePropertyTag, byte[] state)
		{
			PropertyValue property = new PropertyValue(statePropertyTag, state);
			this.propertyBag.SetProperty(property);
		}

		public StorageIcsState ToXsoState()
		{
			return new StorageIcsState(this.GetStateValue(FastTransferIcsState.IdsetGiven), this.GetStateValue(FastTransferIcsState.CnsetSeen), this.GetStateValue(FastTransferIcsState.CnsetSeenAssociated), this.GetStateValue(FastTransferIcsState.CnsetRead));
		}

		public void FromXsoState(StorageIcsState state)
		{
			this.SetStateValue(FastTransferIcsState.IdsetGiven, state.StateIdsetGiven);
			this.SetStateValue(FastTransferIcsState.CnsetSeen, state.StateCnsetSeen);
			this.SetStateValue(FastTransferIcsState.CnsetSeenAssociated, state.StateCnsetSeenFAI);
			this.SetStateValue(FastTransferIcsState.CnsetRead, state.StateCnsetRead);
		}

		private readonly IPropertyBag propertyBag;
	}
}
