using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws
{
	[ComVisible(true)]
	internal class RwsCryptographyHelper
	{
		public static string Decrypt(string data)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(data))
			{
				using (SymmetricAlgorithm symmetricAlgorithm = new AesCryptoServiceProvider())
				{
					byte[] array = Convert.FromBase64String(data);
					if (array.Length < symmetricAlgorithm.IV.Length)
					{
						throw new Exception(string.Concat(new object[]
						{
							"Data less data (",
							array.Length,
							") than the length of the initialization vector (",
							symmetricAlgorithm.IV.Length,
							")"
						}));
					}
					symmetricAlgorithm.Key = RwsCryptographyHelper.key;
					byte[] array2 = new byte[symmetricAlgorithm.IV.Length];
					Array.Copy(array, array2, array2.Length);
					symmetricAlgorithm.IV = array2;
					using (ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor())
					{
						result = new string(Encoding.Unicode.GetChars(cryptoTransform.TransformFinalBlock(array, symmetricAlgorithm.IV.Length, array.Length - symmetricAlgorithm.IV.Length)));
					}
				}
			}
			return result;
		}

		public static string Encrypt(string text)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(text))
			{
				using (SymmetricAlgorithm symmetricAlgorithm = new AesCryptoServiceProvider())
				{
					symmetricAlgorithm.Key = RwsCryptographyHelper.key;
					symmetricAlgorithm.GenerateIV();
					using (ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor())
					{
						byte[] bytes = Encoding.Unicode.GetBytes(text);
						byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
						byte[] array2 = new byte[array.Length + symmetricAlgorithm.IV.Length];
						symmetricAlgorithm.IV.CopyTo(array2, 0);
						array.CopyTo(array2, symmetricAlgorithm.IV.Length);
						result = Convert.ToBase64String(array2);
					}
				}
			}
			return result;
		}

		private static byte[] key = new byte[]
		{
			38,
			220,
			byte.MaxValue,
			0,
			173,
			237,
			122,
			238,
			197,
			254,
			7,
			175,
			77,
			8,
			34,
			60
		};
	}
}
