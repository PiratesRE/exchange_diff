using System;
using System.ComponentModel;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IButtonBaseBulkEditSupport : IOwnerDrawBulkEditSupport, IBulkEditSupport
	{
		event HandledEventHandler CheckedChangedRaising;

		event HandledEventHandler Entering;

		bool CheckedValue { get; set; }
	}
}
