using System.Security;

namespace PemConverter.Backend.LetsEncrypt.Extensions
{
    public static partial class Extensions
    {
        public static SecureString ToSecureString(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            else
            {
                SecureString result = new SecureString();
                foreach (char c in source.ToCharArray())
                {
                    result.AppendChar(c);
                }
                return result;
            }
        }
    }
}