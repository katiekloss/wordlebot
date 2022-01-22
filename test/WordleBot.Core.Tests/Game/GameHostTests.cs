using WordleBot.Core.Game;
using Xunit;
using System.Linq;
using FluentAssertions;

namespace WordleBot.Core.Tests.Game;

public class GameHostTests
{
    /// <summary>
    /// Simply tests that a guess with no duplicate letters works correctly
    /// </summary>
    [Fact]
    public void GuessWithDistinctLetters()
    {
        var host = new GameHost("clothing");
        
        var result = host.Guess("creation");

        // Answers conclusively
        result.Slots.Should().NotContain(SlotStatus.Unknown);

        result.Slots.Should().Equal(new SlotStatus[8]
            {
                SlotStatus.Correct,
                SlotStatus.Out,
                SlotStatus.Out,
                SlotStatus.Out,
                SlotStatus.In,
                SlotStatus.Correct,
                SlotStatus.In,
                SlotStatus.In
            });
    }

    /// <summary>
    /// Tests that a guess with more than one of the same letter will
    /// answer with Correct and In, instead of Correct and Out
    /// </summary>
    [Fact]
    public void GuessWithDuplicates()
    {
        var host = new GameHost("ceremony");
        var result = host.Guess("coverage");

        result.Slots.Should().NotContain(SlotStatus.Unknown);

        result.Slots.Should().Equal(new SlotStatus[8]
        {
            SlotStatus.Correct,
            SlotStatus.In,
            SlotStatus.Out,
            SlotStatus.Correct,
            SlotStatus.In,
            SlotStatus.Out,
            SlotStatus.Out,
            SlotStatus.In
        });
    }
}