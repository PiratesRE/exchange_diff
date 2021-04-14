using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class FacebookImportContactsResult : IExtensibleDataObject
	{
		[DataMember(Name = "count", IsRequired = false)]
		public int ProcessedContacts { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
