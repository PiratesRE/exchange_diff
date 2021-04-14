using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class QueueViewerInputObjectSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(ConfigObjectId), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ObjectState = new SimpleProviderPropertyDefinition("ObjectState", ExchangeObjectVersion.Exchange2010, typeof(ObjectState), PropertyDefinitionFlags.ReadOnly, Microsoft.Exchange.Data.ObjectState.New, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExchangeVersion = new SimpleProviderPropertyDefinition("ExchangeVersion", ExchangeObjectVersion.Exchange2010, typeof(ExchangeObjectVersion), PropertyDefinitionFlags.ReadOnly, ExchangeObjectVersion.Exchange2010, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition QueryFilter = new SimpleProviderPropertyDefinition("QueryFilter", ExchangeObjectVersion.Exchange2010, typeof(QueryFilter), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition QVObjectType = new SimpleProviderPropertyDefinition("QVObjectType", ExchangeObjectVersion.Exchange2010, typeof(QVObjectType), PropertyDefinitionFlags.None, Microsoft.Exchange.Rpc.QueueViewer.QVObjectType.QueueInfo, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SortOrder = new SimpleProviderPropertyDefinition("SortOrder", ExchangeObjectVersion.Exchange2010, typeof(QueueViewerSortOrderEntry), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SearchForward = new SimpleProviderPropertyDefinition("SearchForward", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PageSize = new SimpleProviderPropertyDefinition("PageSize", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 1000, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition BookmarkObject = new SimpleProviderPropertyDefinition("BookmarkObject", ExchangeObjectVersion.Exchange2010, typeof(IConfigurable), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition BookmarkIndex = new SimpleProviderPropertyDefinition("BookmarkIndex", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IncludeBookmark = new SimpleProviderPropertyDefinition("IncludeBookmark", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IncludeDetails = new SimpleProviderPropertyDefinition("IncludeDetails", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IncludeComponentLatencyInfo = new SimpleProviderPropertyDefinition("IncludeComponentLatencyInfo", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
