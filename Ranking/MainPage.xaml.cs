using static Ranking.RankingClass;
using System.Text.RegularExpressions;
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
        ConfirmButton.IsEnabled = false;
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

        ConfirmButton.IsEnabled = true;
        DisplayPlayers(ranking);
    }

    private void DisplayPlayers(RankingClass players)
    {
        for (int i = 3; i < vertLayout.Count; i++)
        {
            if (vertLayout[i].GetType().Equals(typeof(Label)))
            {
                Label l = vertLayout[i] as Label;
                l.Text = players.GetPlayerAtRank(i).Name + " Wins: " + players.GetPlayerAtRank(i).Wins;
            }
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        EditButton.IsVisible = false;

        // Search for player
        Player p = new();
        try
        {
            string text = EditContentEntry.Text;
            if (text is not null && !text.Equals(""))
                p = ranking.GetPlayer(text);
            else
            {
                await DisplayAlert("Edit Player", "Please enter a player name", "Ok");
                EditButton.IsVisible = true;
                return;
            }
        }
        catch
        {
            await DisplayAlert("Edit Player", "Error Finding Specified Player", "Ok");
            EditButton.IsVisible = true;
            return;
        }

        string editAction = await DisplayActionSheet("Edit " + p.Name + " (Pick an Option)", "Cancel", "Delete Player",
            "Name",
            "Description",
            "Wins",
            "Losses");

        if (editAction is not null)
        {
            if (editAction.Equals("Delete Player"))
            {
                ranking.removePlayer(p);
                vertLayout.RemoveAt(vertLayout.Count - 1);
            }
            if (editAction.Equals("Description"))
            {
                string desc = await DisplayPromptAsync("Description", "Description");
                if (desc is not null && !p.Name.Equals(""))
                    p.Description = desc;
            }
            if (editAction.Equals("Name"))
            {
                string name = await DisplayPromptAsync("Name", "Name");
                if (name is not null && !p.Name.Equals(""))
                    p.Name = name;
            }
            if (editAction.Equals("Wins"))
            {
                string wins = await DisplayPromptAsync("Wins", "Wins");
                if (wins is not null && !p.Wins.Equals(""))
                    p.Wins = int.Parse(wins);
            }
            if (editAction.Equals("Losses"))
            {
                string losses = await DisplayPromptAsync("Losses", "Losses");
                if (losses is not null && !p.Losses.Equals(""))
                    p.Losses = int.Parse(losses);
            }

            DisplayPlayers(ranking);
        }
        EditButton.IsVisible = true;
    }

    private void OnViewButtonClicked(object sender, EventArgs e)
    {

    }

    private void OnRosterButtonClicked(object sender, EventArgs e)
    {

    }
}

