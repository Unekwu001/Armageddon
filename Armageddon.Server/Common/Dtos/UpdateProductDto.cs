namespace Armageddon.Server.Common.Dtos
{
    public class UpdateProductDto
    {

        public string ProductCode { get; set; } = string.Empty;

        public int ProductTypeId { get; set; }

        public decimal PricePerGram { get; set; }
    }
}
