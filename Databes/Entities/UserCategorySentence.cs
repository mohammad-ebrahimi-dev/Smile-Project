namespace SmileProject.Databes.Entities
{
    public class UserCategorySentence
    {
        public int Id { get; set; }

        // Foreign Key - User
        public int UserId { get; set; }
        public User User { get; set; }

        // Foreign Key - CategorySentence
        public int CategorySentenceId { get; set; }
        public CategorySentence CategorySentence { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}