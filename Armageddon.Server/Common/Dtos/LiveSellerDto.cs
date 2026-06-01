namespace Armageddon.Server.Common.Dtos
{
    public class LiveSellerDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal Rating { get; set; }
    }
}
