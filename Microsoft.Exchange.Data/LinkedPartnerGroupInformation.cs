using System;

namespace Microsoft.Exchange.Data
{
	internal class LinkedPartnerGroupInformation : IEquatable<LinkedPartnerGroupInformation>
	{
		internal LinkedPartnerGroupInformation()
		{
			this.linkedPartnerGroupInfoTokens = new string[2];
			this.LinkedPartnerGroupId = string.Empty;
			this.LinkedPartnerOrganizationId = string.Empty;
		}

		private LinkedPartnerGroupInformation(string linkedPartnerGroupAndOrganizationId)
		{
			if (linkedPartnerGroupAndOrganizationId == null)
			{
				throw new ArgumentNullException("linkedPartnerGroupAndOrganizationId");
			}
			this.linkedPartnerGroupInfoTokens = LinkedPartnerGroupInformation.GetLinkedPartnerIdValues(linkedPartnerGroupAndOrganizationId);
		}

		public string LinkedPartnerGroupId
		{
			get
			{
				return this.linkedPartnerGroupInfoTokens[0];
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("LinkedPartnerGroupId");
				}
				this.linkedPartnerGroupInfoTokens[0] = value;
			}
		}

		public string LinkedPartnerOrganizationId
		{
			get
			{
				return this.linkedPartnerGroupInfoTokens[1];
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("LinkedPartnerOrganizationId");
				}
				this.linkedPartnerGroupInfoTokens[1] = value;
			}
		}

		public bool IsValidADObject
		{
			get
			{
				return !string.IsNullOrEmpty(this.LinkedPartnerGroupId) && !string.IsNullOrEmpty(this.LinkedPartnerOrganizationId);
			}
		}

		public static LinkedPartnerGroupInformation Parse(string linkedPartnerAndOrganizationId)
		{
			return new LinkedPartnerGroupInformation(linkedPartnerAndOrganizationId);
		}

		public override string ToString()
		{
			return string.Format("{0}{1}{2}", this.linkedPartnerGroupInfoTokens[0], ":", this.linkedPartnerGroupInfoTokens[1]);
		}

		public bool Equals(LinkedPartnerGroupInformation other)
		{
			return other != null && this.LinkedPartnerOrganizationId.Equals(other.LinkedPartnerOrganizationId) && this.LinkedPartnerGroupId.Equals(other.LinkedPartnerGroupId);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as LinkedPartnerGroupInformation);
		}

		public override int GetHashCode()
		{
			return this.LinkedPartnerGroupId.GetHashCode();
		}

		private static string[] GetLinkedPartnerIdValues(string linkedPartnerIdAndOrganizationId)
		{
			string[] array = linkedPartnerIdAndOrganizationId.Split(new string[]
			{
				":"
			}, StringSplitOptions.None);
			if (array.Length == 2)
			{
				return array;
			}
			if (array.Length == 0)
			{
				return new string[]
				{
					string.Empty,
					string.Empty
				};
			}
			if (array.Length == 1)
			{
				if (linkedPartnerIdAndOrganizationId.StartsWith(":"))
				{
					return new string[]
					{
						string.Empty,
						array[0]
					};
				}
				if (linkedPartnerIdAndOrganizationId.EndsWith(":"))
				{
					return new string[]
					{
						array[0],
						string.Empty
					};
				}
			}
			throw new ArgumentException(DataStrings.LinkedPartnerGroupInformationInvalidParameter(linkedPartnerIdAndOrganizationId));
		}

		private const string PartnerGroupIdSeperator = ":";

		private const int InformationTokensCount = 2;

		private const int PartnerGroupIdIndex = 0;

		private const int PartnerOrganizationIdIndex = 1;

		private string[] linkedPartnerGroupInfoTokens;
	}
}
