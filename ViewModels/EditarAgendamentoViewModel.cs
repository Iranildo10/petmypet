namespace petmypet.ViewModels
{
    public class EditarAgendamentoViewModel
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public DateTime Data { get; set; }
        public DateTime Horario { get; set; }
        public string? Observacao { get; set; }
        public bool TaxiDog { get; set; }
        public bool ClienteTraz { get; set; }
        public bool HorarioFixoSemanal { get; set; }
        public List<int> ServicoIds { get; set; } = new List<int>();

        // Confirmação para alterar fixos
        public bool AlterarTodosFixos { get; set; }
    }

}
