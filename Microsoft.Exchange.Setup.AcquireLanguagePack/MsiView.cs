using System;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class MsiView : MsiBase
	{
		public MsiView(MsiDatabase database, string query)
		{
			this.OpenView(database, query);
		}

		public MsiRecord FetchNextRecord()
		{
			return new MsiRecord(this);
		}

		private void OpenView(MsiDatabase database, string query)
		{
			ValidationHelper.ThrowIfNull(database, "database");
			ValidationHelper.ThrowIfNullOrEmpty(query, "query");
			SafeMsiHandle safeMsiHandle;
			uint num = MsiNativeMethods.DatabaseOpenView(database.Handle, query, out safeMsiHandle);
			MsiHelper.ThrowIfNotSuccess(num);
			num = MsiNativeMethods.ViewExecute(safeMsiHandle, IntPtr.Zero);
			if (num != 0U)
			{
				safeMsiHandle.Dispose();
				throw new MsiException(num);
			}
			base.Handle = safeMsiHandle;
		}
	}
}
