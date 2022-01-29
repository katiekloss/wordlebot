namespace WordleBot.Core
{
    /// <summary>
    /// Represents the result of a single guess
    /// </summary>
    public record GuessResult
    {
        public bool Won { get; set; }
        public string Guess { get; init; } = "";
        public SlotStatus[] Slots { get; init; }

        public GuessResult(string answer)
        {
            Slots = new SlotStatus[answer.Length];
        }
    }
}