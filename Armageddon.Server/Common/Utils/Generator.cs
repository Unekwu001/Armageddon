namespace Armageddon.Server.Common.Utils
{
    public static class Generator
    {
        private static readonly char[] _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        public static string GenerateUserCode(int length = 10)
        {
            var result = new char[length];

            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();

            var buffer = new byte[length];
            rng.GetBytes(buffer);

            for (int i = 0; i < length; i++)
            {
                result[i] = _chars[buffer[i] % _chars.Length];
            }

            return new string(result).ToUpperInvariant();
        }

    }
}
