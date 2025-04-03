using PemConverter.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PemConverter.Backend.LetsEncrypt.Entities;

namespace PemConverter.Services.Acme
{
    public interface IAcmeService
    {
        Task<ChallengeStore> CreateChallengeAsync(string fqdn, string email);

        Task<bool> ValidateChallengeAsync(ChallengeStore challengeStore);

        Task<Certificate> GenerateCertificateAsync(Account account, Order order, string certificateCommonName);
        
    }
}
