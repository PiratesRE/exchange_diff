using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface ITableCentricConfigurable
	{
		List<ReaderTaskProfile> BuildReaderTaskProfile();

		List<SaverTaskProfile> BuildSaverTaskProfile();

		List<DataObjectProfile> BuildDataObjectProfile();

		List<ColumnProfile> BuildColumnProfile();

		Dictionary<string, List<string>> BuildPageToDataObjectsMapping();

		bool CanEnableUICustomization();
	}
}
