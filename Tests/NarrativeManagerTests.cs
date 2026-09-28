using GuildManager.Logic.Gameplay.Narrative;
using Xunit;

namespace GuildManager.Tests;

public class NarrativeManagerTests
{
    [Fact]
    public void NewManager_StartsInSeriousVoice()
    {
        var manager = new NarrativeManager();

        Assert.Equal(NarrativeVoice.Serious, manager.CurrentVoice);
        Assert.False(manager.SwitchAlreadyTriggered);
    }

    [Fact]
    public void AcceptImprobableNpcQuest_SwitchesToWtfPermanently()
    {
        var manager = new NarrativeManager();

        manager.AcceptImprobableNpcQuest(ImprobableNpc.BlackMarketDonkey);

        Assert.Equal(NarrativeVoice.Wtf, manager.CurrentVoice);
        Assert.True(manager.SwitchAlreadyTriggered);
    }

    [Fact]
    public void AcceptImprobableNpcQuest_RaisesOnSwitchToWtfEvent_OnlyOnce()
    {
        var manager = new NarrativeManager();
        int raisedCount = 0;
        ImprobableNpc? receivedNpc = null;

        manager.OnSwitchToWtf += npc =>
        {
            raisedCount++;
            receivedNpc = npc;
        };

        manager.AcceptImprobableNpcQuest(ImprobableNpc.BlackMarketDonkey);
        manager.AcceptImprobableNpcQuest(ImprobableNpc.TalkingCat); // second NPC, after the switch

        Assert.Equal(1, raisedCount); // the event only fires the first time
        // ImprobableNpc.BlackMarketDonkey returns a new instance on every
        // access (not a singleton), so compare by value, not by reference.
        Assert.Equal("Ane du marché noir", receivedNpc?.Name);
    }

    [Fact]
    public void AcceptImprobableNpcQuest_SecondCall_DoesNotChangeAnything()
    {
        var manager = new NarrativeManager();

        manager.AcceptImprobableNpcQuest(ImprobableNpc.BlackMarketDonkey);
        manager.AcceptImprobableNpcQuest(ImprobableNpc.MessengerPigeon);

        // Still Wtf, and no exception - the second acceptance is simply a no-op.
        Assert.Equal(NarrativeVoice.Wtf, manager.CurrentVoice);
    }

    [Fact]
    public void DeclineImprobableNpcQuest_KeepsSeriousVoice()
    {
        var manager = new NarrativeManager();

        manager.DeclineImprobableNpcQuest();

        Assert.Equal(NarrativeVoice.Serious, manager.CurrentVoice);
        Assert.False(manager.SwitchAlreadyTriggered);
    }

    [Fact]
    public void GetText_ReturnsSeriousOrWtfText_DependingOnActiveVoice()
    {
        var manager = new NarrativeManager();
        var entry = new DialogueEntry
        {
            Id = "test_entry",
            Day = 1,
            Character = "Narrator",
            SeriousText = "A grim tale begins.",
            WtfText = "lol anyway"
        };

        Assert.Equal("A grim tale begins.", manager.GetText(entry));

        manager.AcceptImprobableNpcQuest(ImprobableNpc.BlackMarketDonkey);

        Assert.Equal("lol anyway", manager.GetText(entry));
    }

    [Fact]
    public void RestoreState_ReinjectsSavedVoiceAndFlag()
    {
        var manager = new NarrativeManager();

        manager.RestoreState(NarrativeVoice.Wtf, switchAlreadyTriggered: true);

        Assert.Equal(NarrativeVoice.Wtf, manager.CurrentVoice);
        Assert.True(manager.SwitchAlreadyTriggered);
    }
}
