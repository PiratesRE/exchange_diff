using System;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class MonadProvider : IMonadDataProvider
	{
		internal MonadProvider()
		{
			this.monadConnection = new MonadConnection("pooled=false");
		}

		public object[] ExecuteCommand(string command)
		{
			if (command == string.Empty)
			{
				throw new ArgumentNullException("command");
			}
			object[] result = null;
			lock (this.monadConnection)
			{
				try
				{
					this.monadConnection.Open();
					using (MonadCommand monadCommand = new MonadCommand(command, this.monadConnection))
					{
						monadCommand.CommandType = CommandType.Text;
						result = monadCommand.Execute();
					}
				}
				finally
				{
					if (this.monadConnection.State == ConnectionState.Open)
					{
						this.monadConnection.Close();
					}
				}
			}
			return result;
		}

		private MonadConnection monadConnection;
	}
}
