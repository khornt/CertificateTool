using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PemConverter.Models
{
    public class AppConfig
    {
        public string ContactEmail => GetValue(nameof(ContactEmail));


        //public List<string> Domains
        //{
        //    get
        //    {
        //        var domains = GetValue(nameof(Domains));
        //        var result = new List<string>();
        //        foreach (var item in domains.Split(','))
        //        {
        //            result.Add(item.Trim());
        //        }

        //        return result;
        //    }
        //}

        //public string CertificateFileName => GetValue(nameof(CertificateFileName));
        public string CertificatePassword => GetValue(nameof(CertificatePassword));


        public List<string> RsaKeys
        {
            get
            {
                var listAsString = GetValue(nameof(RsaKeys)); ; //?? throw new ConfigurationErrorsException($"Missing app.config for rsaKeys");
                return listAsString.Split(",").ToList();
            }
        }

        private string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

