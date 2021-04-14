using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IExchangeFormOwner
	{
		void OnExchangeFormLoad(ExchangeForm form);

		void OnExchangeFormClosed(ExchangeForm formToClose);
	}
}
