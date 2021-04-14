using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FastTransferStateMachineFactory
	{
		internal IEnumerator<FastTransferStateMachine?> Serialize(FastTransferDownloadContext context, IPropertyBag propertyBag, AnnotatedPropertyValue annotatedPropertyValue)
		{
			object instance = this.serializeCacher.GetInstance();
			return this.serializeCacher.GetInitializer()(instance, context, propertyBag, annotatedPropertyValue);
		}

		internal IEnumerator<FastTransferStateMachine?> SerializeFixedSize(FastTransferDownloadContext context, PropertyValue propertyValue)
		{
			object instance = this.serializeFixedSizeCacher.GetInstance();
			return this.serializeFixedSizeCacher.GetInitializer()(instance, context, propertyValue);
		}

		internal IEnumerator<FastTransferStateMachine?> SerializeVariableSize(FastTransferDownloadContext context, PropertyTag propertyTag, Stream propertyReadStream)
		{
			object instance = this.serializeVariableSizeCacher.GetInstance();
			return this.serializeVariableSizeCacher.GetInitializer()(instance, context, propertyTag, propertyReadStream);
		}

		internal IEnumerator<FastTransferStateMachine?> Deserialize(FastTransferUploadContext context, IPropertyBag propertyBag)
		{
			object instance = this.deserializeCacher.GetInstance();
			return this.deserializeCacher.GetInitializer()(instance, context, propertyBag);
		}

		internal IEnumerator<FastTransferStateMachine?> DeserializeVariableSizeProperty(FastTransferUploadContext context, IPropertyBag propertyBag, PropertyTag propertyTag, int codePage)
		{
			object instance = this.deserializeVariableSizePropertyCacher.GetInstance();
			return this.deserializeVariableSizePropertyCacher.GetInitializer()(instance, context, propertyBag, propertyTag, codePage);
		}

		internal IEnumerator<FastTransferStateMachine?> SkipPropertyIfExists(FastTransferUploadContext context, PropertyTag propertyTagToSkip)
		{
			object instance = this.skipPropertyIfExistsCacher.GetInstance();
			return this.skipPropertyIfExistsCacher.GetInitializer()(instance, context, propertyTagToSkip);
		}

		private readonly IteratorCacher<FastTransferStateMachineFactory.SerializeInitializerDelegate> serializeCacher = new IteratorCacher<FastTransferStateMachineFactory.SerializeInitializerDelegate>(new FastTransferStateMachineFactory.SerializeInitializerDelegate(FastTransferPropertyValue.DownloadImpl.Serialize_CreateDisplayClass), (FastTransferStateMachineFactory.SerializeInitializerDelegate del) => del(null, null, null, default(AnnotatedPropertyValue)));

		private readonly IteratorCacher<FastTransferStateMachineFactory.SerializeFixedSizePropertyInitializerDelegate> serializeFixedSizeCacher = new IteratorCacher<FastTransferStateMachineFactory.SerializeFixedSizePropertyInitializerDelegate>(new FastTransferStateMachineFactory.SerializeFixedSizePropertyInitializerDelegate(FastTransferPropertyValue.DownloadImpl.SerializeFixedSizeProperty_CreateDisplayClass), (FastTransferStateMachineFactory.SerializeFixedSizePropertyInitializerDelegate del) => del(null, null, default(PropertyValue)));

		private readonly IteratorCacher<FastTransferStateMachineFactory.SerializeVariableSizePropertyInitializerDelegate> serializeVariableSizeCacher = new IteratorCacher<FastTransferStateMachineFactory.SerializeVariableSizePropertyInitializerDelegate>(new FastTransferStateMachineFactory.SerializeVariableSizePropertyInitializerDelegate(FastTransferPropertyValue.DownloadImpl.SerializeVariableSizeProperty_CreateDisplayClass), (FastTransferStateMachineFactory.SerializeVariableSizePropertyInitializerDelegate del) => del(null, null, default(PropertyTag), null));

		private readonly IteratorCacher<FastTransferStateMachineFactory.DeserializeInitializerDelegate> deserializeCacher = new IteratorCacher<FastTransferStateMachineFactory.DeserializeInitializerDelegate>(new FastTransferStateMachineFactory.DeserializeInitializerDelegate(FastTransferPropertyValue.UploadImpl.Deserialize_CreateDisplayClass), (FastTransferStateMachineFactory.DeserializeInitializerDelegate del) => del(null, null, null));

		private readonly IteratorCacher<FastTransferStateMachineFactory.DeserializeVariableSizePropertyInitializerDelegate> deserializeVariableSizePropertyCacher = new IteratorCacher<FastTransferStateMachineFactory.DeserializeVariableSizePropertyInitializerDelegate>(new FastTransferStateMachineFactory.DeserializeVariableSizePropertyInitializerDelegate(FastTransferPropertyValue.UploadImpl.DeserializeVariableSizeProperty_CreateDisplayClass), (FastTransferStateMachineFactory.DeserializeVariableSizePropertyInitializerDelegate del) => del(null, null, null, default(PropertyTag), 0));

		private readonly IteratorCacher<FastTransferStateMachineFactory.SkipPropertyIfExistsInitializerDelegate> skipPropertyIfExistsCacher = new IteratorCacher<FastTransferStateMachineFactory.SkipPropertyIfExistsInitializerDelegate>(new FastTransferStateMachineFactory.SkipPropertyIfExistsInitializerDelegate(FastTransferPropertyValue.UploadImpl.SkipPropertyIfExists_CreateDisplayClass), (FastTransferStateMachineFactory.SkipPropertyIfExistsInitializerDelegate del) => del(null, null, default(PropertyTag)));

		private delegate IEnumerator<FastTransferStateMachine?> SerializeInitializerDelegate(object instance, FastTransferDownloadContext context, IPropertyBag propertyBag, AnnotatedPropertyValue annotatedPropertyValue);

		private delegate IEnumerator<FastTransferStateMachine?> SerializeFixedSizePropertyInitializerDelegate(object instance, FastTransferDownloadContext context, PropertyValue propertyValue);

		private delegate IEnumerator<FastTransferStateMachine?> SerializeVariableSizePropertyInitializerDelegate(object instance, FastTransferDownloadContext context, PropertyTag propertyTag, Stream propertyReadStream);

		private delegate IEnumerator<FastTransferStateMachine?> DeserializeInitializerDelegate(object instance, FastTransferUploadContext context, IPropertyBag propertyBag);

		private delegate IEnumerator<FastTransferStateMachine?> DeserializeVariableSizePropertyInitializerDelegate(object instance, FastTransferUploadContext context, IPropertyBag propertyBag, PropertyTag propertyTag, int codePage);

		private delegate IEnumerator<FastTransferStateMachine?> SkipPropertyIfExistsInitializerDelegate(object instance, FastTransferUploadContext context, PropertyTag propertyTagToSkip);
	}
}
