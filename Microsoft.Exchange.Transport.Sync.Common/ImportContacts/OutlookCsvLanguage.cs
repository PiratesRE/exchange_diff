using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.ImportContacts
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OutlookCsvLanguage
	{
		public OutlookCsvLanguage(int codePage, Dictionary<string, ImportContactProperties> columnsMapping, CultureInfo culture)
		{
			SyncUtilities.ThrowIfArgumentNull("columnsMapping", columnsMapping);
			SyncUtilities.ThrowIfArgumentNull("culture", culture);
			this.codePage = codePage;
			this.columnsMapping = columnsMapping;
			this.culture = culture;
		}

		public int CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		public Dictionary<string, ImportContactProperties> ColumnsMapping
		{
			get
			{
				return this.columnsMapping;
			}
		}

		private int codePage;

		private CultureInfo culture;

		private Dictionary<string, ImportContactProperties> columnsMapping;
	}
}
