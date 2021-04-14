using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class RecipientsODataConverter : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			return RecipientsODataConverter.ODataCollectionValueToRecipients((ODataCollectionValue)odataPropertyValue);
		}

		public object ToODataPropertyValue(object rawValue)
		{
			return RecipientsODataConverter.RecipientsToODataCollectionValue(((Recipient[])rawValue) ?? new Recipient[0]);
		}

		internal static Recipient[] ODataCollectionValueToRecipients(ODataCollectionValue collection)
		{
			IEnumerable<Recipient> source = from ODataComplexValue x in collection.Items
			select RecipientODataConverter.ODataValueToRecipient(x);
			return source.ToArray<Recipient>();
		}

		internal static ODataCollectionValue RecipientsToODataCollectionValue(Recipient[] recipients)
		{
			ODataCollectionValue odataCollectionValue = new ODataCollectionValue();
			odataCollectionValue.TypeName = typeof(Recipient).MakeODataCollectionTypeName();
			odataCollectionValue.Items = Array.ConvertAll<Recipient, ODataValue>(recipients, (Recipient x) => RecipientODataConverter.RecipientToODataValue(x));
			return odataCollectionValue;
		}
	}
}
