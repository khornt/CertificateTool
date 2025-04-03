using PemConverter.Backend.LetsEncrypt.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PemConverter.Models;
using PemConverter.Models.Dto;
using PemConverter.Backend.LetsEncrypt.Entities;
using Org.BouncyCastle.Asn1.Cmp;
using System.Security.Principal;
using System.Windows;
using Org.BouncyCastle.Security;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace PemConverter.Services.Acme
{
    public class AcmeService : IAcmeService
    {
        private readonly AcmeClient _acmeClient;
        private readonly AppConfig _config;


        public AcmeService(AcmeClient acmeClient, AppConfig config)
        {
            _acmeClient = acmeClient;
            _config = config;

        }


        public async Task<ChallengeStore> CreateChallengeAsync(string fqdn, string email)
        {

            var numberOfPeriods = fqdn.Split('.').Length - 1;

            if (numberOfPeriods == 0)
            {
                throw new InvalidParameterException("Ække noe punktum i angitt fqdn");
            } else if (numberOfPeriods == 1)
            {
                throw new InvalidParameterException("Ække noe hostname i angitt fqdn");
            } else if (numberOfPeriods > 2)
            {
                throw new InvalidParameterException("Støtter ikke subdomene");
            }


            var pieces = fqdn.Split(new[] { '.' }, 2);
            
            var hostName = pieces[0];
            var domainName = pieces[1];


            //Få denne dumma ned til en enkelt string hele veien
            List<string> fqdnList = new List<string>() { fqdn };


            var account = await _acmeClient.CreateNewAccountAsync(email);
            var order = await _acmeClient.NewOrderAsync(account, fqdnList);
            var challenges = await _acmeClient.GetDnsChallenges(account, order);

            var challenge = new ChallengeStore
            {
                Challenges = challenges,
                Account = account,
                Order = order,
            };


            return challenge;
        }

        public async Task<bool> ValidateChallengeAsync(ChallengeStore challengeStore)
        {
            

                if (challengeStore.Account == null)
                {
                    throw new InvalidOperationException(message:"Missing Account");
                }

                if (challengeStore.Order == null)
                {
                    throw new InvalidOperationException(message: "Missing Order");
                }
                

                foreach (var item in challengeStore.Challenges)
                {
                    await _acmeClient.ValidateChallengeAsync(challengeStore.Account, item);

                    // Verify status of challenge
                    var freshChallenge = await _acmeClient.GetChallengeAsync(challengeStore.Account, item);
                    if (freshChallenge.Status == ChallengeStatus.Invalid)
                    {
                        throw new Exception("Something is wrong with your DNS TXT record(s)!");
                    }
                }

                return true;

        }

        public async Task<Certificate> GenerateCertificateAsync(Account account, Order order, string certificateCommonName)
        {
            var certificate = await _acmeClient.GenerateCertificateAsync(account, order, certificateCommonName);

            return certificate;
        }
    }
}
