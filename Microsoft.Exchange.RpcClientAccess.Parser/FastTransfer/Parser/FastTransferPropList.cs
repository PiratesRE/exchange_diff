using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FastTransferPropList : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		internal FastTransferPropList(IPropertyBag propertyBag) : this(propertyBag, IncludeAllPropertyFilter.Instance, null)
		{
		}

		internal FastTransferPropList(IPropertyBag propertyBag, IEnumerable<PropertyTag> requestedProperties) : this(propertyBag, IncludeAllPropertyFilter.Instance, requestedProperties)
		{
			Util.ThrowOnNullArgument(requestedProperties, "requestedProperties");
		}

		internal FastTransferPropList(IPropertyBag propertyBag, IPropertyFilter propertyFilter) : this(propertyBag, propertyFilter, null)
		{
		}

		internal FastTransferPropList(IPropertyBag propertyBag, IPropertyFilter propertyFilter, IEnumerable<PropertyTag> requestedProperties) : base(false)
		{
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			Util.ThrowOnNullArgument(propertyFilter, "propertyFilter");
			this.propertyBag = propertyBag;
			this.requestedProperties = requestedProperties;
			this.propertyFilter = propertyFilter;
		}

		public bool ThrowOnPropertyError { get; set; }

		public bool SkipPropertyError { get; set; }

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			foreach (AnnotatedPropertyValue annotatedPropertyValue in this.GetAnnotatedPropertiesToDownload())
			{
				if (annotatedPropertyValue.PropertyValue.IsError)
				{
					if (this.ThrowOnPropertyError)
					{
						string format = "Required property {0} should have a value";
						AnnotatedPropertyValue annotatedPropertyValue2 = annotatedPropertyValue;
						throw new RopExecutionException(string.Format(format, annotatedPropertyValue2.PropertyTag), (ErrorCode)2147942487U);
					}
					if (this.SkipPropertyError)
					{
						continue;
					}
				}
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, this.propertyBag, annotatedPropertyValue));
			}
			yield break;
		}

		private IEnumerable<AnnotatedPropertyValue> GetAnnotatedPropertiesToDownload()
		{
			IEnumerable<AnnotatedPropertyValue> source;
			if (this.requestedProperties == null)
			{
				source = this.propertyBag.GetAnnotatedProperties();
			}
			else
			{
				source = from propertyTag in this.requestedProperties
				select this.propertyBag.GetAnnotatedProperty(propertyTag);
			}
			return from annotatedPropertyValue in source
			where this.propertyFilter.IncludeProperty(annotatedPropertyValue.PropertyTag)
			select annotatedPropertyValue;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			PropertyTag propertyTag;
			while (!context.NoMoreData && !context.DataInterface.TryPeekMarker(out propertyTag) && !FastTransferPropList.MetaProperties.Contains(propertyTag))
			{
				if (this.requestedProperties != null && !this.requestedProperties.Contains(propertyTag, PropertyTag.PropertyIdComparer))
				{
					IPropertyBag throwPropertyBag = new SingleMemberPropertyBag(propertyTag);
					yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, throwPropertyBag));
				}
				else
				{
					yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, this.propertyBag));
				}
			}
			yield break;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferPropList>(this);
		}

		private readonly IPropertyBag propertyBag;

		private readonly IEnumerable<PropertyTag> requestedProperties;

		private readonly IPropertyFilter propertyFilter;

		internal static readonly ICollection<PropertyTag> MetaProperties = new ReadOnlyCollection<PropertyTag>(new PropertyTag[]
		{
			PropertyTag.FXDelProp,
			PropertyTag.EcWarning,
			PropertyTag.NewFXFolder,
			PropertyTag.IncrSyncGroupId,
			PropertyTag.IncrSyncMsgPartial
		});
	}
}
