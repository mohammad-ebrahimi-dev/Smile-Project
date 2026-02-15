namespace SmileProject.Databes.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
