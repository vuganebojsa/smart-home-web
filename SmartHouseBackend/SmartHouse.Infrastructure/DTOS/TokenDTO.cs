namespace SmartHouse.Infrastructure.DTOS
{
    public class TokenDTO
    {
        public string? Token { get; set; } = string.Empty;
        public bool? IsChangedPassword { get; set; } = true;    
    }
}
