using System.Windows;
using GuildManager.Logic.Gameplay;
using GuildManager.Logic.Gameplay.Narrative;

namespace GuildManager.UI.Dialogues;

public partial class ImprobableNpcChoiceWindow : Window
{
    private readonly Guild _guild;
    private readonly ImprobableNpc _npc;

    public bool Accepted { get; private set; }

    public ImprobableNpcChoiceWindow(Guild guild, ImprobableNpc npc)
    {
        InitializeComponent();
        _guild = guild;
        _npc = npc;
        QuestionText.Text = $"{npc.Name} : « {npc.QuestHook} » Acceptes-tu ?";
    }

    private void YesButton_Click(object sender, RoutedEventArgs e)
    {
        _guild.Narration.AcceptImprobableNpcQuest(_npc);
        Accepted = true;
        Close();
    }

    private void NoButton_Click(object sender, RoutedEventArgs e)
    {
        _guild.Narration.DeclineImprobableNpcQuest();
        Accepted = false;
        Close();
    }
}
