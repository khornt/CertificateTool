
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public static class CertificateGenerator
{
    public static void Generate(
        string outputDirectory,
        string hostname,
        string rootCaPassword,
        string pfxPassword,
        int validDays = 2825)
    {
        Directory.CreateDirectory(outputDirectory);

        var now = DateTimeOffset.UtcNow;
        var notBefore = now.AddMinutes(-5);
        var notAfter = now.AddDays(validDays);

        // 1. Create Root CA private/public key pair
        using var rootKey = RSA.Create(2048);

        var rootRequest = new CertificateRequest(
            "CN=Locus OHV Test CA, OU=OHV, O=Locus Solutions AS, L=Sandefjord, S=Vestfold, C=NO",
            rootKey,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        rootRequest.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(true, false, 0, true));

        rootRequest.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.KeyCertSign |
                X509KeyUsageFlags.CrlSign,
                true));

        rootRequest.CertificateExtensions.Add(
            new X509SubjectKeyIdentifierExtension(
                rootRequest.PublicKey, false));

        using var rootCert = rootRequest.CreateSelfSigned(
            notBefore, notAfter);

        // 2. Export Root CA certificate and encrypted private key
        File.WriteAllText(
            Path.Combine(outputDirectory, "LocusOHVTestRootCaSert.pem"),
            rootCert.ExportCertificatePem());

        File.WriteAllText(
            Path.Combine(outputDirectory, "LocusOHVTestRootCa.key"),
            rootKey.ExportEncryptedPkcs8PrivateKeyPem(
                rootCaPassword,
                new PbeParameters(
                    PbeEncryptionAlgorithm.Aes256Cbc,
                    HashAlgorithmName.SHA256,
                    100_000)));

        // 3. Create a separate server private/public key pair
        using var serverKey = RSA.Create(2048);

        var serverRequest = new CertificateRequest(
            $"CN={hostname}, OU=OHV, O=Locus Solutions AS, L=Sandefjord, S=Vestfold, C=NO",
            serverKey,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        serverRequest.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, true));

        serverRequest.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature |
                X509KeyUsageFlags.KeyEncipherment,
                true));

        var eku = new OidCollection
        {
            new("1.3.6.1.5.5.7.3.1") // Server Authentication
        };

        serverRequest.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(eku, false));

        var san = new SubjectAlternativeNameBuilder();
        san.AddDnsName(hostname);

        serverRequest.CertificateExtensions.Add(san.Build());

        serverRequest.CertificateExtensions.Add(
            new X509SubjectKeyIdentifierExtension(
                serverRequest.PublicKey, false));

        // 4. Sign the server certificate using the Root CA
        var serialNumber = RandomNumberGenerator.GetBytes(16);
        serialNumber[0] &= 0x7F;

        using var signedCert = serverRequest.Create(
            rootCert,
            notBefore,
            notAfter,
            serialNumber);

        using var serverCert =
            signedCert.CopyWithPrivateKey(serverKey);

        // 5. Export server certificate
        File.WriteAllText(
            Path.Combine(outputDirectory, $"{hostname}.pem"),
            serverCert.ExportCertificatePem());

        // 6. Export server public key
        File.WriteAllText(
            Path.Combine(outputDirectory, $"{hostname}.public.pem"),
            serverKey.ExportSubjectPublicKeyInfoPem());

        // 7. Export encrypted server private key
        File.WriteAllText(
            Path.Combine(outputDirectory, $"{hostname}.key"),
            serverKey.ExportEncryptedPkcs8PrivateKeyPem(
                pfxPassword,
                new PbeParameters(
                    PbeEncryptionAlgorithm.Aes256Cbc,
                    HashAlgorithmName.SHA256,
                    100_000)));

        // 8. Create PFX including server certificate,
        // server private key, and public Root CA certificate
        using var publicRootCert = X509Certificate2.CreateFromPem(
            rootCert.ExportCertificatePem());

        var certificates = new X509Certificate2Collection
        {
            serverCert,
            publicRootCert
        };

        byte[] pfx = certificates.Export(
            X509ContentType.Pkcs12,
            pfxPassword)!;

        File.WriteAllBytes(
            Path.Combine(outputDirectory, $"{hostname}.pfx"),
            pfx);

        CryptographicOperations.ZeroMemory(pfx);
    }
}
