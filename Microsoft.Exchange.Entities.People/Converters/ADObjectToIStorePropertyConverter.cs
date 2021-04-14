using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.People.Converters
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ADObjectToIStorePropertyConverter
	{
		internal ADObjectToIStorePropertyConverter(ITracer tracer, PropertyDefinition[] propertyDefinitions)
		{
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("propertyDefinitions", propertyDefinitions);
			this.tracer = tracer;
			this.propertyDefinitions = propertyDefinitions;
			this.hashSetPropertyDefinitions = new HashSet<PropertyDefinition>(propertyDefinitions);
		}

		internal IEnumerable<IStorePropertyBag> ConvertAdObjectsToIStorePropertyBags(IEnumerable<Result<ADRawEntry>> objectsToConvert)
		{
			ArgumentValidator.ThrowIfNull("adObjectsToConvert", objectsToConvert);
			List<IStorePropertyBag> list = new List<IStorePropertyBag>();
			foreach (Result<ADRawEntry> result in objectsToConvert)
			{
				if (result.Data == null)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "ADObjectToIStorePropertyConverter.ConvertAdObjectsToIStorePropertyBags: adRawEntry.Data is null, ignore and continue.");
				}
				else
				{
					IStorePropertyBag storePropertyBag = this.ConvertADObjectToIStorePropertyBag(result.Data);
					if (storePropertyBag != null)
					{
						list.Add(storePropertyBag);
					}
					else
					{
						this.tracer.TraceError<ADRawEntry>((long)this.GetHashCode(), "ADObjectToIStorePropertyConverter.ConvertAdObjectsToIStorePropertyBags: Conversion of AD Raw Entry object {0} resulted in null IStorePropertyBag", result.Data);
					}
				}
			}
			return list;
		}

		private IStorePropertyBag ConvertADObjectToIStorePropertyBag(ADRawEntry objectToConvert)
		{
			ADPersonToContactConverterSet adpersonToContactConverterSet = ADPersonToContactConverterSet.FromContactProperties(this.propertyDefinitions, this.hashSetPropertyDefinitions);
			return adpersonToContactConverterSet.Convert(objectToConvert);
		}

		private readonly ITracer tracer;

		private readonly PropertyDefinition[] propertyDefinitions;

		private readonly HashSet<PropertyDefinition> hashSetPropertyDefinitions;
	}
}
