namespace Armageddon.Server.Data.Models;
public class EnumLookup
{
    public int Id { get; set; }    
    public string EnumType { get; set; } = null!;  
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}