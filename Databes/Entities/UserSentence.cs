using SmileProject.Databes.Entities;

public class UserSentence
{
    public int Id { get; set; }
    public int UserId { get; set; }           
    public User User { get; set; }
    public int SentenceId { get; set; }       
    public Sentences Sentence { get; set; }  
    public DateTime SendAt { get; set; }
}
