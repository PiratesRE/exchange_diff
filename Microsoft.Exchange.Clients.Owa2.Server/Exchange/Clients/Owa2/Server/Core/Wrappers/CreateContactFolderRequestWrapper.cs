using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateContactFolderRequestWrapper
	{
		[DataMember(Name = "parentFolderId")]
		public BaseFolderId ParentFolderId { get; set; }

		[DataMember(Name = "displayName")]
		public string DisplayName { get; set; }

		[DataMember(Name = "priority")]
		public int Priority { get; set; }
	}
}
