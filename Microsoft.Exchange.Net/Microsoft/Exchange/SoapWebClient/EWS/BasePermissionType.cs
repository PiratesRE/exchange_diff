using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[XmlInclude(typeof(PermissionType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(CalendarPermissionType))]
	[Serializable]
	public abstract class BasePermissionType
	{
		public UserIdType UserId;

		public bool CanCreateItems;

		[XmlIgnore]
		public bool CanCreateItemsSpecified;

		public bool CanCreateSubFolders;

		[XmlIgnore]
		public bool CanCreateSubFoldersSpecified;

		public bool IsFolderOwner;

		[XmlIgnore]
		public bool IsFolderOwnerSpecified;

		public bool IsFolderVisible;

		[XmlIgnore]
		public bool IsFolderVisibleSpecified;

		public bool IsFolderContact;

		[XmlIgnore]
		public bool IsFolderContactSpecified;

		public PermissionActionType EditItems;

		[XmlIgnore]
		public bool EditItemsSpecified;

		public PermissionActionType DeleteItems;

		[XmlIgnore]
		public bool DeleteItemsSpecified;
	}
}
