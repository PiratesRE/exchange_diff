using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FastTransferRecipientList : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		internal FastTransferRecipientList(IMessage message) : base(false)
		{
			Util.ThrowOnNullArgument(message, "message");
			this.message = message;
			this.recipients = this.message.GetRecipients();
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			IPropertyFilter filter = context.PropertyFilterFactory.GetRecipientFilter();
			foreach (IRecipient recipient in this.recipients)
			{
				IPropertyBag propertyBag = recipient.PropertyBag;
				PropertyValue rowId = propertyBag.GetAnnotatedProperty(PropertyTag.RowId).PropertyValue;
				if (rowId.IsError)
				{
					throw new NotSupportedException("RowIndex is missing from store data.");
				}
				context.DataInterface.PutMarker(PropertyTag.StartRecip);
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, rowId));
				yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(propertyBag, filter)));
				context.DataInterface.PutMarker(PropertyTag.EndRecip);
				yield return null;
			}
			yield break;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			SingleMemberPropertyBag rowIdPropertBag = new SingleMemberPropertyBag(PropertyTag.RowId);
			int lastRowId = -1;
			PropertyTag marker;
			while (context.DataInterface.TryPeekMarker(out marker) && marker == PropertyTag.StartRecip)
			{
				context.DataInterface.ReadMarker(PropertyTag.StartRecip);
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, rowIdPropertBag));
				int newRowId = rowIdPropertBag.PropertyValue.GetValue<int>();
				if (newRowId <= lastRowId)
				{
					throw new BufferParseException(string.Format("Recipients arrived out of order. Expected > {0}, actual = {1}.", lastRowId, newRowId));
				}
				lastRowId = newRowId;
				IRecipient recipient = this.message.CreateRecipient();
				yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(recipient.PropertyBag)));
				try
				{
					recipient.Save();
				}
				catch (CorruptRecipientException)
				{
					PropertyValue propertyValue = recipient.PropertyBag.GetAnnotatedProperty(PropertyTag.RowId).PropertyValue;
					if (!context.IsMovingMailbox || propertyValue.IsError || propertyValue.IsNotFound || propertyValue.IsNullValue || !(propertyValue.Value is int))
					{
						throw;
					}
					this.message.RemoveRecipient((int)propertyValue.Value);
				}
				context.DataInterface.ReadMarker(PropertyTag.EndRecip);
				yield return null;
			}
			yield break;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferRecipientList>(this);
		}

		private readonly IEnumerable<IRecipient> recipients;

		private readonly IMessage message;
	}
}
