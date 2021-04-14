using System;
using System.Text;
using Microsoft.Office.Compliance.Audit;

namespace Microsoft.Exchange.Data.ApplicationLogic.AuditLog
{
	public class InvalidOrganizationException : AuditException
	{
		public InvalidOrganizationException(string organization)
		{
			this.Organization = organization;
		}

		public string Organization { get; private set; }

		public string DecodedOrganization
		{
			get
			{
				string result;
				try
				{
					byte[] bytes = Convert.FromBase64String(this.Organization);
					result = Encoding.UTF8.GetString(bytes);
				}
				catch (ArgumentException ex)
				{
					result = ex.Message;
				}
				catch (FormatException ex2)
				{
					result = ex2.Message;
				}
				return result;
			}
		}
	}
}
