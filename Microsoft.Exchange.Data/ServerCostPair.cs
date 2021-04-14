using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class ServerCostPair : IEquatable<ServerCostPair>
	{
		public ServerCostPair(Guid serverGuid, string serverName, int cost)
		{
			if (cost < 1 || cost > 100)
			{
				throw new ArgumentException(DataStrings.ErrorCostOutOfRange, "Cost");
			}
			if (serverGuid == Guid.Empty && string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentException(DataStrings.ErrorServerGuidAndNameBothEmpty, "ServerName");
			}
			this.serverGuid = serverGuid;
			this.serverName = serverName;
			this.cost = cost;
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public Guid ServerGuid
		{
			get
			{
				return this.serverGuid;
			}
		}

		public int Cost
		{
			get
			{
				return this.cost;
			}
		}

		public string Name
		{
			get
			{
				return this.ToString();
			}
		}

		public static ServerCostPair Parse(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			string[] array = input.Split(new char[]
			{
				':'
			});
			if (array.Length == 2)
			{
				string text = array[0];
				int num = int.Parse(array[1]);
				return new ServerCostPair(Guid.Empty, text, num);
			}
			throw new FormatException(DataStrings.ErrorInputFormatError);
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.ServerName))
			{
				return this.ServerName + ':' + this.Cost.ToString();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(this.ServerGuid.ToString());
			stringBuilder.Append("},");
			stringBuilder.Append(this.cost.ToString());
			return stringBuilder.ToString();
		}

		public static ServerCostPair ParseFromAD(string adString)
		{
			if (adString == null)
			{
				throw new ArgumentNullException("adString");
			}
			string[] array = adString.Split(new char[]
			{
				','
			});
			if (array.Length == 2 && array[0][0] == '{' && array[0][array[0].Length - 1] == '}')
			{
				string g = array[0].Substring(1, array[0].Length - 2);
				Guid guid = new Guid(g);
				int num = int.Parse(array[1]);
				return new ServerCostPair(guid, string.Empty, num);
			}
			throw new FormatException(DataStrings.ErrorADFormatError);
		}

		public string ToADString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(this.ServerGuid.ToString());
			stringBuilder.Append("},");
			stringBuilder.Append(this.cost.ToString());
			return stringBuilder.ToString();
		}

		public bool Equals(ServerCostPair other)
		{
			if (other == null)
			{
				return false;
			}
			if (this.cost != other.cost)
			{
				return false;
			}
			if (Guid.Empty != other.serverGuid)
			{
				return this.ServerGuid == other.ServerGuid;
			}
			return string.Equals(this.serverName, other.serverName, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ServerCostPair);
		}

		public override int GetHashCode()
		{
			if (this.serverGuid != Guid.Empty)
			{
				return this.ServerGuid.GetHashCode();
			}
			return this.ServerName.GetHashCode();
		}

		public static bool operator ==(ServerCostPair left, ServerCostPair right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(ServerCostPair left, ServerCostPair right)
		{
			return !(left == right);
		}

		public const int MinCostValue = 1;

		public const int MaxCostValue = 100;

		private string serverName;

		private Guid serverGuid;

		private int cost;
	}
}
