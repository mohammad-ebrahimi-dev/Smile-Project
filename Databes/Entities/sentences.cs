namespace SmileProject.Databes.Entities;

public class Sentences
{
    public int Id { get; set; }
    public string SentenceText { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }

    public ICollection<UserSentence> UserSentences { get; set; }
}
