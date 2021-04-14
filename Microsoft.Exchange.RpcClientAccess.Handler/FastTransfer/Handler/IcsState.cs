using System;
using System.IO;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal sealed class IcsState
	{
		internal void Load(IcsStateOrigin origin, IPropertyBag propertyBag)
		{
			EnumValidator.AssertValid<IcsStateOrigin>(origin);
			this.CheckCanLoad(origin);
			IdSet idSet = IcsState.LoadIdSet(propertyBag, FastTransferIcsState.CnsetSeen);
			IdSet idSet2 = IcsState.LoadIdSet(propertyBag, FastTransferIcsState.CnsetSeenAssociated);
			IdSet idSet3 = IcsState.LoadIdSet(propertyBag, FastTransferIcsState.IdsetGiven);
			IdSet idSet4 = IcsState.LoadIdSet(propertyBag, FastTransferIcsState.CnsetRead);
			this.cnsetSeen = (idSet ?? new IdSet());
			this.cnsetSeenAssociated = (idSet2 ?? new IdSet());
			this.idsetGiven = (idSet3 ?? new IdSet());
			this.cnsetRead = (idSet4 ?? new IdSet());
			this.origin = origin;
		}

		internal void Checkpoint(IPropertyBag propertyBag)
		{
			if (this.origin == IcsStateOrigin.ClientInitial || this.origin == IcsStateOrigin.ServerIncremental || this.origin == IcsStateOrigin.ServerFinal)
			{
				IcsState.SaveIdSet(propertyBag, FastTransferIcsState.CnsetSeen, this.cnsetSeen);
				IcsState.SaveIdSet(propertyBag, FastTransferIcsState.CnsetSeenAssociated, this.cnsetSeenAssociated);
				IcsState.SaveIdSet(propertyBag, FastTransferIcsState.IdsetGiven, this.idsetGiven);
				IcsState.SaveIdSet(propertyBag, FastTransferIcsState.CnsetRead, this.cnsetRead);
				return;
			}
			throw new InvalidOperationException("Don't know how to checkpoint IcsState loaded with " + this.origin.ToString());
		}

		internal void CheckCanLoad(IcsStateOrigin originOfNewState)
		{
			if (this.origin != IcsStateOrigin.None && originOfNewState == IcsStateOrigin.ClientInitial)
			{
				throw new RopExecutionException("Initial client state cannot be loaded twice", (ErrorCode)2147500037U);
			}
		}

		private static IdSet LoadIdSet(IPropertyBag propertyBag, PropertyTag property)
		{
			AnnotatedPropertyValue annotatedProperty = propertyBag.GetAnnotatedProperty(property);
			if (annotatedProperty.PropertyValue.IsNotFound)
			{
				return null;
			}
			if (annotatedProperty.PropertyValue.IsError)
			{
				throw new InvalidOperationException("The IdSet property bag is an in-Memory property bag. It should have no errors other than not founds.");
			}
			IdSet result;
			using (Reader reader = Reader.CreateBufferReader(annotatedProperty.PropertyValue.GetValueAssert<byte[]>()))
			{
				IdSet idSet;
				try
				{
					idSet = IdSet.ParseWithReplGuids(reader);
				}
				catch (BufferParseException innerException)
				{
					throw new RopExecutionException("Invalid IdSet format.", ErrorCode.IdSetFormatError, innerException);
				}
				long num = reader.Length - reader.Position;
				if (num != 0L)
				{
					throw new RopExecutionException(string.Format("Property stream contained {0} more bytes after parsing an IdSet", num), ErrorCode.IdSetFormatError);
				}
				result = idSet;
			}
			return result;
		}

		private static void SaveIdSet(IPropertyBag propertyBag, PropertyTag property, IdSet idset)
		{
			using (Stream stream = propertyBag.SetPropertyStream(property, 512L))
			{
				using (Writer writer = new StreamWriter(stream))
				{
					idset.SerializeWithReplGuids(writer);
				}
			}
		}

		public const int DefaultIcsStateStreamCapacity = 512;

		private IdSet cnsetSeen;

		private IdSet cnsetSeenAssociated;

		private IdSet idsetGiven;

		private IdSet cnsetRead;

		private IcsStateOrigin origin;
	}
}
