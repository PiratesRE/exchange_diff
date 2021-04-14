using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ManagedFolderInformationType
	{
		public bool CanDelete;

		[XmlIgnore]
		public bool CanDeleteSpecified;

		public bool CanRenameOrMove;

		[XmlIgnore]
		public bool CanRenameOrMoveSpecified;

		public bool MustDisplayComment;

		[XmlIgnore]
		public bool MustDisplayCommentSpecified;

		public bool HasQuota;

		[XmlIgnore]
		public bool HasQuotaSpecified;

		public bool IsManagedFoldersRoot;

		[XmlIgnore]
		public bool IsManagedFoldersRootSpecified;

		public string ManagedFolderId;

		public string Comment;

		public int StorageQuota;

		[XmlIgnore]
		public bool StorageQuotaSpecified;

		public int FolderSize;

		[XmlIgnore]
		public bool FolderSizeSpecified;

		public string HomePage;
	}
}
