using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.Aggregation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ImportContactsResult : BaseRow
	{
		public ImportContactsResult(ImportContactListResult result) : base(result)
		{
			this.ImportedContactsResult = result;
		}

		public ImportContactListResult ImportedContactsResult { get; private set; }

		[DataMember]
		public int ContactsImported
		{
			get
			{
				return this.ImportedContactsResult.ContactsImported;
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
