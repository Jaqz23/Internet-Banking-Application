namespace IB.Core.Application.Helpers
{
    public static class UniqueIdGenerator
    {
        private static readonly Random _random = new Random();

        public static string GenerateUniqueId()
        {
            return _random.Next(100000000, 999999999).ToString();
        }
    }
}
