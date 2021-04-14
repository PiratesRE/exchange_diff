using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public static class DxStoreConstants
	{
		public const string ServiceNamespace = "http://www.outlook.com/highavailability/dxstore/v1/";

		public const string DefaultGroupIdentifier = "B1563499-EA40-4101-A9E6-59A8EB26FF1E";

		public const string DxStoreAccessName = "Access";

		public const string DxStoreInstanceName = "Instance";

		public const string DxStoreManagerName = "Manager";

		public const string RootKeyName = "\\";

		public const string SnapshotElementName = "SnapshotRoot";

		public const string ContainerRootTag = "Root";

		public const string PrivateSectionLabel = "Private";

		public const string PublicSectionLabel = "Public";

		public const string DefaultGroupNameProperty = "DefaultGroupName";

		public const int DefaultEndPointPortNumber = 808;

		public const string DefaultEndPointProtocolName = "net.tcp";

		public const string DefaultManagerBaseKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\DxStore";

		public const string DefaultSnapshotFileName = "DxStoreSnapshot.xml";

		public const int InstanceProcessExitDelayInMs = 500;
	}
}
