using System;
using System.Collections;

namespace System.Security.Util
{
	[Serializable]
	internal class LocalSiteString : SiteString
	{
		public LocalSiteString(string site)
		{
			this.m_site = site.Replace('|', ':');
			if (this.m_site.Length > 2 && this.m_site.IndexOf(':') != -1)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDirectoryOnUrl"));
			}
			this.m_separatedSite = this.CreateSeparatedString(this.m_site);
		}

		private ArrayList CreateSeparatedString(string directory)
		{
			if (directory == null || directory.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDirectoryOnUrl"));
			}
			ArrayList arrayList = new ArrayList();
			string[] array = directory.Split(LocalSiteString.m_separators);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null || array[i].Equals(""))
				{
					if (i < 2 && directory[i] == '/')
					{
						arrayList.Add("//");
					}
					else if (i != array.Length - 1)
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDirectoryOnUrl"));
					}
				}
				else if (array[i].Equals("*"))
				{
					if (i != array.Length - 1)
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDirectoryOnUrl"));
					}
					arrayList.Add(array[i]);
				}
				else
				{
					arrayList.Add(array[i]);
				}
			}
			return arrayList;
		}

		public virtual bool IsSubsetOf(LocalSiteString operand)
		{
			return this.IsSubsetOf(operand, true);
		}

		public virtual bool IsSubsetOf(LocalSiteString operand, bool ignoreCase)
		{
			if (operand == null)
			{
				return false;
			}
			if (operand.m_separatedSite.Count == 0)
			{
				return this.m_separatedSite.Count == 0 || (this.m_separatedSite.Count > 0 && string.Compare((string)this.m_separatedSite[0], "*", StringComparison.Ordinal) == 0);
			}
			if (this.m_separatedSite.Count == 0)
			{
				return string.Compare((string)operand.m_separatedSite[0], "*", StringComparison.Ordinal) == 0;
			}
			return base.IsSubsetOf(operand, ignoreCase);
		}

		private new static char[] m_separators = new char[]
		{
			'/'
		};
	}
}
