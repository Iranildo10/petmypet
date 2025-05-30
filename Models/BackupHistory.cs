namespace petmypet.Models
{
    public class BackupHistory
    {
        public int Id { get; set; }
        public string FolderPath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime BackupDate { get; set; }
    }
}
