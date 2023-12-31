/// <summary>
/// Represents a message that indicates the game should proceed to the next level.
/// Implements the IMessage interface for use with the MessageBus.
/// </summary>
public class NextLevelMessage : IMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NextLevelMessage"/> class.
    /// </summary>
    public NextLevelMessage()
    {
    }
}
