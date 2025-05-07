namespace petmypet.Models
{
    public class AgendamentoFixoExcecao
    {
        public int Id { get; set; }
        public int AgendamentoFixoId { get; set; }
        public DateTime Data { get; set; }

        public AgendamentoFixo AgendamentoFixo { get; set; }
    }
}
