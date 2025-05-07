using petmypet.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Cliente
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [MaxLength(20)]
    public string Telefone1 { get; set; }

    [MaxLength(20)]
    public string? Telefone2 { get; set; }

    [MaxLength(200)]
    public string Endereco1 { get; set; }

    [MaxLength(200)]
    public string? Endereco2 { get; set; }

    public IEnumerable<Pet>? Pets { get; set; }

}
