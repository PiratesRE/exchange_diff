using System;

namespace AjaxControlToolkit
{
	public interface IClientStateManager
	{
		bool SupportsClientState { get; }

		void LoadClientState(string clientState);

		string SaveClientState();
	}
}
