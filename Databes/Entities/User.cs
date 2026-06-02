namespace SmileProject.Databes.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<UserSentence> UserSentences { get; set; }
    }

}
