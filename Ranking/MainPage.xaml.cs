using static Ranking.RankingClass;
using System.Text.RegularExpressions;
using System.Text;

namespace Ranking;

public partial class MainPage : ContentPage
{
    private readonly RankingClass ranking;

    public MainPage()
    {
        InitializeComponent();

        ranking = new RankingClass();
        if (ranking.IsPosEnabled)
        {
            PositionContentEntry.IsEnabled = true;
            PositionContentEntry.IsVisible = true;
            PosLabel.IsVisible = true;
        }
    }
    private void OnConfirmButtonClicked(Object sender, EventArgs e)
    {
        ConfirmButton.IsEnabled = false;
        Player p = new();

        //Set the values to the player
        try
        {
            if (winsContentEntry.Text is not null)
                p.Wins = int.Parse(winsContentEntry.Text);

            if (lossesContentEntry.Text is not null)
                p.Losses = int.Parse(lossesContentEntry.Text);

            if (ranking.IsPosEnabled)
                if (PositionContentEntry.Text is not null)
                    p.Position = int.Parse(PositionContentEntry.Text);
        }
        catch
        {
            DisplayAlert("Error", "Error setting player values, please check your values and try again.", "Ok");
            ConfirmButton.IsEnabled = true;
            return;
        }

        p.Name = nameContentEntry.Text;

        p.Description = descriptionContentEntry.Text;

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
            // Get the player if the name matches the search
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

        string editAction;
        if (ranking.IsPosEnabled == false)
        {
            editAction = await DisplayActionSheet("Edit " + p.Name + " (Pick an Option)", "Cancel", "Delete Player",
                "Name",
                "Description",
                "Wins",
                "Losses");
        }
        else
        {
            editAction = await DisplayActionSheet("Edit " + p.Name + " (Pick an Option)", "Cancel", "Delete Player",
                            "Name",
                            "Description",
                            "Wins",
                            "Losses",
                            "Position");
        }

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
            if (editAction.Equals("Position"))
            {
                string position = await DisplayPromptAsync("Position", "Position");
                if (position is not null && !p.Position.Equals(""))
                    p.Position = int.Parse(position);
            }

            DisplayPlayers(ranking);
        }
        EditButton.IsVisible = true;
    }

    private async void OnViewButtonClicked(object sender, EventArgs e)
    {
        ViewButton.IsVisible = false;

        // Search for player
        Player p = new();
        try
        {
            // Get the player if the name matches the search
            string text = ViewPlayerEntry.Text;
            if (text is not null && !text.Equals(""))
                p = ranking.GetPlayer(text);
            else
            {
                // If the entry box is empty
                await DisplayAlert("View Player", "Please enter a player name", "Ok");
                ViewButton.IsVisible = true;
                return;
            }
        }
        catch
        {
            await DisplayAlert("View Player", "Error Finding Specified Player", "Ok");
            ViewButton.IsVisible = true;
            return;
        }

        if (ranking.IsPosEnabled)
            await DisplayAlert("Viewing " + p.Name, "Description: " + p.Description + "\n" + "Wins: " + p.Wins + "\n" + "Losses: " + p.Losses + "\n" + "Position: " + p.Position, "Ok");
        else
            await DisplayAlert("Viewing " + p.Name, "Description: " + p.Description + "\n" + "Wins: " + p.Wins + "\n" + "Losses: " + p.Losses, "Ok");
        ViewButton.IsVisible = true;
    }

    private void OnRosterButtonClicked(object sender, EventArgs e)
    {
        StringBuilder sb = new();
        int pos = 1;
        foreach (Player p in ranking.GetAllPlayers())
        {
            sb.Append(pos.ToString() + " " + p.Name + " Wins: " + p.Wins + " Losses: " + p.Losses + "\n");
            pos++;
        }
        DisplayAlert("Roster", sb.ToString(), "Ok");
    }

    private void PosClicked(object sender, EventArgs e)
    {
        ranking.IsPosEnabled = true;
        PositionContentEntry.IsEnabled = true;
        PositionContentEntry.IsVisible = true;
        PosLabel.IsVisible = true;
    }

    private void DisPosClicked(object sender, EventArgs e)
    {
        ranking.IsPosEnabled = false;
        PositionContentEntry.IsEnabled = false;
        PositionContentEntry.IsVisible = false;
        PosLabel.IsVisible = false;
    }
}

