using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class LinkedUserTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			Command command = new Command("Get-LinkedUser");
			Collection<PSObject> collection = base.ExecuteCmdlet(powershell, command);
			if (collection.Count > 0)
			{
				string stringValue = collection[0].GetStringValue("Name");
				ArrayList propertyValue = collection[0].GetPropertyValue("CertificateSubject", null);
				try
				{
					string text = string.Format("X509:<I>CN=Issuer{0}<S>CN=Subject{0}", Guid.NewGuid().ToString().Remove(8));
					base.ExecuteCmdlet(powershell, new Command("Set-LinkedUser")
					{
						Parameters = 
						{
							{
								"Identity",
								stringValue
							},
							{
								"CertificateSubject",
								text
							}
						}
					});
					collection = base.ExecuteCmdlet(powershell, new Command("Get-LinkedUser")
					{
						Parameters = 
						{
							{
								"Identity",
								stringValue
							}
						}
					});
					if (collection.Count <= 0)
					{
						throw new ApplicationException("Get-LinkedUser returns no result");
					}
					string stringValue2 = collection[0].GetStringValue("CertificateSubject");
					if (string.Compare(text, stringValue2) != 0)
					{
						throw new ApplicationException(string.Format("Set-LinkedUser failed, Expected CertificateSubject={0} Actual={1}", text, stringValue2));
					}
				}
				finally
				{
					base.ExecuteCmdlet(powershell, new Command("Set-LinkedUser")
					{
						Parameters = 
						{
							{
								"Identity",
								stringValue
							},
							{
								"CertificateSubject",
								propertyValue
							}
						}
					});
				}
			}
		}
	}
}
