namespace Messages;

public class MyMessage
{
    public MyMessage(int id, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }

    public int Id { get; }
    public DateTimeOffset CreatedAt { get; }
}
