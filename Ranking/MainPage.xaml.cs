using static Ranking.RankingClass;

namespace Ranking;

public partial class MainPage : ContentPage
{
    private readonly RankingClass ranking;

    public MainPage()
    {
        InitializeComponent();

        ranking = new RankingClass();
    }
    private void OnConfirmButtonClicked(Object sender, EventArgs e)
    {
        Player p = new();
        p.Name = nameContentEntry.Text;
        p.Description = descriptionContentEntry.Text;
        if (winsContentEntry.Text is not null)
            p.Wins = int.Parse(winsContentEntry.Text);
        if (lossesContentEntry.Text is not null)
            p.Losses = int.Parse(lossesContentEntry.Text);

        try
        {
            ranking.AddPlayer(p);
            vertLayout.Add(new Label());
        }
        catch (ArgumentException)
        {
            DisplayAlert("Duplicate Name", "This player has already been added", "Ok");
        }
        DisplayPlayers(ranking);
    }

    private void DisplayPlayers(RankingClass players)
    {
        for(int i = 1; i < vertLayout.Count; i++)
        {
            if (vertLayout[i].GetType().Equals(typeof(Label)))
            {
                Label l = vertLayout[i] as Label;
                l.Text = players.GetPlayerAtRank(i).Name + " Wins: " + players.GetPlayerAtRank(i).Wins;
            }
        }
    }
}

