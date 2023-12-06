namespace Lab4.Domain.Dtos;

public record LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
};