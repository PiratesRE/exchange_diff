using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DataMoveReplicationConstraintParameter
	{
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintNone)]
		None,
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintSecondCopy)]
		SecondCopy,
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintSecondDatacenter)]
		SecondDatacenter = 3,
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintAllDatacenters)]
		AllDatacenters,
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintAllCopies)]
		AllCopies,
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintCINoReplication)]
		CINoReplication = 65536,
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintCISecondCopy)]
		CISecondCopy,
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintCISecondDatacenter)]
		CISecondDatacenter = 65539,
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintCIAllDatacenters)]
		CIAllDatacenters,
		[LocDescription(DirectoryStrings.IDs.DataMoveReplicationConstraintCIAllCopies)]
		CIAllCopies
	}
}
