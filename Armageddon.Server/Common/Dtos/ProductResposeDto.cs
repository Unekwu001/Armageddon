namespace Armageddon.Server.Common.Dtos
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }

        public string ProductCode { get; set; } = string.Empty;

        public int ProductTypeId { get; set; }

        public decimal PricePerGram { get; set; }
    }
}
