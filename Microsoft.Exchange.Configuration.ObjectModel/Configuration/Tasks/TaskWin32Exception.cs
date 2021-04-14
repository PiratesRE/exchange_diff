using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TaskWin32Exception : LocalizedException
	{
		public TaskWin32Exception(Win32Exception e) : this((e != null) ? Strings.ErrorTaskWin32Exception(e.Message) : Strings.ErrorTaskWin32Exception(string.Empty), e)
		{
		}

		public TaskWin32Exception(LocalizedString message, Win32Exception e) : base(message, e)
		{
		}

		protected TaskWin32Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public static TaskWin32Exception FromErrorCodeAndVerbose(int error, LocalizedString verbose)
		{
			Win32Exception ex = new Win32Exception(error);
			return new TaskWin32Exception(Strings.ErrorTaskWin32ExceptionVerbose(ex.Message, verbose), ex);
		}
	}
}
