namespace WordleBot.Core
{
    public enum SlotStatus
    {
        ///<summary>The letter hasn't yet been conclusively determined</summary>
        Unknown,
        ///<summary>The letter is in the correct position</summary>
        Correct,
        ///<summary>The letter is in the word, but not in the correct position</summary>
        In,
        ///<summary>The letter is not in the word at all</summary>
        Out
    }
}