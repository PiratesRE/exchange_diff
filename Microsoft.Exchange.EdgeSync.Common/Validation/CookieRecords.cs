using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	[Serializable]
	public class CookieRecords
	{
		public MultiValuedProperty<CookieRecord> Records
		{
			get
			{
				return this.cookieRecords;
			}
			set
			{
				this.cookieRecords = value;
			}
		}

		public void Load(Dictionary<string, Cookie> cookies)
		{
			foreach (Cookie cookie in cookies.Values)
			{
				CookieRecord cookieRecord = new CookieRecord();
				cookieRecord.BaseDN = cookie.BaseDN;
				cookieRecord.DomainController = cookie.DomainController;
				cookieRecord.LastUpdated = cookie.LastUpdated;
				cookieRecord.CookieLength = cookie.CookieValue.Length;
				this.cookieRecords.Add(cookieRecord);
			}
		}

		public override string ToString()
		{
			return "Number of cookies " + this.Records.Count;
		}

		private MultiValuedProperty<CookieRecord> cookieRecords = new MultiValuedProperty<CookieRecord>();
	}
}
