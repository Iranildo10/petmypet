using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


    public class Servico
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A Descrição do serviço é obrigatória")]
        [MaxLength(200)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O Valor do serviço é obrigatório")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

   
        public bool Ativo { get; set; } = true;

        public IEnumerable<AgendamentoServico>? AgendamentoServicos { get; set; }
    }


