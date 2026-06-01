namespace Armageddon.Server.Common.Dtos
{
    public class SellerLocationDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsOnline { get; set; }
        public double DistanceKm { get; set; }
        public decimal Rating { get; set; }
    }
}
