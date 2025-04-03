using System.IO;
using System.Security.Cryptography.X509Certificates;
using PemConverter.Models.Jwk;


namespace PemConverter.Models.Jwk
{
    public class PubKeyHandler
    {
        public string GetPublicKey(string filename)
        {
            var pubKey = File.ReadAllText(filename);
            return pubKey;
        }

        public string? GetPublicKeyFromSertificate(string kid)
        {
            X509Certificate2 cert;
            
            using (var certificateStore = new X509Store(StoreLocation.LocalMachine))
            {                                               
                certificateStore.Open(OpenFlags.ReadOnly);
                var certificate = certificateStore.Certificates.OfType<X509Certificate2>().FirstOrDefault(x => x.Thumbprint != null && string.Equals(x.Thumbprint, kid, StringComparison.CurrentCultureIgnoreCase));

                if (certificate == null ) return null;

                cert = certificate;
            }
                       

            try
            {
                var rsaPublicKey = cert.PublicKey.GetRSAPublicKey();

                if (rsaPublicKey == null) return null;

                var rsaPublicKeyPem = rsaPublicKey.ExportRSAPublicKeyPem();

                if (!string.IsNullOrEmpty(rsaPublicKeyPem))
                {
                    return rsaPublicKeyPem;
                }
                return "";
            }
            catch (Exception) 
            {
                return null;
            }                                     
        }      
    }
}   

