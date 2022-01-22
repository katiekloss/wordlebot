namespace WordleBot.Core.Game
{
    /// <summary>
    /// A thing that can play Wordle with you!
    /// </summary>
    public class GameHost
    {
        public string Answer { get; init; }

        public GameHost(string answer)
        {
            Answer = answer;
        }

        public GuessResult Guess(string attempt)
        {
            if (attempt.Length != Answer.Length) throw new ArgumentException($"Guess must be {Answer.Length} letters");

            GuessResult response = new(Answer)
            {
                Guess = attempt
            };
            
            // Scan the answer and look for exact matches. If it's a mismatch, then
            // try to disqualify the letter entirely. Any that appear SOMEWHERE
            // in the answer are inconclusive for now.
            for (int i = 0; i < Answer.Length; i++)
            {
                if (attempt[i] == Answer[i])
                {
                    response.Slots[i] = SlotStatus.Correct;
                }
                else if (!Answer.Contains(attempt[i]))
                {
                    response.Slots[i] = SlotStatus.Out;
                }
                else
                {
                    response.Slots[i] = SlotStatus.Unknown;
                }
            }

            // For all of the inconclusive guesses:
            // - If the guess contains n instances of a letter, where n > 1, and the answer
            //     contains fewer than n of that letter, then the inconclusive guess is out
            // - Otherwise, we're safe to say that it's just in the wrong position.
            // Ex:
            // Answer: O B L I V I O N
            // Guess:  B A T H R O O M
            // Only the last O is correct, but the actual word contains a second O
            // so the other is just out of order.
            // Ex:
            // Answer: A C C U R A T E
            // Guess:  C O N C R E T E
            // Only the last E is correct, but the answer only contains 1 E
            // so we know that the other can't possibly be correct.
            foreach (int i in Enumerable.Range(0, Answer.Length).Where(i => response.Slots[i] == SlotStatus.Unknown).ToList())
            {
                char thatLetter = attempt[i];

                if (attempt.Count(l => l == thatLetter) <= Answer.Count(l => l == thatLetter))
                {
                    response.Slots[i] = SlotStatus.In;
                }
                else
                {
                    response.Slots[i] = SlotStatus.Out;
                }
            }

            return response;
        }
    }
}