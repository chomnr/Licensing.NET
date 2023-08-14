using License_Server.Services.License.LicenseException;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static License_Server.Services.Licensing.License;

namespace License_Server.Services.Licensing
{
    public class License
    {
        // ACTIVATED = PAYING
        // DEACTIVATED = PAID BUT DID NOT RENEW.
        // SUSPENDED = LICENSE WAS SUSPENDED BY SOMEONE.
        public enum LICENSE_STATUS { ACTIVATED, DEACTIVATED, SUSPENDED, UNCLAIMED }

        [Key]
        public int Id { get; set; }
        public string ProductId { get; set; } = "0";
        public string? Owner { get; set; }
        public string Key { get; set; }
        public long PurchaseDate { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public long Duration { get; set; } = 2592000000; // default duration for license 30 days.
        public LICENSE_STATUS Status { get; set; } = LICENSE_STATUS.ACTIVATED;
        public string KeyFormat { get; set; } = "XXXXX-XXXXX-XXXXX-XXXXX";
    }

    /// <summary>
    /// LicenseGenerator class.
    /// <br></br>
    /// <br></br>
    /// <example>
    /// For example:
    /// <code>
    /// License license = new LicenseGen()
    /// .SetProduct(productId)
    /// .SetOwner(sessionId)
    /// .Build()
    /// 
    /// License license = new LicenseGen()
    /// .SetFormat("XXX-XXX-XXX")
    /// .SetProduct(productId)
    /// .SetOwner(sessionId)
    /// .Build()
    /// </code>
    /// </example>
    /// </summary>
    public class LicenseGen
    {
        private License License { get; set; } = new License();

        /// <summary>
        /// Change the license key format. Default is XXXXX-XXXXX-XXXXX-XXXXX
        /// <br></br>
        /// <br></br>
        /// <example>
        /// For example:
        /// <code>
        /// LicenseGen gen = new();
        /// gen.SetFormat("XXX-XXX-XXX").create();
        /// </code>
        /// results in 1DA-313-DAA (arbitary values)
        /// </example>
        /// </summary>
        public LicenseGen SetFormat(string keyFormat)
        {
            // Note: 
            // The Regex should already check for white spaces. This piece of could is likely
            // redundant and could be considered as a failsafe.
            if (String.IsNullOrEmpty(keyFormat))
            {
                throw new BadLicenseKeyFormat();
            }
            // Ensures that there is at LEAST 3 X's, no white spaces and no characters other
            // than Xs and hyphens.
            if (!Regex.IsMatch(keyFormat, @"^(?:[^X]*X){3,}[^X\s]*$"))
            {
                throw new BadLicenseKeyFormat();
            }

            this.License.KeyFormat = keyFormat;
            return this;
        }

        /// <summary>
        /// Manually set the license key. <br></br> Might be slightly resource intensive, so please avoid using this.
        /// </summary>
        [Obsolete("This method is very dangerous do not use during production.")]
        public LicenseGen SetKey(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new NullReferenceException();
            }

            // Converting the key and format into a char array.
            char[] k = key.ToCharArray();
            char[] f = this.License.KeyFormat.ToCharArray();

            if (k.Length != f.Length)
            {
                throw new FormatAndKeyMismatch();
            }

            // Quick Hyphen Check
            int hK = key.Count(c => c.Equals("-"));
            int hF = this.License.KeyFormat.Count(c => c.Equals("-"));

            if (hK != hF)
            {
                throw new FormatAndKeyMismatch();
            }

            for (int i = 0; i < f.Length; i++)
            {
                // char[index] to string.
                string kL = k[i].ToString();
                string fL = f[i].ToString();

                if (fL.Equals("-") && !kL.Equals("-"))
                {
                    throw new FormatAndKeyMismatch();
                }

                if (string.Equals(fL, "X", StringComparison.OrdinalIgnoreCase))
                {
                    if (!Regex.IsMatch(kL, @"^[a-zA-Z0-9]+$"))
                    {
                        throw new FormatAndKeyMismatch();
                    }
                }
            }
            // The key passes all checks.
            this.License.Key = key;
            return this;
        }

        /// <summary>
        /// Set the product id of the license.
        /// </summary>
        public LicenseGen SetProduct(string identifier)
        {
            if (String.IsNullOrEmpty(identifier))
            {
                throw new NullReferenceException();
            }
            this.License.ProductId = identifier;
            return this;
        }

        /// <summary>
        /// Set the owner of the License.
        /// </summary>
        public LicenseGen SetOwner(string? owner)
        {
            this.License.Owner = owner;
            return this;
        }

        /// <summary>
        /// Set the duration of the license in milliseconds. default: 30 DAYS
        /// </summary>
        public LicenseGen SetDuration(long duration)
        {
            this.License.Duration = duration;
            return this;
        }

        /// <summary>
        /// Set's the status of a license.
        /// </summary>
        public LicenseGen SetStatus(LICENSE_STATUS status)
        {
            /*
            //todo: move to LicenseProcessor...
            // Ensures that a License's status can only be set as UNCLAIMED on inital creation.
            // You cannot go from ACTIVATED TO UNCLAIMED OR DEACTIVATED TO UNCLAIMED ETC;
            // But you can reapply an existing UNCLAIMED status to UNCLAIMED for whatever
            // reason.
            if (this.License.Status != LICENSE_STATUS.UNCLAIMED && status == LICENSE_STATUS.UNCLAIMED)
            {
                this.License.Status = status;
                return this;
            }
            // Ensures that licenses that go from a suspended to an activated state
            // do not use any days from day of suspension. By calling AutoSetPurchaseDate,
            // a new purchase date is set while keeping the duration the same.
            if (this.License.Status == LICENSE_STATUS.SUSPENDED && status == LICENSE_STATUS.ACTIVATED)
            {
                AutoSetPurchaseDate();
            }
            */
            this.License.Status = status;
            return this;
        }

        private void ApplyKeyRandomization()
        {
            char[] f = this.License.KeyFormat.ToCharArray();
            char[] k = new char[f.Length];

            Func<char> rand = () =>
            {
                string pool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Random random = new();
                return pool[random.Next(0, pool.Length)];
            };

            for (int i = 0; i < f.Length; i++)
            {
                // char[index] to string.
                string fL = f[i].ToString();

                if (fL.Equals("-"))
                {
                    k[i] = '-';
                }

                if (string.Equals(fL, "X", StringComparison.OrdinalIgnoreCase))
                {
                    k[i] = rand();
                }
            }
            this.License.Key = string.Join("", k);
        }

        public License Build()
        {
            // Make sure we do not set a purchase date for an unclaimed LICENSE.
            if (this.License.Status == LICENSE_STATUS.UNCLAIMED)
            {
                this.License.PurchaseDate = 0;
            }

            if (String.IsNullOrEmpty((this.License.Key)))
            {
                ApplyKeyRandomization();
            }
            return this.License;
        }
    }
}