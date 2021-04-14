using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class UpdateData
	{
		internal UpdateData(RequestType RequestType)
		{
			this.RequestType = RequestType;
			this.ShadowMetadata = new List<PropertyMetaData>();
		}

		internal bool HasUpdates
		{
			get
			{
				return this.SourceUserConfigXMLStatus != UpdateData.SourceStatus.None || this.ShadowMetadata.Count != 0 || this.IsNtSecurityDescriptorChanged;
			}
		}

		internal readonly RequestType RequestType;

		internal bool IsNtSecurityDescriptorChanged;

		internal List<PropertyMetaData> ShadowMetadata;

		internal string SourceUserConfigXML;

		internal UpdateData.SourceStatus SourceUserConfigXMLStatus;

		internal enum SourceStatus
		{
			None,
			Updated,
			Removed
		}
	}
}
