using WordleBot.Core.Game;
using WordleBot.Core.Utilities;

namespace WordleBot.Core.Intelligence
{
    /// <summary>
    /// I solve Wordles!
    /// </summary>
    public class SolverEngine
    {
        private readonly GameHost game;
        private readonly Func<WordList> wordListGenerator;

        public SolverEngine(GameHost game, Func<WordList> wordListGenerator)
        {
            this.game = game;
            this.wordListGenerator = wordListGenerator;
        }

        /// <summary>
        /// Try to answer the Wordle, and return the answer (or null if we failed)
        /// along with how many guesses it took.
        /// </summary>
        public (string? answer, int numberOfGuesses) Solve()
        {
            // Get a fresh wordlist, since we modify them in-place
            var wordList = wordListGenerator();

            // Make a letter domain dictionary for each slot and all
            // of the possible letters for it (i.e. the full alphabet)
            var possibilities = Enumerable.Range(0, game.Length).ToDictionary(
                i => i,
                i => string.Join("", Enumerable.Range(0, 26).Select(i => (char)(97 + i))));
            var guesses = new List<GuessResult>();
            var rand = new Random();
            string currentGuess = string.Empty;

            for (int currentRound = 1; currentRound < 6; currentRound++)
            {
                // Find a random word we haven't used yet.
                // We prune the wordlist at the end of the loop
                // so we know that they're all valid guesses at this point
                do
                {
                    currentGuess = wordList.Current[rand.Next(0, wordList.Current.Count)];
                }
                while (guesses.Any(g => g.Guess == currentGuess));

                var result = game.Guess(currentGuess);
                if (result.Won) return (result.Guess, currentRound);

                // Update the available letters for each slot based on the results
                foreach (int i in Enumerable.Range(0, result.Slots.Length))
                {
                    string thatLetter = currentGuess[i].ToString();
                    if (result.Slots[i] == SlotStatus.Correct)
                    {
                        // If we got it right, lock that slot to only that letter
                        // and remove it from all others
                        // TODO: I think this breaks if a letter appears twice
                        // in the answer
                        possibilities[i] = thatLetter;
                        RemoveLetterFrom(thatLetter, possibilities);
                    }
                    else if (result.Slots[i] == SlotStatus.In)
                    {
                        // If it's elsewhere in the answer, remove it from only this slot
                        possibilities[i] = RemoveLetterFrom(thatLetter, possibilities[i]);
                    }
                    else
                    {
                        // Nuke it from all of the slots
                        RemoveLetterFrom(thatLetter, possibilities);
                    }
                }

                // Store our guess and prune the available letters based on the results
                guesses.Add(result);
                foreach (string word in wordList.Current.ToList())
                {
                    if (!IsValidGuess(word, possibilities) || guesses.Any(g => g.Guess == word))
                    {
                        wordList.Current.Remove(word);
                    }
                }
            }

            return (null, 6);
        }

        /// <summary>
        /// Removes a letter from all of a letter domain dictionary's values
        /// </summary>
        private static void RemoveLetterFrom(string letter, IDictionary<int, string> domainDict)
        {
            foreach (int i in domainDict.Keys) domainDict[i] = RemoveLetterFrom(letter, domainDict[i]);
        }

        /// <summary>
        /// Removes a letter from a letter domain string, unless the
        /// domain has already been solved
        /// </summary>
        private static string RemoveLetterFrom(string letter, string from)
        {
            return from.Length > 1 ? from.Replace(letter, string.Empty) : from;
        }

        /// <summary>
        /// Given a letter domain dictionary and a word,
        /// determines if the word can be formed from the letters in the domain
        /// </summary>
        private static bool IsValidGuess(string guess, IDictionary<int, string> domainDict)
        {
            return domainDict.Keys.All(i => domainDict[i].Contains(guess[i]));
        }
    }
}