using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ResolveNamesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ResolveNamesRequest : BaseRequest
	{
		[DataMember(Name = "ReturnFullContactData", IsRequired = false, EmitDefaultValue = false, Order = 1)]
		[XmlAttribute("ReturnFullContactData")]
		public bool ReturnFullContactData
		{
			get
			{
				return this.returnFullContactData;
			}
			set
			{
				this.returnFullContactData = value;
			}
		}

		[IgnoreDataMember]
		[XmlAttribute("SearchScope")]
		public ResolveNamesSearchScopeType SearchScope
		{
			get
			{
				return this.searchScope;
			}
			set
			{
				this.searchScope = value;
			}
		}

		[DataMember(Name = "SearchScope", IsRequired = false, EmitDefaultValue = false, Order = 2)]
		[XmlIgnore]
		public string SearchScopeString
		{
			get
			{
				return EnumUtilities.ToString<ResolveNamesSearchScopeType>(this.searchScope);
			}
			set
			{
				this.searchScope = EnumUtilities.Parse<ResolveNamesSearchScopeType>(value);
			}
		}

		[XmlAttribute("ContactDataShape")]
		[IgnoreDataMember]
		public ShapeEnum ContactDataShape
		{
			get
			{
				return this.contactDataShape;
			}
			set
			{
				this.contactDataShape = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "ContactDataShape", IsRequired = false, EmitDefaultValue = false, Order = 3)]
		public string ContactDataShapeString
		{
			get
			{
				return EnumUtilities.ToString<ShapeEnum>(this.contactDataShape);
			}
			set
			{
				this.contactDataShape = EnumUtilities.Parse<ShapeEnum>(value);
			}
		}

		[DataMember(Name = "UnresolvedEntry", IsRequired = true, Order = 4)]
		[XmlElement("UnresolvedEntry", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Type = typeof(string))]
		public string UnresolvedEntry
		{
			get
			{
				return this.unresolvedEntry;
			}
			set
			{
				this.unresolvedEntry = value;
			}
		}

		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "ParentFolderIds", IsRequired = false, Order = 5)]
		[XmlArray("ParentFolderIds")]
		public BaseFolderId[] ParentFolderIds { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new ResolveNames(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.ParentFolderIds != null)
			{
				return BaseRequest.GetServerInfoForFolderIdList(callContext, this.ParentFolderIds);
			}
			return callContext.GetServerInfoForEffectiveCaller();
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		protected override List<ServiceObjectId> GetAllIds()
		{
			if (this.ParentFolderIds != null)
			{
				return new List<ServiceObjectId>(this.ParentFolderIds);
			}
			return null;
		}

		internal const string ElementNameResolveNames = "ResolveNames";

		internal const string ElementNameUnresolvedEntry = "UnresolvedEntry";

		internal const string ElementNameParentFolderIds = "ParentFolderIds";

		internal const string AttributeNameReturnFullContactData = "ReturnFullContactData";

		internal const string AttributeNameSearchScope = "SearchScope";

		internal const string AttributeNameContactDataShape = "ContactDataShape";

		private bool returnFullContactData;

		private ShapeEnum contactDataShape = ShapeEnum.Default;

		private string unresolvedEntry;

		private ResolveNamesSearchScopeType searchScope = ResolveNamesSearchScopeType.ActiveDirectoryContacts;
	}
}
