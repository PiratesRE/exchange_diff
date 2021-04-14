using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class LinkClickedSignalStats
	{
		public LinkClickedSignalStats()
		{
			this.linkHash = "";
			this.userHash = "";
			this.nrRecipients = 0;
			this.nrDLs = 0;
			this.nrOpenDLs = 0;
			this.nrGroups = 0;
			this.nrOpenGroups = 0;
			this.isInternalLink = false;
			this.isSPURLValid = true;
			this.isValidSignal = false;
		}

		internal string GetLinkClickedSignalStatsLogString()
		{
			return string.Format("LinkClickedSignalStats:{0};{1};{2};{3};{4};{5};{6};{7};{8},{9}", new object[]
			{
				this.linkHash,
				this.userHash,
				this.nrRecipients.ToString(),
				this.nrDLs.ToString(),
				this.nrOpenDLs.ToString(),
				this.nrGroups.ToString(),
				this.nrOpenGroups.ToString(),
				this.isInternalLink.ToString(),
				this.isSPURLValid.ToString(),
				this.isValidSignal.ToString()
			});
		}

		internal static string GenerateObfuscatingHash(string input)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			string result;
			using (SHA256 sha = new SHA256Cng())
			{
				byte[] value = sha.ComputeHash(bytes);
				result = BitConverter.ToString(value).Replace("-", string.Empty);
			}
			return result;
		}

		internal string linkHash;

		internal string userHash;

		internal int nrRecipients;

		internal int nrDLs;

		internal int nrOpenDLs;

		internal int nrGroups;

		internal int nrOpenGroups;

		internal bool isInternalLink;

		internal bool isSPURLValid;

		internal bool isValidSignal;
	}
}
