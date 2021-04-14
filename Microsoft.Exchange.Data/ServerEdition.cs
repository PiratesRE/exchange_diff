using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal sealed class ServerEdition
	{
		public ServerEdition()
		{
		}

		public ServerEdition(ServerEditionValue serverType, ServerEditionValue setupType)
		{
			this.serverType = serverType;
			this.setupType = setupType;
		}

		public ServerEdition(string serverTypeInAD)
		{
			if (!string.IsNullOrEmpty(serverTypeInAD) && serverTypeInAD.Length <= 32)
			{
				this.DecryptServerType(serverTypeInAD);
			}
		}

		public bool IsStandard
		{
			get
			{
				return this.serverType == ServerEditionValue.Standard;
			}
		}

		public bool IsEnterprise
		{
			get
			{
				return this.serverType == ServerEditionValue.Enterprise;
			}
		}

		public bool IsCoexistence
		{
			get
			{
				return this.serverType == ServerEditionValue.Coexistence;
			}
		}

		public bool IsEvaluation
		{
			get
			{
				return this.setupType == ServerEditionValue.Evaluation;
			}
		}

		public override string ToString()
		{
			string result;
			if (this.IsEvaluation)
			{
				if (this.IsStandard)
				{
					result = DataStrings.StandardTrialEdition;
				}
				else if (this.IsEnterprise)
				{
					result = DataStrings.EnterpriseTrialEdition;
				}
				else if (this.IsCoexistence)
				{
					result = DataStrings.CoexistenceTrialEdition;
				}
				else
				{
					result = DataStrings.UnknownEdition;
				}
			}
			else if (this.IsStandard)
			{
				result = DataStrings.StandardEdition;
			}
			else if (this.IsEnterprise)
			{
				result = DataStrings.EnterpriseEdition;
			}
			else if (this.IsCoexistence)
			{
				result = DataStrings.CoexistenceEdition;
			}
			else
			{
				result = DataStrings.UnknownEdition;
			}
			return result;
		}

		private string EncryptServerType()
		{
			long value = DateTime.UtcNow.ToFileTimeUtc() & (long)((ulong)-1);
			string s = string.Format("0x{0:x8};0x{1:x8};0x{2:x8}", (int)this.serverType, Convert.ToUInt32(value), (int)this.setupType);
			byte[] bytes = Encoding.Unicode.GetBytes(s);
			byte[] array = bytes;
			int num = 0;
			array[num] ^= 75;
			for (int i = 1; i < 64; i++)
			{
				byte[] array2 = bytes;
				int num2 = i;
				array2[num2] ^= (bytes[i - 1] ^ 73);
			}
			return Encoding.Unicode.GetString(bytes, 0, bytes.Length);
		}

		private void DecryptServerType(string rawString)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(rawString);
			byte[] array = new byte[64];
			for (int i = 0; i < bytes.Length; i++)
			{
				array[i] = bytes[i];
			}
			for (int j = 64; j > 1; j--)
			{
				byte[] array2 = array;
				int num = j - 1;
				array2[num] ^= (array[j - 2] ^ 73);
			}
			byte[] array3 = array;
			int num2 = 0;
			array3[num2] ^= 75;
			string[] array4 = Encoding.Unicode.GetString(array, 0, array.Length).Split(new char[]
			{
				';'
			});
			int num3 = Convert.ToInt32(array4[0], 16);
			int num4 = Convert.ToInt32(array4[2], 16);
			if (Enum.IsDefined(typeof(ServerEditionValue), num3))
			{
				this.serverType = (ServerEditionValue)num3;
			}
			if (Enum.IsDefined(typeof(ServerEditionValue), num4))
			{
				this.setupType = (ServerEditionValue)num4;
			}
		}

		public static ServerEditionType DecryptServerEdition(string serverTypeInAD)
		{
			if (serverTypeInAD == null)
			{
				throw new ArgumentNullException("serverTypeInAD");
			}
			if (serverTypeInAD.Length != 32)
			{
				return ServerEditionType.Unknown;
			}
			ServerEdition serverEdition = new ServerEdition(serverTypeInAD);
			if (serverEdition.IsStandard && !serverEdition.IsEvaluation)
			{
				return ServerEditionType.Standard;
			}
			if (serverEdition.IsStandard && serverEdition.IsEvaluation)
			{
				return ServerEditionType.StandardEvaluation;
			}
			if (serverEdition.IsEnterprise && !serverEdition.IsEvaluation)
			{
				return ServerEditionType.Enterprise;
			}
			if (serverEdition.IsEnterprise && serverEdition.IsEvaluation)
			{
				return ServerEditionType.EnterpriseEvaluation;
			}
			if (serverEdition.IsCoexistence && !serverEdition.IsEvaluation)
			{
				return ServerEditionType.Coexistence;
			}
			if (serverEdition.IsCoexistence && serverEdition.IsEvaluation)
			{
				return ServerEditionType.CoexistenceEvaluation;
			}
			return ServerEditionType.Unknown;
		}

		public static string EncryptServerEdition(ServerEditionType edition)
		{
			ServerEditionValue serverEditionValue;
			ServerEditionValue serverEditionValue2;
			switch (edition)
			{
			case ServerEditionType.Standard:
				serverEditionValue = ServerEditionValue.Standard;
				serverEditionValue2 = ServerEditionValue.Standard;
				break;
			case ServerEditionType.StandardEvaluation:
				serverEditionValue = ServerEditionValue.Standard;
				serverEditionValue2 = ServerEditionValue.Evaluation;
				break;
			case ServerEditionType.Enterprise:
				serverEditionValue = ServerEditionValue.Enterprise;
				serverEditionValue2 = ServerEditionValue.Enterprise;
				break;
			case ServerEditionType.EnterpriseEvaluation:
				serverEditionValue = ServerEditionValue.Enterprise;
				serverEditionValue2 = ServerEditionValue.Evaluation;
				break;
			case ServerEditionType.Coexistence:
				serverEditionValue = ServerEditionValue.Coexistence;
				serverEditionValue2 = ServerEditionValue.Coexistence;
				break;
			case ServerEditionType.CoexistenceEvaluation:
				serverEditionValue = ServerEditionValue.Coexistence;
				serverEditionValue2 = ServerEditionValue.Evaluation;
				break;
			default:
				throw new ArgumentException(DataStrings.UnsupportServerEdition(edition.ToString()), "edition");
			}
			ServerEdition serverEdition = new ServerEdition(serverEditionValue, serverEditionValue2);
			return serverEdition.EncryptServerType();
		}

		private const int MaxRawStringLength = 32;

		private const byte seed = 73;

		private ServerEditionValue serverType = ServerEditionValue.None;

		private ServerEditionValue setupType = ServerEditionValue.None;
	}
}
