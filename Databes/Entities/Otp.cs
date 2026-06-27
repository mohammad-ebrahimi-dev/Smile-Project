namespace SmileProject.Databes.Entities
{
    public class Otp
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
