using System;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADLogContext
	{
		internal int FileLine { get; set; }

		internal string MemberName { get; set; }

		internal string FilePath { get; set; }

		internal IActivityScope ActivityScope
		{
			get
			{
				IActivityScope result;
				try
				{
					result = (this.scope ?? ActivityContext.GetCurrentActivityScope());
				}
				catch (Exception ex)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_GetActivityContextFailed, this.GetCallerInformation(), new object[]
					{
						ex.ToString()
					});
					result = null;
				}
				return result;
			}
			set
			{
				this.scope = value;
			}
		}

		public string GetCallerInformation()
		{
			int num = 0;
			if (string.IsNullOrEmpty(this.FilePath) || (num = this.FilePath.LastIndexOf("\\")) <= 0)
			{
				return string.Empty;
			}
			IActivityScope activityScope = null;
			try
			{
				activityScope = (this.scope ?? ActivityContext.GetCurrentActivityScope());
			}
			catch
			{
			}
			if (activityScope != null && !string.IsNullOrEmpty(activityScope.Action))
			{
				return string.Format("{0}: Method {1}; Line {2}; Action {3}", new object[]
				{
					this.FilePath.Substring(num + 1),
					this.MemberName,
					this.FileLine,
					activityScope.Action
				});
			}
			return string.Format("{0}: Method {1}; Line {2}", this.FilePath.Substring(num + 1), this.MemberName, this.FileLine);
		}

		private IActivityScope scope;
	}
}
