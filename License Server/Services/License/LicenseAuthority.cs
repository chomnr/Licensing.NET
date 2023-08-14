using License_Server.Services.License;
using License_Server.Services.Licensing;
using License_Server.Services.Licensing.Rules;
using Licensing_Server.Services.Licensing;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace License_Server.Services.Licensing
{
    public enum AUTHORITY_STATE {
        APPROVED,
        PENDING,
        REJECTED 
    }

    public interface ILicenseAuthority {
        LicenseAuthority AddRules(IAuthorityRule[] rules);
        Task<LicenseResult> RunOn(LicenseLookUp license);
    }

    public class LicenseAuthority : ILicenseAuthority
    {
        private ILicenseProcessor Processor;

        private List<IAuthorityRule> Rules = new();
        private LicenseResult Result;

        public LicenseAuthority(ILicenseProcessor processor)
        {
            this.Processor = processor;
            this.Result = new LicenseResult();
        }

        public LicenseAuthority AddRules(IAuthorityRule[] rules)
        {
            for (int i = 0; i < rules.Length; i++)
            {
                Rules.Add(rules[i]);
            }
            return this;
        }

        public async Task<LicenseResult> RunOn(LicenseLookUp lookUp)
        {
            License? license = Processor.FindLicense(lookUp);
            if (license == null)
            {
                return await Task.FromResult(new LicenseResult(null, AUTHORITY_STATE.REJECTED));
            }

            for (int i = 0; i < Rules.Count; i++ )
            {
                Result = Rules[i].Execute(license);
                if (Result.AuthorityState == AUTHORITY_STATE.REJECTED)
                {
                    // Immediately stop running the rules when a rejection occurs.
                    break;
                }
            }
            return await Task.FromResult(Result);
        }
    }
}