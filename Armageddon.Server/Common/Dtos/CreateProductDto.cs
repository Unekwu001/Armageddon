namespace Armageddon.Server.Common.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty; 
        public string ProductCode { get; set; } = string.Empty;

        public int ProductTypeId { get; set; }

        public decimal PricePerGram { get; set; }
        public string Description { get; set; } = string.Empty; 

        public string ImageUrl { get; set; } = string.Empty;    
    }
}
